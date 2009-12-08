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
using System;

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

        [ActiveEvent(Name = "InstalledApps-ViewDetailsOfApp")]
        protected void ViewDetailsOfApp(object sender, ActiveEventArgs e)
        {
            Node node = new Node();
            string appName = e.Params["AppName"].Get<string>();
            DirectoryInfo dir = new DirectoryInfo(HttpContext.Current.Server.MapPath("~/bin/" + appName));
            DateTime created = dir.CreationTime;
            int idxNo = 0;
            foreach (FileInfo idxFile in dir.GetFiles())
            {
                node["ModuleSettings"]["Files"]["File" + idxNo].Value = idxFile.Name;
                node["ModuleSettings"]["Files"]["File" + idxNo]["FullPath"].Value = idxFile.FullName;
                node["ModuleSettings"]["Files"]["File" + idxNo]["Created"].Value = idxFile.LastWriteTime;
                node["ModuleSettings"]["Files"]["File" + idxNo]["Size"].Value = idxFile.Length;
                idxNo += 1;
            }

            node["ModuleSettings"]["AppName"].Value = appName;
            node["ModuleSettings"]["Installed"].Value = created;
            ActiveEvents.Instance.RaiseLoadControl(
                "InstalledAppsModules.ViewAppDetails",
                "dynMid",
                node);
        }

        [ActiveEvent(Name = "ViewDetailsOfApplicationFile")]
        protected void ViewDetailsOfApplicationFile(object sender, ActiveEventArgs e)
        {
            string fileFullPath = e.Params["FileFullPath"].Get<string>();
            if (fileFullPath.IndexOf(".dll") != -1)
            {
                ReflectionHelper helper = new ReflectionHelper(fileFullPath);
                Node node = helper.CreateNodeStructure();
                Node input = new Node();
                input["ModuleSettings"].Add(node[0]);
                input["ModuleSettings"]["FileFullPath"].Value = fileFullPath;
                ActiveEvents.Instance.RaiseLoadControl(
                    "InstalledAppsModules.ViewDetailsOfFile",
                    "dynMin",
                    input);
            }
            else
            {
                Node node = new Node();
                node["Message"].Value =
                    Language.Instance["CannotViewNonDll", null, @"
You cannot view the details of a non-DLL file..."];
                node["Duration"].Value = 5000;
                ActiveEvents.Instance.RaiseActiveEvent(
                    this,
                    "ShowInformationMessage",
                    node);
            }
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
