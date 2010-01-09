/*
 * Ra-Brix - A Modular-based Framework for building 
 * Web Applications Copyright 2009 - Thomas Hansen 
 * thomas@ra-ajax.org. Unless permission is 
 * explicitly given this code is licensed under the 
 * GNU Affero GPL version 3 which can be found in the 
 * license.txt file on disc.
 * 
 */


using Ra.Brix.Loader;
using Ra.Brix.Types;
using ForumRecords;
using Ra.Extensions.Widgets;
using System.Collections.Generic;
using System;
using Ra;
using Ra.Brix.Data;
using Ra.Effects;
using Ra.Widgets;
using Ra.Selector;
using LanguageRecords;
using HelperGlobals;
using UserRecords;

namespace ForumModules
{
    [ActiveModule]
    public class ShowPosts : System.Web.UI.UserControl, IModule
    {
        protected global::Ra.Extensions.Widgets.Tree tree;
        protected global::Ra.Extensions.Widgets.TreeNodes root;
        protected global::Ra.Widgets.Panel pnlWrp;
        protected global::Ra.Widgets.TextBox header;
        protected global::Ra.Widgets.TextArea body;
        protected global::Ra.Extensions.Widgets.Window replyWnd;
        protected global::Ra.Widgets.TextBox headerReply;
        protected global::Ra.Widgets.TextArea bodyReply;
        protected global::Ra.Widgets.TextBox anonTxt;
        protected global::Ra.Widgets.Label lblLoggedInUsername;
        protected global::Ra.Widgets.Label count;
        private bool _firstRequest;
        private int _commentCount;

        protected void Page_Load(object sender, EventArgs e)
        {
            anonTxt.Visible = string.IsNullOrEmpty(Users.LoggedInUserName);
            lblLoggedInUsername.Visible = !string.IsNullOrEmpty(Users.LoggedInUserName);
            if (!string.IsNullOrEmpty(Users.LoggedInUserName))
            {
                lblLoggedInUsername.Text = Language.Instance["LoggedInAs", null, "Logged in as: "] + 
                    Users.LoggedInUserName;
            }
            if (!_firstRequest)
            {
                InitializeForum(-1);
            }
        }

        public void InitialLoading(Node node)
        {
            _firstRequest = true;
            Load +=
                delegate
                {
                    Forum forum = node["Forum"].Get<Forum>();
                    Main = forum;
                    InitializeForum(-1);
                    pnlWrp.DataBind();
                };
        }

        private Forum Main
        {
            get { return Forum.SelectByID((int)ViewState["Forum"]); }
            set { ViewState["Forum"] = value.ID; }
        }

        protected void submit_Click(object sender, EventArgs e)
        {
            ForumPost p = new ForumPost();
            p.Header = header.Text.Replace("<", "&lt;").Replace(">", "&gt;");
            p.Body = body.Text;
            p.When = DateTime.Now;
            p.Name = anonTxt.Text;
            if (Users.LoggedInUserName != null)
                p.RegisteredUser = User.SelectFirst(Criteria.Eq("Username", Users.LoggedInUserName));
            Main.Posts.Add(p);
            Main.Save();
            root.Controls.Clear();
            InitializeForum(p.ID);
            root.ReRender();
            new EffectHighlight(tree, 500)
                .Render();
            body.Text = "";
            header.Text = "";
        }

        private void InitializeForum(int commentToShow)
        {
            CreateLevel(root, Main.Posts, commentToShow, 0);
            count.Text = _commentCount.ToString() + Language.Instance["CommentCount", null, " comments in article..."];
            int treeMinHeight = (_commentCount * 15) + 250;
            tree.Style[Styles.minHeight] = treeMinHeight.ToString() + "px";
        }

