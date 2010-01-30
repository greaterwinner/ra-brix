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
using Ra.Brix.Types;
using System;
using Ra.Extensions.Widgets;
using Ra.Widgets;

namespace ArticlePublisherModules
{
    [ActiveModule]
    public class ViewUsers : System.Web.UI.UserControl, IModule
    {
        protected global::System.Web.UI.WebControls.Repeater rep;
        protected global::Ra.Widgets.Label header;

        public void InitialLoading(Node node)
        {
            rep.DataSource = node["Users"];
            rep.DataBind();
            header.Text = node["Header"].Get<string>();
        }
    }
}