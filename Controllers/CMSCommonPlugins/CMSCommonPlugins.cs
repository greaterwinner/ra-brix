/*
 * Ra-Brix - A Modular-based Framework for building 
 * Web Applications Copyright 2009 - Thomas Hansen 
 * thomas@ra-ajax.org. Unless permission is 
 * explicitly given this code is licensed under the 
 * GNU Affero GPL version 3 which can be found in the 
 * license.txt file on disc.
 * 
 */

using LanguageRecords;
using Ra.Brix.Loader;

namespace CMSCommonPlugins
{
    [ActiveController]
    public class CMSCommonPlugins
    {
        [ActiveEvent(Name = "CMSGetPluginTypes")]
        protected void CMSGetPluginTypes(object sender, ActiveEventArgs e)
        {
            e.Params["ContactUs"]["Name"].Value = Language.Instance["ContactUs", null, "Contact us"];
            e.Params["ContactUs"]["Value"].Value = "CMSCommonPluginModules.ContactUs";
        }
    }
}