        private void CreateLevel(TreeNodes level, IList<ForumPost> list, int commentToShow, int curLevel)
        {
            curLevel += 1;
            foreach (ForumPost idx in list)
            {
                _commentCount += 1;
                TreeNode n = new TreeNode();
                n.ID = "pst" + idx.ID;
                if (curLevel > 11)
                {
                    n.Load +=
                        delegate(object sender, EventArgs e)
                        {
                            TreeNode n1 = sender as TreeNode;
                            if (!n1.CssClass.Contains("tree-more"))
                                n1.CssClass += " tree-more";
                        };
                }

                Label l = new Label();
                l.ID = "hdr" + idx.ID;
                l.Text = idx.Header;
                l.CssClass = "headerTxtComment";
                l.Style[Styles.width] = (350 - (Math.Min(curLevel, 11) * 16)) + "px";
                n.Controls.Add(l);

                Label lblDate = new Label();
                lblDate.ID = "ldta" + idx.ID;
                lblDate.CssClass = "dateLbl";
                DateTime dateTmp = idx.When;

                // Intentionally deferring Text property to get an *UPDATED* text value of time...!
                lblDate.Load +=
                    delegate(object sender, EventArgs e)
                    {
                        (sender as Label).Text = DateFormatter.FormatDate(dateTmp);
                    };
                n.Controls.Add(lblDate);

                Label lblUser = new Label();
                lblUser.ID = "lusr" + idx.ID;
                lblUser.CssClass = "userLbl";
                lblUser.Text = idx.GetNameOfPoster();
                n.Controls.Add(lblUser);

                Panel pnl = new Panel();
                pnl.ID = "pnlWrp" + idx.ID;
                pnl.CssClass = "bodyOfCommentWrp";

                Panel pnlInner = new Panel();
                pnlInner.ID = "bdy" + idx.ID;
                if (idx.ID == commentToShow)
                {
                    pnl.Xtra = "visible";
                    PreviouslyShownComment = pnl.ID;
                }
                else
                    pnl.Style[Styles.display] = "none";
                pnlInner.CssClass = "bodyOfComment";
                pnlInner.Style[Styles.left] = ((Math.Min(curLevel, 11) * 16)).ToString() + "px";
                pnlInner.Style[Styles.marginRight] = (25 + (Math.Min(curLevel, 11) * 16)).ToString() + "px";
                Label ltext = new Label();
                ltext.ID = "lTxt" + idx.ID;
                string bodyStr = idx.Body.Trim().Replace("\r\n", "\n");
                bool hasFound = true;
                while (hasFound)
                {
                    if (bodyStr.Contains("\n\n"))
                    {
                        bodyStr = bodyStr.Replace("\n\n", "\n");
                    }
                    else
                        hasFound = false;
                }
                string bodyStrFormatted = "<p";
                bool hasOpened = false;
                bool isFirst = true;
                foreach (char idxChar in bodyStr)
                {
                    if (hasOpened || isFirst)
                    {
                        isFirst = false;
                        if (idxChar == ':')
                            bodyStrFormatted += " class=\"quote\">";
                        else
                            bodyStrFormatted += " class=\"no-quote\">";
                        hasOpened = false;
                        bodyStrFormatted += idxChar;
                    }
                    else if (idxChar == '\n')
                    {
                        bodyStrFormatted += "</p><p";
                        hasOpened = true;
                    }
                    else
                    {
                        bodyStrFormatted += idxChar;
                    }
                }
                bodyStrFormatted += "</p>";
                ltext.Text = bodyStrFormatted;
                pnlInner.Controls.Add(ltext);
                pnl.Controls.Add(pnlInner);
                n.Controls.Add(pnl);

                ExtButton replyButton = new ExtButton();
                replyButton.ID = "btn" + idx.ID;
                replyButton.CssClass = "button downRight";
                replyButton.Xtra = idx.ID.ToString();
                replyButton.Click += replyButton_Clicked;
                replyButton.Text = Language.Instance["Reply", null, "Reply"];
                pnlInner.Controls.Add(replyButton);

                level.Controls.Add(n);
                if (idx.Replies.Count > 0)
                {
                    if (curLevel <= 10)
                    {
                        TreeNodes children = new TreeNodes();
                        children.Expanded = true;
                        children.ID = "chl" + idx.ID;
                        CreateLevel(children, idx.Replies, commentToShow, curLevel);
                        n.Controls.Add(children);
                    }
                    else
                    {
                        // We do NOT indent more than 10 levels...!
                        // After 10 levels, we get a funny folder icon to indicate 
                        // that we're at more than 10 levels...!
                        CreateLevel(level, idx.Replies, commentToShow, curLevel);
                    }
                }
            }
        }

