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
using System.Web.UI;

namespace Ra.Brix.Portal
{
    public partial class MainWebPage : Page
    {
        protected void Page_Init(object sender, EventArgs e)
        {
            InitializeViewport();
            if (!IsPostBack)
            {
                Loader.ActiveEvents.Instance.RaiseActiveEvent(
                    this,
                    "InitialLoadingOfPage");
            }
        }

        private void InitializeViewport()
        {
            string defaultControl = ConfigurationManager.AppSettings["PortalViewDriver"];
            Control ctrl = Loader.PluginLoader.Instance.LoadControl(defaultControl);
            Form.Controls.Add(ctrl);
        }
    }
}



