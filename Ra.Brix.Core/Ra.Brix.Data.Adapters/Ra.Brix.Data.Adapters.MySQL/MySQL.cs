/*
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
using System.Reflection;
using Ra.Brix.Types;
using System.Collections;
using System.IO;
using Ra.Brix.Data.Internal;
using MySql.Data.MySqlClient;

/**
 * Namespace for MySQL Database Adapters in Ra-Brix
 */
namespace Ra.Brix.Data.Adapters.MySQL
{
    /**
     * My SQL Data Adapters for Ra-Brix. Contains all MySQL specific logic for 
     * accessing and using MySQL database as your data storage backend.
     */
    public class MySQL : StdSQLDataAdapter
    {
        private MySqlConnection _connection;
        private static bool _hasInitialised;

        public override void Open(string connectionString)
        {
            _connection = new MySqlConnection(connectionString);
            _connection.Open();
            if (!_hasInitialised)
                InitializeSchema();
        }

        private void InitializeSchema()
        {
            using (Stream stream = Assembly.GetAssembly(GetType()).GetManifestResourceStream("Ra.Brix.Data.Adapters.MySQL.Schema.sql"))
            {
                if (stream == null)
                {
                    throw new ApplicationException("DDL resource missing. Something is seriously wrong...!");
                }
                TextReader reader = new StreamReader(stream);
                string sql = reader.ReadToEnd();
                MySqlCommand cmd = new MySqlCommand(sql, _connection);
                cmd.ExecuteNonQuery();
            }
            _hasInitialised = true;
        }

        public override int CountWhere(Type type, params Criteria[] args)
        {
            string where = CreateCriteriasForDocument(type, null, args);
            MySqlCommand cmd = new MySqlCommand("select count(*) from Documents as d" + where, _connection);
            return (int)((long)cmd.ExecuteScalar());
        }

