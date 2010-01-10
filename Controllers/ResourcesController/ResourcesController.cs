/*
 * Ra-Brix - A Modular-based Framework for building 
 * Web Applications Copyright 2009 - Thomas Hansen 
 * thomas@ra-ajax.org. Unless permission is 
 * explicitly given this code is licensed under the 
 * GNU Affero GPL version 3 which can be found in the 
 * license.txt file on disc.
 * 
 */

using LanguageRecords;
using Ra.Brix.Loader;
using Ra.Brix.Types;
using SettingsRecords;
using System.Web;
using System.Net;
using System.IO;
using System.Xml;

namespace ResourcesController
{
    [ActiveController]
    public class ResourcesController
    {
        [ActiveEvent(Name = "ApplicationStartup")]
        protected static void ApplicationStartup2(object sender, ActiveEventArgs e)
        {
            Language.Instance.SetDefaultValue("ButtonResourceManager", "Resources");
        }

        [ActiveEvent(Name = "GetMenuItems")]
        protected void GetMenuItems(object sender, ActiveEventArgs e)
        {
            e.Params["ButtonAppl"]["ButtonResourceManager"].Value = "Menu-ResourceManager";
        }

        [ActiveEvent(Name = "RequestFlickrResourceDownloadDialog")]
        protected void RequestFlickrResourceDownloadDialog(object sender, ActiveEventArgs e)
        {
            Node node = new Node();
            node["TabCaption"].Value =
                Language.Instance["ResourceExplorer", null, "Search Flickr"];
            node["Width"].Value = 800;
            node["Height"].Value = 500;
            node["ModuleSettings"]["ActiveFolder"].Value = e.Params["ActiveFolder"].Value;
            ActiveEvents.Instance.RaiseLoadControl(
                "ResourcesModules.FlickrExplorer",
                "dynPopup",
                node);
        }

        [ActiveEvent(Name = "SearchFlickrForImages")]
        protected void SearchFlickrForImages(object sender, ActiveEventArgs e)
        {
            string key = Settings.Instance["FlickrAPIKey"];
            string secret = Settings.Instance["FlickrAPISecret"];
            string query = "http://api.flickr.com/services/rest/" +
            "?method=flickr.photos.search&api_key=" + key + 
            "&tags=&text=" +
            HttpContext.Current.Server.UrlPathEncode(e.Params["Query"].Get<string>()) +
            "&license=5"; // 5 == CC Share Alike...!

            HttpWebRequest request = WebRequest.Create(query) as HttpWebRequest;

            using (TextReader reader = new StreamReader(request.GetResponse().GetResponseStream()))
            {
                string xml = reader.ReadToEnd();
                XmlDocument doc = new XmlDocument();
                doc.LoadXml(xml);
                int idxNo = 0;
                foreach (XmlNode idx in doc.SelectNodes("/rsp/photos/photo"))
                {
                    string farm = idx.Attributes["farm"].Value;
                    string server = idx.Attributes["server"].Value;
                    string title = idx.Attributes["title"].Value;
                    if (string.IsNullOrEmpty(title))
                        title = "Empty";
                    string id = idx.Attributes["id"].Value;
                    string owner = idx.Attributes["owner"].Value;
                    string fileSecret = idx.Attributes["secret"].Value;
                    string urlOfThumb = string.Format(
                        "http://farm{0}.static.flickr.com/{1}/{2}_{3}_t.jpg",
                        farm, 
                        server,
                        id,
                        fileSecret);
                    string urlOfFile = string.Format(
                        "http://farm{0}.static.flickr.com/{1}/{2}_{3}.jpg",
                        farm, 
                        server,
                        id,
                        fileSecret);
                    e.Params["Images"]["Image" + idxNo]["Thumb"].Value = urlOfThumb;
                    e.Params["Images"]["Image" + idxNo]["Title"].Value = title;
                    e.Params["Images"]["Image" + idxNo]["Medium"].Value = urlOfFile;
                    idxNo += 1;
                }
            }
        }

        [ActiveEvent(Name = "Menu-ResourceManager")]
        protected void OpenResourceManager(object sender, ActiveEventArgs e)
        {
            Node node = new Node();
            node["TabCaption"].Value = 
                Language.Instance["ResourceExplorer", null, "Resource Explorer"];
            ActiveEvents.Instance.RaiseLoadControl(
                "ResourcesModules.Explorer",
                "dynMid",
                node);
        }
    }
}
