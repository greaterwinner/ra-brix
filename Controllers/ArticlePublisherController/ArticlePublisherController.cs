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
            a.Save();

            // Notifying of saved article...
            Node nodeMessage = new Node();
            string msg = Language.Instance["ArticleSavedNotification", null, @"
Article was saved and will display on the front page at the top."];
            nodeMessage["Message"].Value = msg;
            nodeMessage["Duration"].Value = 2000;
            ActiveEvents.Instance.RaiseActiveEvent(
                this,
                "ShowInformationMessage",
                nodeMessage);
            e.Params["ID"].Value = a.ID;
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
                e.Params["ShouldShow"].Value = article.Author == user;
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
                ((System.Web.UI.Page)HttpContext.Current.CurrentHandler).Title = Settings.Instance["ArticleMainLandingPageTitle"];
                ShowArticles(null);

                // Then showing latest comments...
                Node node = new Node();
                node["AddToExistingCollection"].Value = true;
                ActiveEvents.Instance.RaiseLoadControl(
                    "ForumModules.ShowPostsFromUser",
                    "dynMid",
                    node);
            }
            else if (contentId != null && contentId.Contains("authors/"))
            {
                // Showing articles from specific authors...
                string author = contentId.Substring(contentId.LastIndexOf("/") + 1).Replace(".aspx", "");
                ((System.Web.UI.Page)HttpContext.Current.CurrentHandler).Title = Language.Instance["ShowingArticleFromAuthor", null, "Articles written by "] + author;
                ShowArticles(author);

                // Then showing comments from specific author...
                Node node = new Node();
                node["AddToExistingCollection"].Value = true;
                node["ModuleSettings"]["Username"].Value = author;
                ActiveEvents.Instance.RaiseLoadControl(
                    "ForumModules.ShowPostsFromUser",
                    "dynMid",
                    node);
            }
            else
            {
                ShowSpecificArticle(contentId);
            }
        }

        private static void ShowSpecificArticle(string contentId)
        {
            // First loading actual Article...
            Article a = Article.FindArticle(contentId);
            Node node = new Node();
            node["ModuleSettings"]["Header"].Value = a.Header;
            node["ModuleSettings"]["Body"].Value = a.Body;
            node["ModuleSettings"]["Date"].Value = a.Published;
            node["ModuleSettings"]["Ingress"].Value = a.Ingress;
            node["ModuleSettings"]["ArticleID"].Value = a.ID;
            ((System.Web.UI.Page)HttpContext.Current.CurrentHandler).Title = 
                Language.Instance["RaBrixMagazine", null, "Ra-Brix Magazine - "] + 
                a.Header;
            node["ModuleSettings"]["Author"].Value = a.Author == null ? "unknown" : a.Author.Username;
            node["ModuleSettings"]["MainImage"].Value = "~/" + a.MainImage;
            ActiveEvents.Instance.RaiseLoadControl(
                "ArticlePublisherModules.ViewArticle",
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
                node["AddToExistingCollection"].Value = true;
                ActiveEvents.Instance.RaiseLoadControl(
                    "ForumModules.ShowPosts",
                    "dynMid",
                    node);
            }
        }

        private static void ShowArticles(string userNameFilter)
        {
            Node node = new Node();
            int idxNo = 0;
            List<Criteria> criteria = new List<Criteria>();
            criteria.Add(Criteria.Sort("Published", false));
            if (!string.IsNullOrEmpty(userNameFilter))
            {
                criteria.Add(Criteria.Eq("Author.Username", userNameFilter));
            }
            foreach (Article idx in Article.Select(criteria.ToArray()))
            {
                node["ModuleSettings"]["Articles"]["Article" + idxNo]["Header"].Value = idx.Header;
                node["ModuleSettings"]["Articles"]["Article" + idxNo]["Ingress"].Value = idx.Ingress;
                node["ModuleSettings"]["Articles"]["Article" + idxNo]["Body"].Value = idx.Body;
                node["ModuleSettings"]["Articles"]["Article" + idxNo]["Icon"].Value = "~/" + idx.IconImage;
                node["ModuleSettings"]["Articles"]["Article" + idxNo]["MainImage"].Value = "~/" + idx.MainImage;
                node["ModuleSettings"]["Articles"]["Article" + idxNo]["Author"].Value = idx.Author == null ? "unknown" : idx.Author.Username;
                node["ModuleSettings"]["Articles"]["Article" + idxNo]["DatePublished"].Value = idx.Published;
                node["ModuleSettings"]["Articles"]["Article" + idxNo]["URL"].Value = "~/" + idx.URL + ".aspx";

                // Making sure we only show the 10 latest articles...
                if (++idxNo == 10)
                    break;
            }
            ActiveEvents.Instance.RaiseLoadControl(
                "ArticlePublisherModules.LandingPage",
                "dynMid",
                node);
        }
    }
}
