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
using System.Web.UI;
using Ra.Brix.Loader;
using Ra.Brix.Types;

namespace SitemapModules
{
    [ActiveModule]
    public class ShowSitemap : System.Web.UI.UserControl, IModule
    {
        protected global::System.Web.UI.WebControls.Repeater rep;

        public void InitialLoading(Node node)
        {
            Load +=
                delegate
                {
                    rep.DataSource = node["Items"];
                    rep.DataBind();
                };
        }
    }
}
















