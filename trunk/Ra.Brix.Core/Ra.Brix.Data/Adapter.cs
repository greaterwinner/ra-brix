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
using System.Web.UI;
using System.Web;
using System.Configuration;
using System.Reflection;

/**
 * Namespace mostly used for "internal stuff" in Ra-Brix which are not
 * needed to fiddle with for most developer only looking to consume 
 * Ra-Brix.
 */
namespace Ra.Brix.Data
{
    namespace Internal
    {
        /**
          * Abstract base class for all Database Adapters in Ra-Brix. If you wish
          * to build your own data adapter then inherit from this class and implement
          * the abstract methods, add up a reference to the dll and change the 
          * data-configuration line in your configuration file and it should work.
          * Class provides some common functions needed for all database adapters
          * like caching, instantiation and such in addition.
          */
        public abstract class Adapter
        {
            private static Adapter _adapter;
            private static ConstructorInfo _ctorToAdapter;

            /**
             * Retrieves the configured database adapter. Notice that you would very rarely
             * want to use this directly but instead access it indirectly through the 
             * ActiveRecord class.
             */
            public static Adapter Instance
            {
                get
                {
                    if (HttpContext.Current == null ||
                        HttpContext.Current.CurrentHandler == null ||
                        (HttpContext.Current.CurrentHandler as Page) == null)
                    {
                        // Not web...!
                        if (_adapter == null)
                            _adapter = CreateAdapter();
                        return _adapter;
                    }
                    // Web!!
                    Page page = HttpContext.Current.CurrentHandler as Page;
                    page.Unload += PageUnload;
                    if (page.Items["__LegoDataAdapter"] == null)
                    {
                        Adapter adapter = CreateAdapter();
                        page.Items["__LegoDataAdapter"] = adapter;
                    }
                    return page.Items["__LegoDataAdapter"] as Adapter;
                }
            }

            private static Adapter CreateAdapter()
            {
                if (_ctorToAdapter == null)
                {
                    lock (typeof(Adapter))
                    {
                        if (_ctorToAdapter == null)
                            _ctorToAdapter = GetAdapterConstructor();
                    }
                }

                Adapter adapter = _ctorToAdapter.Invoke(null) as Adapter;
                if (adapter == null)
                {
                    string adapterType = ConfigurationManager.AppSettings["LegoDatabaseAdapter"];
                    throw new ConfigurationErrorsException("Unknown database adapter type '" + adapterType + "' or couldn't cast to type Adapter. Make sure your Database adapter implements the abstract class 'Adapter'");
                }
                string connectionString = ConfigurationManager.AppSettings["LegoConnectionString"];
                adapter.Open(connectionString);
                return adapter;
            }

            private static ConstructorInfo GetAdapterConstructor()
            {
                string adapterType = ConfigurationManager.AppSettings["LegoDatabaseAdapter"];
                foreach (Assembly idxAsm in AppDomain.CurrentDomain.GetAssemblies())
                {
                    if(idxAsm.GlobalAssemblyCache)
                        continue;
                    foreach (Type idxType in idxAsm.GetTypes())
                    {
                        if (adapterType == idxType.FullName)
                        {
                            return idxType.GetConstructor(new Type[] { });
                        }
                    }
                }
                return null;
            }

            private static Dictionary<int, object> Cache
            {
                get
                {
                    if (HttpContext.Current == null || HttpContext.Current.CurrentHandler == null || (HttpContext.Current.CurrentHandler as Page) == null)
                    {
                        // TODO: Implement cache also for non-web scenarios...
                        return new Dictionary<int, object>();
                    }
                    Page page = HttpContext.Current.CurrentHandler as Page;
                    if (page.Items["__LegoDataAdapterCache"] == null)
                    {
                        page.Items["__LegoDataAdapterCache"] = new Dictionary<int, object>();
                    }
                    return page.Items["__LegoDataAdapterCache"] as Dictionary<int, object>;
                }
            }

            static void PageUnload(object sender, EventArgs e)
            {
                Instance.Close();
            }

            /**
             * Retrieves an object with the given ID from your data storage.
             */
            public object SelectByID(Type type, int id)
            {
                if (Cache.ContainsKey(id))
                    return Cache[id];
                object retVal = SelectObjectByID(type, id);
                if (retVal != null)
                    Cache[id] = retVal;
                return retVal;
            }

            /**
             * Deletes the given object from your data storage.
             */
            public void Delete(int id)
            {
                DeleteObject(id);
                Cache.Remove(id);
            }

            /**
             * Should return the number of items of the given type from
             * your data storage with the given criterias.
             */
            public abstract int CountWhere(Type type, params Criteria[] args);

            /**
             * Should return the object from your data storage with the given id
             * being of the given Type.
             */
            protected abstract object SelectObjectByID(Type type, int id);

            /**
             * Should return the first object of type; "type" - with the given criterias.
             */
            public abstract object SelectFirst(Type type, string propertyName, params Criteria[] args);

            /**
             * Should return all objects of the given type with the given criterias.
             */
            public abstract IEnumerable<object> Select(Type type, string propertyName, params Criteria[] args);

            /**
             * Should return all objects in your data storage.
             */
            public abstract IEnumerable<object> Select();

            /**
             * Should delete the object with the given ID from your data storage.
             */
            protected abstract void DeleteObject(int id);

            /**
             * Should save the given object into your data storage.
             */
            public abstract void Save(object value);

            /**
             * Called when data storage should close. Often used to
             * close file handles or database connections etc.
             */
            public abstract void Close();

            /**
             * Called when your data storage should be opened. Often used to
             * open file handles or database connections etc.
             */
            public abstract void Open(string connectionString);
        }
        
    }
}
