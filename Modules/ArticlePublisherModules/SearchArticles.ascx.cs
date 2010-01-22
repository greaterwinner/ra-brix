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
    public class SearchArticles : System.Web.UI.UserControl, IModule
    {
        protected global::Ra.Widgets.TextBox search;

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            Node node = new Node();
            node["Query"].Value = search.Text;
            ActiveEvents.Instance.RaiseActiveEvent(
                this,
                "FilterArticles",
                node);
            search.Focus();
            search.Select();
        }

        public void InitialLoading(Node node)
        {
            Load +=
                delegate
                {
                    if (node["Filter"].Value != null)
                    {
                        search.Text = node["Filter"].Get<string>();
                        search.Select();
                        search.Focus();
                    }
                };
        }

        public string GetCaption()
        {
            return "";
        }
    }
}