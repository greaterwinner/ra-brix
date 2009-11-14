/*
 * Ra-Brix - A Modular-based Framework for building 
 * Web Applications Copyright 2009 - Thomas Hansen 
 * thomas@ra-ajax.org. Unless permission is 
 * explicitly given this code is licensed under the 
 * GNU Affero GPL version 3 which can be found in the 
 * license.txt file on disc.
 * 
 */

using HelperGlobals;
using LanguageRecords;
using Ra.Brix.Loader;
using Ra.Brix.Types;
using Ra;
using UserSettingsRecords;

namespace LanguageController
{
    [ActiveController]
    public class LanguageController
    {
        [ActiveEvent(Name = "ApplicationStartup")]
        protected static void ApplicationStartup(object sender, ActiveEventArgs e)
        {
            Language.Instance.SetDefaultValue("ButtonLang", "Language");
            Language.Instance.SetDefaultValue("ButtonEditLanguage", "Edit");
            Language.Instance.SetDefaultValue("ButtonSelectLanguage", "Switch");
        }

        [ActiveEvent(Name = "GetMenuItems")]
        protected void GetMenuItems(object sender, ActiveEventArgs e)
        {
            e.Params["ButtonAdmin"]["ButtonLang"].Value = "Menu-LanguageGroup";
            e.Params["ButtonAdmin"]["ButtonLang"]["ButtonEditLanguage"].Value = "Menu-EditLanguage";
            e.Params["ButtonAdmin"]["ButtonLang"]["ButtonSelectLanguage"].Value = "Menu-SelectLanguage";
        }

        [ActiveEvent(Name = "Menu-EditLanguage")]
        protected void EditLanguageMethod(object sender, ActiveEventArgs e)
        {
            Node init = new Node();
            init["TabCaption"].Value = Language.Instance["LanguageEditCaption", null, "Edit Language; "] + Language.Instance.UserLanguage;
            ActiveEvents.Instance.RaiseLoadControl(
                "LanguageEditModules.EditLanguage",
                "dynMid",
                init);
        }

        [ActiveEvent(Name = "Menu-SelectLanguage")]
        protected void SelectLanguage(object sender, ActiveEventArgs e)
        {
            Node init = new Node();
            init["TabCaption"].Value = Language.Instance["LanguageSelectCaption", null, "Select Language"];
            ActiveEvents.Instance.RaiseLoadControl(
                "LanguageSelectorModules.LanguageSelector",
                "dynMid",
                init);
        }

        [ActiveEvent(Name = "ChangeUserLanguage")]
        protected void ChangeUserLanguage(object sender, ActiveEventArgs e)
        {
            string language = e.Params["Language"].Get<string>();
            Language.Instance.UserLanguage = language;
            UserSettings.Instance["Language", Users.LoggedInUserName] = language;
            AjaxManager.Instance.Redirect("~/?message=LanguageChanged");
        }

        [ActiveEvent(Name = "UserLoggedInBeforeInit")]
        protected void UserLoggedInBeforeInit(object sender, ActiveEventArgs e)
        {
            string username = e.Params["Username"].Get<string>();
            string language = UserSettings.Instance["Language", username];

            if (!string.IsNullOrEmpty(language))
            {
                Language.Instance.UserLanguage = language;
            }
        }
    }
}