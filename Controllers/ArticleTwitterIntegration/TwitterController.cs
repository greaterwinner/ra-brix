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
using System.Net;
using System.Web;
using System.IO;
using System;
using Ra.Brix.Types;
using LanguageRecords;
using SettingsRecords;
using System.Text;

namespace ArticleTwitterIntegration
{
    [ActiveController]
    public class TwitterController
    {
        [ActiveEvent(Name = "HandleArticleFirstTimePublished")]
        protected void HandleArticleFirstTimePublished(object sender, ActiveEventArgs e)
        {
            TweetArticle(e.Params["URL"].Get<string>());
        }

        [ActiveEvent(Name = "JustBeforeLoadingArticleBottom")]
        protected void JustBeforeLoadingArticleBottom(object sender, ActiveEventArgs e)
        {
            Node node = new Node();
            node["ModuleSettings"]["URL"].Value = "http://twitter.com/" +
                Settings.Instance["TwitterUsername"];
            node["ModuleSettings"]["Username"].Value = Settings.Instance["TwitterUsername"];
            node["AddToExistingCollection"].Value = true;
            ActiveEvents.Instance.RaiseLoadControl(
                "ArticleTwitterIntegrationModules.ShowTwitterLink",
                "dynMid",
                node);
        }

        private void TweetArticle(string url)
        {
            // Checking to see if Twitter integration is turned on...
            if (string.IsNullOrEmpty(Settings.Instance["TwitterUsername"]))
                return;

            // Tweeting our message...
            try
            {
                string user =
                    Convert.ToBase64String(
                    Encoding.UTF8.GetBytes(
                        Settings.Instance["TwitterUsername"] +
                        ":" +
                        Settings.Instance["TwitterPassword"]));
                HttpWebRequest request = WebRequest.Create(
                    "http://twitter.com/statuses/update.xml") as HttpWebRequest;
                request.ServicePoint.Expect100Continue = false;
                request.Headers.Add("Authorization", "Basic " + user);
                request.ContentType = "application/x-www-form-urlencoded";
                string status =
                        HttpContext.Current.Request.Url.OriginalString.Replace("Default.aspx", "")
                            + url + ".aspx";
                if (!string.IsNullOrEmpty(HttpContext.Current.Request.Url.Query))
                    status = status.Replace(HttpContext.Current.Request.Url.Query, "");
                byte[] bytes = System.Text.Encoding.ASCII.GetBytes("status=" + status);
                request.ContentLength = bytes.Length;
                request.Method = "POST";
                using (Stream reqStream = request.GetRequestStream())
                {
                    reqStream.Write(bytes, 0, bytes.Length);
                }
            }
            catch (Exception err)
            {
                Node nodeMessage = new Node();
                nodeMessage["Message"].Value = Language.Instance[
                    "CouldntPostToTwitter2",
                    null,
                    "Couldn't post link to twitter! Message from server was; "] + err.Message;
                nodeMessage["Duration"].Value = 2000;
                ActiveEvents.Instance.RaiseActiveEvent(
                    this,
                    "ShowInformationMessage",
                    nodeMessage);
            }
        }
    }
}
