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

namespace DoxygentDotNetViewDocsModules
{
    public class DocsItem
    {
        private string _name;
        private string _id;
        private string _kind;
        private string _params;
        private string _returns;

        public string Params
        {
            get { return _params; }
            set { _params = value; }
        }

        public string Returns
        {
            get { return _returns; }
            set { _returns = value; }
        }

        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        public string ID
        {
            get { return _id; }
            set { _id = value; }
        }

        public string Kind
        {
            get { return _kind; }
            set { _kind = value; }
        }

        public DocsItem(string name, string id, string kind, string signature)
        {
            Name = name;
            ID = id;
            Kind = kind;
            if (!string.IsNullOrEmpty(signature))
            {
                Returns = signature.Substring(0, signature.IndexOf(name));
                if (Kind == "function" || Kind == "ctor")
                    Params = signature.Substring(signature.IndexOf("("));
            }
        }
    }
}
