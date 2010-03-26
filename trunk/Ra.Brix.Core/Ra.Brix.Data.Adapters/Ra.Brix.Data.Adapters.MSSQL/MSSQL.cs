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
using System.Data.SqlClient;
using System.Reflection;
using Ra.Brix.Types;
using System.Collections;
using System.IO;
using Ra.Brix.Data.Internal;
using System.Text;

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
    public class MSSQL : StdSQLDataAdapter, IPersistViewState
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
            SqlCommand cmd = new SqlCommand("select count(*) from dbo.Documents as d" + where, _connection);
            return (int)cmd.ExecuteScalar();
        }

        protected override object SelectObjectByID(Type type, int id)
        {
            // Checking to see if object exists...
            SqlCommand cmdExists = new SqlCommand(
                string.Format("select count(*) from dbo.Documents where ID={0}", id),
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
                            tableName = "dbo.PropertyBools";
                            break;
                        case "System.DateTime":
                            tableName = "dbo.PropertyDates";
                            break;
                        case "System.Decimal":
                            tableName = "dbo.PropertyDecimals";
                            break;
                        case "System.Int32":
                            tableName = "dbo.PropertyInts";
                            break;
                        case "System.String":
                            tableName = "dbo.PropertyStrings";
                            break;
                        case "System.Byte[]":
                            tableName = "dbo.PropertyBLOBS";
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
            string where = CreateSelectStatementForDocument(type, propertyName, args);
            SqlCommand cmd = new SqlCommand(where, _connection);
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
            string where = CreateSelectStatementForDocument(type, propertyName, args);
            SqlCommand cmd = new SqlCommand(where, _connection);
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
            SqlCommand cmd = new SqlCommand("select ID, TypeName from dbo.Documents as d", _connection);
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
                string.Format("delete from dbo.Documents where ID=" + id),
                _connection,
                transaction);
            cmd.ExecuteNonQuery();
        }

        private void DeleteComposition(int id, SqlTransaction transaction)
        {
            SqlCommand cmdChild = new SqlCommand(
                string.Format("delete from dbo.Documents2Documents where Document1ID={0} or Document2ID={0}", id),
                _connection,
                transaction);
            cmdChild.ExecuteNonQuery();
        }

        private void DeleteChildren(int id, SqlTransaction transaction)
        {
            List<int> childDocuments = new List<int>();
            SqlCommand cmdChild = new SqlCommand(
                string.Format("select ID from dbo.Documents where Parent={0}", id),
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
                    // We do NOT want to tamper with the parent parts of the object unless
                    // we are given a valid parentId explicitly since it might be saving of
                    // a child that belongs to another object in the first place...
                    SqlCommand cmd = new SqlCommand(
                        string.Format("update dbo.Documents set Modified=getdate() where ID={0}",
                            id), _connection, transaction);
                    cmd.ExecuteNonQuery();
                }
                else
                {
                    SqlCommand cmd = new SqlCommand(
                        string.Format("update dbo.Documents set Modified=getdate(), Parent={1}, ParentPropertyName='{2}' where ID={0}",
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
                        "insert into dbo.Documents (TypeName, Created, Modified, Parent, ParentPropertyName) values ('doc{0}', getdate(), getdate(), {1}, {2});select @@Identity;", 
                        type.FullName,
                        parentId == -1 ? "NULL" : parentId.ToString(),
                        parentPropertyName == null ? "null" : "'" + parentPropertyName + "'", _connection, transaction), 
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

                                    SqlCommand deleteFromD2D = new SqlCommand(
                                        string.Format("delete from dbo.Documents2Documents where Document1ID={0} and PropertyName='{1}'",
                                            id,
                                            idxProp.Name), 
                                        _connection, 
                                        transaction);
                                    deleteFromD2D.ExecuteNonQuery();

                                    SqlCommand cmdContains = new SqlCommand(
                                        string.Format(
                                            "insert into dbo.Documents2Documents (Document1ID, Document2ID, PropertyName) values ({0}, {1}, '{2}')",
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
                                            foreach (object idxChild in enumerable)
                                            {
                                                int childId = SaveWithTransaction(idxChild, transaction, id, idxProp.Name);
                                                listOfIDsOfChildren.Add(childId);
                                            }
                                        }
                                        else
                                        {
                                            // Delete old relationships whith this documentid and this property name
                                            SqlCommand sqlDeleteRelationRecords = new SqlCommand(
                                                string.Format("delete from dbo.Documents2Documents where Document1ID={0} and PropertyName='{1}'",
                                                    id,
                                                    idxProp.Name), _connection, transaction);
                                            sqlDeleteRelationRecords.ExecuteNonQuery();

                                            foreach (object idxChild in enumerable)
                                            {
                                                // TODO: Should we really save the related document here...?
                                                // Or should we only save the releationship...?
                                                // If we only save the relationship, we might get
                                                // "dangling pointers", and if we save everything
                                                // we overspend resources, plus that we do save something
                                                // which is only linked, which might feel unintuitive...?
                                                int documentId = SaveWithTransaction(idxChild, transaction, -1, idxProp.Name);

                                                SqlCommand cmdContains = new SqlCommand(
                                                    string.Format(
                                                        "insert into dbo.Documents2Documents (Document1ID, Document2ID, PropertyName) values ({0}, {1}, '{2}')",
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
                                        foreach (object idxChild in enumerable)
                                        {
                                            int childId = SaveWithTransaction(idxChild, transaction, id, idxProp.Name);
                                            listOfIDsOfChildren.Add(childId);
                                        }
                                    }
                                    else
                                    {
                                        // Delete old relationships whith this documentid and this property name
                                        SqlCommand sqlDeleteRelationRecords = new SqlCommand(
                                            string.Format("delete from dbo.Documents2Documents where Document1ID={0} and PropertyName='{1}'",
                                                id,
                                                idxProp.Name), _connection, transaction);
                                        sqlDeleteRelationRecords.ExecuteNonQuery();

                                        foreach (object idxChild in enumerable)
                                        {
                                            // TODO: Should we really save the related document here...?
                                            // Or should we only save the releationship...?
                                            // If we only save the relationship, we might get
                                            // "dangling pointers", and if we save everything
                                            // we overspend resources, plus that we do save something
                                            // which is only linked, which might feel unintuitive...?
                                            int documentId = SaveWithTransaction(idxChild, transaction, -1, idxProp.Name);

                                            SqlCommand cmdContains = new SqlCommand(
                                                string.Format(
                                                    "insert into dbo.Documents2Documents (Document1ID, Document2ID, PropertyName) values ({0}, {1}, '{2}')",
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
                string.Format("select ID from dbo.Documents where Parent={0}{1}", id, andNotIn),
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

        public void Save(string sessionId, string pageUrl, string content)
        {
            // Deleting *OLD* ViewState from table...
            string key = sessionId + "|" + pageUrl;
            SqlCommand sql = new SqlCommand("delete from dbo.ViewStateStorage where ID='" + key + "'", _connection);
            sql.ExecuteNonQuery();

            // Saving new value
            sql = new SqlCommand(
                "insert into dbo.ViewStateStorage (ID, Content, Created) values (@id, @content, @created)",
                _connection);
            sql.Parameters.Add(new SqlParameter("@id", key));
            sql.Parameters.Add(new SqlParameter("@content", content));
            sql.Parameters.Add(new SqlParameter("@created", DateTime.Now));
            sql.ExecuteNonQuery();
        }

        public string Load(string sessionId, string pageUrl)
        {
            // Retrieving ViewState
            string key = sessionId + "|" + pageUrl;
            SqlCommand sql = new SqlCommand("select content from dbo.ViewStateStorage where ID=@id", _connection);
            sql.Parameters.Add(new SqlParameter("@id", key));
            string retVal = sql.ExecuteScalar() as string;
            return retVal;
        }
    }
}
