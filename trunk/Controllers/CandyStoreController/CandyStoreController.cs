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
using Ra.Brix.Loader;
using Ra.Brix.Types;
using System.IO;
using ICSharpCode.SharpZipLib.Zip;
using System.Web;
using Ra;
using CandyStoreController.CandyStore;

namespace CandyStoreController
{
    [ActiveController]
    public class CandyStoreController
    {
        [ActiveEvent(Name = "ApplicationStartup")]
        protected static void ApplicationStartup2(object sender, ActiveEventArgs e)
        {
            Language.Instance.SetDefaultValue("ButtonCandyStore", "Candy Store");
        }

        [ActiveEvent(Name = "GetMenuItems")]
        protected void GetMenuItems(object sender, ActiveEventArgs e)
        {
            e.Params["ButtonAppl"]["ButtonCandyStore"].Value = "Menu-OpenCandyStore";
        }

        [ActiveEvent(Name = "Menu-OpenCandyStore")]
        protected void OpenCandyStore(object sender, ActiveEventArgs e)
        {
            Node node = new Node();
            node["TabCaption"].Value = Language.Instance["CandyStoreHeader", null, "Candy Store"];
            using (Service svc = new Service())
            {
                int idxNo = 0;
                foreach (CandyEntity idx in svc.GetAllModules())
                {
                    node["ModuleSettings"]["Modules"]["Module" + idxNo]["ID"].Value = idx.CandyName;
                    node["ModuleSettings"]["Modules"]["Module" + idxNo]["CandyName"].Value = idx.CandyName;
                    node["ModuleSettings"]["Modules"]["Module" + idxNo]["CandyUrl"].Value = idx.CandyImageUrl;
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
            using (Service svc = new Service())
            {
                byte[] fileContent = svc.GetSpecificModule(fileName);
                using (MemoryStream memStream = new MemoryStream(fileContent))
                {
                    memStream.Position = 0;
                    using (ZipInputStream zipInput = new ZipInputStream(memStream))
                    {
                        ZipEntry current = zipInput.GetNextEntry();
                        while (current != null)
                        {
                            using (FileStream output = new FileStream(
                                HttpContext.Current.Server.MapPath("~/bin/") + current.Name,
                                FileMode.Create,
                                FileAccess.Write))
                            {
                                byte[] buffer = new byte[current.Size];
                                zipInput.Read(buffer, 0, buffer.Length);
                                output.Write(buffer, 0, buffer.Length);
                                current = zipInput.GetNextEntry();
                            }
                        }
                    }
                }
            }
            AjaxManager.Instance.Redirect("~/?message=PluginInstalled");
        }
    }
}












