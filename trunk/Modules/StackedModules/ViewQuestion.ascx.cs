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
using Ra.Brix.Types;
using System;
using Ra.Widgets;
using Ra.Extensions.Widgets;

namespace StackedModules
{
    [ActiveModule]
    public class ViewQuestion : System.Web.UI.UserControl, IModule
    {
        protected global::System.Web.UI.HtmlControls.HtmlGenericControl header;
        protected global::System.Web.UI.HtmlControls.HtmlGenericControl body;

        public void InitialLoading(Node node)
        {
            Load +=
                delegate
                {
                    header.InnerHtml = node["Header"].Get<string>();
                    body.InnerHtml = node["Body"].Get<string>();
                };
        }
    }
}




