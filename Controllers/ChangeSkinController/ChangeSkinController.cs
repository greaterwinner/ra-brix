/*
 * Ra-Brix - A Modular-based Framework for building 
 * Web Applications Copyright 2009 - Thomas Hansen 
 * thomas@ra-ajax.org. Unless permission is 
 * explicitly given this code is licensed under the 
 * GNU Affero GPL version 3 which can be found in the 
 * license.txt file on disc.
 * 
 */

using System.Collections.Generic;
using HelperGlobals;
using LanguageRecords;
using Ra.Brix.Loader;
using Ra.Brix.Types;
using Ra;
using System.IO;
using ICSharpCode.SharpZipLib.Zip;
using System.Web;
using SettingsRecords;
using UserSettingsRecords;

namespace ChangeSkinController
{
    [ActiveController]
    public class ChangeSkinController
    {
        [ActiveEvent(Name = "ApplicationStartup")]
        protected static void ApplicationStartup2(object sender, ActiveEventArgs e)
        {
            Language.Instance.SetDefaultValue("ButtonSkins", "Skins");
            Language.Instance.SetDefaultValue("ButtonChangeSkin", "Change Skin");
            Language.Instance.SetDefaultValue("ButtonDefaultSkin", "Set Default Skin");
            Language.Instance.SetDefaultValue("ButtonUploadNewSkin", "Upload New Skin");
        }

        [ActiveEvent(Name = "GetMenuItems")]
        protected void GetMenuItems(object sender, ActiveEventArgs e)
        {
            e.Params["ButtonAdmin"]["ButtonSkins"].Value = "Menu-SkinRootFolder";
            e.Params["ButtonAdmin"]["ButtonSkins"]["ButtonChangeSkin"].Value = "Menu-ChangeSkin";
            e.Params["ButtonAdmin"]["ButtonSkins"]["ButtonDefaultSkin"].Value = "Menu-DefaultSkin";
            e.Params["ButtonAdmin"]["ButtonSkins"]["ButtonUploadNewSkin"].Value = "Menu-UploadNewSkin";
        }

        [ActiveEvent(Name = "Menu-UploadNewSkin")]
        protected void UploadNewSkin(object sender, ActiveEventArgs e)
        {
            Node node = new Node();
            node["TabCaption"].Value = 
                Language.Instance["ChooseSkinZipFileToUpload", null, "Choose a new Skin-ZIP file to upload"];
            ActiveEvents.Instance.RaiseLoadControl(
                "ChangeSkinModules.UploadNewSkin",
                "dynPopup",
                node);
        }

        [ActiveEvent(Name = "Menu-ChangeSkin")]
        protected void ChangeSkin(object sender, ActiveEventArgs e)
        {
            Node node = new Node();
            node["TabCaption"].Value = Language.Instance["ChangeSkin", null, "Change skin"];
            node["ModuleSettings"]["EventToSend"].Value = "ChangeCssRootFolder";
            ActiveEvents.Instance.RaiseLoadControl(
                "ChangeSkinModules.ChangeSkin",
                "dynMid",
                node);
        }

        [ActiveEvent(Name = "Menu-DefaultSkin")]
        protected void ChangeDefaultSkin(object sender, ActiveEventArgs e)
        {
            Node node = new Node();
            node["TabCaption"].Value = Language.Instance["ChangeSkin", null, "Change skin"];
            node["ModuleSettings"]["EventToSend"].Value = "ChangeDefaultCssRootFolder";
            ActiveEvents.Instance.RaiseLoadControl(
                "ChangeSkinModules.ChangeSkin",
                "dynMid",
                node);
        }

        [ActiveEvent(Name = "ImportNewSkin")]
        protected void ImportNewSkin(object sender, ActiveEventArgs e)
        {
            byte[] fileContent = e.Params["FileContent"].Get<byte[]>();
            using (MemoryStream memStream = new MemoryStream(fileContent))
            {
                memStream.Position = 0;
                using (ZipInputStream zipInput = new ZipInputStream(memStream))
                {
                    ZipEntry current = zipInput.GetNextEntry();
                    while (current != null)
                    {
                        string basePath = HttpContext.Current.Server.MapPath("~/media/skins/");
                        List<string> dirEntities = new List<string>(current.Name.Split('/'));
                        dirEntities.RemoveAt(dirEntities.Count - 1);
                        string idxBasePath = basePath;
                        foreach (string idxEntities in dirEntities)
                        {
                            idxBasePath += idxEntities + "\\";
                            if (!Directory.Exists(idxBasePath))
                            {
                                Directory.CreateDirectory(idxBasePath);
                            }
                        }
                        if (current.Name[current.Name.Length - 1] != '/')
                        {
                            using (FileStream output = new FileStream(
                                basePath + current.Name,
                                FileMode.Create,
                                FileAccess.Write))
                            {
                                byte[] buffer = new byte[current.Size];
                                zipInput.Read(buffer, 0, buffer.Length);
                                output.Write(buffer, 0, buffer.Length);
                            }
                        }
                        current = zipInput.GetNextEntry();
                    }
                }
            }
            ActiveEvents.Instance.RaiseClearControls("dynPopup");
            ActiveEvents.Instance.RaiseLoadControl(
                "ChangeSkinModules.ChangeSkin",
                "dynMid");
        }

        [ActiveEvent(Name = "ChangeCssRootFolder")]
        protected void ChangeCssRootFolder(object sender, ActiveEventArgs e)
        {
            UserSettings.Instance["CssRootFolder", Users.LoggedInUserName] =
                e.Params["RootFolder"].Get<string>();
            Language.Instance.SetDefaultValue("SkinChanged", @"
You changed the skin of the portal. This change will only affect your user.");
            AjaxManager.Instance.Redirect("~/?message=SkinChanged");
        }

        [ActiveEvent(Name = "ChangeDefaultCssRootFolder")]
        protected void ChangeDefaultCssRootFolder(object sender, ActiveEventArgs e)
        {
            Settings.Instance["CssRootFolder"] = e.Params["RootFolder"].Get<string>();
            Language.Instance.SetDefaultValue("SkinChangedGlobally", @"
The default skin of the portal was changed, this will reflect upon every user that 
has not explicitly chosen to override the skin himself");
            AjaxManager.Instance.Redirect("~/?message=SkinChangedGlobally");
        }
    }
}
