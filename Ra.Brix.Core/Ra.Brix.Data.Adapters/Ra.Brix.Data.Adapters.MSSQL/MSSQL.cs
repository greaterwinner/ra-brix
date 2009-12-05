﻿/*
 * Ra-Brix - A Modular-based Framework for building 
 * Web Applications Copyright 2009 - Thomas Hansen 
 * thomas@ra-ajax.org. Unless permission is 
 * explicitly given this code is licensed under the 
 * GNU Affero GPL version 3 which can be found in the 
 * license.txt file on disc.
 * 
 */

using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Reflection;
using Ra.Brix.Types;
using System.Collections;
using System.IO;
using Ra.Brix.Data.Internal;

/**
 * Namespace for Microsoft SQL Server Database Adapters in Ra-Brix
 */
namespace Ra.Brix.Data.Adapters.MSSQL
{
    /**
     * Microsoft SQL Server (probably 2005 and later) Database Adapter for
     * Ra-Brix. Contains all MS SQL specific logic needed to use Ra-Brix 
     * together with MS SQL 2005 and higher.
     */
    public class MSSQL : StdSQLDataAdapter
    {
        private SqlConnection _connection;
        private static bool _hasInitialised;
        private Dictionary<string, Type> _cacheOfTypes;

        public override void Open(string connectionString)
        {
            _connection = new SqlConnection(connectionString);
            _connection.Open();
            if (!_hasInitialised)
                InitializeSchema();
        }

        private void InitializeSchema()
        {
            using (Stream stream = Assembly.GetAssembly(GetType()).GetManifestResourceStream("Ra.Brix.Data.Adapters.MSSQL.Schema.sql"))
            {
                if (stream != null)
                {
                    TextReader reader = new StreamReader(stream);
                    string sql = reader.ReadToEnd();
                    SqlCommand cmd = new SqlCommand(sql, _connection);
                    cmd.ExecuteNonQuery();
                }
                else
                {
                    throw new ApplicationException("Couldn't find DDL resource in assembly. Something is *seriously* wrong ...!");
                }
            }
            _hasInitialised = true;
        }

        public override int CountWhere(Type type, params Criteria[] args)
        {
            string where = CreateCriteriasForDocument(type, null, args);
            SqlCommand cmd = new SqlCommand("select count(*) from Documents as d" + where, _connection);
            return (int)cmd.ExecuteScalar();
        }

