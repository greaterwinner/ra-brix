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
using System.Text;
using System.Security.Cryptography;
using System.Net;
using System.IO;
using System.Web.UI;
using System.Net.Mail;

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
            Language.Instance.SetDefaultValue("ButtonAdministrateTags", "Edit Tags");
        }

        [ActiveEvent(Name = "GetMenuItems")]
        protected void GetMenuItems(object sender, ActiveEventArgs e)
        {
            e.Params["ButtonAdmin"]["ButtonArticles"].Value = "Menu-Articles";
            e.Params["ButtonCreateArticle"].Value = "Menu-CreateArticle";
            e.Params["ButtonAdmin"]["ButtonArticles"]["ButtonAdministrateTags"].Value = "Menu-EditArticleTags";
        }

        [ActiveEvent(Name = "Menu-EditArticleTags")]
        protected void EditArticleTags(object sender, ActiveEventArgs e)
        {
            Node node = new Node();
            int idxNo = 0;
            foreach (Tag idx in Tag.Select())
            {
                node["ModuleSettings"]["Tags"]["Tag" + idxNo]["ID"].Value = idx.ID;
                node["ModuleSettings"]["Tags"]["Tag" + idxNo]["Name"].Value = idx.Name;
                node["ModuleSettings"]["Tags"]["Tag" + idxNo]["Sticky"].Value = idx.ShowInLandingPage;
                node["ModuleSettings"]["Tags"]["Tag" + idxNo]["URL"].Value = "tags/" +
                    HttpContext.Current.Server.UrlEncode(idx.Name) + ".aspx";
                idxNo += 1;
            }
            ActiveEvents.Instance.RaiseLoadControl(
                "ArticlePublisherModules.EditTags",
                "dynMid",
                node);
        }

        [ActiveEvent(Name = "UserCreatedAnArticleComment")]
        protected void UserCreatedAnArticleComment(object sender, ActiveEventArgs e)
        {
            User user = User.SelectFirst(
                Criteria.Eq("Username", e.Params["Username"].Value));
            user.Score += int.Parse(Settings.Instance["PointsForNewComment"]);
            user.Save();
        }

        [ActiveEvent(Name = "UserCreatedAnArticleComment")]
        protected void UserCreatedAnArticleCommentSendEmail(object sender, ActiveEventArgs e)
        {
            // Sending emails to users registered as followers of this article...
            Article article = Article.SelectFirst(
                Criteria.Eq("URL", e.Params["ForumName"].Get<string>()));
            List<string> emails = new List<string>();
            foreach (User idx in article.Followers)
            {
                if (!string.IsNullOrEmpty(idx.Email))
                {
                    if (idx.Email.IndexOf("@") > 0)
                        emails.Add(idx.Email);
                }
            }
            if (emails.Count == 0)
                return;
            string smtpServer = Settings.Instance["SMTPServer"];
            string smtpServerUsername = Settings.Instance["SMTPServerUsername"];
            string smtpServerPassword = Settings.Instance["SMTPServerPassword"];
            string smtpServerUseSsl = Settings.Instance["SMTPServerUseSsl"];
            if (string.IsNullOrEmpty(smtpServer))
                return;
            string adminEmail = Settings.Instance["AdminEmail"];
            string subject =
                Language.Instance[
                    "NewCommentEmailSubject", 
                    null, 
                    "New Comment at TheLightBringer.org"];
            string comment = e.Params["Comment"].Get<string>();
            string body = string.Format(
                Language.Instance[
                    "NewCommentCreatedEmailBody3", 
                    null, 
                    @"A new comment was submitted to the article '{0}' at {1}
If you do not want to get future notification emails for this subject, make sure 
you click the 'Unfollow' button.
The comment was;
"],
                article.Header,
                HttpContext.Current.Request.Url.AbsoluteUri.Substring(
                    0, HttpContext.Current.Request.Url.AbsoluteUri.LastIndexOf("/") + 1) +
                    article.URL + ".aspx#comments") + comment;
            Node node = new Node();
            node["Emails"].Value = emails;
            node["Body"].Value = body;
            node["Subject"].Value = subject;
            node["SystemReplyEmail"].Value = adminEmail;
            node["SmtpServer"].Value = smtpServer;
            node["SmtpUsername"].Value = smtpServerUsername;
            node["SmtpPwd"].Value = smtpServerPassword;
            node["SmtpServer"].Value = smtpServer;
            node["SmtpServerSSL"].Value = smtpServerUseSsl == "True";

            // Since we want to execute the actual sending of the emails on another
            // thread to not stall UI, we just Raise a new event here which is handled below
            // in an Async event handler...
            ActiveEvents.Instance.RaiseActiveEvent(
                this,
                "SendEmailForComment",
                node);
        }

        [ActiveEvent(Name = "SendEmailForComment", Async = true)]
        protected void SendEmailForComment(object sender, ActiveEventArgs e)
        {
            MailMessage msg = new MailMessage();
            foreach (string idxEmail in e.Params["Emails"].Get<List<string>>())
            {
                msg.To.Add(new MailAddress(idxEmail));
            }
            msg.Body = e.Params["Body"].Get<string>();
            msg.Subject = e.Params["Subject"].Get<string>();
            msg.From = new MailAddress(e.Params["SystemReplyEmail"].Get<string>());
            System.Net.Mail.SmtpClient smtp = new System.Net.Mail.SmtpClient();
            smtp.Host = e.Params["SmtpServer"].Get<string>();
            smtp.Credentials = new NetworkCredential(
                e.Params["SmtpUsername"].Get<string>(),
                e.Params["SmtpPwd"].Get<string>());
            smtp.EnableSsl = e.Params["SmtpServerSSL"].Get<bool>();
            smtp.Send(msg);
        }

        [ActiveEvent(Name = "UserCreatedNewArticle")]
        protected void UserCreatedNewArticle(object sender, ActiveEventArgs e)
        {
            User user = User.SelectFirst(
                Criteria.Eq("Username", e.Params["Username"].Value));
            user.Score += int.Parse(Settings.Instance["PointsForNewArticle"]);
            user.Save();
        }

        [ActiveEvent(Name = "ArticleTagStickyChanged")]
        protected void ArticleTagStickyChanged(object sender, ActiveEventArgs e)
        {
            int id = e.Params["ID"].Get<int>();
            Tag tag = Tag.SelectByID(id);
            tag.ShowInLandingPage = e.Params["Sticky"].Get<bool>();
            tag.Save();
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
            bool isNewArticle = false;
            if (id == -1)
            {
                // NEW article...!
                a = new Article();
                a.Published = DateTime.Now;

                Node node = new Node();
                node["Username"].Value = Users.LoggedInUserName;
                ActiveEvents.Instance.RaiseActiveEvent(
                    this,
                    "UserCreatedNewArticle",
                    node);
                isNewArticle = true;
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

            // Saving article...
            a.Save();

            // Signaling that a *NEWLY* created article was saved
            Node nodeSignal = new Node();
            nodeSignal["URL"].Value = a.URL;
            nodeSignal["Header"].Value = a.Header;
            nodeSignal["Body"].Value = a.Body;
            ActiveEvents.Instance.RaiseActiveEvent(
                this,
                "HandleArticleFirstTimePublished",
                nodeSignal);

            // Redirecting to article...
            AjaxManager.Instance.Redirect("~/" + a.URL + ".aspx");
        }

        public static string GetBase64Encoded(string inString)
        {
            byte[] inData;
            char[] charArr;
            charArr = inString.ToCharArray();
            inData = new byte[charArr.Length];
            for (int i = 0; i < charArr.Length; i++)
            {
                inData[i] = (byte)charArr[i];
            }
            return System.Convert.ToBase64String(inData, 0, inData.Length);
        }

        [ActiveEvent(Name = "DeleteArticle")]
        protected void DeleteArticle(object sender, ActiveEventArgs e)
        {
            int articleID = e.Params["ArticleID"].Get<int>();
            Article a = Article.SelectByID(articleID);
            a.Delete();
            AjaxManager.Instance.Redirect("~/");
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
                e.Params["ShouldShowDelete"].Value = false;
            }
            else
            {
                User user = User.SelectFirst(Criteria.Eq("Username", Users.LoggedInUserName));
                Article article = Article.SelectByID(e.Params["ArticleID"].Get<int>());
                e.Params["ShouldShow"].Value = article.Author == user ||
                    user.InRole("Administrator");
                e.Params["ShouldShowDelete"].Value = article.Author == user ||
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

            // Giving user score
            if (score > 0)
                vote.User.Score += int.Parse(Settings.Instance["PointsForUpVote"]);
            else
                vote.User.Score += int.Parse(Settings.Instance["PointsForDownVote"]);
            vote.User.Save();
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

        [ActiveEvent(Name = "ArticleFollowRequested")]
        protected void ArticleFollowRequested(object sender, ActiveEventArgs e)
        {
            int articleID = e.Params["ArticleID"].Get<int>();
            Article article = Article.SelectByID(articleID);
            User user = User.SelectFirst(Criteria.Eq("Username", Users.LoggedInUserName));
            if(article.Followers.Exists(
                delegate(User idx)
                {
                    return idx == user;
                }))
            {
                article.Followers.Remove(user);
            }
            else
            {
                article.Followers.Add(user);
            }
            article.Save();
        }

        [ActiveEvent(Name = "ArticleDeleteTag")]
        protected void ArticleDeleteTag(object sender, ActiveEventArgs e)
        {
            int id = e.Params["ID"].Get<int>();
            Tag tag = Tag.SelectByID(id);
            tag.Delete();

            int idxNo = 0;
            foreach (Tag idx in Tag.Select())
            {
                e.Params["Tags"]["Tag" + idxNo]["ID"].Value = idx.ID;
                e.Params["Tags"]["Tag" + idxNo]["Name"].Value = idx.Name;
                e.Params["Tags"]["Tag" + idxNo]["Sticky"].Value = idx.ShowInLandingPage;
                e.Params["Tags"]["Tag" + idxNo]["URL"].Value = "tags/" +
                    HttpContext.Current.Server.UrlEncode(idx.Name) + ".aspx";
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

            // Showing sticky tags at the top...
            ShowLandingPageTags();

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

                // Display status bar, link to users...
                DisplayStatusAndUsersLink();
            }
            else if (contentId != null && contentId == "authors/all")
            {
                Node nu = new Node();
                int idxNo = 0;
                nu["ModuleSettings"]["Header"].Value =
                    Language.Instance["The100MostActiveAuthors", null, "The 100 most active authors"];
                foreach (User idx in User.Select(Criteria.Sort("Score", false)))
                {
                    if (idxNo > 100)
                        break;
                    nu["ModuleSettings"]["Users"]["User" + idxNo]["Name"].Value = idx.Username;
                    nu["ModuleSettings"]["Users"]["User" + idxNo]["Score"].Value = idx.Score;
                    nu["ModuleSettings"]["Users"]["User" + idxNo]["URL"].Value = "authors/" + idx.Username + ".aspx";
                    nu["ModuleSettings"]["Users"]["User" + idxNo]["ImageSrc"].Value =
                        string.Format(
                            "http://www.gravatar.com/avatar/{0}?s=64&d=identicon",
                            MD5Hash(idx.Email ?? "unknown"));
                    idxNo += 1;
                }
                ActiveEvents.Instance.RaiseLoadControl(
                    "ArticlePublisherModules.ViewUsers",
                    "dynMid",
                    nu);
            }
            else if (contentId != null && contentId.Contains("authors/"))
            {
                // Showing articles from specific authors...
                string author = contentId.Substring(contentId.LastIndexOf("/") + 1).Replace(".aspx", "");
                ((System.Web.UI.Page)HttpContext.Current.CurrentHandler).Title =
                    Language.Instance[
                        "ShowingArticleFromAuthor",
                        null,
                        "Articles written by "] + author;

                // Showing articles from author
                ShowArticles(author, null, null);

                // Display status bar, link to users...
                DisplayStatusAndUsersLink();
            }
            else if (contentId != null && contentId.Contains("tags/"))
            {
                // Showing articles from specific authors...
                string tag = HttpContext.Current.Server.UrlDecode(
                    contentId.Substring(contentId.LastIndexOf("/") + 1)
                        .Replace(".aspx", ""));
                ((System.Web.UI.Page)HttpContext.Current.CurrentHandler).Title =
                    Language.Instance[
                        "ShowingArticleTags",
                        null,
                        "Articles tagged with "] + tag;

                // Showing articles from author
                ShowArticles(null, null, tag);

                // Display status bar, link to users...
                DisplayStatusAndUsersLink();
            }
            else
            {
                // Showing search
                ActiveEvents.Instance.RaiseLoadControl(
                    "ArticlePublisherModules.SearchArticles",
                    "dynMid");

                // Showing specific article
                ShowSpecificArticle(contentId);

                // Display status bar, link to users...
                DisplayStatusAndUsersLink();
            }
        }

        private void ShowLandingPageHeader()
        {
            // We only display the header if user isn't logged in...
            if (Users.LoggedInUserName != null)
                return;

            string header = Settings.Instance["ArticleLandingPageHeader"];
            if (!string.IsNullOrEmpty(header))
            {
                Node node = new Node();
                node["PageURL"].Value = header;
                ActiveEvents.Instance.RaiseActiveEvent(
                    this,
                    "ShowCMSPageAtTop",
                    node);
            }
        }

        private void DisplayStatusAndUsersLink()
        {
            ActiveEvents.Instance.RaiseActiveEvent(
                this,
                "JustBeforeLoadingArticleBottom");
            Node node = new Node();
            node["ModuleSettings"]["UserCount"].Value = User.Count;
            node["ModuleSettings"]["ArticleCount"].Value = Article.Count;
            node["ModuleSettings"]["CommentCount"].Value = ForumPost.Count;
            node["AddToExistingCollection"].Value = true;
            ActiveEvents.Instance.RaiseLoadControl(
                "ArticlePublisherModules.Status",
                "dynMid",
                node);
        }

        private static string MD5Hash(string email)
        {
            if (string.IsNullOrEmpty(email))
                return string.Empty;

            StringBuilder emailHash = new StringBuilder();
            MD5 md5 = MD5.Create();
            byte[] emailBuffer = Encoding.ASCII.GetBytes(email);
            byte[] hash = md5.ComputeHash(emailBuffer);

            foreach (byte hashByte in hash)
                emailHash.Append(hashByte.ToString("x2"));

            return emailHash.ToString();
        }

        private void ShowLandingPageTags()
        {
            // Showing main/landing-page tags...
            Node node = new Node();
            int idxNo = 0;
            foreach (Tag idx in Tag.Select(Criteria.Eq("ShowInLandingPage", true)))
            {
                node["ModuleSettings"]["Tags"]["Tag" + idxNo]["Name"].Value = idx.Name;
                node["ModuleSettings"]["Tags"]["Tag" + idxNo]["URL"].Value = "tags/" +
                    HttpContext.Current.Server.UrlEncode(idx.Name) + ".aspx";
                idxNo += 1;
            }
            node["ModuleSettings"]["AlignBottomRight"].Value = true;
            ActiveEvents.Instance.RaiseLoadControl(
                "ArticlePublisherModules.ShowTags",
                "dynTop",
                node);
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

            // Incrementing view count
            a.ViewCount += 1;
            a.Save();

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
            node["ModuleSettings"]["ViewCount"].Value = a.ViewCount;
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
            node["ModuleSettings"]["BookmarkedBy"].Value =
                Bookmark.CountWhere(
                    Criteria.Eq("Article.URL", a.URL));
            if (!string.IsNullOrEmpty(Users.LoggedInUserName))
            {
                User user = User.SelectFirst(Criteria.Eq("Username", Users.LoggedInUserName));
                node["ModuleSettings"]["ShowFollow"].Value = !string.IsNullOrEmpty(user.Email);
            }
            else
            {
                node["ModuleSettings"]["ShowFollow"].Value = false;
            }
            node["ModuleSettings"]["IsFollowing"].Value = a.Followers.Exists(
                delegate(User idxUser)
                {
                    return idxUser.Username == Users.LoggedInUserName;
                });
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
                node["ModuleSettings"]["Tags"]["Tag" + idxNo]["URL"].Value = "tags/" +
                    HttpContext.Current.Server.UrlEncode(idx.Name) + ".aspx";
                idxNo += 1;
            }
            node["AddToExistingCollection"].Value = true;
            node["ModuleSettings"]["CenterAlign"].Value = true;
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

        private void ShowArticles(string userNameFilter, string filter, string tag)
        {
            int noArticlesToDisplay = int.Parse(Settings.Instance["NumberOfArticlesToDisplay"]);
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

            // Showing landing page header
            ShowLandingPageHeader();

            // If user is not null, we also display the user profile...
            if (!string.IsNullOrEmpty(userNameFilter))
            {
                User user = User.SelectFirst(Criteria.Eq("Username", userNameFilter));
                node = new Node();
                node["AddToExistingCollection"].Value = true;
                node["ModuleSettings"]["Username"].Value = user.Username;
                node["ModuleSettings"]["Email"].Value = user.Email ?? "unknown";
                node["ModuleSettings"]["LastLoggedIn"].Value = user.LastLoggedIn;
                node["ModuleSettings"]["Phone"].Value = user.Phone ?? "555-whatever";
                node["ModuleSettings"]["Roles"].Value = user.GetRolesString();
                node["ModuleSettings"]["IsAdmin"].Value = user.InRole("Administrator");
                node["ModuleSettings"]["Score"].Value = user.Score;
                node["ModuleSettings"]["Biography"].Value = user.Biography;
                node["ModuleSettings"]["ArticleCount"].Value = Article.CountWhere(
                    Criteria.Eq("Author.Username", user.Username));
                node["ModuleSettings"]["CommentCount"].Value = ForumPost.CountWhere(
                    Criteria.Eq("RegisteredUser.Username", user.Username));
                ActiveEvents.Instance.RaiseLoadControl(
                    "ArticlePublisherModules.ViewUser",
                    "dynMid",
                    node);
            }

            // Showing articles
            node = new Node();
            int idxNo = 0;
            List<Criteria> criteria = new List<Criteria>();
            criteria.Add(Criteria.Sort("Published", false));
            if (!string.IsNullOrEmpty(userNameFilter))
            {
                criteria.Add(Criteria.Eq("Author.Username", userNameFilter));
                node["ModuleSettings"]["Header"].Value =
                    string.Format(Language.Instance["WrittenByAuthor", null, "Written by {0}"],
                        userNameFilter);
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

                // Making sure we only show the x latest articles...
                if (++idxNo == noArticlesToDisplay)
                    break;
            }
            if (idxNo < noArticlesToDisplay)
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

                        // Making sure we only show the x latest articles...
                        if (++idxNo == noArticlesToDisplay)
                            break;
                    }
                }
            }
            node["AddToExistingCollection"].Value = true;
            ActiveEvents.Instance.RaiseLoadControl(
                "ArticlePublisherModules.LandingPage",
                "dynMid",
                node);

            // Then getting the articles bookmarked by user (if user is not null)
            if (!string.IsNullOrEmpty(userNameFilter))
            {
                node = new Node();
                idxNo = 0;
                foreach (Bookmark idx in Bookmark.Select(Criteria.Eq("User.Username", userNameFilter)))
                {
                    node["ModuleSettings"]["Articles"]["Article" + idxNo]["Header"].Value = idx.Article.Header;
                    node["ModuleSettings"]["Articles"]["Article" + idxNo]["Ingress"].Value = idx.Article.Ingress;
                    node["ModuleSettings"]["Articles"]["Article" + idxNo]["Body"].Value = idx.Article.Body;
                    node["ModuleSettings"]["Articles"]["Article" + idxNo]["Icon"].Value = "~/" + idx.Article.IconImage;
                    node["ModuleSettings"]["Articles"]["Article" + idxNo]["MainImage"].Value = "~/" + idx.Article.MainImage;
                    node["ModuleSettings"]["Articles"]["Article" + idxNo]["Author"].Value = idx.Article.Author == null ? "unknown" : idx.Article.Author.Username;
                    node["ModuleSettings"]["Articles"]["Article" + idxNo]["DatePublished"].Value = idx.Article.Published;
                    node["ModuleSettings"]["Articles"]["Article" + idxNo]["URL"].Value = "~/" + idx.Article.URL + ".aspx";
                    node["ModuleSettings"]["Articles"]["Article" + idxNo]["CommentCount"].Value = ForumPost.CountWhere(Criteria.Eq("URL", idx.Article.URL + ".aspx")).ToString();
                    idxNo += 1;
                }
                node["ModuleSettings"]["Header"].Value =
                    string.Format(Language.Instance["BookmarkedByUser", null, "Bookmarked by {0}"],
                        userNameFilter);
                node["AddToExistingCollection"].Value = true;
                ActiveEvents.Instance.RaiseLoadControl(
                    "ArticlePublisherModules.LandingPage",
                    "dynMid",
                    node);
            }


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
