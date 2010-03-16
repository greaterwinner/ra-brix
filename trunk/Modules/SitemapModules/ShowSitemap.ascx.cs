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
        protected global::Ra.Widgets.Panel pnlWrp;
        protected global::Ra.Widgets.TextBox filter;

        private Node FilteredItems
        {
            get { return ViewState["Items"] as Node; }
            set { ViewState["Items"] = value; }
        }

        protected void search_Click(object sender, EventArgs e)
        {
            DataBindGrid();
            pnlWrp.ReRender();
            filter.Select();
            filter.Focus();
        }

        public void InitialLoading(Node node)
        {
            Load +=
                delegate
                {
                    FilteredItems = node["Items"];
                    DataBindGrid();
                };
        }

        private void DataBindGrid()
        {
            Node tmp = new Node();
            foreach (Node idx in FilteredItems)
            {
                if (idx["Name"].Get<string>().ToLower().Contains(filter.Text.ToLower()))
                    tmp.Add(idx);
            }
            rep.DataSource = tmp;
            rep.DataBind();
        }
    }
}
















