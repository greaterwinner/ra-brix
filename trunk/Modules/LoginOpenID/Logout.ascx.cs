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

namespace LoginOpenIDModules
{
    [ActiveModule]
    public class Logout : System.Web.UI.UserControl
    {
        protected global::Ra.Widgets.LinkButton logout;

        protected void Page_Load(object sender, EventArgs e)
        {
            logout.DataBind();
        }

        protected void logout_Click(object sender, EventArgs e)
        {
            ActiveEvents.Instance.RaiseActiveEvent(
                this, 
                "UserWantsToLogOut");
        }
    }
}