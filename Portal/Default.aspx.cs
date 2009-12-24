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

namespace Ra.Brix.Portal
{
    public partial class MainWebPage : Page
    {
        protected void Page_Init(object sender, EventArgs e)
        {
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
            baseElement.DataBind();
        }

        void MainWebPage_LoadComplete(object sender, EventArgs e)
        {
            ActiveEvents.Instance.RaiseActiveEvent(
                this,
                "LoadComplete");
        }

        protected string GetBaseURL()
        {
            string retVal = Request.Url.GetComponents(
                UriComponents.HostAndPort | 
                UriComponents.Scheme | 
                UriComponents.Path, 
                UriFormat.SafeUnescaped);
            retVal = retVal.Substring(0, retVal.LastIndexOf("/") + 1);
            return retVal;
        }

        protected override void OnPreRenderComplete(EventArgs e)
        {
            css1.DataBind();
            css2.DataBind();
            css3.DataBind();
            base.OnPreRenderComplete(e);
        }

        protected string GetCssRootFolder()
        {
            string portalDefault = Settings.Instance.Get(
                "CssRootFolder",
                "Gold");
            if (string.IsNullOrEmpty(Users.LoggedInUserName))
            {
                return portalDefault;
            }
            return UserSettings.Instance.Get(
                "CssRootFolder", 
                Users.LoggedInUserName, 
                portalDefault);
        }
        
        protected string GetRedirectUrl()
        {
            return Request.Url.ToString();
        }

        private void InitializeViewport()
        {
            string defaultControl = ConfigurationManager.AppSettings["PortalViewDriver"];
            Control ctrl = PluginLoader.Instance.LoadControl(defaultControl);
            Form.Controls.Add(ctrl);
        }
    }
}