        protected override object SelectObjectByID(Type type, int id)
        {
            // Checking to see if object exists...
            MySqlCommand cmdExists = new MySqlCommand(
                string.Format("select count(*) from Documents where ID={0}", id),
                _connection);
            if ((int)((long)cmdExists.ExecuteScalar()) == 0)
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
                    MySqlCommand cmd = new MySqlCommand(
                        string.Format("select Value from {0} where {1}", tableName, where), _connection);
                    object propValue = cmd.ExecuteScalar();
                    if (propValue is DBNull || propValue == null)
                        propValue = null;
                    else if (idxProp.PropertyType == typeof(bool))
                        propValue = ((ulong)propValue) != 0 ? true : false;
                    idxProp.GetSetMethod(true).Invoke(retVal, new[] { propValue });
                }
            }
            return retVal;
        }

        public override object SelectFirst(Type type, string propertyName, params Criteria[] args)
        {
            string where = CreateSelectStatementForDocument(type, propertyName, args);
            MySqlCommand cmd = new MySqlCommand(where, _connection);
            int retValID;
            using (MySqlDataReader reader = cmd.ExecuteReader())
            {
                if (reader == null || !reader.Read())
                    return null;
                retValID = reader.GetInt32(0);
            }
            return SelectByID(type, retValID);
        }

        public override IEnumerable<object> Select(Type type, string propertyName, params Criteria[] args)
        {
            string where = CreateSelectStatementForDocument(type, propertyName, args);
            MySqlCommand cmd = new MySqlCommand(where, _connection);
            List<int> retValIDs = new List<int>();
            using (MySqlDataReader reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    retValIDs.Add(reader.GetInt32(0));
                }
            }
            foreach(int idxRetValID in retValIDs)
            {
                yield return SelectByID(type, idxRetValID);
            }
        }

        private Dictionary<string, Type> _cacheOfTypes;
        public override IEnumerable<object> Select()
        {
            MySqlCommand cmd = new MySqlCommand("select ID, TypeName from Documents as d", _connection);
            List<Tuple<int, Type>> retValIDs = new List<Tuple<int, Type>>();
            using (MySqlDataReader reader = cmd.ExecuteReader())
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
            foreach (Tuple<int, Type> idxRetValID in retValIDs)
            {
                yield return SelectByID(idxRetValID.Right, idxRetValID.Left);
            }
        }

        protected override void DeleteObject(int id)
        {
            using (MySqlTransaction transaction = _connection.BeginTransaction())
            {
                DeleteWithTransaction(id, transaction);
                transaction.Commit();
            }
        }

        private void DeleteWithTransaction(int id, MySqlTransaction transaction)
        {
            DeleteChildren(id, transaction);
            DeleteComposition(id, transaction);
            MySqlCommand cmd = new MySqlCommand(
                string.Format("delete from Documents where ID=" + id),
                _connection,
                transaction);
            cmd.ExecuteNonQuery();
        }

        private void DeleteComposition(int id, MySqlTransaction transaction)
        {
            MySqlCommand cmdChild = new MySqlCommand(
                string.Format("delete from Documents2Documents where Document1ID={0} or Document2ID={0}", id),
                _connection,
                transaction);
            cmdChild.ExecuteNonQuery();
        }

        private void DeleteChildren(int id, MySqlTransaction transaction)
        {
            List<int> childDocuments = new List<int>();
            MySqlCommand cmdChild = new MySqlCommand(
                string.Format("select ID from Documents where Parent={0}", id),
                _connection,
                transaction);
            using (MySqlDataReader reader = cmdChild.ExecuteReader())
            {
                while (reader.Read())
                {
                    childDocuments.Add(reader.GetInt32(0));
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
            using (MySqlTransaction transaction = _connection.BeginTransaction())
            {
                SaveWithTransaction(value, transaction, -1, null);
                transaction.Commit();
            }
        }

        private int SaveWithTransaction(object value, MySqlTransaction transaction, int parentId, string parentPropertyName)
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
                    MySqlCommand cmd = new MySqlCommand(
                        string.Format("update Documents set Modified=curdate() where ID={0}",
                            id), _connection, transaction);
                    cmd.ExecuteNonQuery();
                }
                else
                {
                    MySqlCommand cmd = new MySqlCommand(
                        string.Format("update Documents set Modified=curdate(), Parent={1}, ParentPropertyName='{2}' where ID={0}",
                            id,
                            parentId,
                            parentPropertyName), _connection, transaction);
                    cmd.ExecuteNonQuery();
                }
            }
            else
            {
                MySqlCommand cmd = new MySqlCommand(
                    string.Format(
                        "insert into Documents (TypeName, Created, Modified, Parent, ParentPropertyName) values ('doc{0}', curdate(), curdate(), {1}, '{2}');select @@Identity;", 
                        type.FullName,
                        parentId == -1 ? "NULL" : parentId.ToString(),
                        parentPropertyName ?? "null"), 
                    _connection, 
                    transaction);
                id = (int)((long)cmd.ExecuteScalar());
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
                    MySqlCommand cmd = new MySqlCommand(sql, _connection, transaction);
                    cmd.ExecuteNonQuery();
                }

                MySqlCommand cmdD2D = new MySqlCommand(
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
                                    MySqlCommand cmdContains = new MySqlCommand(
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
                                                MySqlCommand cmdContains = new MySqlCommand(
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
                                            MySqlCommand cmdContains = new MySqlCommand(
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
                         "insert into {0} (FK_Document, Value, Name) values({1}, ?, '{2}')",
                         tableName,
                         id,
                         Helpers.PropertyName(idxProp));
                    MySqlCommand cmd = new MySqlCommand(sql, _connection, transaction);
                    cmd.Parameters.Add(new MySqlParameter("?", valueOfProperty));
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
            MySqlCommand cmdDeleteAllInfants = new MySqlCommand(
                string.Format("delete from Documents where Parent={0}{1}", id, andNotIn),
                _connection,
                transaction);
            cmdDeleteAllInfants.ExecuteNonQuery();
            return id;
        }

        public override void Close()
        {
            _connection.Close();
        }
    }
}
