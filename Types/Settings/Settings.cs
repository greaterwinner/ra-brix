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
using System.Configuration;
using System.Collections.Generic;
using System.Globalization;

namespace SettingsRecords
{
    [ActiveType]
    public class Settings : ActiveType<Settings>, IList<Settings.Setting>
    {
        /**
         * Represents one setting in the database, but should almost never be directly
         * used unless you wish to traverse all settings in database.
         */
        [ActiveType]
        public class Setting : ActiveType<Setting>
        {
            /**
             * Name of setting (key)
             */
            [ActiveField]
            public string Name { get; set; }

            /**
             * Value of settings
             */
            [ActiveField]
            public string Value { get; set; }
        }

        private static Settings _instance;
        private List<Setting> _values;

        private Settings()
        {
            _values = new List<Setting>();
        }

        /**
         * Get the only object of type Settings
         */
        public static Settings Instance
        {
            get
            {
                // Checking to see if settings haven't been accessed before...
                if (_instance == null)
                {
                    // Need to do some race condition logic here ...
                    lock (typeof(Settings))
                    {
                        if (_instance == null)
                        {
                            // Checking to see if settings have been accessed and saved previously
                            // and then process was shut down or something...
                            _instance = SelectFirst();
                            if (_instance == null)
                            {
                                // Creating our onadonly settings object...
                                _instance = new Settings();
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
        public T Get<T>(string key)
        {
            return Get(key, default(T));
        }

        /**
         * Returns the setting converted to typeof(T), if setting doesn't exist then
         * the defaultValue will be returned.
         */
        public T Get<T>(string key, T defaultValue)
        {
            string tmpRetVal = this[key];
            if (tmpRetVal == null)
            {
                this[key] = (string)Convert.ChangeType(
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
        private List<Setting> Values
        {
            get { return _values; }
// ReSharper disable UnusedMember.Local
            // Being used through reflection from Ra.Brix.Data...!
            set { _values = value; }
// ReSharper restore UnusedMember.Local
        }

        /**
         * Reloads all settings, noe you VERY rarely should actually use this method. In fact you
         * probably never at all need to use it!
         */
        public static void Reload()
        {
            _instance = null;
        }

        /**
         * Returns or sets the key value as a string
         */
        public string this[string key]
        {
            get
            {
                Setting retVal = Values.Find(
                    delegate(Setting idx)
                        {
                            return idx.Name == key;
                        });
                if (retVal != null)
                    return retVal.Value;

                // Checking against application configuration settings to look for defaults
                string tmp = ConfigurationManager.AppSettings[key];
                this[key] = tmp;
                return tmp;
            }
            set
            {
                lock (typeof(Settings))
                {
                    Setting retVal = Values.Find(
                        delegate(Setting idx)
                            {
                                return idx.Name == key;
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
            lock (typeof(Settings))
            {
                Values.Insert(index, item);
                Save();
            }
        }

        public void RemoveAt(int index)
        {
            lock (typeof(Settings))
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
                lock (typeof(Settings))
                {
                    Values[index] = value;
                    Save();
                }
            }
        }

        public void Add(Setting item)
        {
            lock (typeof(Settings))
            {
                Values.Add(item);
                Save();
            }
        }

        public void Clear()
        {
            lock (typeof(Settings))
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
            lock (typeof(Settings))
            {
                bool retVal = Values.Remove(item);
                Save();
                return retVal;
            }
        }

        public void Remove(string item)
        {
            lock (typeof(Settings))
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

        public void Remove(int id)
        {
            lock (typeof(Settings))
            {
                Setting val = null;
                foreach (Setting idx in this)
                {
                    if (idx.ID != id)
                        continue;
                    val = idx;
                    break;
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