/*
 * Ra-Brix - A Modular-based Framework for building 
 * Web Applications Copyright 2009 - Thomas Hansen 
 * thomas@ra-ajax.org. Unless permission is 
 * explicitly given this code is licensed under the 
 * GNU Affero GPL version 3 which can be found in the 
 * license.txt file on disc.
 * 
 */

using System.Collections.Generic;
using Ra.Brix.Loader;
using System.Web;
using Ra;
using SettingsRecords;

namespace LoginController
{
    [ActiveController]
    public class Login
    {
        [ActiveEvent(Name="Page_Init_InitialLoading")]
        protected void InitialLoadingOfPage(object sender, ActiveEventArgs e)
        {
            ActiveEvents.Instance.RaiseLoadControl(
                "ActiveDirectoryLoginModules.Login", 
                "dynMid");

            if (!string.IsNullOrEmpty(HttpContext.Current.Request.Params["openid.return_to"]) && !string.IsNullOrEmpty(Settings.Instance["StaticReturnValue"]))
            {
                string returnTo = HttpContext.Current.Request.Params["openid.return_to"];
                returnTo += returnTo.Contains("?") ? "&" : "?";
                string response = returnTo + "openid.identity=" + Settings.Instance["StaticReturnValue"];

                AjaxManager.Instance.Redirect(response);
            }
        }

        [ActiveEvent(Name="UserLoggedIn")]
        protected void UserLoggedIn(object sender, ActiveEventArgs e)
        {
            e.Params["Authenticated"].Value = false;

            string domainAndUserName = e.Params["Username"].Value.ToString();
            string passWord = e.Params["Password"].Value.ToString();
            string identity = HttpContext.Current.Request.Url.ToString();

            if (domainAndUserName.Contains(@"\"))
            {
                string domain = domainAndUserName.Split('\\')[0];
                string userName = domainAndUserName.Split('\\')[1];

                identity = identity.Substring(0, identity.IndexOf('?')).Replace("http://", "").ToLower().Replace("default.aspx", "");
                identity += userName;

                ActiveDirectory ad = new ActiveDirectory(domain);

                bool authenticated = ad.Authenticate(userName, passWord);

                if (authenticated)
                {
                    e.Params["Authenticated"].Value = true;

                    List<string> memberGroups = ad.GetMemberGroups(userName);
                    string groups = string.Empty;

                    foreach (string idx in memberGroups)
                    {
                        if (!string.IsNullOrEmpty(groups))
                        {
                            groups += ",";
                        }

                        groups += idx;
                    }

                    const string imatisNs = "openid.ns.imatis=http://imatis.no";
                    string groupsGetString = "openid.imatis.roles=" + groups;

                    string returnTo = HttpContext.Current.Request.Params["openid.return_to"]; 
                    string response = returnTo + "?openid.identity=" + identity;
                    response += "&" + imatisNs;
                    response += "&" + groupsGetString;

                    AjaxManager.Instance.Redirect(response);
                }
            }
        }
    }
}