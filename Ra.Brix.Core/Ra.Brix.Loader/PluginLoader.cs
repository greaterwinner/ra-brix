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
using ASP = System.Web.UI;
using System.Collections.Generic;
using System.IO;
using System.Web;
using System.Reflection;
using Ra.Brix.Data;
using Ra.Brix.Types;

namespace Ra.Brix.Loader
{
    /**
     * Helps load UserControls embedded in resources. Relies on that Ra.Brix.Loader.AssemblyResourceProvider
     * is registered as a Virtual Path Provider in e.g. your Global.asax file. Use the Instance method
     * to access the singleton object, then use the LoadControl to load UserControls embedded in resources.
     */
    public sealed class PluginLoader
    {
        private static PluginLoader _instance;
        private readonly Dictionary<string, Tuple<string, Type>> _loadedPlugins = new Dictionary<string, Tuple<string, Type>>();
        private readonly List<Type> _controllerTypes = new List<Type>();

        private delegate void TypeDelegate(Type type);

        private PluginLoader()
        {
            // Making sure all DLLs are loaded
            MakeSureAllDLLsAreLoaded();

            // Initializing all Active Modules
            FindAllTypesWithAttribute<ActiveModuleAttribute>(
                delegate(Type type)
                {
                    string userControlFile = type.FullName + ".ascx";
                    _loadedPlugins[type.FullName] = new Tuple<string, Type>(userControlFile, type);
                    InitializeEventHandlers(null, type);
                });

            // Initializing all Active Controllers
            FindAllTypesWithAttribute<ActiveControllerAttribute>(
                delegate(Type type)
                {
                    _controllerTypes.Add(type);
                    InitializeEventHandlers(null, type);
                });

            // Initializing all Active Records
            FindAllTypesWithAttribute<ActiveRecordAttribute>(
                delegate(Type type)
                {
                    InitializeEventHandlers(null, type);
                });
        }

        // Helper method that loops through every type in AppDomain and 
        // looks for an attribute of a given type and passes it into a delegate 
        // submitted by the caller...
        private static void FindAllTypesWithAttribute<TAttrType>(TypeDelegate functor)
        {
            foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                try
                {
                    foreach (Type type in assembly.GetTypes())
                    {
                        TAttrType[] attributes = type.GetCustomAttributes(
                            typeof(TAttrType),
                            true) as TAttrType[];
                        bool isWhatWereLookingFor = attributes != null && attributes.Length > 0;
                        if (isWhatWereLookingFor)
                        {
                            // Calling our given delegate with the type...
                            functor(type);
                        }
                    }
                }
                catch
                {
                    ; // Intentionally do nothing...
                }
            }
        }

        /**
         * Singleton accessor.
         */
        public static PluginLoader Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (typeof(PluginLoader))
                    {
                        if (_instance == null)
                            _instance = new PluginLoader();
                    }
                }
                return _instance;
            }
        }

        /**
         * Dynamically load a Control with the given FullName (namespace + type name). This
         * is the method which is internally used in Ra-Brix to load UserControls from 
         * embedded resources and also other controls.
         */
        public ASP.Control LoadControl(string fullTypeName)
        {
            ASP.Page page = (HttpContext.Current.Handler as ASP.Page);

            // Checking to see if we've got our UnLoad event handlers event here...
            if (page != null && page.Items["__Ra.Brix.Loader.PluginLoader.hasAddedUnloadEvent"] == null)
            {
                page.Items["__Ra.Brix.Loader.PluginLoader.hasAddedUnloadEvent"] = true;
                page.Unload += page_Unload;
                InstantiateAllControllers();

                // Fire the "Application Startup" event. This one will only trigger
                // ONCE in comparison to the "InitialLoadingOfPage" event which will fire
                // every time the page reloads...
                ActiveEvents.Instance.RaiseActiveEvent(this, "ApplicationStartup");
            }

            if (!_loadedPlugins.ContainsKey(fullTypeName))
                throw new ArgumentException(
                    "Couldn't find the plugin with the name of; '" + fullTypeName + "'");
            Tuple<string, Type> pluginType = _loadedPlugins[fullTypeName];
            if (string.IsNullOrEmpty(pluginType.Left))
            {
                // Non-UserControl plugin...
                ConstructorInfo constructorInfo = pluginType.Right.GetConstructor(new Type[] { });
                ASP.Control retVal = constructorInfo.Invoke(new object[] { }) as ASP.Control;
                InitializeEventHandlers(retVal, pluginType.Right);
                return retVal;
            }
            if (page != null)
            {
                // UserControl plugin type...
                ASP.Control retVal =
                    page.LoadControl(
                        "~/Ra.Brix.Module/" +
                        pluginType.Right.Assembly.ManifestModule.ScopeName +
                        "/" +
                        pluginType.Left);
                InitializeEventHandlers(retVal, pluginType.Right);
                return retVal;
            }
            return null;
        }

        private void page_Unload(object sender, EventArgs e)
        {
            ActiveEvents.Instance.ClearAllInstanceEventHandlers();
        }

        private void InstantiateAllControllers()
        {
            foreach (Type idxType in _controllerTypes)
            {
                object controllerObject = idxType.GetConstructor(System.Type.EmptyTypes).Invoke(null);
                InitializeEventHandlers(controllerObject, idxType);
            }
        }

        private void InitializeEventHandlers(object retVal, Type pluginType)
        {
            // If the context passed is null, then what we're trying to retrieve
            // are the stat event handlers...
            BindingFlags flags = retVal == null ?
                BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static :
                BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance;
            foreach (MethodInfo idx in pluginType.GetMethods(flags))
            {
                ActiveEventAttribute[] attr =
                    idx.GetCustomAttributes(
                        typeof(ActiveEventAttribute),
                        true) as ActiveEventAttribute[];
                if (attr != null && attr.Length > 0)
                {
                    foreach (ActiveEventAttribute idxAttr in attr)
                    {
                        ActiveEvents.Instance.AddListener(retVal, idx, idxAttr.Name);
                    }
                }
            }
        }

        private void MakeSureAllDLLsAreLoaded()
        {
            // Only doing this on WEB...!
            if (HttpContext.Current != null)
            {
                // Sometimes not all DLLs in the bin folder will be included in the
                // current AppDomain. This often happens due to that no types from
                // the DLLs are actually references within the website itself
                // This logic runs through all DLLs in the bin folder of the
                // website to check if they're inside the current AppDomain, and
                // if not loads them up
                List<Assembly> initialAssemblies = new List<Assembly>(AppDomain.CurrentDomain.GetAssemblies());
                DirectoryInfo di = new DirectoryInfo(HttpContext.Current.Server.MapPath("~/bin"));
                foreach (FileInfo idxFile in di.GetFiles("*.dll"))
                {
                    try
                    {
                        if (initialAssemblies.Exists(
                            delegate(Assembly idx)
                            {
                                return idx.ManifestModule.Name.ToLower() == idxFile.Name.ToLower();
                            }))
                            continue;
                        Assembly a = Assembly.ReflectionOnlyLoad(idxFile.Name.Replace(".dll", "").Replace(".DLL", ""));
                        if (!initialAssemblies.Exists(
                            delegate(Assembly idx)
                            {
                                return idx.FullName == a.FullName;
                            }))
                            Assembly.LoadFrom(idxFile.FullName);
                    }
                    catch (Exception)
                    {
                        ; // Intentionally do nothing in case assembly loading throws...!
                        // Especially true for C++ compiled assemblies...
                        // Sample here is the MySQL DLL...
                    }
                }
            }
        }
    }
}
