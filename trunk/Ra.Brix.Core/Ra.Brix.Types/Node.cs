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
using System.Collections;
using System.Text;

namespace Ra.Brix.Types
{
    /**
     * Helper class to pass around data in a "JSON kind of way" without having
     * to convert to JSON strings. Create a nw instance, and just start appending
     * items to it like this;
     * <pre>
     *    Node n = new Node();
     *    n["Customer"]["Name"] = "John Doe";
     *    n["Customer"]["Adr"] = "NY";
     * </pre>
     * In addition this class uses a concept called "Genetic Trees" which
     * can be seen at http://geocities.com/polterguy1000 - Use the DNA property
     * to retrieve a string representing a DNA code. Later use that DNA code to
     * retrieve the specific Node by using the "Find" method.
     */
    [Serializable]
    public class Node : IList<Node>
    {
        // Implementation of list
        private readonly List<Node> _children = new List<Node>();

        // Parent node
        private Node _parent;

        // DNA code size when formatted to string
        private readonly int _dnaSize;

        private string _name;
        private object _value;

        /**
         * Default CTOR, creates a new node with no name and no value and no children
         */
        public Node()
            : this(null)
        { }

        /**
         * Creates a new node with the given name
         */
        public Node(string name)
            : this(name, null)
        { }

        /**
         * Creates a new node with the given name and the given value
         */
        public Node(string name, object value)
            : this(name, value, null)
        { }

        public static Node FromJSONString(string json)
        {
            List<string> tokens = ExtractTokens(json);
            if (tokens.Count == 0)
                throw new ArgumentException("JSON string was empty");
            if (tokens[0] != "{")
                throw new ArgumentException("JSON string wasn't an object, missing opening token at character 0");
            int idxToken = 1;
            Node retVal = new Node();
            while (tokens[idxToken] != "}")
            {
                ParseToken(tokens, ref idxToken, retVal);
            }
            return retVal;
        }

        private static void ParseToken(IList<string> tokens, ref int idxToken, Node node)
        {
            if (tokens[idxToken] == "{")
            {
                // Opening new object...
                Node next = new Node();
                node._children.Add(next);
                next._parent = node;
                idxToken += 1;
                while (tokens[idxToken] != "}")
                {
                    ParseToken(tokens, ref idxToken, next);
                }
            }
            else if (tokens[idxToken] == ":")
            {
                switch(tokens[idxToken - 1])
                {
                    case "\"Name\"":
                        node.Name = tokens[idxToken + 1].Replace("\\\"", "\"").Trim('"');
                        break;
                    case "\"Value\"":
                        node.Value = tokens[idxToken + 1].Replace("\\\"", "\"").Trim('"');
                        break;
                    case "\"Children\"":
                        idxToken += 1;
                        while (tokens[idxToken] != "]")
                        {
                            ParseToken(tokens, ref idxToken, node);
                        }
                        break;
                }
            }
            idxToken += 1;
            return;
        }

        private static List<string> ExtractTokens(string json)
        {
            List<string> tokens = new List<string>();
            for (int idx = 0; idx < json.Length; idx++)
            {
                switch (json[idx])
                {
                    case '{':
                    case '}':
                    case ':':
                    case ',':
                    case '[':
                    case ']':
                        tokens.Add(new string(json[idx], 1));
                        break;
                    case '"':
                        {
                            string str = "\"";
                            idx += 1;
                            while (true)
                            {
                                if (json[idx] == '"' && str.Substring(str.Length - 1, 1) != "\\")
                                {
                                    break;
                                }
                                str += json[idx];
                                idx += 1;
                            }
                            str += "\"";
                            tokens.Add(str);
                        } break;
                    case ' ':
                    case '\t':
                    case '\r':
                    case '\n':
                        break;
                    default:
                        throw new ArgumentException(
                            string.Format(
                                "Illegal token found in JSON string at character {0}, token was {1}, string was; \"{2}\"",
                                idx,
                                json[idx],
                                json.Substring(Math.Max(0, idx - 5))));
                }
            }
            return tokens;
        }

