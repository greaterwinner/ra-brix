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
using System.Text.RegularExpressions;
using System.Net;
using System.IO;
using System.Web;
using CookComputing.XmlRpc;
using System;

namespace ArticlePingbackController
{
    [ActiveController]
    public class PingbackController
    {
        private static readonly Regex REGEX_LINK =
            new Regex(
                @"href=""http://(?<server>[^""]*)",
                RegexOptions.IgnoreCase |
                RegexOptions.Compiled);

        private static readonly Regex PINGBACK_LINK =
            new Regex(
                @"<link rel=""pingpack"" href=""(?<server>.*)""[^>]*/?>",
                RegexOptions.IgnoreCase |
                RegexOptions.Compiled);

        private static readonly Regex PINGBACK_URL =
            new Regex(
                @"http://(?<server>\S*)",
                RegexOptions.IgnoreCase |
                RegexOptions.Compiled);

        [ActiveEvent(Name = "UserCreatedCommentWasSaved", Async = true)]
        protected void UserCreatedAnArticleCommentPingURLs(object sender, ActiveEventArgs e)
        {
            string urlErrors = "";
            string comment = e.Params["Comment"].Get<string>();
            string url = e.Params["URL"].Get<string>();
            foreach (Match match in PINGBACK_URL.Matches(comment))
            {
                string urlToLink = "http://" + match.Groups["server"].Value;
                urlErrors = DoPingback(url, urlErrors, urlToLink);
            }
            if (!string.IsNullOrEmpty(urlErrors))
            {
                // TODO: Log these URL's or something...?
                // urlErrors that is...
            }
        }

        [ActiveEvent(Name = "HandleArticleFirstTimePublished", Async = true)]
        protected void HandleArticleFirstTimePublished(object sender, ActiveEventArgs e)
        {
            string url = e.Params["URL"].Get<string>();
            string header = e.Params["Header"].Get<string>();
            string body = " " + e.Params["Body"].Get<string>() + " ";

            string urlErrors = "";
            foreach (Match match in REGEX_LINK.Matches(body))
            {
                string urlToLink = "http://" + match.Groups["server"].Value;
                urlErrors = DoPingback(url, urlErrors, urlToLink);
            }
            if (!string.IsNullOrEmpty(urlErrors))
            {
                // TODO: Log these URL's or something...?
                // urlErrors that is...
            }
        }

        private static string DoPingback(string url, string urlErrors, string urlToLink)
        {
            try
            {
                HttpWebRequest webRequest = WebRequest.Create(urlToLink) as HttpWebRequest;
                using (HttpWebResponse response = webRequest.GetResponse() as HttpWebResponse)
                {
                    string pingBackServerUrl = null;
                    if (!string.IsNullOrEmpty(response.Headers["X-Pingback"]))
                    {
                        // Pingback header existed...
                        pingBackServerUrl = response.Headers["X-Pingback"];
                    }
                    else
                    {
                        // No pingback header, parsing document for <link...!
                        using (TextReader reader = new StreamReader(response.GetResponseStream()))
                        {
                            string htmlOfLink = reader.ReadToEnd();
                            foreach (Match mPBServer in PINGBACK_LINK.Matches(htmlOfLink))
                            {
                                pingBackServerUrl = "http://" + mPBServer.Groups["server"].Value;
                                break;
                            }
                        }
                    }
                    if (!string.IsNullOrEmpty(pingBackServerUrl))
                    {
                        // We've got a pingback link ...! :)
                        string linkToSelf = url;
                        pingBackServerUrl = pingBackServerUrl.Replace("&amp;", "&")
                            .Replace("&lt;", "<")
                            .Replace("&gt;", ">")
                            .Replace("&quot;", "\"");

                        // Doing the actual Pingback...!
                        IPingback proxy = XmlRpcProxyGen.Create<IPingback>();
                        proxy.Url = pingBackServerUrl;
                        proxy.Pingback(linkToSelf, urlToLink);
                    }
                }
            }
            catch (Exception)
            {
                urlErrors += urlToLink + "\r\n";
            }
            return urlErrors;
        }
    }
}
