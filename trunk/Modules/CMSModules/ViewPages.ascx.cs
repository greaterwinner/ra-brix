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
using Ra.Extensions.Widgets;
using Ra.Widgets;
using Ra.Selector;
using Ra.Effects;

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
        protected global::Ra.Widgets.Panel infoWrp;
        protected global::System.Web.UI.HtmlControls.HtmlAnchor hyperlink;
        protected global::Ra.Extensions.Widgets.RichEdit editor;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!_skipLoad)
            {
                BuildTree();
            }
        }

        protected void menu_SelectedNodeChanged(object sender, EventArgs e)
        {
            TreeNode sel = tree.SelectedNodes[0];

            if (tree.SelectedNodes.Length > 0)
            {
                // Making the editing wrapper visible...
                editWrp.Visible = true;
                editWrp.Style[Styles.display] = "none";
                if(infoWrp.Style[Styles.display] != "none")
                {
                    new EffectFadeOut(infoWrp, 400)
                        .ChainThese(
                            new EffectFadeIn(editWrp, 400)).Render();
                }
                else
                {
                    new EffectFadeIn(editWrp, 400).Render();
                }
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

            // HideFromMenu CheckBox...
            CheckBox ch = Selector.FindControl<CheckBox>(this, "hideFromMenu");
            ch.Checked = node["HideFromMenu"].Get<bool>();
        }

        protected void editor_GetExtraToolbarControls(object sender, Ra.Extensions.Widgets.RichEdit.ExtraToolbarControlsEventArgs e)
        {
            // Inject Plugin select list...
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

            // Then the "Show In Menu" CheckBox...
            CheckBox ch = new CheckBox();
            ch.ID = "hideFromMenu";
            ch.Text = Language.Instance["HideFromMenu", null, "Hide from menu"];
            e.Controls.Add(ch);

            // Then the "Show In Header" CheckBox...
            CheckBox chH = new CheckBox();
            chH.ID = "hideFromHeader";
            chH.Text = Language.Instance["HideFromHeader", null, "Hide header"];
            e.Controls.Add(chH);

            // Delete button
            LinkButton delBtn = new LinkButton();
            delBtn.ID = "delete";
            delBtn.CssClass = "editorBtn delete";
            delBtn.Click += delete_Click;
            delBtn.Text = "&nbsp;";
            e.Controls.Add(delBtn);

            // Save button
            LinkButton submit = new LinkButton();
            submit.ID = "submit";
            submit.CssClass = "editorBtn save";
            submit.Click += submit_Click;
            submit.Text = "&nbsp;";
            e.Controls.Add(submit);
        }

        protected void editor_GetImageDialog(object sender, EventArgs e)
        {
            ActiveEvents.Instance.RaiseActiveEvent(
                this,
                "CMSGetImageDialog");
        }

        protected void editor_GetResourceDialog(object sender, EventArgs e)
        {
            ActiveEvents.Instance.RaiseActiveEvent(
                this,
                "CMSGetResourceDialog");
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

        [ActiveEvent(Name = "ImageFileChosenByFileDialog")]
        protected void ImageFileChosenByFileDialog(object sender, ActiveEventArgs e)
        {
            string fileName = e.Params["File"].Get<string>();
            string html = string.Format(
                @"<img src=""{0}"" alt=""unknown"" />", fileName);
            editor.PasteHTML(html);
        }

        [ActiveEvent(Name = "ResourceFileChosenByFileDialog")]
        protected void ResourceFileChosenByFileDialog(object sender, ActiveEventArgs e)
        {
            string fileName = e.Params["File"].Get<string>();
            string cssClass = "resourceDownload specificResource-" + 
                fileName.Substring(fileName.LastIndexOf('.') + 1).ToLowerInvariant();
            string html = string.Format(
                @"<a target=""_blank"" href=""{0}"" class=""{1}"" />", fileName, cssClass);
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
                    "CMSRequestDeletePage",
                    node);
            }
        }

        protected void submit_Click(object sender, EventArgs e)
        {
            SaveSelectedPage();
        }

        private void SaveSelectedPage()
        {
            if (tree.SelectedNodes.Length > 0)
            {
                string headerTxt = header.Text;
                string bodyTxt = editor.Text;
                Node node = new Node();
                node["URL"].Value = tree.SelectedNodes[0].Xtra;
                node["Header"].Value = headerTxt;
                node["Body"].Value = bodyTxt;
                CheckBox ch = Selector.FindControl<CheckBox>(this, "hideFromMenu"); 
                node["HideFromMenu"].Value = ch.Checked;

                CheckBox chH = Selector.FindControl<CheckBox>(this, "hideFromHeader");
                node["HideFromHeader"].Value = chH.Checked;
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

        protected void editor_CtrlKeys(object sender, RichEdit.CtrlKeysEventArgs e)
        {
            if(e.Key == 's')
            {
                SaveSelectedPage();
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