        /**
         * Creates a new node with the given name and the given value. In addition
         * it sets the "DNA resolution" to the given size. The DNA resolution
         * basically defines how wide (ZERO paddings) the DNA code will be
         * represented by when retrieved. This property is inherited to child nodes
         * such that always the outer most rooted value will be used and override
         * and child node's DNA sizes.
         */
        public Node(string name, object value, int dnaSize)
            : this(name, value, null)
        {
            _dnaSize = dnaSize;
        }

        private Node(string name, object value, Node parent)
        {
            _name = name;
            _value = value;
            _parent = parent;
            _dnaSize = 3;
        }

        /**
         * Returns the Parent node of the current node in the hierarchy
         */
        public Node Parent
        {
            get { return _parent; }
        }

        /**
         * Returns the name of the node
         */
        public string Name
        {
            get { return _name; }
            private set { _name = value; }
        }

        /**
         * Returns the value of the object
         */
        public object Value
        {
            get { return _value; }
            set { _value = value; }
        }

        /**
         * Returns the value of the object to type of T
         */
        public T Get<T>()
        {
            return _value == null ? default(T) : (T)_value;
        }

        /**
         * Returns the value of the object to type of T, and if
         * object cannot for some reason be converted will return 
         * the "default" value...
         */
        public T Get<T>(T defaultValue)
        {
            if (_value == null)
                return defaultValue;
            return (T)_value;
        }

        /**
         * Returns the DNA code for the object, can later be used to retrieve 
         * the specific object owning the DNA code by using the "Find" method.
         */
        public string DNA
        {
            get
            {
                Node index = this;
                List<int> retVal = new List<int>();
                int dnaSize = 0;
                while (index != null)
                {
                    if (index._parent == null)
                    {
                        dnaSize = index._dnaSize;
                        break;
                    }
                    retVal.Insert(0, index._parent.IndexOf(index));
                    index = index._parent;
                }
                string actualRetVal = "";
                foreach (int idx in retVal)
                {
                    if (actualRetVal.Length != 0)
                    {
                        actualRetVal += "-";
                    }
                    actualRetVal += idx.ToString().PadLeft(dnaSize, '0');
                }
                return actualRetVal;
            }
        }

        /**
         * If given a DNA code previously retrieved from the DNA property will
         * return that same node.
         */
        public Node Find(string dna)
        {
            string[] dnaEntities = dna.Split('-');
            int idxChildNo = int.Parse(dnaEntities[0]);
            if (idxChildNo >= Count)
                return null;
            Node child = this[idxChildNo];
            if (dna.IndexOf("-") == -1)
                return child;
            return child.Find(dna.Substring(dna.IndexOf('-') + 1));
        }

        /**
         * Returns the first node that matches the given Predicate
         */
        public Node Find(Predicate<Node> functor)
        {
            if (functor(this))
                return this;
            foreach (Node idx in this)
            {
                if (functor(idx))
                    return idx;
                Node tmp = idx.Find(functor);
                if (tmp != null)
                    return tmp;
            }
            return null;
        }

        /**
         * Returns true if node exists in children collection
         */
        public bool Exists(Predicate<Node> functor)
        {
            return Exists(functor, false);
        }

        /**
         * Returns true if node exists in children collection
         */
        public bool Exists(Predicate<Node> functor, bool flat)
        {
            if (!flat && functor(this))
                return true;
            foreach (Node idx in this._children)
            {
                if (functor(idx))
                    return true;
                if (flat)
                    continue;
                bool tmp = idx.Exists(functor);
                if (tmp)
                    return true;
            }
            return false;
        }

        /**
         * Will "disconnect" the node from its parent node. Useful for parsing subtrees
         * where you're dependant upon the DNA code or something...
         */
        public Node UnTie()
        {
            _parent.Remove(this);
            _parent = null;
            return this;
        }