        protected void headerReply_EscPressed(object sender, EventArgs e)
        {
            replyWnd.Visible = false;
        }

        void replyButton_Clicked(object sender, EventArgs e)
        {
            ExtButton b = sender as ExtButton;
            int idOfPost = int.Parse(b.Xtra);
            replyWnd.Visible = true;
            replyWnd.Xtra = idOfPost.ToString();
            replyWnd.Caption = Language.Instance["ReplyToComment", null, "Reply to comment"];
            replyWnd.DataBind();
            ForumPost p = ForumPost.SelectByID(idOfPost);
            if (p.Header.IndexOf("Re: ") == 0)
                headerReply.Text = p.Header;
            else
                headerReply.Text = "Re: " + p.Header;
            string bodyStr = p.Body.Trim().Replace("\r\n", "\n");
            bool hasFound = true;
            while (hasFound)
            {
                if (bodyStr.Contains("\n\n"))
                {
                    bodyStr = bodyStr.Replace("\n\n", "\n");
                }
                else
                    hasFound = false;
            }
            string bodyFormatted = ":";
            foreach (char idxChar in bodyStr)
            {
                if (idxChar == '\n')
                    bodyFormatted += "\n:";
                else
                    bodyFormatted += idxChar;

            }
            bodyReply.Text = bodyFormatted;
            headerReply.Select();
            headerReply.Focus();
        }

        protected void submitComment_Click(object sender, EventArgs e)
        {
            replyWnd.Visible = false;
            int idOfPost = int.Parse(replyWnd.Xtra);
            string headerTxt = headerReply.Text.Replace("<", "&lt;").Replace(">", "&gt;");
            string bodyTxt = bodyReply.Text.Replace("<", "&lt;").Replace(">", "&gt;");
            ForumPost n = new ForumPost();
            n.Body = bodyTxt;
            n.Header = headerTxt;
            n.When = DateTime.Now;
            n.Name = anonTxt.Text;
            if (Users.LoggedInUserName != null)
                n.RegisteredUser = User.SelectFirst(Criteria.Eq("Username", Users.LoggedInUserName));
            ForumPost parent = ForumPost.SelectByID(idOfPost);
            parent.Replies.Add(n);
            parent.Save();
            root.Controls.Clear();
            InitializeForum(n.ID);
            root.ReRender();
            new EffectHighlight(tree, 500)
                .Render();
        }

        private string PreviouslyShownComment
        {
            get { return ViewState["PreviouslyShownComment"] as string; }
            set { ViewState["PreviouslyShownComment"] = value; }
        }

        protected void menu_SelectedNodeChanged(object sender, EventArgs e)
        {
            TreeNode node = tree.SelectedNodes[0];
            Panel lblBody = Selector.SelectFirst<Panel>(node,
                delegate(System.Web.UI.Control idx)
                {
                    if (idx is RaWebControl)
                    {
                        if ((idx as RaWebControl).CssClass == "bodyOfCommentWrp")
                            return true;
                    }
                    return false;
                });
            if (lblBody.Xtra != "visible")
            {
                lblBody.Xtra = "visible";
                if (!string.IsNullOrEmpty(PreviouslyShownComment))
                {
                    Panel previous = Selector.FindControl<Panel>(tree, PreviouslyShownComment);
                    previous.Xtra = "";
                    new EffectRollUp(previous, 500)
                        .JoinThese(
                            new EffectFadeOut())
                        .ChainThese(
                            new EffectRollDown(lblBody, 500)
                                .JoinThese(
                                    new EffectFadeIn())
                        )
                        .Render();
                }
                else
                {
                    new EffectRollDown(lblBody, 500)
                        .JoinThese(new EffectFadeIn())
                        .Render();
                }
                PreviouslyShownComment = lblBody.ID;
            }
            else
            {
                lblBody.Xtra = "";
                new EffectRollUp(lblBody, 500)
                    .JoinThese(
                        new EffectFadeOut())
                    .Render();
                PreviouslyShownComment = null;
            }
        }

        public string GetCaption()
        {
            return "";
        }
    }
}