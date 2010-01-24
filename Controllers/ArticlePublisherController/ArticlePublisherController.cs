/*
 * Ra-Brix - A Modular-based Framework for building 
 * Web Applications Copyright 2009 - Thomas Hansen 
 * thomas@ra-ajax.org. Unless permission is 
 * explicitly given this code is licensed under the 
 * GNU Affero GPL version 3 which can be found in the 
 * license.txt file on disc.
 * 
 */

using System.Web;
using Ra.Brix.Loader;
using Ra.Brix.Types;
using SettingsRecords;
using ArticlePublisherRecords;
using Ra.Brix.Data;
using LanguageRecords;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using HelperGlobals;
using System.Collections.Generic;
using ForumRecords;
using UserRecords;
using Ra;

namespace ArticlePublisherController
{
    [ActiveController]
    public class ArticlePublisherController
    {
        [ActiveEvent(Name = "ApplicationStartup")]
        protected static void ApplicationStartup(object sender, ActiveEventArgs e)
        {
            Language.Instance.SetDefaultValue("ButtonArticles", "Articles");
            Language.Instance.SetDefaultValue("ButtonCreateArticle", "Create Article");
        }

        [ActiveEvent(Name = "GetMenuItems")]
        protected void GetMenuItems(object sender, ActiveEventArgs e)
        {
            e.Params["ButtonAdmin"]["ButtonArticles"].Value = "Menu-Articles";
            e.Params["ButtonAdmin"]["ButtonArticles"]["ButtonCreateArticle"].Value = "Menu-CreateArticle";
        }

        [ActiveEvent(Name = "Menu-CreateArticle")]
        protected void CreateArticle(object sender, ActiveEventArgs e)
        {
            Node node = new Node();
            ActiveEvents.Instance.RaiseLoadControl(
                "ArticlePublisherModules.CreateArticle",
                "dynMid",
                node);
        }

        [ActiveEvent(Name = "SaveArticle")]
        protected void SaveArticle(object sender, ActiveEventArgs e)
        {
            string header = e.Params["Header"].Get<string>();
            string ingress = e.Params["Ingress"].Get<string>();
            string body = e.Params["Body"].Get<string>();
            string image = e.Params["Image"].Get<string>();
            string[] tags = e.Params["Tags"].Get<string[]>();
            int id = e.Params["ID"].Get<int>();

            // Extracting image and saving in different formats...
            string imagePath = HttpContext.Current.Server.MapPath("~/" + image);

            string fileName = imagePath.Substring(imagePath.LastIndexOf("\\") + 1);
            fileName = fileName.Substring(0, fileName.IndexOf("."));

            // Saving server main resource folder
            string resourceFolder = HttpContext.Current.Server.MapPath("~/Resources/");
            using (Image original = Image.FromFile(imagePath))
            {
                double ratio = (double)original.Height / (double)original.Width;
                using (Image icon = new Bitmap(100, (int)(100 * ratio)))
                {
                    using (Graphics g = Graphics.FromImage(icon))
                    {
                        g.DrawImage(original, new Rectangle(0, 0, icon.Width, icon.Height));
                    }
                    icon.Save(resourceFolder + "Images\\Small\\" + fileName + ".png", ImageFormat.Png);
                }
                using (Image main = new Bitmap(350, (int)(350 * ratio)))
                {
                    using (Graphics g = Graphics.FromImage(main))
                    {
                        g.DrawImage(original, new Rectangle(0, 0, main.Width, main.Height));
                    }
                    main.Save(resourceFolder + "Images\\Medium\\" + fileName + ".png", ImageFormat.Png);
                }
            }

            // Creating actual article and saving it...
            Article a;
            if (id == -1)
            {
                // NEW article...!
                a = new Article();
                a.Published = DateTime.Now;
            }
            else
            {
                // Editing old article...
                a = Article.SelectByID(id);
            }
            a.Header = header;
            a.Ingress = ingress;
            a.OriginalImage = image;
            a.IconImage = "Resources/Images/Small/" + fileName + ".png";
            a.MainImage = "Resources/Images/Medium/" + fileName + ".png";
            a.Body = body;

            // Adding up tags
            a.Tags.Clear();
            foreach (string tagIdx in tags)
            {
                string tagName = tagIdx.ToLowerInvariant().Trim();
                if (tagName == string.Empty)
                    continue;
                Tag tag = Tag.SelectFirst(Criteria.Eq("Name", tagName));
                if (tag == null)
                {
                    tag = new Tag();
                    tag.Name = tagName;
                    tag.Save();
                }
                a.Tags.Add(tag);
            }

            a.Save();
            AjaxManager.Instance.Redirect("~/" + a.URL + ".aspx");
        }

