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
using HelperGlobals;
using System;

namespace ArticlePublisherModules
{
    [ActiveModule]
    public class ViewArticle : System.Web.UI.UserControl, IModule
    {
        protected global::System.Web.UI.HtmlControls.HtmlGenericControl header;
        protected global::System.Web.UI.HtmlControls.HtmlGenericControl ingress;
        protected global::System.Web.UI.HtmlControls.HtmlGenericControl date;
        protected global::System.Web.UI.HtmlControls.HtmlGenericControl content;
        protected global::System.Web.UI.HtmlControls.HtmlImage image;

        public void InitialLoading(Node node)
        {
            header.InnerHtml = node["Header"].Get<string>();
            ingress.InnerHtml = node["Ingress"].Get<string>();
            content.InnerHtml = node["Body"].Get<string>();
            date.InnerHtml = DateFormatter.FormatDate(node["Date"].Get<DateTime>());
            image.Src = node["MainImage"].Get<string>();
            image.Alt = node["Ingress"].Get<string>();
        }

        public string GetCaption()
        {
            return "";
        }
    }
}