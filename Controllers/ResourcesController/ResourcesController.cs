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

        [ActiveEvent(Name = "ImageSelectedFromFlickr")]
        protected void ImageSelectedFromFlickr(object sender, ActiveEventArgs e)
        {
            string activeFolder = e.Params["ActiveFolder"].Get<string>();
            string imageUrl = e.Params["ImageURL"].Get<string>();
            string imageName = e.Params["ImageName"].Get<string>();

            int index = 0;
            while (index < imageName.Length)
            {
                if (("ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789()_").IndexOf(imageName[index]) == -1)
                {
                    imageName = imageName.Substring(0, index) + "-" + imageName.Substring(index + 1);
                }
                index += 1;
            }
            imageName = imageName.Trim('-');
            bool found = true;
            while (found)
            {
                found = false;
                if (imageName.IndexOf("--") != -1)
                {
                    imageName = imageName.Replace("--", "-");
                    found = true;
                }
            }            
            
            imageName += imageUrl.Substring(imageUrl.LastIndexOf("."));
            HttpWebRequest request = WebRequest.Create(imageUrl) as HttpWebRequest;
            using (HttpWebResponse response = request.GetResponse() as HttpWebResponse)
            {
                Stream input = response.GetResponseStream();
                using (Stream output = new FileStream(activeFolder + "\\" + imageName, FileMode.Create))
                {
                    byte[] buffer = new byte[8096];
                    int read;
                    while ((read = input.Read(buffer, 0, buffer.Length)) > 0)
                    {
                        output.Write(buffer, 0, read);
                    }
                }
            }

            // Reopening up Resource Dialog...
            Node node = new Node();
            node["TabCaption"].Value =
                Language.Instance["SelectImage", null, "Select Image"];
            node["Width"].Value = 800;
            node["Height"].Value = 310;
            node["ModuleSettings"]["Mode"].Value = "Select";
            node["ModuleSettings"]["EventToRaise"].Value = "FileExplorerImageFileChosen";
            ActiveEvents.Instance.RaiseLoadControl(
                "ResourcesModules.Explorer",
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
