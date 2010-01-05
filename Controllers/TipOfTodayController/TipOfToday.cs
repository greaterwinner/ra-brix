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
using UserSettingsRecords;

namespace TipOfTodayController
{
    [ActiveController]
    public class TipOfToday
    {
        [ActiveEvent(Name = "AfterUserLoggedIn")]
        protected void AfterUserLoggedIn(object sender, ActiveEventArgs e)
        {
            if (UserSettings.Instance.Get("ShowTipOfToday", Users.LoggedInUserName, true))
            {
                Node node = new Node();
                node["ModuleSettings"]["Username"].Value = Users.LoggedInUserName;
                node["Caption"].Value = Language.Instance["TipOfToday", null, "Tip of today"];
                ActiveEvents.Instance.RaiseLoadControl(
                    "TipOfTodayModules.TipOfToday",
                    "dynMid",
                    node);
            }
        }
    }
}
