/*
 * Ra-Brix - A Modular-based Framework for building 
 * Web Applications Copyright 2009 - Thomas Hansen 
 * thomas@ra-ajax.org. Unless permission is 
 * explicitly given this code is licensed under the 
 * GNU Affero GPL version 3 which can be found in the 
 * license.txt file on disc.
 * 
 */

using System;
using HelperGlobals;
using LanguageRecords;
using Ra;
using Ra.Brix.Loader;
using System.Web;
using System.Web.UI;
using Ra.Widgets;
using Ra.Brix.Types;
using Ra.Selector;
using Ra.Brix.Data;
using SettingsRecords;
using UserRecords;

namespace InitializeController
{
    [ActiveController]
    public class InitAndLogin
    {
        [ActiveEvent(Name = "ApplicationStartup")]
        protected static void ApplicationStartup(object sender, ActiveEventArgs e)
        {
            // Must do this just in case we've got "dead references" laying around ...
            Users.Instance.Clear();
            Language.Instance.SetDefaultValue("ButtonAdmin", "Admin");
            Language.Instance.SetDefaultValue("ButtonAppl", "Applications");
        }

        [ActiveEvent(Name = "GetMenuItems")]
        protected static void GetMenuItems(object sender, ActiveEventArgs e)
        {
            e.Params["ButtonAdmin"].Value = "Menu-ButtonAdmin";
            e.Params["ButtonAppl"].Value = "Menu-ButtonApplications";
        }

        [ActiveEvent(Name = "Page_Init_InitialLoading")]
        protected void InitialLoadingOfPage(object sender, ActiveEventArgs e)
        {
            if (HttpContext.Current.Request.Params["message"] != null)
            {
                string msgId = HttpContext.Current.Request.Params["message"];
                string msg = Language.Instance[msgId];
                Node nodeMessage = new Node();
                nodeMessage["Message"].Value = msg;
                nodeMessage["Duration"].Value = 2000;
                ActiveEvents.Instance.RaiseActiveEvent(
                    this,
                    "ShowInformationMessage",
                    nodeMessage);
            }
            LoadMenu();
            if (!string.IsNullOrEmpty(Users.LoggedInUserName))
            {
                Node node = new Node();
                node["AddToExistingCollection"].Value = true;
                ActiveEvents.Instance.RaiseLoadControl(
                    "LoginOpenIDModules.Logout",
                    "dynTop",
                    node);
            }
            else
            {
                LoadLoginModule();
            }
        }

        [ActiveEvent(Name = "GetTipOfToday")]
        protected static void GetTipOfToday(object sender, ActiveEventArgs e)
        {
            const string tmp = @"
You can override the websites which are by default opened in the portal through the Settings
editing module.

This is a global setting though and will make those changes for all users of the Portal.
";
            e.Params["Tip"]["TipOfInitialLoading"].Value = Language.Instance["TipOfInitialLoading", null, tmp];
        }

