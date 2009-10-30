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
using Ra.Brix.Data;
using System.Web.UI;
using Ra.Brix.Loader;
using System.IO;
using SettingsRecords;
using UserSettingsRecords;

namespace Ra.Brix.Portal
{
    public partial class MainWebPage : System.Web.UI.Page
    {
        protected void Page_Init(object sender, EventArgs e)
        {
            InitializeViewport();
            ActiveEvents.Instance.RaiseActiveEvent(
                this,
                "RequestBegin");
            if (!IsPostBack)
            {
                ActiveEvents.Instance.RaiseActiveEvent(
                    this, 
                    "InitialLoadingOfPage");
            }
            this.LoadComplete += MainWebPage_LoadComplete;
            Context.RewritePath(Path.GetFileName(Request.AppRelativeCurrentExecutionFilePath), true);
            baseElement.DataBind();
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
            string portalDefault = Settings.Instance.Get<string>(
                "CssRootFolder",
                "Light");
            if (string.IsNullOrEmpty(Users.LoggedInUserName))
            {
                return portalDefault;
            }
            return UserSettings.Instance.Get<string>(
                "CssRootFolder", 
                Users.LoggedInUserName, 
                portalDefault);
        }
        
        protected string GetRedirectUrl()
        {
            return this.Request.Url.ToString();
        }

        void MainWebPage_LoadComplete(object sender, EventArgs e)
        {
            ActiveEvents.Instance.RaiseActiveEvent(
                this,
                "LoadComplete");
        }

        private void InitializeViewport()
        {
            string defaultControl = ConfigurationManager.AppSettings["PortalViewDriver"];
            Control ctrl = PluginLoader.Instance.LoadControl(defaultControl);
            Form.Controls.Add(ctrl);
        }
    }
}