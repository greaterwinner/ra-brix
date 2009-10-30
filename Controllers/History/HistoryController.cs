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
using HistoryRecords;
using LanguageRecords;
using Ra.Brix.Loader;
using Ra.Brix.Data;
using System.Collections.Generic;
using Ra.Brix.Types;
using SettingsRecords;

namespace HistoryController
{
    [ActiveController]
    public class HistoryController
    {
        [ActiveEvent(Name = "ApplicationStartup")]
        protected static void ApplicationStartup(object sender, ActiveEventArgs e)
        {
            Language.Instance.SetDefaultValue("ButtonHistory", "History");
        }

        [ActiveEvent(Name = "GetMenuItems")]
        protected void GetMenuItems(object sender, ActiveEventArgs e)
        {
            e.Params["ButtonAppl"]["ButtonHistory"].Value = "Menu-ViewHistory";
        }

        [ActiveEvent(Name = "Menu-ViewHistory")]
        protected void ViewHistory(object sender, ActiveEventArgs e)
        {
            Node node = new Node();

            node["Message"].Value = Language.Instance["HistoryViewInfo", null, @"To view history: 
Move the cursor over the bull's eye on the right side of this information message."];
            node["Duration"].Value = 5000;
            ActiveEvents.Instance.RaiseActiveEvent(
                this,
                "ShowInformationMessage",
                node);

            node["TabCaption"].Value = Language.Instance["HistoryCaption", null, "View History"];

            List<HistoryData> list = new List<HistoryData>(
                ActiveRecord<HistoryData>.Select(
                    Criteria.Eq("Username", Users.LoggedInUserName)));
            list.Sort(
                delegate(HistoryData left, HistoryData right)
                    {
                        return right.WhenRaised.CompareTo(left.WhenRaised);
                    });

            node["ModuleSettings"]["Grid"]["Columns"]["MenuItem"]["Caption"].Value = Language.Instance["ViewHistoryMenuItemCaption", null, "Menu Item"];
            node["ModuleSettings"]["Grid"]["Columns"]["MenuItem"]["ControlType"].Value = "LinkButton";
            node["ModuleSettings"]["Grid"]["Columns"]["When"]["Caption"].Value = Language.Instance["ViewHistoryWhen", null, "When"];
            node["ModuleSettings"]["Grid"]["Columns"]["When"]["ControlType"].Value = "Label";

            
            int idxNo = 0;
            foreach (HistoryData idx in list)
            {
                if (idx.MenuEventName != "Menu-ViewHistory")
                {
                    node["ModuleSettings"]["Grid"]["Rows"]["Row" + idxNo]["ID"].Value = idx.ID;
                    node["ModuleSettings"]["Grid"]["Rows"]["Row" + idxNo]["MenuItem"].Value = idx.MenuText;
                    node["ModuleSettings"]["Grid"]["Rows"]["Row" + idxNo]["When"].Value = idx.WhenRaised;
                    node["ModuleSettings"]["Grid"]["Rows"]["Row" + idxNo]["When"]["Format"].Value = "d MMM yy";
                    idxNo += 1;
                }
            }
            node["Width"].Value = 500;
            node["Height"].Value = 380;

            ActiveEvents.Instance.RaiseLoadControl(
                "HistoryModules.History",
                "dynPopup2",
                node);
        }

        [ActiveEvent(Name = "OpenHistoryItem")]
        protected void OpenHistoryItem(object sender, ActiveEventArgs e)
        {
            int id = int.Parse(e.Params["ID"].Get<string>());
            HistoryData h = ActiveRecord<HistoryData>.SelectByID(id);
            Node node = new Node();
            node["Params"].Value = h.Params;
            ActiveEvents.Instance.RaiseActiveEvent(
                this,
                h.MenuEventName,
                node);
        }

        [ActiveEvent(Name = "MenuItemClicked")]
        protected void MenuItemClicked(object sender, ActiveEventArgs e)
        {
            if (string.IsNullOrEmpty(Users.LoggedInUserName))
            {
                return;
            }
            string menuEventToRaise = e.Params["MenuEventName"].Get<string>();
            string menuText = e.Params["MenuText"].Get<string>();
            string pars = e.Params["Params"].Get<string>();

            // Checking to see if we need to delete older items...
            // This is done since we only store the x last items, where x == a setting which
            // defaults to 5...
            int numberOfExistingItems = ActiveRecord<HistoryData>.CountWhere(
                Criteria.Eq("Username", Users.LoggedInUserName));
            int maxItems = Settings.Instance.Get("NumberOfHistoryItemsPerUser", 50);
            if (numberOfExistingItems >= maxItems)
            {
                int itemsToDelete = (numberOfExistingItems - maxItems) + 1;
                List<HistoryData> items = 
                    new List<HistoryData>(
                        ActiveRecord<HistoryData>.Select(
                            Criteria.Eq("Username", Users.LoggedInUserName)));
                items.Sort(
                    delegate(HistoryData left, HistoryData right)
                        {
                            return left.WhenRaised.CompareTo(right.WhenRaised);
                        });
                for (int idx = 0; idx < itemsToDelete; idx++)
                {
                    items[idx].Delete();
                }
            }

            // Now we can create a new History Item and insert...
            HistoryData h = new HistoryData
            {
                Username = Users.LoggedInUserName,
                MenuEventName = menuEventToRaise,
                MenuText = menuText,
                Params = pars
            };
            h.Save();


            // Sending the "HistoryUpdated" event...
            Node node = new Node();
            List<HistoryData> list = new List<HistoryData>(
                ActiveRecord<HistoryData>.Select(
                    Criteria.Eq("Username", Users.LoggedInUserName)));
            list.Sort(
                delegate(HistoryData left, HistoryData right)
                    {
                        return right.WhenRaised.CompareTo(left.WhenRaised);
                    });
            node["Grid"]["Columns"]["MenuItem"]["Caption"].Value = Language.Instance["HistoryMenuItemCaption", null, "Menu Item"];
            node["Grid"]["Columns"]["MenuItem"]["ControlType"].Value = "LinkButton";
            node["Grid"]["Columns"]["When"]["Caption"].Value = Language.Instance["HistoryWhenCaption", null, "When"];
            node["Grid"]["Columns"]["When"]["ControlType"].Value = "Label";


            int idxNo = 0;
            foreach (HistoryData idx in list)
            {
                if (idx.MenuEventName == "Menu-ViewHistory")
                    continue;
                node["Grid"]["Rows"]["Row" + idxNo]["ID"].Value = idx.ID;
                node["Grid"]["Rows"]["Row" + idxNo]["MenuItem"].Value = idx.MenuText;
                node["Grid"]["Rows"]["Row" + idxNo]["When"].Value = idx.WhenRaised;
                node["Grid"]["Rows"]["Row" + idxNo]["When"]["Format"].Value = "d MMM yy";
                idxNo += 1;
            }
            ActiveEvents.Instance.RaiseActiveEvent(
                this,
                "HistoryWasUpdated",
                node);
        }
    }
}