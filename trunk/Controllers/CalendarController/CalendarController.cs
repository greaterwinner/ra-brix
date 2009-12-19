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
using CalendarRecords;
using HelperGlobals;
using LanguageRecords;
using Ra.Brix.Loader;
using Ra.Brix.Types;
using Ra.Brix.Data;
using UserRecords;

namespace CalendarController
{
    [ActiveController]
    public class CalendarController
    {
        [ActiveEvent(Name = "ApplicationStartup")]
        protected static void ApplicationStartup2(object sender, ActiveEventArgs e)
        {
            Language.Instance.SetDefaultValue("ButtonViewCalendar", "View Calendar");
        }

        [ActiveEvent("GetHelpContents")]
        protected static void GetHelpContents(object sender, ActiveEventArgs e)
        {
            e.Params[Language.Instance["CalendarModule", null, "Calendar Module"]].Value = "Help-AboutCalendarModule";
        }

        [ActiveEvent("Help-AboutCalendarModule")]
        protected static void Help_AboutSettingsModule(object sender, ActiveEventArgs e)
        {
            const string helpSettingsDefault = @"
The Calendar Module makes it possible for you to create activities that you attach to specific dates.
You can create one time occuring activities, and repeteive activities, such that repeats once per month, 
once per week or once per year.

NOTICE! - If you create repetetive tasks [or edit existing ones] the repetition will be from the date you're editing the
activity from. This means for instance that if you edit some item that repeats every week, and you're editing it
6 months from now, then all items will be deleted and the repetition will start form 6 months from now.
";
            e.Params["Text"].Value = Language.Instance["SettingsHelp", null, helpSettingsDefault];
        }

        [ActiveEvent(Name = "GetTipOfToday")]
        protected void GetTipOfToday(object sender, ActiveEventArgs e)
        {
            const string tmp = @"
If you edit a Calendar Activity that repeats, for instance every week. Then all items in that
calendar activity before the date you're editing it from will be deleted and a new repetition starting
from the date you're editing the activity from will be created.

This means that you in general should *ONLY* edit calendar items starting from the next date coming up
from the current date...
";
            e.Params["Tip"]["SettingsRepetitionCalendarActivities"].Value = Language.Instance["SettingsRepetitionCalendarActivities", null, tmp];
        }

        [ActiveEvent(Name = "GetMenuItems")]
        protected void GetMenuItems(object sender, ActiveEventArgs e)
        {
            e.Params["ButtonAppl"]["ButtonViewCalendar"].Value = "Menu-ViewCalendar";
        }

        [ActiveEvent(Name = "Menu-ViewCalendar")]
        protected void ViewCalendar(object sender, ActiveEventArgs e)
        {
            Node node = new Node();
            node["TabCaption"].Value = Language.Instance["CandyStoreHeader", null, "Candy Store"];
            ActiveEvents.Instance.RaiseLoadControl(
                "CalendarModules.ViewCalendar",
                "dynMid",
                node);
        }

        [ActiveEvent(Name = "UserRequestsToCreateActivity")]
        protected void UserRequestsToCreateActivity(object sender, ActiveEventArgs e)
        {
            DateTime dt = e.Params["Date"].Get<DateTime>();
            Activity a = new Activity();
            a.Start = dt;
            a.End = dt.AddHours(1);
            a.Creator = ActiveType<User>.SelectFirst(Criteria.Eq("Username", Users.LoggedInUserName));
            a.Header = "Name of activity";
            a.Body = "Body of activity";
            a.Save();
            e.Params["IDOfSaved"].Value = a.ID;
        }
    }
}















