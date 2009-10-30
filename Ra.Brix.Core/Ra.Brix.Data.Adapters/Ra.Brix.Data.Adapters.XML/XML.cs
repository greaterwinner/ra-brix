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
using System.Globalization;
using System.Collections;
using System.IO;
using Ra.Brix.Data.Internal;
using System.Xml;
using System.Web;
using System.Text.RegularExpressions;

/**
 * Namespace for Database Adapters in Ra-Brix
 */
namespace Ra.Brix.Data.Adapters.XML
{
    /**
     * XML data storage backend Adapter. Useful for creating applications since
     * it requires *ZERO* setup. Also useful for testing concepts and getting
     * fast into development mode. Not suitable for production environments!
     */
    public class XML : Adapter
    {
        private XmlDocument _document;
        private string _fileName;

        public override void Open(string connectionString)
        {
            _document = new XmlDocument();
            if (connectionString.Contains("~"))
            {
                if (HttpContext.Current != null)
                {
                    connectionString = HttpContext.Current.Server.MapPath(connectionString);
                }
                else
                {
                    string assemblyPath = typeof(XML).Assembly.CodeBase.Replace("file:///", "");
                    assemblyPath = assemblyPath.Substring(0, assemblyPath.LastIndexOf("/"));
                    connectionString = connectionString.Replace("~", assemblyPath);
                }
            }
            _fileName = connectionString;
            if (File.Exists(_fileName))
            {
                _document.Load(connectionString);
            }
            else
            {
                XmlElement root = _document.CreateElement("Ra.Brix");
                _document.AppendChild(root);
            }
        }

        public override void Close()
        {
            lock (typeof(XML))
            {
                _document.Save(_fileName);
            }
        }

        public override int CountWhere(Type type, params Criteria[] args)
        {
            lock (typeof(XML))
            {
                List<XmlNode> retVal = GetNodesOfType(type);
                RemoveAllFromCriteria(args, retVal);
                return retVal.Count;
            }
        }

        private void RemoveAllFromCriteria(ICollection<Criteria> args, List<XmlNode> retVal)
        {
            if (args == null || args.Count == 0)
                return;
            retVal.RemoveAll(
                delegate(XmlNode idxNode)
                {
                    bool shouldRemove = false;
                    foreach (Criteria idxCrit in args)
                    {
                        if (idxCrit is Equals)
                        {
                            XmlNodeList list = GetCriteriaValue(idxNode, idxCrit);
                            if (list.Count == 0)
                                return true;
                            string objString = GetCriteriaValueString(idxCrit);
                            string xmlString = GetCriteriaValueStringXML(idxCrit, list[0].InnerText);
                            shouldRemove = objString.ToLower() != xmlString.ToLower();
                        }
                        else if (idxCrit is NotEquals)
                        {
                            XmlNodeList list = GetCriteriaValue(idxNode, idxCrit);
                            if (list.Count == 0)
                                continue;
                            string objString = GetCriteriaValueString(idxCrit);
                            string xmlString = GetCriteriaValueStringXML(idxCrit, list[0].InnerText);
                            shouldRemove = objString == xmlString;
                        }
                        else if (idxCrit is LikeEquals)
                        {
                            XmlNodeList list = GetCriteriaValue(idxNode, idxCrit);
                            if (list.Count == 0)
                                return true;
                            string crit = "\\b" + idxCrit.Value.ToString().Replace("%", ".*").Replace("_", ".{1}");
                            Regex reg = new Regex(crit, RegexOptions.IgnoreCase);
                            shouldRemove = !reg.IsMatch(list[0].InnerText);
                        }
                        else if (idxCrit is LessThen)
                        {
                            XmlNodeList list = GetCriteriaValue(idxNode, idxCrit);
                            if (list.Count == 0)
                                return true;
                            string objString = GetCriteriaValueString(idxCrit);
                            string xmlString = GetCriteriaValueStringXML(idxCrit, list[0].InnerText);
                            shouldRemove = xmlString.CompareTo(objString) >= 0;
                        }
                        else if (idxCrit is MoreThen)
                        {
                            XmlNodeList list = GetCriteriaValue(idxNode, idxCrit);
                            if (list.Count == 0)
                                return true;
                            string objString = GetCriteriaValueString(idxCrit);
                            string xmlString = GetCriteriaValueStringXML(idxCrit, list[0].InnerText);
                            shouldRemove = xmlString.CompareTo(objString) <= 0;
                        }
                        else if (idxCrit is ParentIdEquals)
                        {
                            XmlAttribute attr = idxNode.Attributes["ParentID"];
                            if (attr == null)
                                return true;
                            shouldRemove = attr.Value != idxCrit.Value.ToString();
                        }
                        else if (idxCrit is ExistsInEquals)
                        {
                            shouldRemove = _document.SelectNodes("/Ra.Brix/Document2Document[@Document2ID='" + idxNode.Attributes["ID"].Value + "']").Count > 0;
                        }
                        if (shouldRemove)
                            return true;
                    }
                    return false;
                });
        }