        protected override object SelectObjectByID(Type type, int id)
        {
            // Checking to see if object exists...
            SqlCommand cmdExists = new SqlCommand(
                string.Format("select count(*) from Documents where ID={0}", id),
                _connection);
            if ((int)cmdExists.ExecuteScalar() == 0)
                return null;

            // Object exists, starting to retrieve it...
            ConstructorInfo ctor = type.GetConstructor(
                BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance,
                null,
                new Type[] { },
                null);
            if (ctor == null)
                throw new ApplicationException("Cannot have a LegoDocument which doesn't have a default constructor, type name is '" + type.FullName + "'");
            object retVal = ctor.Invoke(null);
            type.GetProperty("ID", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic).GetSetMethod(true).Invoke(retVal, new object[] { id });

            PropertyInfo[] props =
                    type.GetProperties(
                        BindingFlags.Instance | 
                        BindingFlags.Public | 
                        BindingFlags.NonPublic);
            foreach (PropertyInfo idxProp in props)
            {
                ActiveFieldAttribute[] attrs =
                    idxProp.GetCustomAttributes(typeof(ActiveFieldAttribute), true) as ActiveFieldAttribute[];
                if (attrs != null && attrs.Length > 0)
                {
                    // Serializable property
                    string where = " FK_Document=" + id + " and Name='" + Helpers.PropertyName(idxProp) + "'";
                    string tableName;
                    switch (idxProp.PropertyType.FullName)
                    {
                        case "System.Boolean":
                            tableName = "PropertyBools";
                            break;
                        case "System.DateTime":
                            tableName = "PropertyDates";
                            break;
                        case "System.Decimal":
                            tableName = "PropertyDecimals";
                            break;
                        case "System.Int32":
                            tableName = "PropertyInts";
                            break;
                        case "System.String":
                            tableName = "PropertyStrings";
                            break;
                        case "System.Byte[]":
                            tableName = "PropertyBLOBS";
                            break;
                        default:
                            if (idxProp.PropertyType.FullName.IndexOf("Ra.Brix.Types.LazyList") == 0)
                            {
                                // LazyList...
                                Type typeOfList = idxProp.PropertyType;
                                Type typeOfListGenericArgument = idxProp.PropertyType.GetGenericArguments()[0];
                                LazyHelper helper = new LazyHelper(typeOfListGenericArgument, id, attrs[0].IsOwner, idxProp.Name);
                                FunctorGetItems del = helper.GetItems;
                                object tmp = typeOfList.GetConstructors()[0].Invoke(new object[] { del });
                                idxProp.GetSetMethod(true).Invoke(retVal, new[] { tmp });
                            }
                            else if (idxProp.PropertyType.FullName.IndexOf("System.Collections.Generic.List") == 0)
                            {
                                // NOT LazyList, but still List...!
                                Type typeOfListGenericArgument = idxProp.PropertyType.GetGenericArguments()[0];
                                List<object> tmpValues = attrs[0].IsOwner ? 
                                    new List<object>(Instance.Select(typeOfListGenericArgument, idxProp.Name, Criteria.ParentId(id))) : 
                                    new List<object>(Instance.Select(typeOfListGenericArgument, idxProp.Name, Criteria.ExistsIn(id)));
                                MethodInfo addMethod = idxProp.PropertyType.GetMethod("Add");
                                object listContent = idxProp.GetGetMethod(true).Invoke(retVal, new object[] { });
                                foreach (object idxTmpValue in tmpValues)
                                {
                                    addMethod.Invoke(listContent, new[] { idxTmpValue });
                                }
                            }
                            else
                            {
                                if (attrs[0].IsOwner)
                                {
                                    object tmp = SelectFirst(idxProp.PropertyType, idxProp.Name, Criteria.ParentId(id));
                                    idxProp.GetSetMethod().Invoke(retVal, new[] { tmp });
                                }
                                else
                                {
                                    object tmp = SelectFirst(idxProp.PropertyType, idxProp.Name, Criteria.ExistsIn(id));
                                    idxProp.GetSetMethod().Invoke(retVal, new[] { tmp });
                                }
                            }
                            continue; // Possibly composition
                    }
                    SqlCommand cmd = new SqlCommand(
                        string.Format("select Value from {0} where {1}", tableName, where), _connection);
                    object propValue = cmd.ExecuteScalar();
                    idxProp.GetSetMethod(true).Invoke(retVal, new[] { propValue });
                }
            }
            return retVal;
        }

        public override object SelectFirst(Type type, string propertyName, params Criteria[] args)
        {
            string where = CreateCriteriasForDocument(type, propertyName, args);
            SqlCommand cmd = new SqlCommand("select ID from Documents as d" + where, _connection);
            int retValID;
            using (SqlDataReader reader = cmd.ExecuteReader())
            {
                if (reader == null || !reader.Read())
                    return null;
                retValID = reader.GetInt32(0);
            }
            return SelectByID(type, retValID);
        }

        public override IEnumerable<object> Select(Type type, string propertyName, params Criteria[] args)
        {
            string where = CreateCriteriasForDocument(type, propertyName, args);
            SqlCommand cmd = new SqlCommand("select ID from Documents as d" + where, _connection);
            List<int> retValIDs = new List<int>();
            using (SqlDataReader reader = cmd.ExecuteReader())
            {
                while (reader != null && reader.Read())
                {
                    retValIDs.Add(reader.GetInt32(0));
                }
            }
            foreach(int idxRetValID in retValIDs)
            {
                yield return SelectByID(type, idxRetValID);
            }
        }

        public override IEnumerable<object> Select()
        {
            SqlCommand cmd = new SqlCommand("select ID, TypeName from Documents as d", _connection);
            List<Tuple<int, Type>> retValIDs = new List<Tuple<int, Type>>();
            using (SqlDataReader reader = cmd.ExecuteReader())
            {
                if (reader != null)
                {
                    while (reader.Read())
                    {
                        if (_cacheOfTypes == null)
                        {
                            _cacheOfTypes = new Dictionary<string, Type>();
                            foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
                            {
                                foreach (Type idxType in assembly.GetTypes())
                                {
                                    _cacheOfTypes[idxType.FullName] = idxType;
                                }
                            }
                        }
                        string typeFullName = reader.GetString(1).Substring(3);
                        if (_cacheOfTypes.ContainsKey(typeFullName))
                        {
                            Type type = _cacheOfTypes[typeFullName];
                            retValIDs.Add(new Tuple<int, Type>(reader.GetInt32(0), type));
                        }
                    }
                    
                }
            }
            foreach (Tuple<int, Type> idxRetValID in retValIDs)
            {
                yield return SelectByID(idxRetValID.Right, idxRetValID.Left);
            }
        }