        [ActiveEvent(Name = "EditArticle")]
        protected void EditArticle(object sender, ActiveEventArgs e)
        {
            int articleID = e.Params["ArticleID"].Get<int>();
            Article a = Article.SelectByID(articleID);
            Node node = new Node();
            node["ModuleSettings"]["IsEditing"].Value = true;
            node["ModuleSettings"]["ID"].Value = a.ID;
            node["ModuleSettings"]["Body"].Value = a.Body;
            node["ModuleSettings"]["Header"].Value = a.Header;
            node["ModuleSettings"]["Ingress"].Value = a.Ingress;
            node["ModuleSettings"]["Image"].Value = a.OriginalImage;
            string tagList = "";
            foreach (Tag idxTag in a.Tags)
            {
                tagList += idxTag.Name + ",";
            }
            tagList = tagList.Trim(',');
            node["ModuleSettings"]["TagList"].Value = tagList;
            ActiveEvents.Instance.RaiseLoadControl(
                "ArticlePublisherModules.CreateArticle",
                "dynMid",
                node);
        }

        [ActiveEvent(Name = "ShouldAllowArticleEditing")]
        protected void ShouldAllowArticleEditing(object sender, ActiveEventArgs e)
        {
            if (string.IsNullOrEmpty(Users.LoggedInUserName))
            {
                e.Params["ShouldShow"].Value = false;
            }
            else
            {
                User user = User.SelectFirst(Criteria.Eq("Username", Users.LoggedInUserName));
                Article article = Article.SelectByID(e.Params["ArticleID"].Get<int>());
                e.Params["ShouldShow"].Value = article.Author == user || 
                    user.InRole("Administrator");
            }
        }

        [ActiveEvent(Name = "FilterArticles")]
        protected void FilterArticles(object sender, ActiveEventArgs e)
        {
            // Showing all articles...
            ShowArticles(null, e.Params["Query"].Get<string>(), null);
        }

        [ActiveEvent(Name = "ArticleLikeComment")]
        [ActiveEvent(Name = "ArticleDislikeComment")]
        protected void ArticleLikeDislikeComment(object sender, ActiveEventArgs e)
        {
            int commentId = e.Params["CommentID"].Get<int>();
            int score = e.Params["Score"].Get<int>();
            ForumPost post = ForumPost.SelectByID(commentId);
            if (post.RegisteredUser != null && 
                post.RegisteredUser.Username == Users.LoggedInUserName)
            {
                Node nodeMessage = new Node();
                nodeMessage["Message"].Value = 
                    Language.Instance["YouCannotVoteForOwnComments", null, "You cannot vote for your own comments"];
                nodeMessage["Duration"].Value = 2000;
                ActiveEvents.Instance.RaiseActiveEvent(
                    this,
                    "ShowInformationMessage",
                    nodeMessage);
                e.Params["Refresh"].Value = false;
                return;
            }
            ForumPostVote vote = ForumPostVote.SelectFirst(
                Criteria.Eq("User.Username", Users.LoggedInUserName),
                Criteria.ExistsIn(post.ID));
            if (vote != null)
            {
                post.Score -= vote.Score;
            }
            else
            {
                vote = new ForumPostVote();
                vote.ForumPost = post;
                vote.User = User.SelectFirst(Criteria.Eq("Username", Users.LoggedInUserName));
            }
            post.Score += score;
            vote.Score = score;
            vote.Save();
            post.Save();
            e.Params["Refresh"].Value = true;
        }

        [ActiveEvent(Name = "GetArticleBookmarks")]
        protected void GetArticleBookmarks(object sender, ActiveEventArgs e)
        {
            e.Params["Grid"]["Columns"]["Bookmarks"]["Caption"].Value = Language.Instance["Bookmarks", null, "Bookmarks"];
            e.Params["Grid"]["Columns"]["Bookmarks"]["ControlType"].Value = "Link";
            int idxNo = 0;
            foreach (Bookmark idx in Bookmark.Select(Criteria.Eq("User.Username", Users.LoggedInUserName)))
            {
                e.Params["Grid"]["Rows"]["Row" + idxNo]["ID"].Value = idx.ID;
                e.Params["Grid"]["Rows"]["Row" + idxNo]["Bookmarks"].Value = idx.Article.Header;
                e.Params["Grid"]["Rows"]["Row" + idxNo]["Bookmarks"]["href"].Value = idx.Article.URL + ".aspx";
                e.Params["Grid"]["Rows"]["Row" + idxNo]["Bookmarks"]["target"].Value = "same";
                idxNo += 1;
            }
        }

