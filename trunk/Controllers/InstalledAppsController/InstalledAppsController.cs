/*
 * Ra-Brix - A Modular-based Framework for building 
 * Web Applications Copyright 2009 - Thomas Hansen 
 * thomas@ra-ajax.org. Unless permission is 
 * explicitly given this code is licensed under the 
 * GNU Affero GPL version 3 which can be found in the 
 * license.txt file on disc.
 * 
 */

using System.Web;
using LanguageRecords;
using Ra.Brix.Loader;
using Ra.Brix.Types;
using System.IO;

namespace InstalledAppsController
{
    [ActiveController]
    public class InstalledAppsController
    {
        [ActiveEvent(Name = "ApplicationStartup")]
        protected static void ApplicationStartup(object sender, ActiveEventArgs e)
        {
            Language.Instance.SetDefaultValue("ButtonInstalledApps", "Installed Apps");
            Language.Instance.SetDefaultValue("ButtonViewInstalledApps", "Edit Apps");
        }

        [ActiveEvent(Name = "GetMenuItems")]
        protected void GetMenuItems(object sender, ActiveEventArgs e)
        {
            e.Params["ButtonAdmin"]["ButtonInstalledApps"].Value = "Menu-InstalledApps";
            e.Params["ButtonAdmin"]["ButtonInstalledApps"]["ButtonViewInstalledApps"].Value = "Menu-ViewInstalledApps";
        }

        [ActiveEvent(Name = "Menu-ViewInstalledApps")]
        protected void ViewInstalledApps(object sender, ActiveEventArgs e)
        {
            Node node = new Node();
            node["ModuleSettings"]["Grid"]["Columns"]["Name"]["Caption"].Value = Language.Instance["Name", null, "Name"];
            node["ModuleSettings"]["Grid"]["Columns"]["Name"]["ControlType"].Value = "LinkButton";
            int idxNo = 0;
            foreach (DirectoryInfo idxDir in new DirectoryInfo(HttpContext.Current.Server.MapPath("~/bin")).GetDirectories())
            {
                string dirName = idxDir.Name;
                if (dirName == ".svn")
                    continue;
                node["ModuleSettings"]["Grid"]["Rows"]["Row" + idxNo]["ID"].Value = dirName;
                node["ModuleSettings"]["Grid"]["Rows"]["Row" + idxNo]["Name"].Value = dirName;
                idxNo += 1;
            }
            ActiveEvents.Instance.RaiseLoadControl(
                "InstalledAppsModules.ViewAllApps",
                "dynMid",
                node);
        }
    }
}
