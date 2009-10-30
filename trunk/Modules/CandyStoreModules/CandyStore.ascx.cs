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
using LanguageRecords;
using Ra.Widgets;
using Ra.Brix.Loader;
using ASP = System.Web.UI.WebControls;
using Ra.Brix.Types;

namespace CandyStoreModules
{
    [ActiveModule]
    public class CandyStore : System.Web.UI.UserControl, IModule
    {
        protected global::System.Web.UI.WebControls.Repeater rep;

        protected void SelectModule(object sender, EventArgs e)
        {
            string fileName = ((Panel)sender).Xtra;

            Node node = new Node();
            node["FileName"].Value = fileName;

            ActiveEvents.Instance.RaiseActiveEvent(
                this,
                "CandyStoreModuleSelectedForInstall",
                node);
        }

        [ActiveEvent(Name = "GetTipOfToday")]
        private static void GetTipOfToday(object sender, ActiveEventArgs e)
        {
            string tmp = @"
The Candy Store makes it possible for you to integrate towards either your own module repository or 
a 3rd party repository where you can download and install new modules, updates and such.

These modules will be downloadable to your portal and easily integrate towards your existing code.

Whenever you install new modules, you will have to ""reboot"" your portal, which might take some few
minutes. You might also - dependent upon your portal installation - be forced to login again.
";
            e.Params["Tip"]["TipOfCandyStore"].Value = Language.Instance["TipOfCandyStore", null, tmp];
        }

        public void InitialLoading(Node node)
        {
            Load +=
                delegate
                {
                    rep.DataSource = node["Modules"];
                    rep.DataBind();
                };
        }

        public string GetCaption()
        {
            return "";
        }
    }
}