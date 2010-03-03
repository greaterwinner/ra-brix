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
using HelperGlobals;
using System;
using LanguageRecords;
using SettingsRecords;
using System.Configuration;

namespace ArticlePublisherModules
{
    [ActiveModule]
    public class ViewArticle : System.Web.UI.UserControl, IModule
    {
        protected global::System.Web.UI.HtmlControls.HtmlGenericControl header;
        protected global::System.Web.UI.HtmlControls.HtmlGenericControl ingress;
        protected global::System.Web.UI.HtmlControls.HtmlGenericControl date;
        protected global::System.Web.UI.HtmlControls.HtmlGenericControl content;
        protected global::System.Web.UI.HtmlControls.HtmlImage image;
        protected global::System.Web.UI.HtmlControls.HtmlAnchor author;
        protected global::Ra.Extensions.Widgets.ExtButton edit;
        protected global::Ra.Extensions.Widgets.ExtButton delete;
        protected global::Ra.Extensions.Widgets.ExtButton follow;
        protected global::Ra.Widgets.LinkButton bookmark;

        private int ArticleID
        {
            get { return (int)ViewState["ArticleID"]; }
            set { ViewState["ArticleID"] = value; }
        }

        public void InitialLoading(Node node)
        {
            Load +=
                delegate
                {
                    edit.DataBind();
                    delete.DataBind();
                    follow.DataBind();
                    Node n2 = new Node();
                    ArticleID = node["ArticleID"].Get<int>();
                    n2["ArticleID"].Value = ArticleID;
                    ActiveEvents.Instance.RaiseActiveEvent(
                        this,
                        "ShouldAllowArticleEditing",
                        n2);
                    edit.Visible = n2["ShouldShow"].Get<bool>();
                    delete.Visible = n2["ShouldShowDelete"].Get<bool>();
                    follow.Visible = node["ShowFollow"].Get<bool>();
                    follow.Text = node["IsFollowing"].Get<bool>() ?
                        Language.Instance["Unfollow", null, "Unfollow"] :
                        Language.Instance["Follow", null, "Follow"];
                };
            header.InnerHtml = node["Header"].Get<string>();
            ingress.InnerHtml = node["Ingress"].Get<string>();
            content.InnerHtml = node["Body"].Get<string>();
            date.InnerHtml = Language.Instance["Published", null, "Published"] + " " +
                DateFormatter.FormatDate(node["Date"].Get<DateTime>()) +
                " - " +
                string.Format(
                    Language.Instance["ArticleViewed", null, "viewed {0} times"], node["ViewCount"].Value) +
                " - " +
                string.Format(
                    Language.Instance["BookmarkedBy", null, "bookmarked {0} times"], node["BookmarkedBy"].Value);
            image.Src = node["MainImage"].Get<string>();
            image.Alt = node["Header"].Get<string>() + " - " + node["Ingress"].Get<string>();
            author.HRef = "~/authors/" + node["Author"].Get<string>().Replace(".", "-") + ConfigurationManager.AppSettings["DefaultPageExtension"];
            author.InnerHtml = "/.~ " + node["Author"].Get<string>();
            bookmark.Visible = Users.LoggedInUserName != null;
            bookmark.CssClass = node["Bookmarked"].Get<bool>() ? "bookmarked" : "bookmark";
        }

        protected void bookmark_Click(object sender, EventArgs e)
        {
            Node node = new Node();
            node["ArticleID"].Value = ArticleID;
            ActiveEvents.Instance.RaiseActiveEvent(
                this,
                "ToggleArticleBookmark",
                node);
            if (node["Bookmarked"].Get<bool>())
            {
                bookmark.CssClass = "bookmarked";
            }
            else
            {
                bookmark.CssClass = "bookmark";
            }
        }

        protected void delete_Click(object sender, EventArgs e)
        {
            Node node = new Node();
            node["ArticleID"].Value = ArticleID;
            ActiveEvents.Instance.RaiseActiveEvent(
                this,
                "DeleteArticle",
                node);
        }

        protected void follow_Click(object sender, EventArgs e)
        {
            Node node = new Node();
            node["ArticleID"].Value = ArticleID;
            ActiveEvents.Instance.RaiseActiveEvent(
                this,
                "ArticleFollowRequested",
                node);
            follow.Text = follow.Text == Language.Instance["Follow", null, "Follow"] ?
                Language.Instance["Unfollow", null, "Unfollow"] :
                Language.Instance["Follow", null, "Follow"];
        }

        protected void edit_Click(object sender, EventArgs e)
        {
            Node node = new Node();
            node["ArticleID"].Value = ArticleID;
            ActiveEvents.Instance.RaiseActiveEvent(
                this,
                "EditArticle",
                node);
        }
    }
}