        protected override void DeleteObject(int id)
        {
            using (SqlTransaction transaction = _connection.BeginTransaction())
            {
                DeleteWithTransaction(id, transaction);
                transaction.Commit();
            }
        }

        protected void DeleteObject(int id, SqlTransaction transaction)
        {
            DeleteWithTransaction(id, transaction);
        }

        private void DeleteWithTransaction(int id, SqlTransaction transaction)
        {
            DeleteChildren(id, transaction);
            DeleteComposition(id, transaction);
            SqlCommand cmd = new SqlCommand(
                string.Format("delete from Documents where ID=" + id),
                _connection,
                transaction);
            cmd.ExecuteNonQuery();
        }

        private void DeleteComposition(int id, SqlTransaction transaction)
        {
            SqlCommand cmdChild = new SqlCommand(
                string.Format("delete from Documents2Documents where Document1ID={0} or Document2ID={0}", id),
                _connection,
                transaction);
            cmdChild.ExecuteNonQuery();
        }

        private void DeleteChildren(int id, SqlTransaction transaction)
        {
            List<int> childDocuments = new List<int>();
            SqlCommand cmdChild = new SqlCommand(
                string.Format("select ID from Documents where Parent={0}", id),
                _connection,
                transaction);
            using (SqlDataReader reader = cmdChild.ExecuteReader())
            {
                if (reader != null)
                {
                    while (reader.Read())
                    {
                        childDocuments.Add(reader.GetInt32(0));
                    }
                    
                }
            }
            foreach (int idxChild in childDocuments)
            {
                DeleteWithTransaction(idxChild, transaction);
            }
        }

        public override void Save(object value)
        {
            Type type = value.GetType();
            int id = (int)type.GetProperty(
                "ID",
                BindingFlags.Instance |
                BindingFlags.NonPublic |
                BindingFlags.Public)
                .GetGetMethod()
                .Invoke(value, null);
            bool isUpdate = id != 0;
            PropertyInfo[] props =
                    type.GetProperties(
                        BindingFlags.Instance |
                        BindingFlags.Public |
                        BindingFlags.NonPublic);
            if (isUpdate)
            {
                // We need to iterator through all IsOwner=false children
                // But before we need to iterate through all LazyLists where IsOwner=false
                // and actually *RETRIEVE* them. This can be significantly optimized by
                // checking to see if any of the LazyLists have been retrieved, and if NOT
                // completely avoid touvhing the Many2Many relationships...
                foreach (PropertyInfo idxProp in props)
                {
                    if (idxProp.PropertyType.FullName.IndexOf("Ra.Brix.Types.LazyList") == 0)
                    {
                        ActiveFieldAttribute[] attrs =
                            idxProp.GetCustomAttributes(typeof(ActiveFieldAttribute), true) as ActiveFieldAttribute[];
                        if (attrs != null && attrs.Length > 0 && attrs[0].IsOwner == false)
                        {
                            object tmpLazyReference = idxProp.GetGetMethod(true).Invoke(value, null);
                            tmpLazyReference.GetType().GetMethod(
                                "FillList",
                                BindingFlags.Instance |
                                BindingFlags.NonPublic |
                                BindingFlags.Public).Invoke(tmpLazyReference, null);
                        }
                    }
                }
            }
            using (SqlTransaction transaction = _connection.BeginTransaction())
            {
                SaveWithTransaction(value, transaction, -1, null);
                transaction.Commit();
            }
        }