        [ActiveEvent(Name = "BlockUser")]
        protected void BlockUser(object sender, ActiveEventArgs e)
        {
            string username = e.Params["Username"].Value.ToString();
            User user = ActiveType<User>.SelectFirst(Criteria.Eq("Username", username));
            if (user == null)
            {
                throw new ApplicationException("User; " + username + " doesn't exist");
            }
            if (user.InRole("Administrator"))
            {
                // Checking to see if this is the ONLY administrator and if
                // so we cannot blck this user since then the portal would be useless...
                bool shouldBlock = false;
                foreach (User idxUser in ActiveType<User>.Select())
                {
                    if (idxUser.Username != username && 
                        idxUser.InRole("Administrator") && 
                        !idxUser.InRole("Blocked"))
                    {
                        shouldBlock = true;
                    }
                }
                if (!shouldBlock)
                {
                    Node nodeNotBlock = new Node();
                    nodeNotBlock["Message"].Value = string.Format(Language.Instance["InitializeCannotBlockInfo", null, @"The user 
{0} is the last unblocked administrator in the portal and hence cannot be blocked"], username);
                    nodeNotBlock["Duration"].Value = 5000;
                    ActiveEvents.Instance.RaiseActiveEvent(
                        this,
                        "ShowInformationMessage",
                        nodeNotBlock);
                    return;
                }
            }
            user.Roles.Add(ActiveType<Role>.SelectFirst(Criteria.Eq("Name", "Blocked")));
            user.Save();
            Node node = new Node();
            node["Message"].Value = string.Format(Language.Instance["InitializeUserIsBlockedInfo", null, @"User; {0} is now blocked"], username);
            node["Duration"].Value = 5000;
            ActiveEvents.Instance.RaiseActiveEvent(
                this,
                "ShowInformationMessage",
                node);

            // Checking to see if this is the currently logged in user...
            if (Users.LoggedInUserName == username)
            {
                // If this is "us" we're going to log out...!
                ActiveEvents.Instance.RaiseActiveEvent(
                    this,
                    "UserWantsToLogOut");
            }
        }

        [ActiveEvent(Name = "ReportUserNameAbuse")]
        protected void ReportUserNameAbuse(object sender, ActiveEventArgs e)
        {
            Node node = new Node();
            node["TabCaption"].Value = Language.Instance["InitAndLoginReportAbuseCaption", null, "Block my user"];
            node["ModuleSettings"]["Username"].Value = Users.LoggedInUserName;
            ActiveEvents.Instance.RaiseLoadControl(
                "LoginOpenIDModules.ReportAbuse",
                "dynMid",
                node);
        }

        [ActiveEvent(Name = "GetInformationAboutWhyBlocked")]
        protected void GetInformationAboutWhyBlocked(object sender, ActiveEventArgs e)
        {
        }

        [ActiveEvent(Name = "UserLoggedIn")]
        protected void UserLoggedIn(object sender, ActiveEventArgs e)
        {
            string username = e.Params.Value.ToString();
            User user = ActiveType<User>.SelectFirst(Criteria.Eq("Username", username));
            
            // Need to check if user exists from before
            if (user == null)
            {
                // Need to create our user
                user = new User {Username = username};
                if (ActiveType<User>.Count == 0)
                {
                    user.Roles.Add(ActiveType<Role>.SelectFirst(
                        Criteria.Eq("Name", "Administrator")));
                    Settings.Instance["PowerUser"] = user.Username;
                }
                else
                {
                    user.Roles.Add(ActiveType<Role>.SelectFirst(
                        Criteria.Eq("Name", Settings.Instance["DefaultRoleForUsers"])));
                }
                user.Save();
            }
            else
            {
                if (user.InRole("Blocked"))
                {
                    Node blockedNode = new Node();
                    blockedNode["Message"].Value = string.Format(Language.Instance["InitializeBlockedUserInfo", null, 
                                                                                   @"Your user have been blocked, 
please contact administrator of portal to fix this."]);
                    blockedNode["Duration"].Value = 8000;
                    blockedNode["EventToFire"].Value = "GetInformationAboutWhyBlocked";
                    ActiveEvents.Instance.RaiseActiveEvent(
                        this,
                        "ShowInformationMessage",
                        blockedNode);
                    return;
                }
            }

            Users.LoggedInUserName = username;
            Users.Instance.Add(Users.LoggedInUserName);

            Node userNode = new Node();
            userNode["Username"].Value = username;
            ActiveEvents.Instance.RaiseActiveEvent(
                this,
                "UserLoggedInBeforeInit",
                userNode);

            LoadMenu();
            ActiveEvents.Instance.RaiseLoadControl(
                "LoginOpenIDModules.Logout", 
                "dynTop");

            DateTime lastLoggedIn = user.LastLoggedIn;
            user.LastLoggedIn = DateTime.Now;
            user.Save();

            // Showing message with "Last logged in message" in notification area...
            if (lastLoggedIn == DateTime.MinValue)
            {
                // First loggin...!
                Node node = new Node();
                node["Message"].Value = string.Format(
                    Language.Instance["InitializeWelcomeInfo", null, 
                                      @"Hello {0} and welcome to this portal for the first time. 
You might want to take a look at the help system since this is your first time here..."],
                    Users.LoggedInUserName);
                node["Duration"].Value = 8000;
                node["EventToFire"].Value = "ViewHelpModule";
                ActiveEvents.Instance.RaiseActiveEvent(
                    this,
                    "ShowInformationMessage",
                    node);
            }
            else
            {
                // Not first login...
                Node node = new Node();
                node["Message"].Value = string.Format(Language.Instance["InitializeLastLoggedInInfo", null,
@"Hello {0}, the last time you logged on was 
<span style=""text-decoration:underline;"">{1}</span>. 
If this is not correct, then please click the button..."],
                                                      Users.LoggedInUserName,
                                                      lastLoggedIn.ToString("dddd 2 MMM yyyy HH:mm"));
                node["Duration"].Value = 2000;
                node["EventToFire"].Value = "ReportUserNameAbuse";
                ActiveEvents.Instance.RaiseActiveEvent(
                    this,
                    "ShowInformationMessage",
                    node);
            }
            ActiveEvents.Instance.RaiseActiveEvent(
                this, 
                "AfterUserLoggedIn", 
                e.Params);
        }

        [ActiveEvent(Name = "UserWantsToLogOut")]
        protected void UserWantsToLogOut(object sender, ActiveEventArgs e)
        {
            Users.Instance.Remove(Users.LoggedInUserName);
            Users.LoggedInUserName = null;
            string url = "~/";
            if (!string.IsNullOrEmpty(HttpContext.Current.Request.Params["ContentID"]))
            {
                url += HttpContext.Current.Request.Params["ContentID"].Trim('/') + ".aspx";
            }
            AjaxManager.Instance.Redirect(url);
        }

        private void LoadLoginModule()
        {
            Node node = new Node();
            node["GiveFocus"].Value = Settings.Instance["AutoGiveLoginFocus"] == "True";
            node["AddToExistingCollection"].Value = true;
            ActiveEvents.Instance.RaiseLoadControl(
                "LoginOpenIDModules.Login",
                "dynTop",
                node);
            if (Settings.Instance["HideLogin"] == "True")
            {
                if (HttpContext.Current.Request.Params["ShowLogin"] != "true")
                {
                    ActiveEvents.Instance.RaiseActiveEvent(
                        this,
                        "HideLoginModule");
                }
            }
        }

        private static void LoadMenu()
        {
            // We don't load menu if this is just the OpenID login re-direct request...
            // This to avoid wasted resources, but also to avoid a bug in the SlidingMenu...
            if (HttpContext.Current.Session["Ra.Brix.PluinsViews.LoginpenID.LoggedIn"] != null)
                return;

            if (Settings.Instance.Get("MenuType", "SlidingMenu") == "SlidingMenu")
            {
                Node tmp = new Node();
                tmp["ModuleSettings"]["CustomBreadCrumb"].Value =
                    Selector.FindControl<RaWebControl>(
                        (HttpContext.Current.Handler as Page),
                        "customBreadParent");
                ActiveEvents.Instance.RaiseLoadControl(
                    "SlidingMenuModules.SlidingMenu",
                    "dynLeft",
                    tmp);
            }
            else
            {
                Node tmp = new Node();
                tmp["ModuleSettings"]["CustomBreadCrumb"].Value =
                    Selector.FindControl<RaWebControl>(
                        (HttpContext.Current.Handler as Page),
                        "customBreadParent");
                ActiveEvents.Instance.RaiseLoadControl(
                    "TreeMenuModules.TreeMenu",
                    "dynLeft",
                    tmp);
            }
        }
    }
}