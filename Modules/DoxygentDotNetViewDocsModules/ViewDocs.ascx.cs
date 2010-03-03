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
using System.Collections.Generic;
using Doxygen.NET;
using System.IO;
using Ra.Widgets;

namespace DoxygentDotNetViewDocsModules
{
    [ActiveModule]
    public class ViewDocs : System.Web.UI.UserControl, IModule
    {
        protected global::Ra.Extensions.Widgets.TreeNodes root;
        protected global::Ra.Extensions.Widgets.TreeNodes rootTutorials;
        protected global::Ra.Extensions.Widgets.Tree tree;
        protected global::Ra.Widgets.TextBox filter;

        private void Page_Load(object sender, EventArgs e)
        {
            BuildRoot();
            BuildRootTutorials();
        }

        private void BuildRoot()
        {
            List<TreeNode> l = new List<TreeNode>();
            foreach (Class idx in RaDocs.GetAllClasses())
            {
                switch (idx.FullName)
                {
                    // removing the classes we don't really NEED documentation for...
                    case "Ra.Extensions.Helpers.CometQueue":
                    case "Ra.Core.CallbackFilter":
                    case "Ra.Extensions.Widgets.AccordionView.EffectChange":
                    case "Ra.Extensions.Widgets.AutoCompleter.RetrieveAutoCompleterItemsEventArgs":
                    case "Ra.Core.PostbackFilter":
                    case "Ra.Widgets.ListItemCollection":
                    case "Ra.Widgets.StyleCollection":
                    case "Ra.Widgets.StyleCollection.StyleValue":
                    case "Ra.Widgets.ListItem":
                        break;
                    default:
                        {
                            if ((!string.IsNullOrEmpty(Filter) && 
                                idx.FullName.ToLower().Contains(Filter.ToLower())) || 
                                (string.IsNullOrEmpty(Filter)))
                            {
                                TreeNode n = new TreeNode();
                                n.ID = idx.FullName;
                                if (File.Exists(Server.MapPath("~/Docs-Controls/" + idx.FullName + ".ascx")))
                                {
                                    n.CssClass += " hasSample";
                                    n.Tooltip = "Have sample code";
                                }
                                else
                                    n.CssClass += " noSample";
                                n.Text = idx.FullName;
                                l.Add(n);
                            }
                        } break;
                }
            }
            l.Sort(
                delegate(TreeNode left, TreeNode right)
                {
                    return left.Text.CompareTo(right.Text);
                });
            foreach (TreeNode idx in l)
            {
                root.Controls.Add(idx);
            }
        }

        private void BuildRootTutorials()
        {
            foreach (string idx in Directory.GetFiles(Server.MapPath("~/tutorials/"), "*.txt"))
            {
                if ((!string.IsNullOrEmpty(Filter) &&
                    idx.ToLower().Contains(Filter.ToLower())) ||
                    (string.IsNullOrEmpty(Filter)))
                {
                    TreeNode n = new TreeNode();
                    n.ID = "tutorial_" + idx.Replace(".txt", "").Replace(" ", "_").Replace(Server.MapPath("~/tutorials/"), "");
                    n.Text = idx.Replace(".txt", "").Replace(Server.MapPath("~/tutorials/"), "");
                    n.Xtra = idx;
                    n.CssClass = "noSample";
                    rootTutorials.Controls.Add(n);
                }
            }
        }

        private Docs RaDocs
        {
            get
            {
                if (Application["RaDocs"] == null)
                    Application["RaDocs"] = new Docs(Server.MapPath("~/docs-xml"));
                return (Docs)Application["RaDocs"];
            }
        }

        private string Filter
        {
            get { return ViewState["Filter"] == null ? "" : (string)ViewState["Filter"]; }
            set { ViewState["Filter"] = value; }
        }

        protected void filterSearch_Click(object sender, EventArgs e)
        {
            Filter = filter.Text;

            // Classes...
            root.Controls.Clear();
            BuildRoot();
            if (root.Controls.Count > 0)
                root.RollDown();
            tree.ReRender();

            // Tutorials...
            rootTutorials.Controls.Clear();
            BuildRootTutorials();
            if (rootTutorials.Controls.Count > 0)
                rootTutorials.RollDown();

            filter.Select();
            filter.Focus();
        }

        protected void tree_SelectedNodeChanged(object sender, EventArgs e)
        {
            string itemToLookAt = tree.SelectedNodes[0].ID;
            if (itemToLookAt == "rootNode" || itemToLookAt == "tutorials")
                return;
            if (itemToLookAt.Contains("tutorial_"))
            {
                ShowTutorial(itemToLookAt, tree.SelectedNodes[0].Xtra);
            }
            else
            {
                ShowClass(itemToLookAt);
            }
        }

        private void ShowClass(string itemToLookAt)
        {
            Class classToShow = RaDocs.GetTypeByName(itemToLookAt) as Class;
            Node node = new Node();
            node["Class"].Value = classToShow;
            ActiveEvents.Instance.RaiseActiveEvent(
                this,
                "DoxygentDotNetShowClass",
                node);
        }

        private void ShowTutorial(string itemToLookAt, string fileName)
        {
            string tutorial = itemToLookAt.Replace("tutorial_", "").Replace("_", " ");
            Node node = new Node();
            node["TutorialName"].Value = tutorial;
            node["TutorialFile"].Value = fileName;
            ActiveEvents.Instance.RaiseActiveEvent(
                this,
                "DoxygentDotNetShowTutorial",
                node);
        }

        public void InitialLoading(Node node)
        {
        }
    }
}