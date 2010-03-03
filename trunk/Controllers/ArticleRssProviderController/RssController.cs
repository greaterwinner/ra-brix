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
using System.Web;
using System.Xml;
using SettingsRecords;
using ArticlePublisherRecords;
using Ra.Brix.Data;
using System.Globalization;
using Ra.Brix.Types;
using System.Web.UI.HtmlControls;
using System.Web.UI;
using System.Configuration;
using HelperGlobals;

namespace ArticleRssProviderController
{
    [ActiveController]
    public class RssController
    {
        [ActiveEvent(Name = "Page_Init_InitialLoading")]
        protected void InitialLoadingOfPage(object sender, ActiveEventArgs e)
        {
            string contentId = HttpContext.Current.Request.Params["ContentID"];
            if (contentId != null)
            {
                contentId = contentId.Trim('/');
                if (contentId == "rss")
                {
                    CreateRSS();
                    return;
                }
            }
            CreateRssLink();
        }

        private void CreateRssLink()
        {
            // Injecting the RSS link into the document header...
            HtmlGenericControl c = new HtmlGenericControl();
            c.TagName = "link";
            c.Attributes.Add("rel", "alternate");
            c.Attributes.Add("type", "application/rss+xml");
            c.Attributes.Add("title", Settings.Instance["RSSTitleText"]);
            c.Attributes.Add("href",
                ApplicationRoot.Root + "rss.aspx");
            (HttpContext.Current.Handler as Page).Header.Controls.Add(c);
        }

        private static void CreateRSS()
        {
            // Request for article RSS items...
            HttpContext.Current.Response.Clear();
            HttpContext.Current.Response.ContentType = "text/xml";

            XmlDocument doc = new XmlDocument();

            // RSS node
            XmlNode rss = doc.CreateNode(XmlNodeType.Element, "rss", "");
            XmlAttribute version = doc.CreateAttribute("version");
            version.Value = "2.0";
            rss.Attributes.Append(version);
            doc.AppendChild(rss);

            // channel node
            XmlNode channel = doc.CreateNode(XmlNodeType.Element, "channel", "");
            rss.AppendChild(channel);

            // Title node
            XmlNode title = doc.CreateNode(XmlNodeType.Element, "title", "");
            XmlNode titleContent = doc.CreateNode(XmlNodeType.Text, null, null);
            titleContent.Value = Settings.Instance["RSSTitleText"];
            title.AppendChild(titleContent);
            channel.AppendChild(title);

            // link node
            XmlNode link = doc.CreateNode(XmlNodeType.Element, "link", "");
            XmlNode linkContent = doc.CreateNode(XmlNodeType.Text, "", "");
            linkContent.Value = ApplicationRoot.Root;
            link.AppendChild(linkContent);
            channel.AppendChild(link);

            // description
            XmlNode description = doc.CreateNode(XmlNodeType.Element, "description", "");
            XmlNode descriptionContent = doc.CreateNode(XmlNodeType.Text, "", "");
            descriptionContent.Value = Settings.Instance["RSSDescriptionText"];
            description.AppendChild(descriptionContent);
            channel.AppendChild(description);

            // generator
            XmlNode generator = doc.CreateNode(XmlNodeType.Element, "generator", "");
            XmlNode generatorContent = doc.CreateNode(XmlNodeType.Text, "", "");
            generatorContent.Value = "Ra-Brix - TheLightBringer.Org";
            generator.AppendChild(generatorContent);
            channel.AppendChild(generator);

            int idxNo = 0;
            foreach (Article idx in Article.Select(Criteria.Sort("Published", false)))
            {
                if (++idxNo > 10)
                    break;
                // Item
                XmlNode item = doc.CreateNode(XmlNodeType.Element, "item", "");

                // Title
                XmlNode titleR = doc.CreateNode(XmlNodeType.Element, "title", "");
                XmlNode titleRContent = doc.CreateNode(XmlNodeType.Text, "", "");
                titleRContent.Value = idx.Header;
                titleR.AppendChild(titleRContent);
                item.AppendChild(titleR);

                // link
                XmlNode linkR = doc.CreateNode(XmlNodeType.Element, "link", "");
                XmlNode linkRContent = doc.CreateNode(XmlNodeType.Text, "", "");
                linkRContent.Value =
                    ApplicationRoot.Root +
                        idx.URL +
                        ConfigurationManager.AppSettings["DefaultPageExtension"];
                linkR.AppendChild(linkRContent);
                item.AppendChild(linkR);

                // description
                XmlNode descriptionR = doc.CreateNode(XmlNodeType.Element, "description", "");
                XmlNode descriptionRContent = doc.CreateNode(XmlNodeType.Text, "", "");
                string wholeBody =
                    string.Format(@"<img src=""{0}"" alt=""{1}"" style=""float:right;margin-left:15px;"" />",
                    ApplicationRoot.Root +
                        idx.MainImage,
                    idx.Header);
                wholeBody += idx.Body;
                wholeBody += string.Format(@"
<p>Copyright 2010 - <a href=""{0}"">{1}</a>. Licensed as <a href=""http://creativecommons.org/licenses/by-sa/3.0/"" />Creative Commons Attribution-Share Alike 3.0</a>.</p>
",
                        ApplicationRoot.Root +
                        "authors/" + idx.Author.Username + ConfigurationManager.AppSettings["DefaultPageExtension"],
                        idx.Author.Username);
                descriptionRContent.Value = wholeBody;
                descriptionR.AppendChild(descriptionRContent);
                item.AppendChild(descriptionR);

                // pubDate
                XmlNode pub = doc.CreateNode(XmlNodeType.Element, "pubDate", "");
                XmlNode pubContent = doc.CreateNode(XmlNodeType.Text, "", "");
                pubContent.Value = idx.Published.ToString(
                    "ddd, dd MMM yyyy HH:mm:ss",
                    CultureInfo.InvariantCulture);
                pub.AppendChild(pubContent);
                item.AppendChild(pub);

                channel.AppendChild(item);
            }

            // Saving to response
            doc.Save(HttpContext.Current.Response.OutputStream);
            try
            {
                // Expected to throw exception...
                HttpContext.Current.Response.End();
            }
            catch
            {
                // Catching idiotic "by design" exception...
            }
        }
    }
}
