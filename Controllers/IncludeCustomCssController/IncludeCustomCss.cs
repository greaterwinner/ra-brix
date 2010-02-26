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
using System.Web.UI;
using System.Web;
using SettingsRecords;
using System;

namespace IncludeCustomCssController
{
    [ActiveController]
    public class IncludeCustomCss
    {
        [ActiveEvent(Name = "Page_Init_InitialLoading")]
        protected void Page_Init_InitialLoading(object sender, ActiveEventArgs e)
        {
            Page page = HttpContext.Current.Handler as Page;

            // Injecting a CSS link reference if additional CSS files are to be included
            string custFiles = Settings.Instance["CustomCssFiles"];
            if (!string.IsNullOrEmpty(custFiles))
            {
                string[] files = custFiles.Split(
                    new char[] { ',' }, 
                    StringSplitOptions.RemoveEmptyEntries);
                foreach (string file in files)
                {
                    string fileName = file.Trim();
                    LiteralControl lit = new LiteralControl();
                    lit.Text = string.Format(@"
<link 
    href=""{0}"" 
    rel=""stylesheet""
    type=""text/css"" />",
                             fileName);
                    page.Header.Controls.Add(lit);
                }
            }
        }
    }
}
