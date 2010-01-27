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
using Ra.Brix.Types;
using Ra.Widgets;

namespace CMSModules
{
    [ActiveModule]
    public class CreatePage : System.Web.UI.UserControl, IModule
    {
        protected global::Ra.Widgets.TextBox cmsPageName;
        protected global::Ra.Widgets.Panel pnlWrp;
        protected global::Ra.Widgets.SelectList parent;

        protected void Page_Load(object sender, EventArgs e)
        {
            pnlWrp.DataBind();
        }

        protected void cmsPageName_EscPressed(object sender, EventArgs e)
        {
            ActiveEvents.Instance.RaiseClearControls("dynPopup");
        }

        protected void submit_Click(object sender, EventArgs e)
        {
            Node node = new Node();
            node["Name"].Value = cmsPageName.Text;
            node["Parent"].Value = parent.SelectedItem.Value;
            ActiveEvents.Instance.RaiseActiveEvent(
                this,
                "CMSCreateNewPage",
                node);
        }

        public void InitialLoading(Node node)
        {
            Load +=
                delegate
                {
                    foreach (Node idx in node["Pages"])
                    {
                        ListItem l = new ListItem
                        {
                            Text = idx["Name"].Get<string>(), 
                            Value = idx["URL"].Get<string>()
                        };
                        parent.Items.Add(l);
                    }
                };
            cmsPageName.Text = Language.Instance["CMSNewPageName", null, "New page name"];
            cmsPageName.Select();
            cmsPageName.Focus();
        }
    }
}