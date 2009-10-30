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
using Ra.Brix.Data;
using System.Collections.Generic;
using System.Globalization;

namespace UserSettingsRecords
{
    [ActiveRecord]
    public class UserSettings : ActiveRecord<UserSettings>, IList<UserSettings.Setting>
    {
        /**
         * Represents one setting in the database, but should almost never be directly
         * used unless you wish to traverse all settings in database.
         */
        [ActiveRecord]
        public class Setting : ActiveRecord<Setting>
        {
            /**
             * Name of setting (key)
             */
            [ActiveField]
            public string Name { get; set; }

            [ActiveField]
            public string Username { get; set; }
            
            /**
             * Value of settings
             */
            [ActiveField]
            public string Value { get; set; }
        }

        private static UserSettings _instance;

        private UserSettings()
        {
            Values = new List<Setting>();
        }

        /**
         * Get the only object of type Settings
         */
        public static UserSettings Instance
        {
            get
            {
                // Checking to see if settings haven't been accessed before...
                if (_instance == null)
                {
                    // Need to do some race condition logic here ...
                    lock (typeof(UserSettings))
                    {
                        if (_instance == null)
                        {
                            // Checking to see if settings have been accessed and saved previously
                            // and then process was shut down or something...
                            _instance = SelectFirst();
                            if (_instance == null)
                            {
                                // Creating our onadonly settings object...
                                _instance = new UserSettings();
                                _instance.Save();
                            }
                        }
                    }
                }
                return _instance;
            }
        }

        /**
         * Returns the setting value converted to typeof(T). If setting doesn't exist, then
         * the default(T) value of the type will be returned.
         */
        public T Get<T>(string key, string username)
        {
            return Get(key, username, default(T));
        }

        /**
         * Returns the setting converted to typeof(T), if setting doesn't exist then
         * the defaultValue will be returned.
         */
        public T Get<T>(string key, string username, T defaultValue)
        {
            string tmpRetVal = this[key, username];
            if (tmpRetVal == null)
            {
                this[key, username] = (string)Convert.ChangeType(
                                                  defaultValue, 
                                                  typeof(string), 
                                                  CultureInfo.InvariantCulture);
                return defaultValue;
            }
            return (T)Convert.ChangeType(
                          tmpRetVal, 
                          typeof(T), 
                          CultureInfo.InvariantCulture);
        }

        [ActiveField]
        private List<Setting> Values { get; set; }

        /**
         * Reloads all settings. 
         * Notice you VERY rarely should actually use this method. In fact you
         * probably never at all need to use it!
         */
        public static void Reload()
        {
            _instance = null;
        }

        /**
         * Returns or sets the key value as a string
         */
        public string this[string key, string username]
        {
            get
            {
                Setting retVal = Values.Find(
                    delegate(Setting idx)
                        {
                            return idx.Name == key && idx.Username == username;
                        });
                if (retVal != null)
                    return retVal.Value;
                return null;
            }
            set
            {
                lock (typeof(UserSettings))
                {
                    Setting retVal = Values.Find(
                        delegate(Setting idx)
                            {
                                return idx.Name == key && idx.Username == username;
                            });
                    if (retVal != null)
                    {
                        retVal.Value = value;
                    }
                    else
                    {
                        Setting n = new Setting();
                        n.Name = key;
                        n.Value = value;
                        n.Username = username;
                        Values.Add(n);
                    }
                    // Must save "this" to get the parent relationship correct...
                    Save();
                }
            }
        }

        /**
         * Returns the number of settings in total in database
         */
        public new int Count
        {
            get
            {
                return Values.Count;
            }
        }

        public int IndexOf(Setting item)
        {
            return Values.IndexOf(item);
        }

        public void Insert(int index, Setting item)
        {
            lock (typeof(UserSettings))
            {
                Values.Insert(index, item);
                Save();
            }
        }

        public void RemoveAt(int index)
        {
            lock (typeof(UserSettings))
            {
                Values.RemoveAt(index);
                Save();
            }
        }

        public Setting this[int index]
        {
            get
            {
                return Values[index];
            }
            set
            {
                lock (typeof(UserSettings))
                {
                    Values[index] = value;
                    Save();
                }
            }
        }

        public void Add(Setting item)
        {
            lock (typeof(UserSettings))
            {
                Values.Add(item);
                Save();
            }
        }

        public void Clear()
        {
            lock (typeof(UserSettings))
            {
                Values.Clear();
                Save();
            }
        }

        public bool Contains(Setting item)
        {
            return Values.Contains(item);
        }

        public void CopyTo(Setting[] array, int arrayIndex)
        {
            Values.CopyTo(array, arrayIndex);
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

        public bool Remove(Setting item)
        {
            lock (typeof(UserSettings))
            {
                bool retVal = Values.Remove(item);
                Save();
                return retVal;
            }
        }

        public void Remove(string item)
        {
            lock (typeof(UserSettings))
            {
                Setting val = null;
                foreach (Setting idx in this)
                {
                    if (idx.Name == item)
                    {
                        val = idx;
                        break;
                    }
                }
                Remove(val);
                Save();
            }
        }

        public IEnumerator<Setting> GetEnumerator()
        {
            return Values.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return Values.GetEnumerator();
        }
    }
}