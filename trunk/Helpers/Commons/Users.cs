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
using System.Web;
using System.Web.UI;

namespace HelperGlobals
{
    public sealed class Users : IList<string>
    {
        private static Users _instance;

        private readonly List<string> _usernames;

        private Users()
        {
            _usernames = new List<string>();
        }

        public static Users Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new Users();
                return _instance;
            }
        }

        public static string LoggedInUserName
        {
            get
            {
                string loggedInUser =
                    ((Page)HttpContext.Current.Handler)
                        .Session["Common.Users.LoggedInUserName"] as string;
                return loggedInUser;
            }
            set
            {
                ((Page)HttpContext.Current.Handler)
                    .Session["Common.Users.LoggedInUserName"] = value;
            }
        }

        public int IndexOf(string item)
        {
            return _usernames.IndexOf(item);
        }

        public void Insert(int index, string item)
        {
            _usernames.Insert(index, item);
        }

        public void RemoveAt(int index)
        {
            _usernames.RemoveAt(index);
        }

        public string this[int index]
        {
            get
            {
                return _usernames[index];
            }
            set
            {
                _usernames[index] = value;
            }
        }

        public void Add(string item)
        {
            bool exists = false;
            foreach (string idx in this)
            {
                if (idx == item)
                {
                    exists = true;
                    break;
                }
            }
            if (!exists)
                _usernames.Add(item);
        }

        public void Clear()
        {
            _usernames.Clear();
        }

        public bool Contains(string item)
        {
            return _usernames.Contains(item);
        }

        public void CopyTo(string[] array, int arrayIndex)
        {
            _usernames.CopyTo(array, arrayIndex);
        }

        public int Count
        {
            get { return _usernames.Count; }
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

        public bool Remove(string item)
        {
            return _usernames.Remove(item);
        }

        public IEnumerator<string> GetEnumerator()
        {
            return _usernames.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException();
        }
    }
}