        [ActiveEvent(Name = "Page_Init_InitialLoading")]
        protected void InitialLoadingOfPage(object sender, ActiveEventArgs e)
        {
            string contentId = HttpContext.Current.Request.Params["ContentID"];
            if (contentId != null)
            {
                contentId = contentId.Trim('/');
            }
            if (contentId == null)
            {
                // Showing all articles, but ONLY if Article system is NOT turned off in settings...
                if (Settings.Instance["ArticlePublisherHideLandingPage"] == "True")
                    return;

                // Setting title of page
                ((System.Web.UI.Page)HttpContext.Current.CurrentHandler).Title = 
                    Settings.Instance["ArticleMainLandingPageTitle"];

                // Showing all articles...
                ShowArticles(null, null, null);
            }
            else if (contentId != null && contentId.Contains("authors/"))
            {
                // Showing articles from specific authors...
                string author = contentId.Substring(contentId.LastIndexOf("/") + 1).Replace(".aspx", "");
                ((System.Web.UI.Page)HttpContext.Current.CurrentHandler).Title = Language.Instance["ShowingArticleFromAuthor", null, "Articles written by "] + author;

                // Showing articles from author
                ShowArticles(author, null, null);
            }
            else if (contentId != null && contentId.Contains("tags/"))
            {
                // Showing articles from specific authors...
                string tag = contentId.Substring(contentId.LastIndexOf("/") + 1).Replace(".aspx", "");
                ((System.Web.UI.Page)HttpContext.Current.CurrentHandler).Title = Language.Instance["ShowingArticleTags", null, "Articles tagged with "] + tag;

                // Showing articles from author
                ShowArticles(null, null, tag);
            }
            else
            {
                // Showing search
                Node node = new Node();
                ActiveEvents.Instance.RaiseLoadControl(
                    "ArticlePublisherModules.SearchArticles",
                    "dynMid",
                    node);

                // Showing specific article
                ShowSpecificArticle(contentId);
            }
        }

        private static void ShowSpecificArticle(string contentId)
        {
            Node node = new Node();

            // Showing bookmarks module
            if (Users.LoggedInUserName != null)
            {
                ActiveEvents.Instance.RaiseLoadControl(
                    "ArticlePublisherModules.Favorites",
                    "dynMid");
                node["AddToExistingCollection"].Value = true;
            }

            // Showing search
            ActiveEvents.Instance.RaiseLoadControl(
                "ArticlePublisherModules.SearchArticles",
                "dynMid",
                node);

            // Loading actual Article...
            Article a = Article.FindArticle(contentId);
            if (a == null)
                return;

            // Setting title of page
            ((System.Web.UI.Page)HttpContext.Current.CurrentHandler).Title =
                Language.Instance["RaBrixMagazine", null, "Ra-Brix Magazine - "] +
                a.Header;

            node = new Node();
            node["AddToExistingCollection"].Value = true;
            node["ModuleSettings"]["Header"].Value = a.Header;
            node["ModuleSettings"]["Body"].Value = a.Body;
            node["ModuleSettings"]["Date"].Value = a.Published;
            node["ModuleSettings"]["Ingress"].Value = a.Ingress;
            node["ModuleSettings"]["ArticleID"].Value = a.ID;
            node["ModuleSettings"]["Author"].Value = a.Author == null ? "unknown" : a.Author.Username;
            node["ModuleSettings"]["MainImage"].Value = "~/" + a.MainImage;
            if (!string.IsNullOrEmpty(Users.LoggedInUserName))
            {
                Bookmark bookmark = Bookmark.SelectFirst(
                    Criteria.Eq("User.Username", Users.LoggedInUserName),
                    Criteria.Eq("Article.URL", a.URL));
                node["ModuleSettings"]["Bookmarked"].Value = bookmark != null;
            }
            else
            {
                node["ModuleSettings"]["Bookmarked"].Value = false;
            }
            ActiveEvents.Instance.RaiseLoadControl(
                "ArticlePublisherModules.ViewArticle",
                "dynMid",
                node);

            // Then loading tags
            node = new Node();
            int idxNo = 0;
            foreach (Tag idx in a.Tags)
            {
                node["ModuleSettings"]["Tags"]["Tag" + idxNo]["Name"].Value = idx.Name;
                node["ModuleSettings"]["Tags"]["Tag" + idxNo]["URL"].Value = "tags/" + idx.Name + ".aspx";
                idxNo += 1;
            }
            node["AddToExistingCollection"].Value = true;
            ActiveEvents.Instance.RaiseLoadControl(
                "ArticlePublisherModules.ShowTags",
                "dynMid",
                node);

            // Then loading Forum
            if (Settings.Instance["UseForumsForArticles"] == "True")
            {
                node = new Node();
                Forum forum = Forum.SelectFirst(Criteria.Eq("Name", a.URL));
                if (forum == null)
                {
                    // Making sure we're creating a forum if none already exists for this article...
                    forum = new Forum();
                    forum.Name = a.URL;
                    forum.Save();
                }
                node["ModuleSettings"]["Forum"].Value = forum;
                node["ModuleSettings"]["AllowVoting"].Value = 
                    !string.IsNullOrEmpty(Users.LoggedInUserName);
                node["AddToExistingCollection"].Value = true;
                ActiveEvents.Instance.RaiseLoadControl(
                    "ForumModules.ShowPosts",
                    "dynMid",
                    node);
            }
        }