        /**
         * Returns the node with the given Name. If that node doesn't exist
         * a new node will be created with the given name and appended into the
         * Children collection and then be returned.
         */
        public Node this[string name]
        {
            get
            {
                Node retVal = _children.Find(
                    delegate(Node idx)
                    {
                        return idx.Name == name;
                    });
                if (retVal == null)
                {
                    retVal = new Node(name, null, this);
                    _children.Add(retVal);
                }
                return retVal;
            }
            set
            {
                int idxNo = -1;
                if (_children.Exists(
                    delegate(Node idx)
                    {
                        idxNo += 1;
                        return idx.Name == name;
                    }))
                {
                    _children[idxNo] = value;
                }
                else
                    _children.Add(value);
            }
        }

        public Node this[string name, bool forceCreation]
        {
            get
            {
                if (forceCreation)
                    return this[name];
                Node retVal = _children.Find(
                    delegate(Node idx)
                    {
                        return idx.Name == name;
                    });
                return retVal;
            }
        }

        /*
         * IList members
         */
        public int IndexOf(Node item)
        {
            return _children.IndexOf(item);
        }

        public void Insert(int index, Node item)
        {
            _children.Insert(index, item);
            item._parent = this;
        }

        public void RemoveAt(int index)
        {
            _children[index]._parent = null;
            _children.RemoveAt(index);
        }

        public Node this[int index]
        {
            get
            {
                return _children[index];
            }
            set
            {
                _children[index] = value;
                value._parent = this;
            }
        }

        public void Add(Node item)
        {
            _children.Add(item);
            item._parent = this;
        }

        public void AddRange(IEnumerable<Node> items)
        {
            foreach (Node idx in items)
            {
                Add(idx);
            }
        }

        public void Clear()
        {
            foreach (Node idx in _children)
            {
                idx._parent = null;
            }
            _children.Clear();
        }

        public bool Contains(Node item)
        {
            return _children.Contains(item);
        }

        public void CopyTo(Node[] array, int arrayIndex)
        {
            foreach (Node idx in _children)
            {
                idx._parent = null;
            }
            _children.CopyTo(array, arrayIndex);
        }

        public int Count
        {
            get { return _children.Count; }
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

        public bool Remove(Node item)
        {
            bool retVal = _children.Remove(item);
            if (retVal)
                item._parent = null;
            return retVal;
        }

        public IEnumerator<Node> GetEnumerator()
        {
            return _children.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _children.GetEnumerator();
        }

        public override string ToString()
        {
            string retVal = "";
            retVal += _name;
            if (_value != null)
                retVal += ":" + _value;
            if (_children != null)
                retVal += ":" + _children.Count;
            return retVal;
        }

        public string ToJSONString()
        {
            StringBuilder builder = new StringBuilder();
            builder.Append("{");
            bool hasChild = false;
            if(!string.IsNullOrEmpty(Name))
            {
                builder.AppendFormat(@"""Name"":""{0}""", Name);
                hasChild = true;
            }
            if(Value != null)
            {
                if (hasChild)
                    builder.Append(",");
                string value = "";
                switch(Value.GetType().FullName)
                {
                    case "System.String":
                        value = Value.ToString().Replace("\"", "\\\"");
                        break;
                    default:
                        value = Value.ToString();
                        break;
                }
                builder.AppendFormat(@"""Value"":""{0}""", value);
                hasChild = true;
            }
            if(_children.Count > 0)
            {
                if (hasChild)
                    builder.Append(",");
                builder.Append(@"""Children"":[");
                bool first = true;
                foreach(Node idx in _children)
                {
                    if (!first)
                        builder.Append(",");
                    first = false;
                    builder.Append(idx.ToJSONString());
                }
                builder.Append("]");
            }
            builder.Append("}");
            return builder.ToString();
        }
    }
}
