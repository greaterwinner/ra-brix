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
using Ra.Brix.Data;
using Ra.Brix.Loader;
using Ra.Brix.Types;
using SettingsRecords;

namespace SettingsController
{
    [ActiveController]
    public class SettingsController
    {
        [ActiveEvent(Name = "ApplicationStartup")]
        protected static void ApplicationStartup2(object sender, ActiveEventArgs e)
        {
            Language.Instance.SetDefaultValue("ButtonSettings", "Settings");
        }

        [ActiveEvent(Name = "GetMenuItems")]
        protected void GetMenuItems(object sender, ActiveEventArgs e)
        {
            e.Params["ButtonAdmin"]["ButtonSettings"].Value = "Menu-EditSettings";
        }

        [ActiveEvent(Name = "Menu-EditSettings")]
        protected void EditSettings(object sender, ActiveEventArgs e)
        {
            Node node = new Node();
            node["TabCaption"].Value = "Settings";

            node["ModuleSettings"]["DeleteEventName"].Value = "SettingDeleted";
            node["ModuleSettings"]["EditEventName"].Value = "SettingEdited";

            // Columns...
            node["ModuleSettings"]["Grid"]["Columns"]["Key"]["Caption"].Value = Language.Instance["SettingsKeyButton", null, "Key"];
            node["ModuleSettings"]["Grid"]["Columns"]["Key"]["ControlType"].Value = "Label";

            node["ModuleSettings"]["Grid"]["Columns"]["Value"]["Caption"].Value = Language.Instance["SettingsValueButton", null, "Value"];
            node["ModuleSettings"]["Grid"]["Columns"]["Value"]["ControlType"].Value = "InPlaceEdit";

            int idxNo = 0;
            foreach (Settings.Setting idx in Settings.Instance)
            {
                node["ModuleSettings"]["Grid"]["Rows"]["Row" + idxNo]["ID"].Value = idx.ID;
                node["ModuleSettings"]["Grid"]["Rows"]["Row" + idxNo]["Key"].Value = idx.Name;
                node["ModuleSettings"]["Grid"]["Rows"]["Row" + idxNo]["Value"].Value = idx.Value;
                idxNo += 1;
            }

            ActiveEvents.Instance.RaiseLoadControl(
                "SettingsModules.ViewSettings",
                "dynMid",
                node);
        }

        [ActiveEvent(Name = "SettingEdited")]
        protected void SettingEdited(object sender, ActiveEventArgs e)
        {
            Settings.Setting setting = ActiveType<Settings.Setting>.SelectByID(int.Parse(e.Params["ID"].Get<string>()));
            Settings.Instance[setting.Name] = e.Params["Value"].Get<string>();
        }

        [ActiveEvent(Name = "SettingDeleted")]
        protected void SettingDeleted(object sender, ActiveEventArgs e)
        {
            Settings.Instance.Remove(int.Parse(e.Params["ID"].Get<string>()));

            Node node = new Node();
            node["Message"].Value =
                Language.Instance["ViwSettingsInfo", null, @"Note that setting might be re-created when accessed by 
another module from the default value from your configuration file"];
            node["Duration"].Value = 10000;
            ActiveEvents.Instance.RaiseActiveEvent(this, "ShowInformationMessage", node);
        }

        [ActiveEvent(Name = "GetTipOfToday")]
        protected void GetTipOfToday(object sender, ActiveEventArgs e)
        {
            const string tmp = @"
Every settings can be edited from the 'Setting' menu item, which normally is within the admin menu hierarchy.

Just remember that all these settings are *global* and will have effect for *all* users.
";
            e.Params["Tip"]["SettingsTipOfToday"].Value = Language.Instance["TipOfTodayFromSettings", null, tmp];
        }

        [ActiveEvent("GetHelpContents")]
        protected static void GetHelpContents(object sender, ActiveEventArgs e)
        {
            e.Params[Language.Instance["SettingsHelpLabel", null, "Settings Module"]].Value = "Help-AboutSettingsModule";
        }

        [ActiveEvent("Help-AboutSettingsModule")]
        protected static void Help_AboutSettingsModule(object sender, ActiveEventArgs e)
        {
            const string helpSettingsDefault = @"
The Settings Module is used generically around the entire Portal. Its main purpose is
to keep track of Global Settings which are affecting all users of the system.

Most Settings will have a default value in the portal configuration file, or through
some other mechanism have a default value the first time it is being used. You can change
any setting value, and you can also delete any Setting value. But when a setting value
is deleted it will often be re-created with its default value whenever it's accessed again
in the module that uses it somehow.

This means that effectively to delete a setting will revert that value back to its default value.
";
            e.Params["Text"].Value = Language.Instance["SettingsHelp", null, helpSettingsDefault];
        }
    }
}