        private XmlNodeList GetCriteriaValue(XmlNode idxNode, Criteria idxCrit)
        {
            if (idxCrit.PropertyName.Contains("."))
            {
                string parentPropertyName = idxCrit.PropertyName.Split('.')[0];
                string propertyName = idxCrit.PropertyName.Split('.')[1];

                return _document.SelectNodes(
                    string.Format(
                        "/Ra.Brix/Document[@ParentPropertyName='{0}' and @ParentID='{1}']/{2}",
                        parentPropertyName,
                        idxNode.Attributes[1].Value,
                        propertyName));
            }
            XmlNodeList list = idxNode.SelectNodes(idxCrit.PropertyName);
            return list;
        }

        private static string GetCriteriaValueStringXML(Criteria idxCrit, string xmlValue)
        {
            string objString = "";
            switch (idxCrit.Value.GetType().FullName)
            {
                case "System.Int32":
                    objString = int.Parse(xmlValue, CultureInfo.InvariantCulture).ToString("0000000000000000", CultureInfo.InvariantCulture);
                    break;
                case "System.Boolean":
                    objString = bool.Parse(xmlValue) ? "1" : "0";
                    break;
                case "System.Decimal":
                    objString = decimal.Parse(xmlValue, CultureInfo.InvariantCulture).ToString("0000000000000000.################", CultureInfo.InvariantCulture);
                    break;
                case "System.DateTime":
                    objString = xmlValue;
                    break;
                case "System.String":
                    objString = xmlValue.ToLower();
                    break;
            }
            return objString;
        }

        private static string GetCriteriaValueString(Criteria idxCrit)
        {
            string objString = "";
            switch (idxCrit.Value.GetType().FullName)
            {
                case "System.Int32":
                    objString = ((int)idxCrit.Value).ToString("0000000000000000", CultureInfo.InvariantCulture);
                    break;
                case "System.Boolean":
                    objString = ((bool)idxCrit.Value) ? "1" : "0";
                    break;
                case "System.String":
                    objString = idxCrit.Value.ToString().ToLower();
                    break;
                case "System.Decimal":
                    objString = ((decimal)idxCrit.Value).ToString("0000000000000000.################", CultureInfo.InvariantCulture);
                    break;
                case "System.DateTime":
                    objString = ((DateTime)idxCrit.Value).ToString("yyyy.MM.dd HH:mm:ss", CultureInfo.InvariantCulture);
                    break;
            }
            return objString;
        }

        private List<XmlNode> GetNodesOfType(Type type)
        {
            List<XmlNode> retVal = new List<XmlNode>();
            foreach (XmlNode idx in _document.SelectNodes("/Ra.Brix/Document"))
            {
                if (type != null)
                {
                    if (idx.Attributes["TypeName"].Value != type.FullName)
                        continue;
                }
                retVal.Add(idx);
            }
            return retVal;
        }

        protected override object SelectObjectByID(Type type, int id)
        {
            lock (typeof(XML))
            {
                List<XmlNode> retVal = GetNodesOfType(type);
                foreach (XmlNode idxNode in retVal)
                {
                    if (idxNode.Attributes["ID"].Value == id.ToString())
                    {
                        return BuildObjectFromXmlNode(type, idxNode);
                    }
                }
                return null;
            }
        }

