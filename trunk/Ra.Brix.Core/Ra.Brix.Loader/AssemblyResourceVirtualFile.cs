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
using System.Web.Hosting;
using System.IO;
using System.Reflection;
using System.Web;

namespace Ra.Brix.Loader
{
    /*
     * The internal implementation of our VirtualFile or VPP (Virtual Path Provider)
     */
    internal class AssemblyResourceVirtualFile : VirtualFile
    {
        readonly string _path;

        /*
         * CTORtaking the path and storing to later...
         */
        public AssemblyResourceVirtualFile(string virtualPath)
            : base(virtualPath)
        {
            _path = VirtualPathUtility.ToAppRelative(virtualPath);
        }

        /*
         * Expects either a relative DLL coming from the Bin folder of our
         * application or a complete path pointing to a DLL another place. Will split
         * the "path" string into two different parts where the first is the assembly name
         * and the second is the fully qaulified resource identifier of the resource to load.
         */
        public override Stream Open()
        {
            string[] parts;
            if (_path.IndexOf(":") == -1)
                parts = _path.Split('/');
            else
            {
                parts = _path.ToLower().Split(
                    new[] { ".dll" }, 
                    StringSplitOptions.RemoveEmptyEntries);
                parts[0] += ".dll";
            }
            string assemblyName = parts[2];
            string resourceName = parts[3];

            // Checking to see if assmebly is already loaded...
            foreach (Assembly idx in AppDomain.CurrentDomain.GetAssemblies())
            {
                if (idx.CodeBase.Substring(idx.CodeBase.LastIndexOf("/") + 1).ToLower() ==
                    assemblyName.ToLower())
                {
                    Stream retVal = idx.GetManifestResourceStream(resourceName);
                    if (retVal == null)
                        throw new ArgumentException("Could not find the Virtual File; '" + _path + "'");
                    return retVal;
                }
            }

            // We do NOT combine the path if it's an absolute path...
            if (assemblyName.IndexOf(":") == -1)
                assemblyName = Path.Combine(HttpRuntime.BinDirectory, assemblyName);

            // If assembly is not loaded we must explicitly load it...
            Assembly assembly = Assembly.LoadFile(assemblyName);
            if (assembly != null)
            {
                Stream retVal = assembly.GetManifestResourceStream(resourceName);
                if (retVal == null)
                    throw new ArgumentException("Could not find the Virtual File; '" + _path + "'");
                return retVal;
            }
            throw new ArgumentException(
                "Could not find the assmelby pointed to by the Virtual File; '" + _path + "'");
        }
    }
}



