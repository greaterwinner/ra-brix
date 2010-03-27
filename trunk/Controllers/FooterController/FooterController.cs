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
using Ra.Brix.Loader;

namespace FooterController
{
    [ActiveController]
    public class FooterController
    {
        [ActiveEvent(Name="Page_Init_InitialLoading")]
        protected void Initialize(object sender, EventArgs e)
        { 
            ActiveEvents.Instance.RaiseLoadControl(
                "FooterModules.Footer", 
                "dynFooter");
        }
    }
}
