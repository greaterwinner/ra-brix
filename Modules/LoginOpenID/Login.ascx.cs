﻿/*
 * Ra-Brix - A Modular-based Framework for building 
 * Web Applications Copyright 2009 - Thomas Hansen 
 * thomas@ra-ajax.org. Unless permission is 
 * explicitly given this code is licensed under the 
 * GNU Affero GPL version 3 which can be found in the 
 * license.txt file on disc.
 * 
 */

using System;
using LanguageRecords;
using Ra.Brix.Loader;
using Ra.Effects;
using System.Web;
using System.Text;
using System.Net;
using System.Text.RegularExpressions;
using Ra.Brix.Types;
using Ra;
using Ra.Widgets;
using System.Globalization;

namespace LoginOpenIDModules
{
    [ActiveModule]
    public class Login : System.Web.UI.UserControl, IModule
    {
        protected global::Ra.Widgets.TextBox openIdURL;
        protected global::Ra.Widgets.Button logInButton;
        protected global::Ra.Widgets.Panel openIdWrp;
 
        private static readonly Regex REGEX_LINK = 
            new Regex(
                @"<link rel=""openid.server"" href=""(?<server>.*)""[^>]*/?>", 
                RegexOptions.IgnoreCase | 
                RegexOptions.Compiled);

        protected void Page_Load(object sender, EventArgs e)
        {
            logInButton.DataBind();
            Page.LoadComplete += Page_LoadComplete;
        }

        [ActiveEvent(Name = "HideLoginModule")]
        protected void HideLoginModule(object sender, ActiveEventArgs e)
        {
            openIdWrp.Style[Styles.display] = "none";
        }

        private void Page_LoadComplete(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                string openIdIdentity = Request.Params["openid.identity"];
                if (!string.IsNullOrEmpty(openIdIdentity))
                {
                    // Successful login...
                    // Doing nother roundtrip to strip away all parameters retrieved from the
                    // OpenID provider. And after that raising our Logged In Event
                    openIdIdentity = openIdIdentity.Replace("http://", "").Replace("https://", "");
                    Session["Ra.Brix.PluinsViews.LoginpenID.LoggedIn"] = openIdIdentity;
                    string roles = Request.Params["openid.imatis.roles"];
                    if (!string.IsNullOrEmpty(roles))
                    {
                        Session["Ra.Brix.PluinsViews.LoginpenID.Roles"] = roles;
                    }
                    string url = HttpContext.Current.Request.Url.ToString();
                    url = url.Substring(0, url.IndexOf("?"));
                    if (url.EndsWith("default.aspx", true, CultureInfo.InvariantCulture))
                    {
                        url = url.Replace("default.aspx", "");
                        url = url.Replace("Default.aspx", "");
                        url = url.Replace("DEFAULT.ASPX", "");
                    }
                    Response.Redirect(url, true);
                }
                else
                {
                    // Checking to see if this is our second roundtrip around...
                    if (Session["Ra.Brix.PluinsViews.LoginpenID.LoggedIn"] != null)
                    {
                        string openIdentity = (string)Session["Ra.Brix.PluinsViews.LoginpenID.LoggedIn"];
                        StoreOpenIDInCookie(openIdentity);
                        Node node = new Node("Username", openIdentity);

                        string roles = Session["Ra.Brix.PluinsViews.LoginpenID.Roles"] as string;
                        if (!string.IsNullOrEmpty(roles))
                        {
                            string[] groups = roles.Split(',');
                            int idxNo = 0;
                            foreach (string idx in groups)
                            {
                                node["Groups"]["Group" + idxNo].Value = idx;
                                idxNo += 1;
                            }
                        }

                        Session["Ra.Brix.PluinsViews.LoginpenID.LoggedIn"] = null;
                        ActiveEvents.Instance.RaiseActiveEvent(
                            this,
                            "UserLoggedIn",
                            node);
                    }
                }
            }
        }

        private void StoreOpenIDInCookie(string openIdentity)
        {
            // Storing Username in cookie
            HttpCookie usernameCookie = new HttpCookie("LoginOpenIDModules.Login.Username")
            {
                Expires = DateTime.Now.AddMonths(1),
                HttpOnly = true,
                Value = openIdentity
            };
            (HttpContext.Current.Response).Cookies.Add(usernameCookie);
        }
 
        protected void logInButton_Click(object sender, EventArgs e)
        {
            string url = openIdURL.Text;
            if (!url.Contains("http"))
                url = "http://" + url;

            WebClient getOpenIdPage = new WebClient();
            string html = getOpenIdPage.DownloadString(url);

            string openIdServer = null;
            foreach (Match match in REGEX_LINK.Matches(html))
            {
                openIdServer = match.Groups["server"].Value;
            }
            if (string.IsNullOrEmpty(openIdServer))
                throw new Exception("Couldn't find any OpenID providers there");

            if (!openIdServer.Contains("http"))
                openIdServer = "http://" + openIdServer;

            string currentURL = HttpContext.Current.Request.Url.ToString();
            if (currentURL.EndsWith("default.aspx", true, CultureInfo.InvariantCulture))
            {
                currentURL = currentURL.Replace("default.aspx", "");
                currentURL = currentURL.Replace("Default.aspx", "");
                currentURL = currentURL.Replace("DEFAULT.ASPX", "");
            }

            StringBuilder getRequest = new StringBuilder();
            getRequest.Append(openIdServer);
            getRequest.Append("?openid.mode=checkid_setup");
            getRequest.Append("&openid.identity=" + 
                Server.UrlEncode(url));
            getRequest.Append("&openid.return_to=" + 
                Server.UrlEncode(currentURL));
            getRequest.Append("&openid.trust_root=" + 
                Server.UrlEncode(currentURL));

            AjaxManager.Instance.Redirect(getRequest.ToString());
        }

        private bool _shouldGiveTextBoxFocus;
        public void InitialLoading(Node node)
        {
            GetUserNameFromCookie();
            _shouldGiveTextBoxFocus = node["GiveFocus"].Get<bool>();
        }

        protected override void OnPreRender(EventArgs e)
        {
            if (_shouldGiveTextBoxFocus)
            {
                new EffectFocusAndSelect(openIdURL)
                    .Render();
            }
            base.OnPreRender(e);
        }

        private void GetUserNameFromCookie()
        {
            HttpCookie userNameCookie = Request.Cookies["LoginOpenIDModules.Login.Username"];
            if (userNameCookie != null && !string.IsNullOrEmpty(userNameCookie.Value))
            {
                openIdURL.Text = userNameCookie.Value;
            }
        }

        public string GetCaption()
        {
            return Language.Instance["OpenIDCaption", null, "Login with OpenID"];
        }
    }
}