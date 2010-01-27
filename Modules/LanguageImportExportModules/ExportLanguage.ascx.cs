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

namespace LanguageImportExportModules
{
    [ActiveModule]
    public class ExportLanguage : System.Web.UI.UserControl, IModule
    {
        protected global::Ra.Widgets.TextBox ExportTxt;
        protected global::Ra.Widgets.Panel pnlWrp;

        protected void Page_Load(object sender, EventArgs e)
        {
            pnlWrp.DataBind();
        }

        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            ActiveEvents.Instance.RaiseActiveEvent(
                this, 
                "ExportLanguage");
        }

        [ActiveEvent(Name = "GetTipOfToday")]
        protected static void GetTipOfToday(object sender, ActiveEventArgs e)
        {
            const string tmp = @"
You can export languages once you have translated the portal, these languages can then
be imported into either other installations or kept as backups for the future.
";
            e.Params["Tip"]["TipOfExportLanguage"].Value = Language.Instance["TipOfExportLanguage", null, tmp];
        }

        public void InitialLoading(Node init)
        {
        }
    }
}