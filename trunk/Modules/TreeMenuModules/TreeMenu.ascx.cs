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
using Ra.Brix.Loader;
using System.Collections.Generic;
using Ra.Extensions.Widgets;
using Ra.Brix.Types;
using Ra.Widgets;
using Ra.Effects;
using SettingsRecords;

namespace TreeMenuModules
{
    [ActiveModule]
    public class TreeMenu : UserControl, IModule
    {
        protected global::Ra.Extensions.Widgets.Tree menu;
        protected global::Ra.Extensions.Widgets.TreeNodes root;
        protected global::Ra.Widgets.TextBox search;
        protected global::System.Web.UI.WebControls.Repeater repSearch;
        protected global::Ra.Widgets.Panel pnlSearchResults;

        // Menu items retrieved from static module event handlers...
        readonly Node _items = new Node("Root Menu");

        protected void Page_Load(object sender, EventArgs e)
        {
            search.Visible = Settings.Instance.Get("PortalGlobalSearch", true);
            BuildRootMenu();
        }

        protected void search_KeyUp(object sender, EventArgs e)
        {
            // NOT searching if text is less than 3 characters long...
            if (search.Text.Length < 3)
            {
                pnlSearchResults.Style[Styles.display] = "none";
                return;
            }
            List<Node> itemsMatching = new List<Node>();
            foreach (Node idx in _items)
            {
                SearchNode(idx, itemsMatching, search.Text);
            }
            if (itemsMatching.Count > 0)
            {
                repSearch.DataSource = itemsMatching;
                repSearch.DataBind();
                pnlSearchResults.ReRender();
                pnlSearchResults.Visible = true;
                pnlSearchResults.Style[Styles.display] = "none";
                new EffectFadeIn(pnlSearchResults, 200)
                    .Render();
            }
            else
            {
                pnlSearchResults.Style[Styles.display] = "none";
            }
        }

        protected void SearchClicked(object sender, EventArgs e)
        {
            ExtButton btn = sender as ExtButton;
            if (btn == null)
            {
                return;
            }
            string xtra = btn.Xtra;
            Node node = _items.Find(xtra);
            Node nodeEvt = new Node();
            nodeEvt["MenuEventName"].Value = node.Value;
            nodeEvt["MenuText"].Value = btn.Text;
            if (node.Count == 1 && node[0].Name == "Params")
            {
                nodeEvt["Params"].Value = node[0].Get<string>();
            }
            ActiveEvents.Instance.RaiseActiveEvent(
                this,
                "MenuItemClicked",
                nodeEvt);
            ActiveEvents.Instance.RaiseActiveEvent(
                this,
                node.Get<string>(),
                nodeEvt);

            new EffectFadeOut(pnlSearchResults, 200)
                .ChainThese(
                    new EffectFocusAndSelect(search))
                .Render();
        }

        private static void SearchNode(Node idx, ICollection<Node> itemsMatching, string query)
        {
            if ((idx.Count == 0 || idx.Count == 1 && idx[0].Name == "Params") && 
                Language.Instance[idx.Name].ToLower().Contains(query.ToLower()))
            {
                itemsMatching.Add(idx);
            }
            foreach (Node idx2 in idx)
            {
                SearchNode(idx2, itemsMatching, query);
            }
        }

        [ActiveEvent(Name = "RefreshMenu")]
        protected void RefreshMenu(object sender, ActiveEventArgs e)
        {
            root.Controls.Clear();
            BuildRootMenu();
            menu.ReRender();
        }

        private void BuildRootMenu()
        {
            // Retrieveing items from all modules
            ActiveEvents.Instance.RaiseActiveEvent(
                this,
                "GetMenuItems",
                _items);
            ActiveEvents.Instance.RaiseActiveEvent(
                this,
                "FilterMenuItems",
                _items);

            foreach (Node idx in _items)
            {
                CreateSingleMenuItem(idx, root);
            }
        }

        private void CreateSingleMenuItem(
            Node node,
            Control parent)
        {
            TreeNode item = new TreeNode
            {
                ID = "node" + node.DNA.Replace("-", "x"),
                Text = Language.Instance[node.Name]
            };

            bool hasTrigger = false;

            if (node.Count > 0)
            {
                // Checking to see if the only child node is the "params node"...
                if (!(node.Count == 1 && node["Params"].Value != null))
                {
                    TreeNodes l = new TreeNodes {ID = "ch" + node.DNA.Replace("-", "x")};
                    item.Controls.Add(l);
                    l.Xtra = node.DNA;
                    l.GetChildNodes += l_GetChildNodes;
                    item.Xtra = node.DNA;
                }
                else
                    hasTrigger = true;
            }
            else
            {
                hasTrigger = true;
            }
            if (hasTrigger)
            {
                if (node["Params"].Value != null)
                {
                    item.Xtra = "trigger" +
                        node.Value +
                        "-__params" +
                        node["Params"].Value;
                }
                else
                {
                    item.Xtra = "trigger" + node.Value;
                }
            }
            parent.Controls.Add(item);
        }

        private void l_GetChildNodes(object sender, EventArgs e)
        {
            TreeNodes l = sender as TreeNodes;
            if (l == null)
            {
                return;
            }
            Node node = _items.Find(l.Xtra);
            foreach (Node idx in node)
            {
                // Skipping the "params node"...
                if (idx.Name != "Params")
                    CreateSingleMenuItem(idx, l);
            }
        }

        protected void menu_SelectedNodeChanged(object sender, EventArgs e)
        {
            TreeNode item = menu.SelectedNodes[0];
            if (item.Xtra.IndexOf("trigger") == 0)
            {
                string eventToRaise = item.Xtra.Substring(7);
                string wholeEventToRaise = eventToRaise;
                if (eventToRaise.Contains("-__params"))
                {
                    eventToRaise = eventToRaise.Substring(0, eventToRaise.IndexOf("-__params"));
                }

                // Raising the MenuClicked event...
                Node node = new Node();
                node["MenuEventName"].Value = eventToRaise;
                node["MenuText"].Value = item.Text;
                if (wholeEventToRaise.Contains("-__params"))
                {
                    node["Params"].Value = wholeEventToRaise.Split(new[] { "-__params" }, StringSplitOptions.RemoveEmptyEntries)[1];
                }
                ActiveEvents.Instance.RaiseActiveEvent(
                    this,
                    "MenuItemClicked",
                    node);

                // Raising the event the Node defines in the "Value" property
                node = new Node();
                if (wholeEventToRaise.Contains("-__params"))
                {
                    node["Params"].Value = wholeEventToRaise.Split(new[] { "-__params" }, StringSplitOptions.RemoveEmptyEntries)[1];
                }
                ActiveEvents.Instance.RaiseActiveEvent(
                    this, 
                    eventToRaise,
                    node);
            }
        }

        public void InitialLoading(Node node)
        {
        }

        public string GetCaption()
        {
            return Language.Instance["TreeMenuCaption", null, "Tree Menu"];
        }
    }
}
