        private object BuildObjectFromXmlNode(Type type, XmlNode idxNode)
        {
            if (type == null)
            {
                string typeName = idxNode.Attributes["TypeName"].Value;
                foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
                {
                    foreach (Type idxType in assembly.GetTypes())
                    {
                        if (idxType.FullName == typeName)
                        {
                            type = idxType;
                            break;
                        }
                    }
                }
            }
            if (type == null)
            {
                return null;
            }
            object retVal = type.GetConstructor(
                BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance, 
                null, 
                new Type[] { }, 
                null).Invoke(null);
            int idOfObject = int.Parse(idxNode.Attributes["ID"].Value);
            type.GetProperty("ID").GetSetMethod(true).Invoke(retVal, new object[] { idOfObject });
            foreach (PropertyInfo idxProp in type.GetProperties(
                BindingFlags.Instance | 
                BindingFlags.NonPublic | 
                BindingFlags.Public))
            {
                ActiveFieldAttribute[] attrs = 
                    idxProp.GetCustomAttributes(typeof(ActiveFieldAttribute), true) as ActiveFieldAttribute[];
                if (attrs != null && attrs.Length > 0)
                {
                    switch (idxProp.PropertyType.FullName)
                    {
                        case "System.Int32":
                        case "System.Boolean":
                            {
                                XmlNode contentNode = idxNode.SelectSingleNode(idxProp.Name);
                                if (contentNode != null)
                                {
                                    if (contentNode.FirstChild != null)
                                    {
                                        idxProp.GetSetMethod(true).Invoke(retVal,
                                            new[] { 
                                    Convert.ChangeType(contentNode.FirstChild.Value, idxProp.PropertyType) });
                                    }
                                }
                            } break;
                        case "System.String":
                            {
                                XmlNode contentNode = idxNode.SelectSingleNode(idxProp.Name);
                                if (contentNode != null)
                                {
                                    if (contentNode.FirstChild != null)
                                    {
                                        idxProp.GetSetMethod(true).Invoke(retVal,
                                            new[] { 
                                            Convert.ChangeType(contentNode.FirstChild.Value, idxProp.PropertyType) });
                                    }
                                    else
                                    {
                                        idxProp.GetSetMethod(true).Invoke(retVal,
                                            new[] { 
                                            Convert.ChangeType("", idxProp.PropertyType) });
                                    }
                                }
                            } break;
                        case "System.Decimal":
                            {
                                XmlNode contentNode = idxNode.SelectSingleNode(idxProp.Name);
                                if (contentNode != null)
                                {
                                    if (contentNode.FirstChild != null)
                                    {
                                        idxProp.GetSetMethod(true).Invoke(retVal,
                                            new[] { 
                                                Convert.ChangeType(contentNode.FirstChild.Value, idxProp.PropertyType, CultureInfo.InvariantCulture) });
                                    }
                                }
                            } break;
                        case "System.DateTime":
                            {
                                XmlNode contentNode = idxNode.SelectSingleNode(idxProp.Name);
                                if (contentNode != null)
                                {
                                    if (contentNode.FirstChild != null)
                                    {
                                        idxProp.GetSetMethod(true).Invoke(retVal,
                                            new object[] { 
                                                DateTime.ParseExact(contentNode.FirstChild.Value, "yyyy.MM.dd HH:mm:ss", CultureInfo.InvariantCulture) });
                                    }
                                }
                            } break;
                        case "System.Byte[]":
                            {
                                XmlNode contentNode = idxNode.SelectSingleNode(idxProp.Name);
                                if (contentNode != null)
                                {
                                    if (contentNode.FirstChild != null)
                                    {
                                        byte[] bytes = Convert.FromBase64String(contentNode.FirstChild.Value);
                                        idxProp.GetSetMethod(true).Invoke(retVal,
                                            new[] { 
                                    Convert.ChangeType(bytes, idxProp.PropertyType) });
                                    }
                                }
                            } break;
                        default:
                            // Complex type...!
                            if (idxProp.PropertyType.FullName.IndexOf("Ra.Brix.Types.LazyList") == 0 ||
                                idxProp.PropertyType.FullName.IndexOf("System.Collections.Generic.List") == 0)
                            {
                                // List of children...
                                Type genericType = idxProp.PropertyType.GetGenericArguments()[0];
                                List<XmlNode> children = GetNodesOfType(genericType);
                                if (attrs[0].IsOwner)
                                {
                                    PropertyInfo info = idxProp;
                                    children.RemoveAll(
                                        delegate(XmlNode idxChild)
                                        {
                                            return idxChild.Attributes["ParentID"] == null ||
                                                idxChild.Attributes["ParentPropertyName"] == null ||
                                                (idxChild.Attributes["ParentID"].Value != idOfObject.ToString() ||
                                                    idxChild.Attributes["ParentPropertyName"].Value != info.Name);
                                        });
                                }
                                else
                                {
                                    PropertyInfo info = idxProp;
                                    children.RemoveAll(
                                        delegate(XmlNode idxChild)
                                        {
                                            string xPath = "/Ra.Brix/Document2Document[@Document1ID='" + idOfObject +
                                                "' and @Document2ID='" + idxChild.Attributes["ID"].Value + 
                                                "' and @PropertyName='" + info.Name + "']";
                                            return _document.SelectNodes(xPath).Count == 0;
                                        });
                                }
                                if (children.Count > 0)
                                {
                                    MethodInfo addMethod = idxProp.PropertyType.GetMethod("Add");
                                    object listObject = idxProp.GetGetMethod(true).Invoke(retVal, null);
                                    foreach (XmlNode idxChildNode in children)
                                    {
                                        object tmp = BuildObjectFromXmlNode(genericType, idxChildNode);
                                        addMethod.Invoke(listObject, new[] { tmp });
                                    }
                                }
                            }
                            else
                            {
                                List<XmlNode> children = GetNodesOfType(idxProp.PropertyType);
                                if (attrs[0].IsOwner)
                                {
                                    PropertyInfo info = idxProp;
                                    children.RemoveAll(
                                        delegate(XmlNode idxChild)
                                        {
                                            return idxChild.Attributes["ParentID"] == null || 
                                                idxChild.Attributes["ParentPropertyName"] == null || 
                                                (idxChild.Attributes["ParentID"].Value != idOfObject.ToString() || 
                                                    idxChild.Attributes["ParentPropertyName"].Value != info.Name);
                                        });
                                }
                                else
                                {
                                    PropertyInfo info = idxProp;
                                    children.RemoveAll(
                                        delegate(XmlNode idxChild)
                                        {
                                            string xPath = "/Ra.Brix/Document2Document[@Document1ID='" + idOfObject +
                                                "' and @Document2ID='" + idxChild.Attributes["ID"].Value + 
                                                "' and @PropertyName='" + info.Name + "']";
                                            return _document.SelectNodes(xPath).Count == 0;
                                        });
                                }
                                if (children.Count > 0)
                                {
                                    object valueOfChildProperty = BuildObjectFromXmlNode(idxProp.PropertyType, children[0]);
                                    idxProp.GetSetMethod(true).Invoke(retVal, new[] { valueOfChildProperty });
                                }
                            }
                            break;
                    }
                }
            }
            return retVal;
        }

