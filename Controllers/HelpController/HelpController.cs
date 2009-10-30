/*
 * Ra-Brix - A Modular-based Framework for building 
 * Web Applications Copyright 2009 - Thomas Hansen 
 * thomas@ra-ajax.org. Unless permission is 
 * explicitly given this code is licensed under the 
 * GNU Affero GPL version 3 which can be found in the 
 * license.txt file on disc.
 * 
 */

using LanguageRecords;
using Ra.Brix.Loader;
using Ra.Brix.Types;

namespace HelpController
{
    [ActiveController]
    public class HelpController
    {
        [ActiveEvent(Name = "ApplicationStartup")]
        protected static void ApplicationStartup2(object sender, ActiveEventArgs e)
        {
            Language.Instance.SetDefaultValue("ButtonHelp", "Help");
        }

        [ActiveEvent(Name = "GetMenuItems")]
        protected void GetMenuItems(object sender, ActiveEventArgs e)
        {
            e.Params["ButtonHelp"].Value = "ViewHelpModule";
        }

        [ActiveEvent(Name = "ViewHelpModule")]
        protected void ViewHelpModule(object sender, ActiveEventArgs e)
        {
            Node node = new Node();
            node["TabCaption"].Value = Language.Instance["HelpTabCaption", null, "Help"];
            node["Width"].Value = 600;
            node["Height"].Value = 480;
            ActiveEvents.Instance.RaiseLoadControl(
                "HelpModules.Help",
                "dynPopup2",
                node);

            node["Message"].Value = Language.Instance["HelpViewInfo", null, @"To view help: 
Move the cursor over the bull's eye on the right side of this information message."];
            node["Duration"].Value = 5000;
            ActiveEvents.Instance.RaiseActiveEvent(
                this,
                "ShowInformationMessage",
                node);
        }
    }
}
