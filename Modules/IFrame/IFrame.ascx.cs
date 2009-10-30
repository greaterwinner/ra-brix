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
using Ra.Brix.Types;

namespace IFrameModules
{
    [ActiveModule]
    public class IFrame : System.Web.UI.UserControl, IModule
    {
        protected global::System.Web.UI.HtmlControls.HtmlGenericControl iframe;
        protected global::Ra.Widgets.Panel iframeWrp;

        protected void Page_Load(object sender, EventArgs e)
        {
        }

        [ActiveEvent("GetHelpContents")]
        protected static void GetHelpContents(object sender, ActiveEventArgs e)
        {
            e.Params[Language.Instance["IFrameHelpLabel", null, "About the IFrames"]].Value = "Help-AboutTheIframe";
        }

        [ActiveEvent("Help-AboutTheIframe")]
        protected static void Help_AboutTheIframe(object sender, ActiveEventArgs e)
        {
            const string iFrameAboutDefault = @"
<p>
The IFrame Module is used to load other websites and host them as modules inside of the portal.
These websites will seem as an integrated part of the Portal, but really aren's since they're
just hosted completely as their own websites inside of the portal.
</p>
<p>
Most portals that uses this module will have support for changing whatever the IFrames are linking
to, for instance through some settings or similar.
</p>
";
            e.Params["Text"].Value = Language.Instance["IFrameHelp", null, iFrameAboutDefault];
        }

        public void InitialLoading(Node node)
        {
            string src = node["URL"].Value.ToString();
            if (string.IsNullOrEmpty(src))
            {
                iframe.Visible = false;
            }
            else
            {
                iframe.Attributes.Add("src", src);
            }
        }

        public string GetCaption()
        {
            return "";
        }
    }
}