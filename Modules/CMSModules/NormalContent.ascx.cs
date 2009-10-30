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

namespace CMSModules
{
    [ActiveModule]
    public class NormalContent : System.Web.UI.UserControl, IModule
    {
        protected global::System.Web.UI.HtmlControls.HtmlGenericControl header;
        protected global::System.Web.UI.HtmlControls.HtmlGenericControl content;

        public void InitialLoading(Node node)
        {
            header.InnerHtml = node["Header"].Get<string>();
            content.InnerHtml = node["Content"].Get<string>();
            Page.Title = node["Header"].Get<string>();
        }

        public string GetCaption()
        {
            return "";
        }
    }
}