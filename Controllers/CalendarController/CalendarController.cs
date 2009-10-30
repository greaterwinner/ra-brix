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
            a.Creator = ActiveRecord<User>.SelectFirst(Criteria.Eq("Username", Users.LoggedInUserName));
            a.Header = "Name of activity";
            a.Body = "Body of activity";
            a.Save();
        }
    }
}















