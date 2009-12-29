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
using CandyStoreController.CandyStore;
using SettingsRecords;
using System.IO;
using System.Web;
using System.Collections.Generic;
using System;

namespace CandyStoreController
{
    [ActiveController]
    public class CandyStoreController
    {
        private Service GetCandyStoreWebService()
        {
            Service svc = new Service();
            string webSvc = GetWebSvcURL();
            svc.Url = webSvc;
            return svc;
        }

        private string GetWebSvcURL()
        {
            string webSvc = Settings.Instance["CandyStoreDefaultURL"];
            if (string.IsNullOrEmpty(webSvc))
                webSvc = "http://localhost:8081/CandyStoreWebService/CandyStore.asmx";
            return webSvc;
        }

        [ActiveEvent(Name = "ApplicationStartup")]
        protected static void ApplicationStartup(object sender, ActiveEventArgs e)
        {
            Language.Instance.SetDefaultValue("ButtonCandyStore", "Bazaar");
            Language.Instance.SetDefaultValue("ButtonVisitBazaar", "Visit Bazaar");
            Language.Instance.SetDefaultValue("ButtonInstalledApps", "Manage Apps");
            Language.Instance.SetDefaultValue("ButtonViewInstalledApps", "View Apps");
            Language.Instance.SetDefaultValue("ButtonInstallNewApp", "Install App");
        }

        [ActiveEvent(Name = "GetMenuItems")]
        protected void GetMenuItems(object sender, ActiveEventArgs e)
        {
            e.Params["ButtonCandyStore"].Value = "Menu-OpenCandyStore";
            e.Params["ButtonAdmin"]["ButtonInstalledApps"]["ButtonVisitBazaar"].Value = "Menu-OpenCandyStore";
        }

        [ActiveEvent(Name = "Menu-OpenCandyStore")]
        protected void OpenCandyStore(object sender, ActiveEventArgs e)
        {
            Node node = new Node();
            node["TabCaption"].Value = Language.Instance["CandyStoreHeader", null, "Candy Store"];
            using (Service svc = GetCandyStoreWebService())
            {
                string webSvc = GetWebSvcURL().Replace("CandyStore.asmx", "");
                int idxNo = 0;
                foreach (CandyEntity idx in svc.GetAllModules())
                {
                    node["ModuleSettings"]["Modules"]["Module" + idxNo]["ID"].Value = idx.CandyName;
                    node["ModuleSettings"]["Modules"]["Module" + idxNo]["CandyName"].Value = idx.CandyName;
                    node["ModuleSettings"]["Modules"]["Module" + idxNo]["CandyUrl"].Value = 
                        webSvc.Trim('/') + "/Candies/" + idx.CandyImageUrl;
                    node["ModuleSettings"]["Modules"]["Module" + idxNo]["Description"].Value = "" + idx.Description;
                    node["ModuleSettings"]["Modules"]["Module" + idxNo]["Date"].Value = idx.Updated;
                    CandyEntity tmpIdx = idx;
                    int no = idxNo;
                    if (new List<DirectoryInfo>(new DirectoryInfo(HttpContext.Current.Server.MapPath("~/bin")).GetDirectories()).Exists(
                        delegate(DirectoryInfo idx2)
                        {
                            if (idx2.Name.ToLowerInvariant() == tmpIdx.CandyName.Replace(".zip", "").ToLowerInvariant())
                            {
                                DateTime dateOfInstalledApp = idx2.CreationTime;
                                if (tmpIdx.Updated > dateOfInstalledApp)
                                {
                                    node["ModuleSettings"]["Modules"]["Module" + no]["HasUpdate"].Value = true;
                                }
                                else
                                    node["ModuleSettings"]["Modules"]["Module" + no]["HasUpdate"].Value = false;
                                return true;
                            }
                            return false;
                        }))
                        node["ModuleSettings"]["Modules"]["Module" + idxNo]["Installed"].Value = true;
                    else
                    {
                        node["ModuleSettings"]["Modules"]["Module" + idxNo]["Installed"].Value = false;
                        node["ModuleSettings"]["Modules"]["Module" + idxNo]["HasUpdate"].Value = false;
                    }
                    idxNo += 1;
                }
            }
            ActiveEvents.Instance.RaiseLoadControl(
                "CandyStoreModules.CandyStore",
                "dynMid",
                node);
        }

        [ActiveEvent(Name = "CandyStoreModuleSelectedForInstall")]
        protected void CandyStoreModuleSelectedForInstall(object sender, ActiveEventArgs e)
        {
            string fileName = e.Params["FileName"].Get<string>();
            using (Service svc = GetCandyStoreWebService())
            {
                byte[] fileContent = svc.GetSpecificModule(fileName);
                AppInstaller.InstallApplication(fileName, fileContent);
            }
        }
    }
}












