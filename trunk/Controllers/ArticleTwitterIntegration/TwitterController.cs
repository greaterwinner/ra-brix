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
using UserRecords;
using Ra.Brix.Data;
using HelperGlobals;
using UserSettingsRecords;
using System.Configuration;

namespace ArticleTwitterIntegration
{
    [ActiveController]
    public class TwitterController
    {
        [ActiveEvent(Name = "ApplicationStartup")]
        protected static void ApplicationStartup(object sender, ActiveEventArgs e)
        {
            Language.Instance.SetDefaultValue("ButtonSetTwitterSettings", "Twitter");
        }

        [ActiveEvent(Name = "GetMenuItems")]
        protected void GetMenuItems(object sender, ActiveEventArgs e)
        {
            e.Params["ButtonSetTwitterSettings"].Value = "Menu-EditTwitter";
        }

        [ActiveEvent(Name = "Menu-EditTwitter")]
        protected void EditTwitter(object sender, ActiveEventArgs e)
        {
            Node node = new Node();
            node["ModuleSettings"]["Username"].Value = UserSettings.Instance["TwitterUsername", Users.LoggedInUserName];
            node["ModuleSettings"]["Password"].Value = UserSettings.Instance["TwitterPassword", Users.LoggedInUserName];
            ActiveEvents.Instance.RaiseLoadControl(
                "ArticleTwitterIntegrationModules.EditTwitterSettings",
                "dynMid",
                node);
        }

        [ActiveEvent(Name = "SaveTwitterUsernameAndPassword")]
        protected void SaveTwitterUsernameAndPassword(object sender, ActiveEventArgs e)
        {
            string username = e.Params["Username"].Get<string>();
            string password = e.Params["Password"].Get<string>();
            UserSettings.Instance["TwitterUsername", Users.LoggedInUserName] = username;
            UserSettings.Instance["TwitterPassword", Users.LoggedInUserName] = password;
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

        // TODO: Make Async...!
        [ActiveEvent(Name = "HandleArticleFirstTimePublished")]
        protected void HandleArticleFirstTimePublished(object sender, ActiveEventArgs e)
        {
            // Checking to see if Twitter integration is turned on...
            if (string.IsNullOrEmpty(Settings.Instance["TwitterUsername"]))
                return;
            if (string.IsNullOrEmpty(Settings.Instance["TwitterPassword"]))
                return;

            try
            {
                // Tweeting to The MAIN / SYSTEM Twitter account...
                TweetArticle(
                    e.Params["URL"].Get<string>(),
                    e.Params["Header"].Get<string>(),
                    Settings.Instance["TwitterUsername"],
                    Settings.Instance["TwitterPassword"]);

                // Then stweeting per user level...
                string twitterUsername = UserSettings.Instance["TwitterUsername", Users.LoggedInUserName];
                string twitterPassword = UserSettings.Instance["TwitterPassword", Users.LoggedInUserName];
                TweetArticle(
                    e.Params["URL"].Get<string>(),
                    e.Params["Header"].Get<string>(),
                    twitterUsername,
                    twitterPassword);
            }
            catch(Exception err)
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

        private void TweetArticle(string url, string header, string username, string pwd)
        {
            // Tweeting our message...
            string user =
                Convert.ToBase64String(
                Encoding.UTF8.GetBytes(
                    username +
                    ":" +
                    pwd));
            HttpWebRequest request = WebRequest.Create(
                "http://twitter.com/statuses/update.xml") as HttpWebRequest;
            request.ServicePoint.Expect100Continue = false;
            request.Headers.Add("Authorization", "Basic " + user);
            request.ContentType = "application/x-www-form-urlencoded";
            string status =
                    HttpContext.Current.Request.Url.OriginalString.Replace(":80", "")
                        .Replace("Default.aspx", "")
                        .Replace("default.aspx", "")
                        + url +
                        ConfigurationManager.AppSettings["DefaultPageExtension"];
            if (!string.IsNullOrEmpty(HttpContext.Current.Request.Url.Query))
                status = status.Replace(HttpContext.Current.Request.Url.Query, "");
            status += " - ";
            status += header;
            byte[] bytes = System.Text.Encoding.ASCII.GetBytes("status=" + status);
            request.ContentLength = bytes.Length;
            request.Method = "POST";
            using (Stream reqStream = request.GetRequestStream())
            {
                reqStream.Write(bytes, 0, bytes.Length);
            }
        }
    }
}
