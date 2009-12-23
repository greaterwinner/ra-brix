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
using LanguageRecords;
using System.Collections.Generic;
using Ra.Widgets;
using Ra.Brix.Loader;
using Ra.Effects;
using ASP = System.Web.UI.WebControls;
using Ra.Brix.Types;
using Ra.Extensions.Widgets;

namespace RolesModules
{
    [ActiveModule]
    public class AccessControl : UserControl, IModule
    {
        protected global::Ra.Extensions.Widgets.Tree tree;
        protected global::Ra.Extensions.Widgets.TreeNodes root;
        protected global::Ra.Widgets.Label lblNodeName;
        protected global::Ra.Widgets.SelectList selectRoles;
        protected global::Ra.Widgets.Panel editWrp;
        protected global::Ra.Widgets.Panel wholeUnit;
        protected global::System.Web.UI.WebControls.Repeater grd;
        protected global::Ra.Widgets.Panel grdWrp;

        protected void Page_Load(object sender, EventArgs e)
        {
            selectRoles.Items[0].Text = Language.Instance["AccessControlListItem", null, "Choose role..."];
            BuildLevel(root, MenuItems);
        }

        [ActiveEvent(Name = "GetTipOfToday")]
        protected static void GetTipOfToday(object sender, ActiveEventArgs e)
        {
            const string tmp = @"
You can fine-grain the access to every item in your portal according to which roles your
user belongs to. This way you can have multiple different roles, which each have access to
different things.

Then users can be members of roles which have access - or don't have access - to specific 
features of the Portal.

This is being done through the Roles/Access Control menu item in the portal.
";
            e.Params["Tip"]["TipOfAccessControl"].Value = Language.Instance["TipOfAccessControl", null, tmp];
        }

        private void BuildLevel(Control level, IEnumerable<Node> current)
        {
            foreach (Node idx in current)
            {
                TreeNode t = new TreeNode {ID = "n" + idx.DNA.Replace("-", ""), Xtra = idx.DNA};
                if (idx.Count > 0)
                {
                    if (idx.Count > 1 || idx[0].Name != "Params")
                    {
                        TreeNodes children = new TreeNodes {Xtra = idx.DNA, ID = "l" + idx.DNA.Replace("-", "x")};
                        children.GetChildNodes +=
                            delegate(object sender, EventArgs e)
                            {
                                TreeNodes caller = sender as TreeNodes;
                                if (caller == null) 
                                    return;
                                string dna = caller.Xtra;
                                Node tmp = MenuItems.Find(dna);
                                BuildLevel(caller, tmp);
                            };
                        t.Controls.Add(children);
                    }
                }

                // Creating the LinkButton as a child of TreeNode which is the thing 
                // you click to actually "select" a node...
                LinkButton btn = new LinkButton
                {
                    ID = "b" + idx.DNA.Replace("-", ""),
                    Text = Language.Instance[idx.Name],
                    CssClass = "treeLinkButton",
                    Xtra = idx.DNA
                };
                btn.Click += btn_Click;
                t.Controls.Add(btn);

                // Adding TreeNode to parent...
                level.Controls.Add(t);
            }
        }

        private Node MenuItems
        {
            get { return Session["RolesModules.AccessControl.MenuItems"] as Node; }
            set { Session["RolesModules.AccessControl.MenuItems"] = value; }
        }

        private Node MenuAccess
        {
            get { return Session["RolesModules.AccessControl.MenuAccess"] as Node; }
            set { Session["RolesModules.AccessControl.MenuAccess"] = value; }
        }

        private string SelectedNode
        {
            get { return ViewState["SelectedNode"] as string; }
            set { ViewState["SelectedNode"] = value; }
        }

        protected void tree_SelectedNodeChanged(object sender, EventArgs e)
        {
            TreeNode treeNode = tree.SelectedNodes[0];
            SelectedNode = treeNode.Xtra;
            Node node = MenuItems.Find(SelectedNode);
            SetSelectedNodeForEditing(node);
        }

