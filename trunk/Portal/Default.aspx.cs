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
            // Changing the URL to un-escaped to get rid of the ContentID parameter and
            // make the URL for the form "beautiful"...
            Form.Action = Request.RawUrl.Replace("default.aspx", "");

            // ORDER COUNTS HERE....!
            // Since all DLL's are loaded into the AppDomain by the plugin loader
            // we are unfortunately stuck in a logic where order counts (for now)
            // TODO: Fix cohesion here ...!
            InitializeViewport();
            InitializeChromeFrame();
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

        private void InitializeChromeFrame()
        {
            if (Settings.Instance["UseChromeFrame"] == "True")
            {
                LiteralControl lit = new LiteralControl();
                lit.Text = string.Format(@"
        <!--[if IE]>
        <script type=""text/javascript"" src=""http://ajax.googleapis.com/ajax/libs/chrome-frame/1/CFInstall.min.js""></script>
        <div id=""placeholder""></div>
        <script>
            CFInstall.check({{
                node: ""placeholder"",
                destination: ""{0}""
            }});
        </script>
        <![endif]-->", GetRedirectUrl());
                Form.Controls.Add(lit);
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
            return Request.Url.ToString().Replace("default.aspx", "");
        }

        private void InitializeViewport()
        {
            string defaultControl = ConfigurationManager.AppSettings["PortalViewDriver"];
            Control ctrl = PluginLoader.Instance.LoadControl(defaultControl);
            Form.Controls.Add(ctrl);
        }
    }
}