        public override object SelectFirst(Type type, string propertyName, params Criteria[] args)
        {
            lock (typeof(XML))
            {
                List<XmlNode> retVal = GetNodesOfType(type);
                RemoveAllFromCriteria(args, retVal);
                if (retVal.Count == 0)
                    return null;
                return BuildObjectFromXmlNode(type, retVal[0]);
            }
        }

        public override IEnumerable<object> Select(Type type, string propertyName, params Criteria[] args)
        {
            lock (typeof(XML))
            {
                List<XmlNode> retVal = GetNodesOfType(type);
                RemoveAllFromCriteria(args, retVal);
                foreach (XmlNode idx in retVal)
                {
                    yield return BuildObjectFromXmlNode(type, idx);
                }
            }
        }

        public override IEnumerable<object> Select()
        {
            return Select(null, null);
        }

        protected override void DeleteObject(int id)
        {
            lock (typeof(XML))
            {
                Delete(id, true);
            }
        }

        public static string GetXML()
        {
            return ((XML)Instance)._document.OuterXml;
        }

        protected void Delete(int id, bool recursive)
        {
            XmlNode obj = _document.SelectSingleNode("/Ra.Brix/Document[@ID='" + id + "']");
            if (obj == null)
                return;
            _document.FirstChild.RemoveChild(obj);
            foreach (XmlNode idx in _document.SelectNodes("/Ra.Brix/Document2Document[@Document1ID='" + id + "']"))
            {
                _document.FirstChild.RemoveChild(idx);
            }
            if (recursive)
            {
                    foreach (XmlNode idx in _document.SelectNodes("/Ra.Brix/Document2Document[@Document2ID='" + id + "']"))
                {
                    _document.FirstChild.RemoveChild(idx);
                }
                foreach (XmlNode idx in _document.SelectNodes("/Ra.Brix/Document[@ParentID='" + id + "']"))
                {
                    DeleteObject(int.Parse(idx.Attributes["ID"].Value));
                }
            }
        }

