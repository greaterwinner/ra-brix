/*
 * Ra-Brix - A Modular-based Framework for building 
 * Web Applications Copyright 2009 - Thomas Hansen 
 * thomas@ra-ajax.org. Unless permission is 
 * explicitly given this code is licensed under the 
 * GNU Affero GPL version 3 which can be found in the 
 * license.txt file on disc.
 * 
 */

using System.Web.UI;
using Ra.Brix.Loader;
using Ra.Brix.Types;
using System;
using Ra.Widgets;

namespace CMSModules
{
    [ActiveModule]
    public class NormalContent : System.Web.UI.UserControl, IModule
    {
        protected global::System.Web.UI.HtmlControls.HtmlGenericControl header;
        protected global::Ra.Widgets.Panel content;

        private string Content
        {
            get { return ViewState["Content"] as string; }
            set { ViewState["Content"] = value; }
        }

        private bool _first;
        public void InitialLoading(Node node)
        {
            _first = true;
            header.InnerHtml = node["Header"].Get<string>();
            Page.Title = node["Header"].Get<string>();
            Load +=
                delegate
                {
                    Content = node["Content"].Get<string>();
                    CreateContent(Content);
                };
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Content != null && !_first)
            {
                CreateContent(Content);
            }
        }

        private void CreateContent(string text)
        {
            string[] entities = 
                text.Split(
                    new[]{"{||", "||}"}, 
                    StringSplitOptions.RemoveEmptyEntries);
            int idxNo = 0;
            foreach (string idx in entities)
            {
                if (idxNo % 2 == 0)
                {
                    LiteralControl lbl = new LiteralControl();
                    lbl.ID = "lblCont" + idxNo;
                    lbl.Text = idx;
                    content.Controls.Add(lbl);
                }
                else
                {
                    try
                    {
                        Control ctrl = PluginLoader.Instance.LoadControl(idx);
                        ctrl.ID = "lblCont" + idxNo;
                        content.Controls.Add(ctrl);
                    }
                    catch (Exception)
                    {
                        Label lbl = new Label();
                        lbl.Text = "Control '" + idx + "' doesn't exists...!";
                        lbl.Style[Styles.color] = "Red";
                        content.Controls.Add(lbl);
                    }
                }
                idxNo += 1;
            }
        }

        public string GetCaption()
        {
            return "";
        }
    }
}