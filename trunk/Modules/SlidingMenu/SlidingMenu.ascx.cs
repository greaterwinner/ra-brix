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
using Ra.Selector;
using Ra.Effects;
using System.Web.UI.HtmlControls;
using Ra;
using SettingsRecords;

namespace SlidingMenuModules
{
    [ActiveModule]
    public class SlidingMenu : System.Web.UI.UserControl, IModule
    {
        protected global::Ra.Extensions.Widgets.SlidingMenu menu;
        protected global::Ra.Extensions.Widgets.SlidingMenuLevel root;
        protected global::Ra.Widgets.TextBox search;
        protected global::System.Web.UI.WebControls.Repeater repSearch;
        protected global::Ra.Widgets.Panel pnlSearchResults;

        // Menu items retrieved from static module event handlers...
        readonly Node _items = new Node("Root Menu");

        private string _breadCrumbID;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (_breadCrumbID != null)
                BreadCrumbID = _breadCrumbID;
            if (!string.IsNullOrEmpty(BreadCrumbID))
            {
                menu.BreadCrumbControl = Selector.FindControl<RaWebControl>(Page, BreadCrumbID);
            }
            search.Visible = Settings.Instance.Get("PortalGlobalSearch", true);
            BuildRootMenu();
        }

        private void SetActiveLevel(string id)
        {
            string whatToFind = "url:~" + id + ".aspx";
            Node node = FindNode(_items, whatToFind);
            string idOfControl = "node" + node.DNA.Replace("-", "x");
            SlidingMenuItem item = Selector.FindControl<SlidingMenuItem>(root, idOfControl);
            SlidingMenuLevel level;
            if (item.Controls.Count > 0)
            {
                level = Selector.SelectFirst<SlidingMenuLevel>(item);
            }
            else
            {
                level = item.Parent as SlidingMenuLevel;
            }
            menu.ExpandTo(level);
        }

        private static Node FindNode(Node node, string whatToFind)
        {
            if (node.Get<string>() == whatToFind)
            {
                return node;
            }
            foreach (Node idx in node)
            {
                Node tmp = FindNode(idx, whatToFind);
                if (tmp != null)
                {
                    return tmp;
                }
            }
            return null;
        }

        [ActiveEvent(Name = "GetTipOfToday")]
        protected static void GetTipOfToday(object sender, ActiveEventArgs e)
        {
            const string tmp = @"
The Sliding Menu - which you use to navigate - have a search function, which you can use
for faster access to menu items, if you know what you're searching for.
";
            e.Params["Tip"]["TipOfSlidingMenu"].Value = Language.Instance["TipOfSlidingMenu", null, tmp];
        }

        protected void searchBtn_Click(object sender, EventArgs e)
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
            search.Select();
            search.Focus();
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
            string nodeValue = node.Get<string>();
            if (nodeValue.IndexOf("url:") == 0)
            {
                string url = nodeValue.Replace("url:", "");
                AjaxManager.Instance.Redirect(url);
            }
            else
            {
                Node nodeEvt = new Node();
                nodeEvt["MenuEventName"].Value = nodeValue;
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
        }

        private void SearchNode(Node idx, ICollection<Node> itemsMatching, string query)
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

        [ActiveEvent("GetHelpContents")]
        protected static void GetHelpContents(object sender, ActiveEventArgs e)
        {
            e.Params[Language.Instance["SlidingMenuHelpLabel", null, "About the Menu"]].Value = "Help-AboutTheMenu";
        }