        private int SaveWithTransaction(object value, SqlTransaction transaction, int parentId, string parentPropertyName)
        {
            Type type = value.GetType();
            int id = (int)type.GetProperty(
                "ID",
                BindingFlags.Instance |
                BindingFlags.NonPublic |
                BindingFlags.Public)
                .GetGetMethod()
                .Invoke(value, null);
            bool isUpdate = id != 0;
            PropertyInfo[] props =
                    type.GetProperties(
                        BindingFlags.Instance |
                        BindingFlags.Public |
                        BindingFlags.NonPublic);
            if (isUpdate)
            {
                if (parentId == -1)
                {
                    SqlCommand cmd = new SqlCommand(
                        string.Format("update Documents set Modified=getdate() where ID={0}",
                            id), _connection, transaction);
                    cmd.ExecuteNonQuery();
                }
                else
                {
                    SqlCommand cmd = new SqlCommand(
                        string.Format("update Documents set Modified=getdate(), Parent={1}, ParentPropertyName='{2}' where ID={0}",
                            id,
                            parentId,
                            parentPropertyName), _connection, transaction);
                    cmd.ExecuteNonQuery();
                }
            }
            else
            {
                SqlCommand cmd = new SqlCommand(
                    string.Format(
                        "insert into Documents (TypeName, Created, Modified, Parent, ParentPropertyName) values ('doc{0}', getdate(), getdate(), {1}, '{2}');select @@Identity;", 
                        type.FullName,
                        parentId == -1 ? "NULL" : parentId.ToString(),
                        parentPropertyName ?? "null"), 
                    _connection, 
                    transaction);
                id = (int)((decimal)cmd.ExecuteScalar());
                type.GetProperty(
                    "ID",
                    BindingFlags.Instance |
                    BindingFlags.NonPublic |
                    BindingFlags.Public)
                    .GetSetMethod(true)
                    .Invoke(value, new object[] { id });
            }
            if (isUpdate)
            {
                // We need to iterate through everything and delete "old values" before saving new values...
                foreach (string idxTableName in new[] { "PropertyBLOBS", "PropertyBools", "PropertyDates", "PropertyDates", "PropertyDecimals", "PropertyInts", "PropertyLongStrings", "PropertyStrings" })
                {
                    string sql = string.Format("delete from {0} where FK_Document={1}", idxTableName, id);
                    SqlCommand cmd = new SqlCommand(sql, _connection, transaction);
                    cmd.ExecuteNonQuery();
                }

                SqlCommand cmdD2D = new SqlCommand(
                    string.Format("DELETE from Documents2Documents where Document1ID={0}", id), 
                    _connection, 
                    transaction);
                cmdD2D.ExecuteNonQuery();
            }
            List<int> listOfIDsOfChildren = new List<int>();
            foreach (PropertyInfo idxProp in props)
            {
                ActiveFieldAttribute[] attrs =
                    idxProp.GetCustomAttributes(typeof(ActiveFieldAttribute), true) as ActiveFieldAttribute[];
                if (attrs != null && attrs.Length > 0)
                {
                    string tableName;
                    object valueOfProperty = idxProp.GetGetMethod(true).Invoke(value, null);
                    if (valueOfProperty == null)
                        continue;
                    switch (idxProp.PropertyType.FullName)
                    {
                        case "System.Boolean":
                            tableName = "PropertyBools";
                            break;
                        case "System.DateTime":
                            if (((DateTime)valueOfProperty) == DateTime.MinValue)
                                continue; // "NULL" value...
                            tableName = "PropertyDates";
                            break;
                        case "System.Decimal":
                            tableName = "PropertyDecimals";
                            break;
                        case "System.Int32":
                            tableName = "PropertyInts";
                            break;
                        case "System.String":
                            tableName = "PropertyStrings";
                            break;
                        case "System.Byte[]":
                            tableName = "PropertyBLOBS";
                            break;
                        default:
                            IEnumerable enumerable = valueOfProperty as IEnumerable;
                            if (enumerable == null)
                            {
                                if (attrs[0].IsOwner)
                                {
                                    int childId = SaveWithTransaction(valueOfProperty, transaction, id, idxProp.Name);
                                    listOfIDsOfChildren.Add(childId);
                                }
                                else
                                {
                                    int childId = (int)valueOfProperty.GetType().GetProperty(
                                        "ID",
                                        BindingFlags.Instance |
                                        BindingFlags.NonPublic |
                                        BindingFlags.Public)
                                        .GetGetMethod()
                                        .Invoke(valueOfProperty, null);
                                    SqlCommand cmdContains = new SqlCommand(
                                        string.Format(
                                            "insert into Documents2Documents (Document1ID, Document2ID, PropertyName) values ({0}, {1}, '{2}')",
                                            id,
                                            childId,
                                            idxProp.Name),
                                        _connection,
                                        transaction);
                                    cmdContains.ExecuteNonQuery();
                                }
                            }
                            else
                            {
                                PropertyInfo listRetrieved = 
                                    enumerable.GetType().GetProperty(
                                        "ListRetrieved", 
                                        BindingFlags.Instance | 
                                        BindingFlags.Public | 
                                        BindingFlags.NonPublic);
                                if (listRetrieved != null)
                                {
                                    // LazyList...
                                    if ((bool)listRetrieved.GetGetMethod(true).Invoke(enumerable, null))
                                    {
                                        if (attrs[0].IsOwner)
                                        {
                                            // List somehow...
                                            foreach (object idxChild in enumerable)
                                            {
                                                int childId = SaveWithTransaction(idxChild, transaction, id, idxProp.Name);
                                                listOfIDsOfChildren.Add(childId);
                                            }
                                        }
                                        else
                                        {
                                            // List somehow...
                                            foreach (object idxChild in enumerable)
                                            {
                                                int documentId = SaveWithTransaction(idxChild, transaction, -1, idxProp.Name);
                                                SqlCommand cmdContains = new SqlCommand(
                                                    string.Format(
                                                        "insert into Documents2Documents (Document1ID, Document2ID, PropertyName) values ({0}, {1}, '{2}')",
                                                        id,
                                                        documentId,
                                                        idxProp.Name),
                                                    _connection,
                                                    transaction);
                                                cmdContains.ExecuteNonQuery();
                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    // NOT LazyList...
                                    if (attrs[0].IsOwner)
                                    {
                                        // List somehow...
                                        foreach (object idxChild in enumerable)
                                        {
                                            int childId = SaveWithTransaction(idxChild, transaction, id, idxProp.Name);
                                            listOfIDsOfChildren.Add(childId);
                                        }
                                    }
                                    else
                                    {
                                        // List somehow...
                                        foreach (object idxChild in enumerable)
                                        {
                                            int documentId = SaveWithTransaction(idxChild, transaction, -1, idxProp.Name);
                                            SqlCommand cmdContains = new SqlCommand(
                                                string.Format(
                                                    "insert into Documents2Documents (Document1ID, Document2ID, PropertyName) values ({0}, {1}, '{2}')",
                                                    id,
                                                    documentId,
                                                    idxProp.Name),
                                                _connection,
                                                transaction);
                                            cmdContains.ExecuteNonQuery();
                                        }
                                    }
                                }
                            }
                            continue;
                    }
                    string sql = string.Format(
                         "insert into {0} (FK_Document, Value, Name) values({1}, @value, '{2}')",
                         tableName,
                         id,
                         Helpers.PropertyName(idxProp));
                    SqlCommand cmd = new SqlCommand(sql, _connection, transaction);
                    cmd.Parameters.Add(new SqlParameter("@value", valueOfProperty));
                    cmd.ExecuteNonQuery();
                }
            }
            string whereDelete = "";
            bool first = true;
            foreach (int idx in listOfIDsOfChildren)
            {
                if (!first)
                    whereDelete += ",";
                first = false;
                whereDelete += idx.ToString();
            }
            string andNotIn = string.IsNullOrEmpty(whereDelete) ? "" : string.Format(" and ID not in({0})", whereDelete);
            SqlCommand cmdDeleteAllInfants = new SqlCommand(
                string.Format("select ID from Documents where Parent={0}{1}", id, andNotIn),
                _connection,
                transaction);
            List<int> idsToDelete = new List<int>();
            using (SqlDataReader reader = cmdDeleteAllInfants.ExecuteReader())
            {
                if (reader != null)
                {
                    while (reader.Read())
                    {
                        int idxChildId = reader.GetInt32(0);
                        idsToDelete.Add(idxChildId);
                    }
                    
                }
            }
            foreach (int idxItemToDelete in idsToDelete)
            {
                DeleteObject(idxItemToDelete, transaction);
            }
            return id;
        }

        public override void Close()
        {
            _connection.Close();
        }
    }
}