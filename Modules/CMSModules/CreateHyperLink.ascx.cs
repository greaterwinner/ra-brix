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
using Ra.Effects;

namespace CMSModules
{
    [ActiveModule]
    public class CreateHyperLink : System.Web.UI.UserControl, IModule
    {
        protected global::Ra.Widgets.SelectList internalPages;
        protected global::Ra.Widgets.TextBox urlText;
        protected global::Ra.Widgets.Panel wrp;

        protected void internalPages_SelectedIndexChanged(object sender, EventArgs e)
        {
            string url = internalPages.SelectedItem.Value;
            urlText.Text = url;
            internalPages.SelectedIndex = 0;
            urlText.Focus();
            urlText.Select();
        }

        protected void submit_Click(object sender, EventArgs e)
        {
            string url = urlText.Text;
            Node node = new Node();
            node["URL"].Value = url;
            ActiveEvents.Instance.RaiseActiveEvent(
                this,
                "CMSCreateNewLink",
                node);
        }

        public void InitialLoading(Node node)
        {
            Load += 
                delegate
                {
                    ListItem root = new ListItem(
                        Language.Instance["SelectLink", null, "Select link..."],
                        "xxx");
                    internalPages.Items.Add(root);
                    foreach (Node idx in node["Pages"])
                    {
                        ListItem l = new ListItem(
                            idx["Header"].Get<string>(),
                            idx["URL"].Get<string>());
                        internalPages.Items.Add(l);
                    }
                };
            wrp.DataBind();
            urlText.Text = Language.Instance["LinkURL", null, "Link URL..."];
            new EffectFocusAndSelect(urlText).Render();
        }

        public string GetCaption()
        {
            return "";
        }
    }
}