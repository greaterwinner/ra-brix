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
using Ra.Effects;
using Ra.Selector;
using System.Web.UI;

namespace CandyStoreModules
{
    [ActiveModule]
    public class CandyStore : System.Web.UI.UserControl, IModule
    {
        protected global::System.Web.UI.WebControls.Repeater rep;
        protected global::Ra.Widgets.Panel repWrp;
        protected global::Ra.Widgets.TextBox filter;

        private string OldMaximized
        {
            get { return ViewState["OldMaximized"] as string; }
            set { ViewState["OldMaximized"] = value; }
        }

        protected void SelectModule(object sender, EventArgs e)
        {
            Panel pnl = (Panel) sender;
            if(pnl.Style[Styles.height] == "400px")
            {
                new EffectSize(pnl, 500, 110, 150)
                    .Render();
                OldMaximized = "";
            }
            else
            {
                if (!string.IsNullOrEmpty(OldMaximized))
                {
                    new EffectSize(Selector.SelectFirst<Panel>(
                        this, 
                        delegate(Control idx)
                            {
                                RaWebControl ctrl = idx as RaWebControl;
                                if(ctrl == null)
                                    return false;
                                return ctrl.Xtra == OldMaximized;
                            }), 500, 110, 150)
                        .ChainThese(new EffectSize(pnl, 500, 400, 400))
                        .Render();
                }
                else
                {
                    new EffectSize(pnl, 500, 400, 400)
                        .Render();
                }
                OldMaximized = pnl.Xtra;
            }
        }

        private void InstallModule(Panel pnl)
        {
            if (pnl.CssClass.Contains("already-installed"))
            {
                Node nodeMessage = new Node();
                nodeMessage["Message"].Value = Language.Instance["", null, @"
Application is already installed, if you want to update it you need to uninstall it first"];
                nodeMessage["Duration"].Value = 2000;
                ActiveEvents.Instance.RaiseActiveEvent(
                    this,
                    "ShowInformationMessage",
                    nodeMessage);
            }
            else
            {
                string fileName = pnl.Xtra;
                Node node = new Node();
                node["FileName"].Value = fileName;
                ActiveEvents.Instance.RaiseActiveEvent(
                    this,
                    "CandyStoreModuleSelectedForInstall",
                    node);
            }
        }

        [ActiveEvent(Name = "GetTipOfToday")]
        protected static void GetTipOfToday(object sender, ActiveEventArgs e)
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

        private Node Modules
        {
            get { return ViewState["Modules"] as Node; }
            set { ViewState["Modules"] = value; }
        }

        public void InitialLoading(Node node)
        {
            Load +=
                delegate
                {
                    filter.Text = Language.Instance["Popular", null, "Popular"];
                    Modules = node["Modules"];
                    rep.DataSource = FilterApplications();
                    rep.DataBind();
                };
            filter.Focus();
            filter.Select();
        }

        private Node FilterApplications()
        {
            Node retVal = new Node();
            int idxNo = 0;
            foreach (Node idxNode in Modules)
            {
                ++idxNo;
                if (filter.Text.ToLowerInvariant() == 
                    Language.Instance["Popular", null, "Popular"].ToLowerInvariant())
                {
                    if (idxNo >= 10)
                        break;
                    retVal.Add(idxNode);
                }
                else
                {
                    bool hasValue = false;
                    if (string.IsNullOrEmpty(filter.Text))
                        hasValue = true;
                    else
                    {
                        if (
                            idxNode["CandyName"].Get<string>().ToLowerInvariant().Contains(
                                filter.Text.ToLowerInvariant()))
                            hasValue = true;
                        if (
                            idxNode["Description"].Get<string>().ToLowerInvariant().Contains(
                                filter.Text.ToLowerInvariant()))
                            hasValue = true;
                    }
                    if (hasValue)
                        retVal.Add(idxNode);
                }
            }
            return retVal;
        }

        protected void doFilter_Click(object sender, EventArgs e)
        {
            rep.DataSource = FilterApplications();
            rep.DataBind();
            repWrp.ReRender();
            filter.Select();
            filter.Focus();
        }

        public string GetCaption()
        {
            return "";
        }

        protected string GetCssClassAccordingToIsInstalled(object isInstalledObj)
        {
            bool isInstalled = (bool) isInstalledObj;
            return isInstalled ? "already-installed" : "not-installed";
        }

        protected string GetToolTip(object isInstalledObj)
        {
            bool isInstalled = (bool) isInstalledObj;
            return isInstalled
                       ?
                           Language.Instance["AppAlreadyInstalled", null, "Application is already installed"]
                       :
                           Language.Instance["ClickToInstallApplications", null, "Click to install application"];
        }
    }
}