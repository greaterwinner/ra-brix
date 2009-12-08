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
using System;
using System.Globalization;
using Ra.Widgets;
using Ra.Effects;
using Ra.Selector;

namespace InstalledAppsModules
{
    [ActiveModule]
    public class ViewAppDetails : System.Web.UI.UserControl, IModule
    {
        protected global::System.Web.UI.HtmlControls.HtmlGenericControl header;
        protected global::System.Web.UI.HtmlControls.HtmlGenericControl date;
        protected global::System.Web.UI.WebControls.Repeater rep;

        protected string GetCssClassForFile(object fileNameObj)
        {
            string fileName = fileNameObj as string;
            fileName = fileName.Substring(fileName.LastIndexOf(".") + 1);
            return "file-class file-class-" + fileName;
        }

        protected string GetCutFileName(object fileNameObj)
        {
            string fileName = fileNameObj as string;
            string retVal = "";
            for (int idx = 0; idx < fileName.Length; idx++)
            {
                if (idx % 15 == 0 && idx > 0)
                    retVal += "<br/>";
                retVal += fileName[idx];
            }
            return retVal;
        }

        private string ExpandedFile
        {
            get { return ViewState["ExpandedFile"] as string; }
            set { ViewState["ExpandedFile"] = value; }
        }

        protected void MouseClick(object sender, EventArgs e)
        {
            Panel pnl = (Panel)sender;
            if (pnl.Xtra == ExpandedFile)
            {
                ExpandedFile = null;
                new EffectSize(pnl, 500, 110, 130)
                    .Render();
            }
            else
            {
                if(ExpandedFile != null)
                {
                    new EffectSize(Selector.SelectFirst<Panel>(this,
                        delegate(System.Web.UI.Control idx)
                            {
                                if(idx is RaWebControl)
                                    return ((RaWebControl) idx).Xtra == ExpandedFile;
                                return false;
                            }), 500, 110, 130)
                        .Render();
                }
                new EffectSize(pnl, 500, 240, 260)
                    .Render();
                ExpandedFile = pnl.Xtra;
            }
        }

        public void InitialLoading(Node node)
        {
            DateTime dateInstalled = node["Installed"].Get<DateTime>();
            header.InnerHtml = node["AppName"].Get<string>();
            date.InnerHtml =
                Language.Instance["ApplicationWasInstalled", null, "Application was installed: "] +
                dateInstalled.ToString("dddd d. MMMM yyyy", CultureInfo.InvariantCulture) +
                Language.Instance["AndHasBeenRunningFor", null, " and has been running for: "] + 
                (DateTime.Now - dateInstalled).TotalDays.ToString("0", CultureInfo.InvariantCulture) + 
                Language.Instance["Days", null, " day(s)"];
            rep.DataSource = node["Files"];
            rep.DataBind();
        }

        public string GetCaption()
        {
            return "";
        }
    }
}

