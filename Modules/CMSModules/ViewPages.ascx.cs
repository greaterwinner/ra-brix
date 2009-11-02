﻿/*
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
using Ra.Extensions.Widgets;
using Ra.Widgets;
using Ra;

namespace CMSModules
{
    [ActiveModule]
    public class ViewPages : System.Web.UI.UserControl, IModule
    {
        protected global::Ra.Extensions.Widgets.Tree tree;
        protected global::Ra.Extensions.Widgets.TreeNodes root;
        protected global::Ra.Extensions.Widgets.InPlaceEdit header;
        protected global::Ra.Widgets.Panel url;
        protected global::Ra.Widgets.Panel editWrp;
        protected global::System.Web.UI.HtmlControls.HtmlAnchor hyperlink;
        protected global::Components.RichEdit editor;
        protected global::Ra.Extensions.Widgets.ExtButton submit;
        protected global::Ra.Extensions.Widgets.ExtButton delete;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!_skipLoad)
            {
                BuildTree();
            }
            submit.DataBind();
            delete.DataBind();
        }

        protected void menu_SelectedNodeChanged(object sender, EventArgs e)
        {
            TreeNode sel = tree.SelectedNodes[0];

            if (tree.SelectedNodes.Length > 0)
            {
                // Making the editing wrapper visible...
                editWrp.Visible = true;
                editWrp.Style[Styles.display] = "";
            }
            // Header
            header.Text = sel.Text;

            // Hyperlink and URL
            hyperlink.Attributes.Remove("href");
            string urlString = "~/";
            if (sel.Xtra != "home")
            {
                urlString += sel.Xtra + ".aspx";
            }
            hyperlink.Attributes.Add("href", urlString);
            hyperlink.InnerHtml = sel.Xtra;
            url.ReRender();

            // Body
            Node node = new Node();
            node["URL"].Value = sel.Xtra;
            ActiveEvents.Instance.RaiseActiveEvent(
                this,
                "CMSGetBodyForURL",
                node);
            editor.Text = node["Body"].Get<string>();
        }

        protected void editor_GetPluginControls(object sender, Components.RichEdit.PluginEventArgs e)
        {
            Node node = new Node();
            ActiveEvents.Instance.RaiseActiveEvent(
                this,
                "CMSGetPluginTypes",
                node);
            SelectList list = new SelectList();
            list.ID = "plugins";
            list.CssClass = "editorSelect";
            list.SelectedIndexChanged +=
                delegate(object sender2, EventArgs e2)
                {
                    SelectList l = sender2 as SelectList;
                    if (l == null)
                        return;
                    string pluginValue = l.SelectedItem.Value;
                    editor.PasteHTML(string.Format(@"
{{||{0}||}}", pluginValue));
                    l.SelectedIndex = 0;
                };
            ListItem item = new ListItem(
                Language.Instance["InsertPlugin", null, "Insert Plugin..."],
                "xxx");
            list.Items.Add(item);
            foreach (Node idx in node)
            {
                ListItem l = new ListItem(
                    idx["Name"].Get<string>(), 
                    idx["Value"].Get<string>().Replace(" ", "q"));
                list.Items.Add(l);
            }
            e.Controls.Add(list);
        }

        [ActiveEvent(Name = "MainViewportResized")]
        protected void MainViewportResized(object sender, ActiveEventArgs e)
        {
            AjaxManager.Instance.WriterAtBack.Write(
                string.Format(@"
(function() {{
  var T = Ra.Control.$({0});
  setTimeout(function() {{
    T.modifyHeight();
  }}, 2000);
}})();
", "'" + editor.ClientID + "'"));
        }

        protected void editor_GetImageDialog(object sender, EventArgs e)
        {
            ActiveEvents.Instance.RaiseActiveEvent(
                this,
                "CMSGetImageDialog");
        }

        protected void editor_GetHyperLinkDialog(object sender, EventArgs e)
        {
            ActiveEvents.Instance.RaiseActiveEvent(
                this,
                "CMSGetHyperLinkDialog");
        }

        [ActiveEvent(Name = "CMSInsertLink")]
        protected void CMSInsertLink(object sender, ActiveEventArgs e)
        {
            string html = string.Format(
                @"<a href=""{0}"">{1}</a>", 
                e.Params["URL"].Get<string>(), 
                editor.Selection);
            editor.PasteHTML(html);
        }

        [ActiveEvent(Name = "FileChosenByFileDialog")]
        protected void FileChosenByFileDialog(object sender, ActiveEventArgs e)
        {
            string fileName = e.Params["File"].Get<string>();
            string html = string.Format(
                @"<img src=""{0}"" alt=""unknown"" />", fileName);
            editor.PasteHTML(html);
        }

        protected void delete_Click(object sender, EventArgs e)
        {
            if (tree.SelectedNodes.Length > 0)
            {
                Node node = new Node();
                node["URL"].Value = tree.SelectedNodes[0].Xtra;
                ActiveEvents.Instance.RaiseActiveEvent(
                    this,
                    "CMSDeletePage",
                    node);
            }
        }

        protected void submit_Click(object sender, EventArgs e)
        {
            if (tree.SelectedNodes.Length > 0)
            {
                string headerTxt = header.Text;
                string bodyTxt = editor.Text;
                Node node = new Node();
                node["URL"].Value = tree.SelectedNodes[0].Xtra;
                node["Header"].Value = headerTxt;
                node["Body"].Value = bodyTxt;
                ActiveEvents.Instance.RaiseActiveEvent(
                    this,
                    "CMSSaveEditedPage",
                    node);
            }
        }

        private Node Data
        {
            get { return ViewState["Data"] as Node; }
            set { ViewState["Data"] = value; }
        }

        private bool _skipLoad;
        public void InitialLoading(Node node)
        {
            _skipLoad = true;
            Load +=
                delegate
                {
                    Data = node["Pages"];
                    BuildTree();
                };
        }

        private void BuildTree()
        {
            foreach (Node idx in Data)
            {
                AddTreeNode(idx, root);
            }
        }

        private static void AddTreeNode(Node node, TreeNodes level)
        {
            foreach (Node idx in node)
            {
                if (idx.Name == "Page")
                {
                    TreeNode n = new TreeNode
                    {
                        ID = idx.DNA.Replace("-", ""),
                        Text = idx["Name"].Get<string>(),
                        Xtra = idx["URL"].Get<string>()
                    };
                    if (idx["Children", false] != null)
                    {
                        TreeNodes child = new TreeNodes {ID = "l" + n.ID};
                        n.Controls.Add(child);
                        foreach (Node idxInner in idx["Children"])
                        {
                            AddTreeNode(idxInner, child);
                        }
                    }
                    level.Controls.Add(n);
                }
            }
        }

        public string GetCaption()
        {
            return "";
        }
    }
}