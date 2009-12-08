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
using System.Reflection;
using Ra.Brix.Data;
using Ra.Brix.Loader;
using Ra.Brix.Types;

namespace InstalledAppsController
{
    public class ReflectionHelper
    {
        public ReflectionHelper(string dllFullPath)
        {
            FullPath = dllFullPath.ToLowerInvariant().Replace("\\", "/");
        }

        private string FullPath { get; set; }

        public Node CreateNodeStructure()
        {
            Node retVal = new Node();
            foreach (Assembly idx in PluginLoader.PluginAssemblies)
            {
                string codebase = idx.CodeBase;
                codebase = codebase.Replace("file:///", "");
                codebase = codebase.Replace("\\", "/");
                codebase = codebase.ToLowerInvariant();
                if(codebase == FullPath)
                {
                    // Found our DLL...
                    foreach (Type idxType in idx.GetTypes())
                    {
                        ActiveControllerAttribute[] attrsController =
                            idxType.GetCustomAttributes(typeof (ActiveControllerAttribute), true) as
                            ActiveControllerAttribute[];
                        if (attrsController != null && attrsController.Length > 0)
                        {
                            CreateControllerDocs(idxType, retVal);
                        }
                        else
                        {
                            ActiveModuleAttribute[] attrsModules =
                                idxType.GetCustomAttributes(typeof(ActiveModuleAttribute), true) as
                                ActiveModuleAttribute[];
                            if (attrsModules != null && attrsModules.Length > 0)
                            {
                                CreateModuleDocs(idxType, retVal);
                            }
                            else
                            {
                                ActiveTypeAttribute[] attrsTypes =
                                    idxType.GetCustomAttributes(typeof(ActiveTypeAttribute), true) as
                                    ActiveTypeAttribute[];
                                if (attrsTypes != null && attrsTypes.Length > 0)
                                {
                                    CreateTypesDocs(idxType, retVal);
                                }
                            }
                        }
                    }
                }
            }
            return retVal;
        }

        private void CreateTypesDocs(Type idxType, Node retVal)
        {
            Node tmp = retVal["Types"]["Type" + (retVal["Types"].Count + 1)];
            tmp["Name"].Value = idxType.Name;
            tmp["FullName"].Value = idxType.FullName;
            int idxNo = 0;
            foreach (MethodInfo idx in idxType.GetMethods(
                BindingFlags.Instance |
                BindingFlags.NonPublic |
                BindingFlags.Instance |
                BindingFlags.Static))
            {
                ActiveEventAttribute[] attrs =
                    idx.GetCustomAttributes(typeof(ActiveEventAttribute), true)
                    as ActiveEventAttribute[];
                if (attrs != null && attrs.Length > 0)
                {
                    tmp["Methods"]["Method" + idxNo]["MethodName"].Value = idx.Name;
                    tmp["Methods"]["Method" + idxNo]["ActiveEventName"].Value = attrs[0].Name;
                    idxNo += 1;
                }
            }
        }

        private void CreateModuleDocs(Type idxType, Node retVal)
        {
            Node tmp = retVal["Modules"]["Module" + (retVal["Modules"].Count + 1)];
            tmp["Name"].Value = idxType.Name;
            tmp["FullName"].Value = idxType.FullName;
            int idxNo = 0;
            foreach (MethodInfo idx in idxType.GetMethods(
                BindingFlags.Instance |
                BindingFlags.NonPublic |
                BindingFlags.Instance |
                BindingFlags.Static))
            {
                ActiveEventAttribute[] attrs =
                    idx.GetCustomAttributes(typeof(ActiveEventAttribute), true)
                    as ActiveEventAttribute[];
                if (attrs != null && attrs.Length > 0)
                {
                    tmp["Methods"]["Method" + idxNo]["MethodName"].Value = idx.Name;
                    tmp["Methods"]["Method" + idxNo]["ActiveEventName"].Value = attrs[0].Name;
                    idxNo += 1;
                }
            }
        }

        private void CreateControllerDocs(Type idxType, Node retVal)
        {
            Node tmp = retVal["Controllers"]["Controller" + (retVal["Controllers"].Count + 1)];
            tmp["Name"].Value = idxType.Name;
            tmp["FullName"].Value = idxType.FullName;
            int idxNo = 0;
            foreach (MethodInfo idx in idxType.GetMethods(
                BindingFlags.Instance | 
                BindingFlags.NonPublic | 
                BindingFlags.Instance | 
                BindingFlags.Static))
            {
                ActiveEventAttribute[] attrs =
                    idx.GetCustomAttributes(typeof (ActiveEventAttribute), true) 
                    as ActiveEventAttribute[];
                if (attrs != null && attrs.Length > 0)
                {
                    tmp["Methods"]["Method" + idxNo]["MethodName"].Value = idx.Name;
                    tmp["Methods"]["Method" + idxNo]["ActiveEventName"].Value = attrs[0].Name;
                    idxNo += 1;
                }
            }
        }
    }
}
