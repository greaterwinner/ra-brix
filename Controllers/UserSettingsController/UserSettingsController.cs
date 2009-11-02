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
using Ra.Brix.Data;
using Ra.Brix.Loader;
using Ra.Brix.Types;
using UserSettingsRecords;

namespace UserSettingsController
{
    [ActiveController]
    public class UserSettingsController
    {
        [ActiveEvent(Name = "ApplicationStartup")]
        protected static void ApplicationStartup2(object sender, ActiveEventArgs e)
        {
            Language.Instance.SetDefaultValue("ButtonUserSettings", "User Settings");
        }

        [ActiveEvent(Name = "GetMenuItems")]
        protected void GetMenuItems(object sender, ActiveEventArgs e)
        {
            e.Params["ButtonAdmin"]["ButtonUserSettings"].Value = "Menu-EditUserSettings";
        }

        [ActiveEvent(Name = "Menu-EditUserSettings")]
        protected void EditUserSettings(object sender, ActiveEventArgs e)
        {
            Node node = new Node();
            node["TabCaption"].Value = "Settings";

            node["ModuleSettings"]["DeleteEventName"].Value = "UserSettingDeleted";
            node["ModuleSettings"]["EditEventName"].Value = "UserSettingEdited";

            // Columns...
            node["ModuleSettings"]["Grid"]["Columns"]["Key"]["Caption"].Value = Language.Instance["SettingsKeyButton", null, "Key"];
            node["ModuleSettings"]["Grid"]["Columns"]["Key"]["ControlType"].Value = "Label";

            node["ModuleSettings"]["Grid"]["Columns"]["Value"]["Caption"].Value = Language.Instance["SettingsValueButton", null, "Value"];
            node["ModuleSettings"]["Grid"]["Columns"]["Value"]["ControlType"].Value = "InPlaceEdit";

            int idxNo = 0;
            foreach (UserSettings.Setting idx in UserSettings.Instance)
            {
                if (idx.Username == Users.LoggedInUserName)
                {
                    node["ModuleSettings"]["Grid"]["Rows"]["Row" + idxNo]["ID"].Value = idx.ID;
                    node["ModuleSettings"]["Grid"]["Rows"]["Row" + idxNo]["Key"].Value = idx.Name;
                    node["ModuleSettings"]["Grid"]["Rows"]["Row" + idxNo]["Value"].Value = idx.Value;
                    idxNo += 1;
                }
            }

            ActiveEvents.Instance.RaiseLoadControl(
                "SettingsModules.ViewSettings",
                "dynMid",
                node);
        }

        [ActiveEvent(Name = "UserSettingEdited")]
        protected void UserSettingEdited(object sender, ActiveEventArgs e)
        {
            UserSettings.Setting setting = ActiveType<UserSettings.Setting>.SelectByID(int.Parse(e.Params["ID"].Get<string>()));
            UserSettings.Instance[setting.Name, Users.LoggedInUserName] = e.Params["Value"].Get<string>();
        }

        [ActiveEvent(Name = "UserSettingDeleted")]
        protected void UserSettingDeleted(object sender, ActiveEventArgs e)
        {
            UserSettings.Setting set = ActiveType<UserSettings.Setting>.SelectByID(int.Parse(e.Params["ID"].Get<string>()));
            UserSettings.Instance.Remove(set);

            Node node = new Node();
            node["Message"].Value =
Language.Instance["ViwSettingsInfo", null, @"Note that setting might be re-created when accessed by 
another module from the default value from your configuration file"];
            node["Duration"].Value = 10000;
            ActiveEvents.Instance.RaiseActiveEvent(this, "ShowInformationMessage", node);
        }
    }
}




