        [ActiveEvent("Help-AboutTheMenu")]
        protected static void Help_AboutTheMenu(object sender, ActiveEventArgs e)
        {
            const string slidingMenuHelp = @"
<p>
The Sliding Menu is the thing to the left which makes navigation possible. It might seem different at 
first, but will hopefully prove invaliable and quite intuitve after you've clicked it a couple of times.
</p>
<p>
The Sliding Menu basically works such that when you click an item, if that item has child menu items, 
then those child menu items will ""fade in"" and be shown and ""push"" the old items to the left.
</p>
<p>
Menu Items can also have advanced controls contained inside of them, if they do you can interact directly
with those controls inside of the menu instead of having to open up the application or module that you
would have had to done in other similar applications.
</p>
";
            e.Params["Text"].Value = Language.Instance["SlidingMenuHelp", null, slidingMenuHelp];
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
            SlidingMenuItem item = new SlidingMenuItem {ID = "node" + node.DNA.Replace("-", "x")};

            if (Settings.Instance["UseFlatMenu"] == "True")
            {
                item.CssClass = "sliding-item menuFlat";
            }

            if (node.Get<string>().IndexOf("url:") == 0)
            {
                HtmlGenericControl btn = new HtmlGenericControl("span");
                btn.Attributes.Add("class", Settings.Instance["UseFlatMenu"] == "True" ?
                    "buttonFlat menuBtn" :
                    "button menuBtn");

                HtmlGenericControl right = new HtmlGenericControl("span");
                right.Attributes.Add("class", "bRight");
                btn.Controls.Add(right);

                HtmlGenericControl left = new HtmlGenericControl("span");
                left.Attributes.Add("class", "bLeft");
                right.Controls.Add(left);

                HtmlGenericControl center = new HtmlGenericControl("span");
                center.Attributes.Add("class", "bCenter");
                left.Controls.Add(center);

                HtmlGenericControl cont = new HtmlGenericControl("span");
                center.Controls.Add(cont);

                HtmlAnchor a = new HtmlAnchor
                {
                    HRef = node.Get<string>().Replace("url:", ""),
                    InnerHtml = Language.Instance[node.Name]
                };
                a.Attributes.Add("class", "menuLink");
                center.Controls.Add(a);

                item.Content.Controls.Add(btn);

                if (node.Count > 0)
                {
                    SlidingMenuLevel l = new SlidingMenuLevel
                    {
                        Caption = a.InnerHtml,
                        ID = "ch" + node.DNA.Replace("-", "x")
                    };
                    item.Controls.Add(l);
                    foreach (Node idx in node)
                    {
                        CreateSingleMenuItem(idx, l);
                    }
                }
            }
            else
            {
                // Creating a button inside to make the menu appear more beautiful...
                ExtButton btn = new ExtButton {ID = "b" + item.ID};
                string btnText = Language.Instance[node.Name];
                btn.Text = btnText;
                btn.CssClass = Settings.Instance["UseFlatMenu"] == "True" ? 
                    "buttonFlat menuBtn" : 
                    "button menuBtn";
                item.Content.Controls.Add(btn);

                bool hasTrigger = false;

                if (node.Count > 0)
                {
                    // Checking to see if the only child node is the "params node"...
                    if (!(node.Count == 1 && node["Params"].Value != null))
                    {
                        SlidingMenuLevel l = new SlidingMenuLevel
                        {
                            Caption = btnText,
                            ID = "ch" + node.DNA.Replace("-", "x")
                        };
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
                
            }
            parent.Controls.Add(item);
        }

        private void l_GetChildNodes(object sender, EventArgs e)
        {
            SlidingMenuLevel l = sender as SlidingMenuLevel;
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

        private ExtButton PreviouslySelectedButton
        {
            get
            {
                if (ViewState["PreviouslySelectedButton"] == null)
                    return null;
                return Selector.FindControl<ExtButton>(this, ViewState["PreviouslySelectedButton"].ToString());
            }
            set
            {
                ViewState["PreviouslySelectedButton"] = value == null ? null : value.ID;
            }
        }

        protected void menu_ItemClicked(object sender, EventArgs e)
        {
            SlidingMenuItem item = sender as SlidingMenuItem;
            if (item == null)
            {
                return;
            }
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
                node["MenuText"].Value = Selector.SelectFirst<ExtButton>(item).Text;
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
            ExtButton btn = Selector.SelectFirst<ExtButton>(item);
            if (btn == null)
                return;
            if (Settings.Instance["UseFlatMenu"] == "True")
                return;
            btn.CssClass = "button button-selected menuBtn";
            if (PreviouslySelectedButton != null && PreviouslySelectedButton.ID != btn.ID)
            {
                PreviouslySelectedButton.CssClass = "mumbo";
                PreviouslySelectedButton.CssClass = "button menuBtn";
            }
            PreviouslySelectedButton = btn;
        }

        private string BreadCrumbID
        {
            get { return ViewState["BreadCrumbID"] as string; }
            set { ViewState["BreadCrumbID"] = value; }
        }

        public void InitialLoading(Node node)
        {
            _breadCrumbID = ((RaWebControl)node["CustomBreadCrumb"].Value).ID;
            Load +=
                delegate
                    {
                        string id = Request.Params["ContentID"];
                        if (id != null)
                        {
                            SetActiveLevel(id);
                        }
                    };
        }

        public string GetCaption()
        {
            return "";
        }
    }
}
















