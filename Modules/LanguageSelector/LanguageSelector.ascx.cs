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
using Ra.Brix.Types;

namespace LanguageSelectorModules
{
    [ActiveModule]
    public class LanguageSelector : System.Web.UI.UserControl, IModule
    {
        protected global::System.Web.UI.HtmlControls.HtmlGenericControl buttons;

        protected void Page_Load(object sender, EventArgs e)
        {
            buttons.DataBind();
        }

        protected void SelectLanguage(object sender, EventArgs e)
        {
            string newLanguage = ((LinkButton)sender).Xtra;

            Node init = new Node();
            init["Language"].Value = newLanguage;
            init["LanguageFriendlyName"].Value = ((LinkButton)sender).Tooltip;

            ActiveEvents.Instance.RaiseActiveEvent(
                this,
                "ChangeUserLanguage",
                init);
        }

        [ActiveEvent(Name = "GetTipOfToday")]
        protected static void GetTipOfToday(object sender, ActiveEventArgs e)
        {
            const string tmp = @"
You can switch the language by going to the Admin menu - if you have access to it.
This will change the language of your portal, but only locally for the user you're logged in as.

By default the portal will use the language defined in the browser, if that language doesn't
exist in your portal installation the default language - which is English - will be used.
";
            e.Params["Tip"]["TipOfLanguageSelector"].Value = Language.Instance["TipOfLanguageSelector", null, tmp];
        }

        public void InitialLoading(Node node)
        {
        }

        public string GetCaption()
        {
            return "";
        }
    }
}