        protected void btn_Click(object sender, EventArgs e)
        {
            LinkButton btn = sender as LinkButton;
            if (btn == null)
            {
                return;
            }
            SelectedNode = btn.Xtra;
            Node node = MenuItems.Find(SelectedNode);
            SetSelectedNodeForEditing(node);

            // Setting selected node to parent of button clicked...
            TreeNode treeNode = btn.Parent as TreeNode;
            tree.SelectedNodes = new[] { treeNode };
        }

        private void SetSelectedNodeForEditing(Node node)
        {
            lblNodeName.Text = Language.Instance[node.Name];
            if (editWrp.Style[Styles.display] == "none")
            {
                // Initial showing...
                new EffectFadeIn(editWrp, 500)
                    .JoinThese(new EffectRollDown())
                    .Render();
            }
            DataBindAccessGrid(node);
        }

        private void DataBindAccessGrid(Node node)
        {
            List<Node> accessNodesForMenu = new List<Node>();
            foreach (Node idxMenuAccess in MenuAccess)
            {
                if (idxMenuAccess["MenuValue"].Get<string>() == node.Get<string>())
                {
                    if( idxMenuAccess["Params"].Value == null ||
                        (node["Params"].Value != null && 
                            idxMenuAccess["Params"].Get<string>() == node["Params"].Get<string>()))
                    accessNodesForMenu.Add(idxMenuAccess["RoleName"]);
                }
            }
            grd.DataSource = accessNodesForMenu;
            grd.DataBind();
            grdWrp.ReRender();
        }

        protected void selectRoles_SelectedIndexChanged(object sender, EventArgs e)
        {
            Node node = MenuItems.Find(SelectedNode);
            string roleName = selectRoles.SelectedItem.Value;
            string menuValue = node.Get<string>();
            selectRoles.SelectedIndex = 0;

            string pars = null;
            if (node.Count > 0 && node[0].Name == "Params")
                pars = node["Params"].Get<string>();

            Node signalNode = new Node();
            signalNode["MenuValue"].Value = menuValue;
            signalNode["RoleName"].Value = roleName;
            signalNode["Params"].Value = pars;
            Node tmpParent = node.Parent;
            Node tmpNode = signalNode;
            while (tmpParent != null)
            {
                tmpNode = tmpNode["Parents"];
                tmpNode["RoleName"].Value = roleName;
                tmpNode["MenuValue"].Value = tmpParent.Get<string>();
                tmpParent = tmpParent.Parent;
            }
            ActiveEvents.Instance.RaiseActiveEvent(
                this, 
                "CreateNewRoleMenuMapping",
                signalNode);

            if (signalNode["Access"].Count > 0)
            {
                // Rebinding grid again
                MenuAccess = signalNode["Access"].UnTie();
                Node activeNode = MenuItems.Find(SelectedNode);
                DataBindAccessGrid(activeNode);
            }
        }

        protected void DeleteRoleMenuMapping(object sender, EventArgs e)
        {
            LinkButton btn = sender as LinkButton;
            if (btn == null)
            {
                return;
            }
            string roleName = btn.Xtra;

            Node treeSelectedNode = MenuItems.Find(SelectedNode);
            string menuValue = treeSelectedNode.Get<string>();

            string pars = null;
            if (treeSelectedNode.Count > 0 && treeSelectedNode[0].Name == "Params")
                pars = treeSelectedNode["Params"].Get<string>();

            Node node = new Node();
            node["MenuValue"].Value = menuValue;
            node["RoleName"].Value = roleName;
            node["Params"].Value = pars;
            ActiveEvents.Instance.RaiseActiveEvent(
                this,
                "DeleteRoleMenuMapping",
                node);

            if (node["Access"].Value != null)
            {
                // Rebinding grid again
                MenuAccess = node["Access"].UnTie();
                Node activeNode = MenuItems.Find(SelectedNode);
                DataBindAccessGrid(activeNode);
            }
        }

        public void InitialLoading(Node init)
        {
            MenuItems = init["MenuItems"].UnTie();
            MenuAccess = init["Access"].UnTie();

            // Populating SelectList...
            foreach (Node idx in init["Roles"])
            {
                ListItem l = new ListItem {Value = idx.Get<string>(), Text = idx.Get<string>()};
                selectRoles.Items.Add(l);
            }
        }

        public string GetCaption()
        {
            return "";
        }
    }
}
