        [ActiveEvent(Name = "ToggleArticleBookmark")]
        protected void ToggleArticleBookmark(object sender, ActiveEventArgs e)
        {
            int articleId = e.Params["ArticleID"].Get<int>();
            Article article = Article.SelectByID(articleId);
            Bookmark bookmark = Bookmark.SelectFirst(
                Criteria.Eq("Article.URL", article.URL),
                Criteria.Eq("User.Username", Users.LoggedInUserName));
            if (bookmark == null)
            {
                // Create bookmark
                bookmark = new Bookmark();
                bookmark.Article = article;
                bookmark.User = User.SelectFirst(Criteria.Eq("Username", Users.LoggedInUserName));
                bookmark.Save();
                e.Params["Bookmarked"].Value = true;

                // Showing message
                Node nodeMessage = new Node();
                nodeMessage["Message"].Value = Language.Instance[
                    "ArticleWasBookmarked", 
                    null, 
                    "Article was bookmarked and will appear in your bookmarks"];
                nodeMessage["Duration"].Value = 2000;

                ActiveEvents.Instance.RaiseActiveEvent(
                    this,
                    "ShowInformationMessage",
                    nodeMessage);
            }
            else
            {
                // Delete bookmark
                bookmark.Delete();
                e.Params["Bookmarked"].Value = false;

                // Showing message
                Node nodeMessage = new Node();
                nodeMessage["Message"].Value = 
                    Language.Instance[
                        "BookmarkedWasDeleted", 
                        null, 
                        "Bookmark was deleted and article will no longer appear in your bookmarks"];
                nodeMessage["Duration"].Value = 2000;

                ActiveEvents.Instance.RaiseActiveEvent(
                    this,
                    "ShowInformationMessage",
                    nodeMessage);
            }
        }

