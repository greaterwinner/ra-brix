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
using System.Xml;
using System.Web;
using System.Collections.Generic;

namespace LanguageImportExportController
{
    [ActiveController]
    public class LanguageImportExportController
    {
        [ActiveEvent(Name = "ApplicationStartup")]
        protected static void ApplicationStartup(object sender, ActiveEventArgs e)
        {
            Language.Instance.SetDefaultValue("ButtonLangExport", "Export Language");
            Language.Instance.SetDefaultValue("ButtonLangImport", "Import Language");
            Language.Instance.SetDefaultValue("ButtonLang", "Language");
        }

        [ActiveEvent(Name = "GetMenuItems")]
        protected void GetMenuItems(object sender, ActiveEventArgs e)
        {
            e.Params["ButtonAdmin"]["ButtonLang"].Value = "Menu-LanguageGroup";
            e.Params["ButtonAdmin"]["ButtonLang"]["ButtonLangExport"].Value = "Menu-ExportLanguage";
            e.Params["ButtonAdmin"]["ButtonLang"]["ButtonLangImport"].Value = "Menu-ImportLanguage";
        }

        [ActiveEvent(Name = "Menu-ExportLanguage")]
        protected void ExportLanguageLoadControl(object sender, ActiveEventArgs e)
        {
            Node init = new Node();
            init["TabCaption"].Value = Language.Instance["ExportLanguageCaption", null, "Export Language"];
            init["Width"].Value = 250;
            init["Height"].Value = 130;
            ActiveEvents.Instance.RaiseLoadControl(
                "LanguageImportExportModules.ExportLanguage",
                "dynPopup",
                init);
        }

        [ActiveEvent(Name = "Menu-ImportLanguage")]
        protected void ImportLanguageLoadControl(object sender, ActiveEventArgs e)
        {
            Node init = new Node();
            init["TabCaption"].Value = Language.Instance["ImportLanguageCaption", null, "Import Language"];
            init["Width"].Value = 250;
            init["Height"].Value = 130;
            ActiveEvents.Instance.RaiseLoadControl(
                "LanguageImportExportModules.ImportLanguage",
                "dynPopup",
                init);
        }

        [ActiveEvent(Name = "ImportLanguageFile")]
        protected void ImportLanguageFile(object sender, ActiveEventArgs e)
        {
            string fileContent = e.Params["FileContent"].Get<string>();
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(fileContent);
            string language = doc.FirstChild.Attributes["language"].Value;

            XmlNodeList list = doc.SelectNodes("Ra.Brix.Language/Entity");
            if (list != null)
            {
                foreach (XmlNode idx in list)
                {
                    string key = idx.Attributes["key"].Value;
                    string value = idx.Attributes["value"].Value;
                    Language.Instance.ChangeValue(key, language, value, true);
                }
                Language.Instance.Save();
                
            } ActiveEvents.Instance.RaiseClearControls("dynPopup");
        }

        [ActiveEvent(Name = "ExportLanguage")]
        protected void ExportLanguage(object sender, ActiveEventArgs e)
        {
            XmlDocument doc = new XmlDocument();
            XmlElement root = doc.CreateElement("Ra.Brix.Language");
            XmlAttribute lang = doc.CreateAttribute("language");
            lang.Value = Language.Instance.UserLanguage;
            root.Attributes.Append(lang);
            doc.AppendChild(root);

            List<string> x = new List<string>(Language.Instance.Keys);

            foreach (string idx in x)
            {
                XmlElement el = doc.CreateElement("Entity");
                XmlAttribute value = doc.CreateAttribute("value");
                XmlAttribute key = doc.CreateAttribute("key");
                value.Value = Language.Instance[idx];
                key.Value = idx;

                el.Attributes.Append(key);
                el.Attributes.Append(value);

                root.AppendChild(el);
            }

            using (FileStream fileStream = File.Create(HttpContext.Current.Server.MapPath("~/TemporaryFiles/lang.zzip")))
            {
                doc.Save(fileStream);
            }
            ActiveEvents.Instance.RaiseClearControls("dynPopup");
            HttpContext.Current.Response.Redirect("~/TemporaryFiles/lang.zzip", true);
        }
    }
}
