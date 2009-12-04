/*
 * Ra-Brix - A Modular-based Framework for building 
 * Web Applications Copyright 2009 - Thomas Hansen 
 * thomas@ra-ajax.org. Unless permission is 
 * explicitly given this code is licensed under the 
 * GNU Affero GPL version 3 which can be found in the 
 * license.txt file on disc.
 * 
 */

using System.Web;
using System.Web.UI;
using Ra.Brix.Loader;
using SettingsRecords;

namespace GoogleAnalyticsController
{
    [ActiveController]
    public class GoogleAnalyticsController
    {
        [ActiveEvent(Name = "Page_Init")]
        protected static void Page_Init(object sender, ActiveEventArgs e)
        {
            Page page = HttpContext.Current.Handler as Page;
            if (page != null)
            {
                if(!page.IsPostBack)
                {
                    // Inject the Google Analytics tracking code...
                    string trackingID = Settings.Instance["GoogleAnalyticsTrackingID"];
                    if(!string.IsNullOrEmpty(trackingID))
                    {
                        string trackingCode = string.Format(@"
<script type=""text/javascript"">
var _gaq = _gaq || [];
_gaq.push(['_setAccount', '{0}']);
_gaq.push(['_trackPageview']);
(function() {{
var ga = document.createElement('script');
ga.src = ('https:' == document.location.protocol ?
    'https://ssl' : 'http://www') +
    '.google-analytics.com/ga.js';
ga.setAttribute('async', 'true');
document.documentElement.firstChild.appendChild(ga);
}})();
</script>", trackingID);
                        LiteralControl ctrl = new LiteralControl();
                        ctrl.Text = trackingCode;
                        page.Form.Controls.Add(ctrl);
                    }
                }
            }
        }
    }
}
