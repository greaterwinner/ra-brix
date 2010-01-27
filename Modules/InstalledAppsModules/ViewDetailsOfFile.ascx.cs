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
using Ra.Effects;
using Ra.Widgets;

namespace InstalledAppsModules
{
    [ActiveModule]
    public class ViewDetailsOfFile : System.Web.UI.UserControl, IModule
    {
        protected global::Ra.Extensions.Widgets.Tree tree;
        protected global::Ra.Extensions.Widgets.TreeNodes root;
        protected global::Ra.Widgets.Label headerLbl;
        protected global::Ra.Widgets.Label lblFullName;
        protected global::Ra.Widgets.Panel infoContent;
        protected global::Ra.Widgets.Panel infoContentTip;
        protected global::Components.Grid grd;
        private Node _node;

        protected void Page_Load(object sender, EventArgs e)
        {
            if(_node != null)
                Data = _node;
            BuildTree();
        }

        private void BuildTree()
        {
            foreach(Node idx in Data)
            {
                if(idx.Name == "Controllers")
                {
                    TreeNode node = new TreeNode();
                    node.ID = "idController";
                    node.Text = LanguageRecords.Language.Instance["Controllers", null, "Controllers"];
                    root.Controls.Add(node);
                    BuildChildren(idx, node);
                }
                else if(idx.Name == "Modules")
                {
                    TreeNode node = new TreeNode();
                    node.ID = "idModules";
                    node.Text = LanguageRecords.Language.Instance["Modules", null, "Modules"];
                    root.Controls.Add(node);
                    BuildChildren(idx, node);
                }
                else if(idx.Name == "Types")
                {
                    TreeNode node = new TreeNode();
                    node.ID = "idTypes";
                    node.Text = LanguageRecords.Language.Instance["Types", null, "Types"];
                    root.Controls.Add(node);
                    BuildChildren(idx, node);
                }
            }
        }

        private void BuildChildren(Node node, TreeNode treeNode)
        {
            TreeNodes nodes = new TreeNodes();
            nodes.ID = "ch_" + treeNode.ID;
            treeNode.Controls.Add(nodes);
            foreach (Node idx in node)
            {
                string name = idx["Name"].Get<string>();
                string fullName = idx["FullName"].Get<string>();
                TreeNode n = new TreeNode();
                n.CssClass = node.Name;
                n.ID = fullName.Replace(".", "_").Replace(" ", "").Replace("+", "");
                n.Xtra = idx.DNA;
                n.Text = name;
                n.Tooltip = fullName;
                nodes.Controls.Add(n);
            }
        }

        protected void menu_SelectedNodeChanged(object sender, EventArgs e)
        {
            TreeNode node = tree.SelectedNodes[0];
            if (!string.IsNullOrEmpty(node.Xtra))
            {
                AnimateDetails();
                Node tmp = Data.Find(node.Xtra);
                lblFullName.Text = 
                    Language.Instance["FullName", null, "Full name: "] + 
                    tmp["FullName"].Get<string>();
                Node d = new Node();
                d["Columns"]["MethodName"]["Caption"].Value = 
                    Language.Instance["MethodName", null, "Method name"];
                d["Columns"]["MethodName"]["ControlType"].Value = "Label";
                d["Columns"]["EventName"]["Caption"].Value = 
                    Language.Instance["EventName", null, "Event name"];
                d["Columns"]["EventName"]["ControlType"].Value = "Label";
                int idxNo = 0;
                foreach (Node idx in tmp["Methods"])
                {
                    d["Rows"]["Row" + idxNo]["ID"].Value = idx.DNA;
                    d["Rows"]["Row" + idxNo]["MethodName"].Value = idx["MethodName"].Get<string>();
                    d["Rows"]["Row" + idxNo]["EventName"].Value = idx["ActiveEventName"].Get<string>();
                    idxNo += 1;
                }
                grd.DataSource = d;
                grd.Rebind();
            }
        }

        private void AnimateDetails()
        {
            if (infoContent.Visible == false)
            {
                infoContent.Visible = true;
                infoContent.Style[Styles.display] = "none";
                new EffectFadeOut(infoContentTip, 300)
                    .ChainThese(
                    new EffectFadeIn(infoContent, 500))
                    .Render();
            }
            else
            {
                infoContentTip.Visible = false;
            }
        }

        private Node Data
        {
            get { return ViewState["Data"] as Node; }
            set { ViewState["Data"] = value; }
        }

        public void InitialLoading(Node node)
        {
            _node = node.UnTie();
            headerLbl.Text = node["FileFullPath"].Get<string>();
        }
    }
}

