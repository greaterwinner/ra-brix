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
                    Node n2 = new Node();
                    ArticleID = node["ArticleID"].Get<int>();
                    n2["ArticleID"].Value = ArticleID;
                    ActiveEvents.Instance.RaiseActiveEvent(
                        this,
                        "ShouldAllowArticleEditing",
                        n2);
                    edit.Visible = n2["ShouldShow"].Get<bool>();
                };
            header.InnerHtml = node["Header"].Get<string>();
            ingress.InnerHtml = node["Ingress"].Get<string>();
            content.InnerHtml = node["Body"].Get<string>();
            date.InnerHtml = DateFormatter.FormatDate(node["Date"].Get<DateTime>());
            image.Src = node["MainImage"].Get<string>();
            image.Alt = node["Header"].Get<string>() + " - " + node["Ingress"].Get<string>();
            author.HRef = "~/authors/" + node["Author"].Get<string>() + ".aspx";
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

        protected void edit_Click(object sender, EventArgs e)
        {
            Node node = new Node();
            node["ArticleID"].Value = ArticleID;
            ActiveEvents.Instance.RaiseActiveEvent(
                this,
                "EditArticle",
                node);
        }

        public string GetCaption()
        {
            return "";
        }
    }
}