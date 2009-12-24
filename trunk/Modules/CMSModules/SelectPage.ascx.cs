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
using System.Collections.Generic;
using System.Web.UI;
using Ra.Brix.Loader;
using Ra.Brix.Types;
using Ra.Extensions.Widgets;

namespace CMSModules
{
    [ActiveModule]
    public class SelectPage : System.Web.UI.UserControl, IModule
    {
        protected global::Ra.Extensions.Widgets.Tree tree;
        protected global::Ra.Extensions.Widgets.TreeNodes root;

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
            Node node = new Node();
            node["Text"].Value = sel.Text;
            node["Xtra"].Value = sel.Xtra;
            ActiveEvents.Instance.RaiseActiveEvent(
                this, 
                "CMSSetActiveEditingPage",
                node);
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

        private static void AddTreeNode(IEnumerable<Node> node, Control level)
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