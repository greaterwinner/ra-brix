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
                using (Image main = new Bitmap(250, (int)(250 * ratio)))
                {
                    using (Graphics g = Graphics.FromImage(main))
                    {
                        g.DrawImage(original, new Rectangle(0, 0, main.Width, main.Height));
                    }
                    main.Save(resourceFolder + "Images\\Medium\\" + fileName + ".png", ImageFormat.Png);
                }
            }

            // Creating actual article and saving it...
            Article a = new Article();
            a.Header = header;
            a.Ingress = ingress;
            a.IconImage = "Resources/Images/Small/" + fileName + ".png";
            a.MainImage = "Resources/Images/Medium/" + fileName + ".png";
            a.Body = body;
            a.Published = DateTime.Now;
            a.Save();
        }

        [ActiveEvent(Name = "Page_Init_InitialLoading")]
        protected void InitialLoadingOfPage(object sender, ActiveEventArgs e)
        {
            string contentId = HttpContext.Current.Request.Params["ContentID"];
            if (contentId != null)
            {
                contentId = contentId.Trim('/');
            }
            if (contentId != null)
            {
                Article a = Article.FindArticle(contentId);
                Node node = new Node();
                node["ModuleSettings"]["Header"].Value = a.Header;
                node["ModuleSettings"]["Body"].Value = a.Body;
                node["ModuleSettings"]["Date"].Value = a.Published;
                node["ModuleSettings"]["Ingress"].Value = a.Ingress;
                node["ModuleSettings"]["MainImage"].Value = "~/" + a.MainImage;
                ActiveEvents.Instance.RaiseLoadControl(
                    "ArticlePublisherModules.ViewArticle",
                    "dynMid",
                    node);
            }
            else
            {
                if (Settings.Instance["ArticlePublisherHideLandingPage"] == "True")
                    return;
                Node node = new Node();
                int idxNo = 0;
                foreach (Article idx in Article.Select(Criteria.Sort("Published", false)))
                {
                    node["ModuleSettings"]["Articles"]["Article" + idxNo]["Header"].Value = idx.Header;
                    node["ModuleSettings"]["Articles"]["Article" + idxNo]["Ingress"].Value = idx.Ingress;
                    node["ModuleSettings"]["Articles"]["Article" + idxNo]["Body"].Value = idx.Body;
                    node["ModuleSettings"]["Articles"]["Article" + idxNo]["Icon"].Value = "~/" + idx.IconImage;
                    node["ModuleSettings"]["Articles"]["Article" + idxNo]["MainImage"].Value = "~/" + idx.MainImage;
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
}
