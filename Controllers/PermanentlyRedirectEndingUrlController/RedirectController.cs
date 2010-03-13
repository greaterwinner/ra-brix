/*
 * Ra-Brix - A Modular-based Framework for building 
 * Web Applications Copyright 2009 - Thomas Hansen 
 * thomas@ra-ajax.org. Unless permission is 
 * explicitly given this code is licensed under the 
 * GNU Affero GPL version 3 which can be found in the 
 * license.txt file on disc.
 * 
 */

using Ra.Brix.Loader;
using System.Web;
using SettingsRecords;

namespace PermanentlyRedirectEndingUrlController
{
    [ActiveController]
    public class RedirectController
    {
        [ActiveEvent(Name = "Page_Init")]
        protected void Page_Init(object sender, ActiveEventArgs e)
        {
            // Dummy issue for old Ra-Ajax blogs...
            string oldExtension = Settings.Instance["OldPageExtensionsToRemove"];
            if (HttpContext.Current.Request.Params["ContentID"] != null &&
                HttpContext.Current.Request.Params["ContentID"].Contains(oldExtension))
            {
                string rewriteUrl =
                    HttpContext.Current.Request.Params["ContentID"].Replace(oldExtension, "");
                HttpContext.Current.Response.Clear();
                HttpContext.Current.Response.Status = "301 Moved Permanently";
                HttpContext.Current.Response.AddHeader("Location", rewriteUrl);
                HttpContext.Current.Response.End();
            }
        }
    }
}
