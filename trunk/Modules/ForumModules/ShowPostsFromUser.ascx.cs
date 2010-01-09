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
using System.Text.RegularExpressions;

namespace ForumModules
{
    [ActiveModule]
    public class ShowPostsFromUser : System.Web.UI.UserControl, IModule
    {
        protected global::Ra.Extensions.Widgets.Tree tree;
        protected global::Ra.Extensions.Widgets.TreeNodes root;
        protected global::Ra.Widgets.Label count;
        private bool _isFirstLoad;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!_isFirstLoad)
            {
                LoadPostsFromUser();
            }
        }

        private void LoadPostsFromUser()
        {
            int commentCount = 0;
            foreach (ForumPost idx in ForumPost.Select(
                Criteria.Eq("RegisteredUser.Username", Username),
                Criteria.Sort("When", false)))
            {
                if (commentCount > 100)
                    break;
                TreeNode n = new TreeNode();
                n.ID = "cmt" + idx.ID;

                Label l = new Label();
                l.ID = "hdr" + idx.ID;
                l.Text = idx.Header;
                l.CssClass = "headerTxtComment";
                l.Style[Styles.width] = "350px";
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

                // Actual comment text
                Panel pnl = new Panel();
                pnl.ID = "pnlWrp" + idx.ID;
                pnl.CssClass = "bodyOfCommentWrp";

                Panel pnlInner = new Panel();
                pnlInner.ID = "bdy" + idx.ID;
                pnl.Style[Styles.display] = "none";
                pnlInner.CssClass = "bodyOfComment";
                pnlInner.Style[Styles.left] = "16px";
                pnlInner.Style[Styles.marginRight] = "41px";
                Label ltext = new Label();
                ltext.ID = "lTxt" + idx.ID;
                string bodyStr = idx.Body.Trim().Replace("\r\n", "\n");
                bodyStr = FormatComment(bodyStr);
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

                root.Controls.Add(n);
                commentCount += 1;
            }

            // Setting tree minimum height (to avoid flashing while animated)
            int treeMinHeight = (commentCount * 15) + 250;
            tree.Style[Styles.minHeight] = treeMinHeight.ToString() + "px";

            // Setting count
            count.Text = commentCount.ToString() + Language.Instance["CommentsByUser", null, " comments by user"];
        }

        private string FormatComment(string content)
        {
            content = Regex.Replace(
                content.Replace("http://", " http://"),
                "(?<spaceChar>\\s+)(?<linkType>http://|https://)(?<link>\\S+)",
                "${spaceChar}<a href=\"${linkType}${link}\" rel=\"nofollow\">${link}</a>",
                RegexOptions.Compiled).Replace(" <a", "<a");
            content = Regex.Replace(content,
                "(?<begin>\\*{1})(?<content>.+?)(?<end>\\*{1})",
                "<strong>${content}</strong>",
                RegexOptions.Compiled);
            return content;
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
                new EffectRollDown(lblBody, 500)
                    .JoinThese(
                        new EffectFadeIn())
                    .Render();
            }
            else
            {
                lblBody.Xtra = "";
                new EffectRollUp(lblBody, 500)
                    .JoinThese(
                        new EffectFadeOut())
                    .Render();
            }
        }

        private string Username
        {
            get { return ViewState["Username"] as string; }
            set { ViewState["Username"] = value; }
        }

        public void InitialLoading(Node node)
        {
            _isFirstLoad = true;
            Load +=
                delegate
                {
                    Username = node["Username"].Get<string>();
                    LoadPostsFromUser();
                };
        }

        public string GetCaption()
        {
            return "";
        }
    }
}