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
using LanguageRecords;

namespace ArticlePublisherModules
{
    [ActiveModule]
    public class ImportRSS : System.Web.UI.UserControl, IModule
    {
        protected global::Ra.Extensions.Widgets.ExtButton submit;
        protected global::Ra.Widgets.TextBox url;

        protected void submit_Click(object sender, EventArgs e)
        {
            string urlText = url.Text;
            Node node = new Node();
            node["URL"].Value = urlText;
            ActiveEvents.Instance.RaiseActiveEvent(
                this,
                "ArticleRSSImport",
                node);
        }

        public void InitialLoading(Node node)
        {
            Load +=
                delegate
                {
                    submit.DataBind();
                    url.Select();
                    url.Focus();
                };
        }
    }
}