        private static void ShowArticles(string userNameFilter, string filter, string tag)
        {
            Node node = new Node();

            // Showing bookmarks module
            if (Users.LoggedInUserName != null)
            {
                ActiveEvents.Instance.RaiseLoadControl(
                    "ArticlePublisherModules.Favorites",
                    "dynMid");
                node["AddToExistingCollection"].Value = true;
            }

            // Showing search
            if (filter != null)
                node["ModuleSettings"]["Filter"].Value = filter;
            ActiveEvents.Instance.RaiseLoadControl(
                "ArticlePublisherModules.SearchArticles",
                "dynMid",
                node);

            // Showing articles
            node = new Node();
            int idxNo = 0;
            List<Criteria> criteria = new List<Criteria>();
            criteria.Add(Criteria.Sort("Published", false));
            if (!string.IsNullOrEmpty(userNameFilter))
            {
                criteria.Add(Criteria.Eq("Author.Username", userNameFilter));
            }
            if (filter != null && filter.Length > 0)
            {
                criteria.AddRange(DataHelper.CreateSearchFilter("Header", filter));
            }
            foreach (Article idx in Article.Select(criteria.ToArray()))
            {
                if (!string.IsNullOrEmpty(tag))
                {
                    if (!idx.Tags.Exists(
                        delegate(Tag idxTag)
                        {
                            return idxTag.Name == tag.ToLowerInvariant().Trim();
                        }))
                        continue;
                }
                node["ModuleSettings"]["Articles"]["Article" + idxNo]["Header"].Value = idx.Header;
                node["ModuleSettings"]["Articles"]["Article" + idxNo]["Ingress"].Value = idx.Ingress;
                node["ModuleSettings"]["Articles"]["Article" + idxNo]["Body"].Value = idx.Body;
                node["ModuleSettings"]["Articles"]["Article" + idxNo]["Icon"].Value = "~/" + idx.IconImage;
                node["ModuleSettings"]["Articles"]["Article" + idxNo]["MainImage"].Value = "~/" + idx.MainImage;
                node["ModuleSettings"]["Articles"]["Article" + idxNo]["Author"].Value = idx.Author == null ? "unknown" : idx.Author.Username;
                node["ModuleSettings"]["Articles"]["Article" + idxNo]["DatePublished"].Value = idx.Published;
                node["ModuleSettings"]["Articles"]["Article" + idxNo]["URL"].Value = "~/" + idx.URL + ".aspx";
                node["ModuleSettings"]["Articles"]["Article" + idxNo]["CommentCount"].Value = ForumPost.CountWhere(Criteria.Eq("URL", idx.URL + ".aspx")).ToString();

                // Making sure we only show the 10 latest articles...
                if (++idxNo == 10)
                    break;
            }
            if (idxNo < 10)
            {
                if (filter != null && filter.Length > 0)
                {
                    // Further looking through ingress...
                    criteria = new List<Criteria>();
                    criteria.Add(Criteria.Sort("Published", false));
                    if (!string.IsNullOrEmpty(userNameFilter))
                    {
                        criteria.Add(Criteria.Eq("Author.Username", userNameFilter));
                    }
                    criteria.AddRange(DataHelper.CreateSearchFilter("Ingress", filter));
                    foreach (Article idx in Article.Select(criteria.ToArray()))
                    {
                        if (node["ModuleSettings"]["Articles"].Exists(
                            delegate(Node idxNode)
                            {
                                return idxNode["URL"].Get<string>() == "~/" + idx.URL + ".aspx";
                            }, true))
                            continue;
                        node["ModuleSettings"]["Articles"]["Article" + idxNo]["Header"].Value = idx.Header;
                        node["ModuleSettings"]["Articles"]["Article" + idxNo]["Ingress"].Value = idx.Ingress;
                        node["ModuleSettings"]["Articles"]["Article" + idxNo]["Body"].Value = idx.Body;
                        node["ModuleSettings"]["Articles"]["Article" + idxNo]["Icon"].Value = "~/" + idx.IconImage;
                        node["ModuleSettings"]["Articles"]["Article" + idxNo]["MainImage"].Value = "~/" + idx.MainImage;
                        node["ModuleSettings"]["Articles"]["Article" + idxNo]["Author"].Value = idx.Author == null ? "unknown" : idx.Author.Username;
                        node["ModuleSettings"]["Articles"]["Article" + idxNo]["DatePublished"].Value = idx.Published;
                        node["ModuleSettings"]["Articles"]["Article" + idxNo]["URL"].Value = "~/" + idx.URL + ".aspx";
                        node["ModuleSettings"]["Articles"]["Article" + idxNo]["CommentCount"].Value = ForumPost.CountWhere(Criteria.Eq("URL", idx.URL + ".aspx")).ToString();

                        // Making sure we only show the 10 latest articles...
                        if (++idxNo == 10)
                            break;
                    }
                }
            }
            node["AddToExistingCollection"].Value = true;
            ActiveEvents.Instance.RaiseLoadControl(
                "ArticlePublisherModules.LandingPage",
                "dynMid",
                node);

            // Then showing latest comments...
            if (Settings.Instance["UseForumsForArticles"] == "True")
            {
                node = new Node();
                if (!string.IsNullOrEmpty(userNameFilter))
                {
                    node["ModuleSettings"]["Username"].Value = userNameFilter;
                }
                node["AddToExistingCollection"].Value = true;
                if (filter != null)
                    node["ModuleSettings"]["Filter"].Value = filter;
                ActiveEvents.Instance.RaiseLoadControl(
                    "ForumModules.ShowPostsFromUser",
                    "dynMid",
                    node);
            }
        }
    }
}
