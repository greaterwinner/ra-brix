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

namespace Ra.Brix.Loader
{
    /**
     * Mark your Active Modules with this attribute. If you mark your Modules with this attribute
     * you can load them using the PluginLoader.LoadControl method.
     */
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public class ActiveModuleAttribute : Attribute
    {
    }
}