        public override void Save(object value)
        {
            lock (typeof(XML))
            {
                Type type = value.GetType();
                int idOfElement = (int)type.GetProperty("ID").GetGetMethod(true).Invoke(value, null);
                int parentId = -1;
                string parentPropertyName = null;
                if (idOfElement != 0)
                {
                    // Need to fetch previous document to see if it has a ParentID
                    XmlNode previous = _document.SelectSingleNode("/Ra.Brix/Document[@ID='" + idOfElement + "']");
                    XmlAttribute parentIdAttr = previous.Attributes["ParentID"];
                    if (parentIdAttr != null && parentIdAttr.Value != null)
                    {
                        parentId = int.Parse(parentIdAttr.Value);
                        XmlAttribute parentPropertyTypeEl = previous.Attributes["ParentPropertyName"];
                        parentPropertyName = parentPropertyTypeEl.Value;
                    }
                }

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
                            if (attrs != null && attrs.Length > 0)
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
                Save(value, parentId, parentPropertyName);
            }
        }

        private int Save(object value, int parentId, string parentPropertyName)
        {
            Type type = value.GetType();

            // Delete old XML nodes
            Delete((int)type.GetProperty("ID").GetGetMethod(true).Invoke(value, null), false);

            // Creating document XML node
            XmlNode objNode = _document.CreateElement("Document");
            _document.FirstChild.AppendChild(objNode);

            // Creating TypeName
            XmlAttribute docType = _document.CreateAttribute("TypeName");
            docType.Value = type.FullName;
            objNode.Attributes.Append(docType);

            // Creating parentPropertyName, but only if we SHOULD
            if (parentPropertyName != null)
            {
                XmlAttribute parentPropEl = _document.CreateAttribute("ParentPropertyName");
                parentPropEl.Value = parentPropertyName;
                objNode.Attributes.Append(parentPropEl);
            }

            // Creating ID
            XmlAttribute id = _document.CreateAttribute("ID");
            int idOfElement = (int)type.GetProperty("ID").GetGetMethod(true).Invoke(value, null);
            if (idOfElement == 0)
            {
                //idOfElement = int.Parse(_document.SelectSingleNode("/Ra.Brix/Document/[not(../Document/@ID > @ID)]/@ID").Value);
                idOfElement = _document.SelectNodes("/Ra.Brix/Document").Count;
                type.GetProperty("ID").GetSetMethod(true).Invoke(value, new object[] { idOfElement });
            }
            id.Value = idOfElement.ToString();
            objNode.Attributes.Append(id);

            // Creating ParentID
            if (parentId != -1)
            {
                XmlAttribute parentIdNode = _document.CreateAttribute("ParentID");
                parentIdNode.Value = parentId.ToString();
                objNode.Attributes.Append(parentIdNode);
            }

            List<int> listOfIDsOfChildren = new List<int>();

            foreach (PropertyInfo idxProp in type.GetProperties(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public))
            {
                ActiveFieldAttribute[] attrs = idxProp.GetCustomAttributes(typeof(ActiveFieldAttribute), true) as ActiveFieldAttribute[];
                if (attrs != null && attrs.Length > 0)
                {
                    object propValue = idxProp.GetGetMethod(true).Invoke(value, null);
                    if (propValue != null)
                    {
                        switch (propValue.GetType().FullName)
                        {
                            case "System.Int32":
                            case "System.Boolean":
                            case "System.String":
                                {
                                    XmlNode contentNode = _document.CreateElement(idxProp.Name);
                                    XmlNode contentValueTextNode = _document.CreateTextNode(propValue.ToString());
                                    contentNode.AppendChild(contentValueTextNode);
                                    objNode.AppendChild(contentNode);
                                } break;
                            case "System.Decimal":
                                {
                                    XmlNode contentNode = _document.CreateElement(idxProp.Name);
                                    XmlNode contentValueTextNode = _document.CreateTextNode(((decimal)propValue).ToString(CultureInfo.InvariantCulture));
                                    contentNode.AppendChild(contentValueTextNode);
                                    objNode.AppendChild(contentNode);
                                } break;
                            case "System.DateTime":
                                {
                                    XmlNode contentNode = _document.CreateElement(idxProp.Name);
                                    XmlNode contentValueTextNode = _document.CreateTextNode(((DateTime)propValue).ToString("yyyy.MM.dd HH:mm:ss", CultureInfo.InvariantCulture));
                                    contentNode.AppendChild(contentValueTextNode);
                                    objNode.AppendChild(contentNode);
                                } break;
                            case "System.Byte[]":
                                {
                                    XmlNode contentNode = _document.CreateElement(idxProp.Name);
                                    string byteStrContent = Convert.ToBase64String((byte[])propValue);
                                    XmlNode contentValueTextNode = _document.CreateTextNode(byteStrContent);
                                    contentNode.AppendChild(contentValueTextNode);
                                    objNode.AppendChild(contentNode);
                                } break;
                            default:
                                if (attrs[0].IsOwner)
                                {
                                    IEnumerable enumerable = propValue as IEnumerable;
                                    if (enumerable == null)
                                    {
                                        listOfIDsOfChildren.Add(Save(propValue, idOfElement, idxProp.Name));
                                    }
                                    else
                                    {
                                        foreach (object idxChild in enumerable)
                                        {
                                            listOfIDsOfChildren.Add(Save(idxChild, idOfElement, idxProp.Name));
                                        }
                                    }
                                }
                                else
                                {
                                    // Not the owner of the object
                                    // Need to insert a Document2Document XML node
                                    IEnumerable enumerable = propValue as IEnumerable;
                                    if (enumerable == null)
                                    {
                                        // Saving object
                                        int newDocId = (int)propValue.GetType().GetProperty("ID").GetGetMethod().Invoke(propValue, null);

                                        // Inserting a many2many relationship.
                                        XmlNode doc2Doc = _document.CreateElement("Document2Document");

                                        XmlAttribute doc1IDNode = _document.CreateAttribute("Document1ID");
                                        doc1IDNode.Value = idOfElement.ToString();
                                        doc2Doc.Attributes.Append(doc1IDNode);

                                        XmlAttribute doc2IDNode = _document.CreateAttribute("Document2ID");
                                        doc2IDNode.Value = newDocId.ToString();
                                        doc2Doc.Attributes.Append(doc2IDNode);

                                        XmlAttribute docTypeNode = _document.CreateAttribute("PropertyName");
                                        docTypeNode.Value = idxProp.Name;
                                        doc2Doc.Attributes.Append(docTypeNode);

                                        _document.FirstChild.AppendChild(doc2Doc);
                                    }
                                    else
                                    {
                                        foreach (object idxChild in enumerable)
                                        {
                                            int newDocId = (int)idxChild.GetType().GetProperty("ID").GetGetMethod().Invoke(idxChild, null);

                                            // Inserting a many2many relationship.
                                            XmlNode doc2Doc = _document.CreateElement("Document2Document");

                                            XmlAttribute doc1IDNode = _document.CreateAttribute("Document1ID");
                                            doc1IDNode.Value = idOfElement.ToString();
                                            doc2Doc.Attributes.Append(doc1IDNode);

                                            XmlAttribute doc2IDNode = _document.CreateAttribute("Document2ID");
                                            doc2IDNode.Value = newDocId.ToString();
                                            doc2Doc.Attributes.Append(doc2IDNode);

                                            XmlAttribute docTypeNode = _document.CreateAttribute("PropertyName");
                                            docTypeNode.Value = idxProp.Name;
                                            doc2Doc.Attributes.Append(docTypeNode);

                                            _document.FirstChild.AppendChild(doc2Doc);
                                        }
                                    }
                                }
                                break;
                        }
                    }
                }
            }
            List<XmlNode> nodesToDelete = new List<XmlNode>();
            foreach (XmlNode idxNode in _document.SelectNodes("/Ra.Brix/Document[@ParentID=" + idOfElement + "]"))
            {
                bool shouldDelete = true;
                int idOfChild = int.Parse(idxNode.Attributes["ID"].Value);
                if (listOfIDsOfChildren.Contains(idOfChild))
                {
                    shouldDelete = false;
                }
                if (shouldDelete)
                    nodesToDelete.Add(idxNode);
            }
            foreach (XmlNode idxToDelete in nodesToDelete)
            {
                _document.DocumentElement.RemoveChild(idxToDelete);
            }
            return idOfElement;
        }
    }
}
