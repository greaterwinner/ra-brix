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
using LanguageRecords;
using Ra.Brix.Loader;
using ASP = System.Web.UI.WebControls;
using Ra.Brix.Types;
using Ra.Extensions.Widgets;

namespace ExternalLinksModules
{
    [ActiveModule]
    public class ViewLinks : System.Web.UI.UserControl, IModule
    {
        protected global::Ra.Extensions.Widgets.ExtButton create;
        protected global::Ra.Widgets.Panel repWrp;
        protected global::System.Web.UI.WebControls.Repeater rep;

        protected void create_Click(object sender, EventArgs e)
        {
            Node node = new Node();
            ActiveEvents.Instance.RaiseActiveEvent(
                this,
                "CreateNewExternalLink",
                node);
            rep.DataSource = node["Links"];
            rep.DataBind();
            repWrp.ReRender();
        }

        protected void NameChanged(object sender, EventArgs e)
        {
            InPlaceEdit edit = sender as InPlaceEdit;
            int id = int.Parse(edit.Xtra);
            Node node = new Node();
            node["ID"].Value = id;
            node["Name"].Value = edit.Text;
            ActiveEvents.Instance.RaiseActiveEvent(
                this,
                "ExternalLinksValueUpdated",
                node);
        }

        protected void URLChanged(object sender, EventArgs e)
        {
            InPlaceEdit edit = sender as InPlaceEdit;
            int id = int.Parse(edit.Xtra);
            Node node = new Node();
            node["ID"].Value = id;
            node["URL"].Value = edit.Text;
            ActiveEvents.Instance.RaiseActiveEvent(
                this,
                "ExternalLinksValueUpdated",
                node);
        }

        public void InitialLoading(Node node)
        {
            create.DataBind();
            Load +=
                delegate
                {
                    rep.DataSource = node["Links"];
                    rep.DataBind();
                };
        }
    }
}