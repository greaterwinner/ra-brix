/*
 * Ra-Brix - A Modular-based Framework for building 
 * Web Applications Copyright 2009 - Thomas Hansen 
 * thomas@ra-ajax.org. Unless permission is 
 * explicitly given this code is licensed under the 
 * GNU Affero GPL version 3 which can be found in the 
 * license.txt file on disc.
 * 
 */

using System.Web;

namespace HelperGlobals
{
    public static class ApplicationRoot
    {
        public static string Root
        {
            get
            {
                string baseUrl = HttpContext.Current.Request.Url.ToString();
                if (baseUrl.Contains("Default.aspx"))
                    baseUrl = baseUrl.Substring(0, baseUrl.IndexOf("Default.aspx"));
                if (baseUrl.Contains("default.aspx"))
                    baseUrl = baseUrl.Substring(0, baseUrl.IndexOf("default.aspx"));
                return baseUrl;
            }
        }
    }
}
