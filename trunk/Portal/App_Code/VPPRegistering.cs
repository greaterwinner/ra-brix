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
using Ra.Brix.Loader;
using System.Reflection;

public static class VPPRegistering
{
    public static void AppInitialize()
    {
        AssemblyResourceProvider providerInstance = new AssemblyResourceProvider();

        // we get the current instance of HostingEnvironment class. We can't create a new one
        // because it is not allowed to do so. An AppDomain can only have one HostingEnvironment
        // instance.
        HostingEnvironment hostingEnvironmentInstance = 
            (HostingEnvironment)typeof(HostingEnvironment).InvokeMember(
            "_theHostingEnvironment",  
            BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.GetField, 
            null, 
            null, 
            null);
        if (hostingEnvironmentInstance == null)
            return;

        // we get the MethodInfo for RegisterVirtualPathProviderInternal method which is internal
        // and also static.
        MethodInfo mi = typeof(HostingEnvironment).GetMethod(
            "RegisterVirtualPathProviderInternal", 
            BindingFlags.NonPublic | BindingFlags.Static);
        if (mi == null)
            return;

        // finally we invoke RegisterVirtualPathProviderInternal method with one argument which
        // is the instance of our own VirtualPathProvider.
        mi.Invoke(
            hostingEnvironmentInstance, 
            new object[] { providerInstance });
    }
}
