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
using System.Configuration;
using HelperGlobals;
using System.Web.UI;
using Ra.Brix.Loader;
using SettingsRecords;
using UserSettingsRecords;
using Ra.Brix.Data;
using System.Text;
using System.IO;

namespace Ra.Brix.Portal
{
    public partial class MainWebPage : Page
    {
        protected void Page_Init(object sender, EventArgs e)
        {
            RewriteForm();

            // Setting the base for the page to get correct path to images and such
            baseElement.Attributes.Add("href", ApplicationRoot.Root);

            // ORDER COUNTS HERE....!
            // Since all DLL's are loaded into the AppDomain by the plugin loader
            // we are unfortunately stuck in a logic where order counts (for now)
            // TODO: Fix cohesion here ...!
            InitializeViewport();
            ActiveEvents.Instance.RaiseActiveEvent(
                this,
                "Page_Init");
            if (!IsPostBack)
            {
                ActiveEvents.Instance.RaiseActiveEvent(
                    this, 
                    "Page_Init_InitialLoading");
            }
            LoadComplete += MainWebPage_LoadComplete;
        }

        private void RewriteForm()
        {
            if (Request.Url.Port > 500)
            {
                // Assuming WebDev, rewriting internally just to make development easier...
                Form.Action = Request.Url.ToString();
            }
            else
            {
                // Changing the URL to un-escaped to get rid of the ContentID parameter and
                // make the URL for the form "beautiful"...
                Form.Action = Request.RawUrl.Replace("default.aspx", "").Replace("Default.aspx", "");
            }
        }

        void MainWebPage_LoadComplete(object sender, EventArgs e)
        {
            ActiveEvents.Instance.RaiseActiveEvent(
                this,
                "LoadComplete");
        }

        protected override void OnPreRenderComplete(EventArgs e)
        {
            css1.DataBind();
            css3.DataBind();
            base.OnPreRenderComplete(e);
        }

        protected string GetCssRootFolder()
        {
            string portalDefault = Settings.Instance.Get(
                "CssRootFolder",
                "Gold");
            string retVal = ApplicationRoot.Root + "media/skins/";
            if (string.IsNullOrEmpty(Users.LoggedInUserName))
            {
                retVal += portalDefault;
            }
            else
            {
                retVal += UserSettings.Instance.Get(
                    "CssRootFolder",
                    Users.LoggedInUserName,
                    portalDefault);
            }
            return retVal;
        }
        
        protected string GetRedirectUrl()
        {
            return ApplicationRoot.Root;
        }

        private void InitializeViewport()
        {
            string defaultControl = ConfigurationManager.AppSettings["PortalViewDriver"];
            Control ctrl = PluginLoader.Instance.LoadControl(defaultControl);
            Form.Controls.Add(ctrl);
        }

        private PageStatePersister _pageStatePersister;
        protected override PageStatePersister PageStatePersister
        {
            get
            {
                if (Ra.Brix.Data.Internal.Adapter.Instance is IPersistViewState)
                {
                    if (Settings.Instance.Get<bool>("StoreViewStateInDataBaseIfPossible", true))
                    {
                        if (_pageStatePersister == null)
                            _pageStatePersister = new RaBrixPageStatePersister(this);
                        return _pageStatePersister;
                    }
                    else
                        return base.PageStatePersister;
                }
                else
                    return base.PageStatePersister;
            }
        }
    }
}