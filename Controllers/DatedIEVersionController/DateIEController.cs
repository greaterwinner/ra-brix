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
using Ra.Brix.Loader;
using System.Web;
using SettingsRecords;
using System.Web.UI;

namespace DatedIEVersionController
{
    [ActiveController]
    public class DateIEController
    {
        [ActiveEvent(Name = "Page_Init_InitialLoading")]
        protected void InitialLoading(object sender, ActiveEventArgs e)
        {
            InitializeChromeFrame();
        }

        private void InitializeChromeFrame()
        {
            if (HttpContext.Current.Request.Browser.Browser == "IE")
            {
                if (Settings.Instance.Get<bool>("UseChromeFrame", true))
                {
                    ActiveEvents.Instance.RaiseLoadControl(
                        "DatedIEVersionModules.DatedIE",
                        "dynTop");

                    LiteralControl lit = new LiteralControl();
                    lit.Text = @"
<!--[if lt IE 8]>
<meta http-equiv=""X-UA-Compatible"" content=""chrome=1"" />
<![endif]-->";
                    (HttpContext.Current.Handler as Page).Header.Controls.Add(lit);
                }
            }
        }
    }
}
