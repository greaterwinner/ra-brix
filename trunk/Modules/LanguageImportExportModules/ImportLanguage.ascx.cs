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
using Ra.Brix.Loader;
using ASP = System.Web.UI.WebControls;
using Ra.Brix.Types;
using System.IO;

namespace LanguageImportExportModules
{
    [ActiveModule]
    public class ImportLanguage : System.Web.UI.UserControl, IModule
    {
        protected global::Components.ExtButtonPost btn;
        protected global::System.Web.UI.WebControls.FileUpload upload;

        protected void Page_Load(object sender, EventArgs e)
        {
            btn.DataBind();
        }

        protected void btn_Click(object sender, EventArgs e)
        { 
            using(TextReader reader = new StreamReader(upload.FileContent))
            {
                string fileContent = reader.ReadToEnd();
                Node node = new Node();
                node["FileContent"].Value = fileContent;

                ActiveEvents.Instance.RaiseActiveEvent(
                    this,
                    "ImportLanguageFile",
                    node);
            }
        }

        [ActiveEvent(Name = "GetTipOfToday")]
        protected static void GetTipOfToday(object sender, ActiveEventArgs e)
        {
            const string tmp = @"
You can import new languages in the portal.

Ofte you can also either find or purchase new languages online for your portal installation.
You might want to ask your portal vendor if he have languages that you need in his repository.
";
            e.Params["Tip"]["TipOfImportLanguage"].Value = Language.Instance["TipOfImportLanguage", null, tmp];
        }

        public void InitialLoading(Node init)
        {
        }
    }
}