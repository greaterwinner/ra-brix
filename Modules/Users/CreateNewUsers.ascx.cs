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
using UserRecords;
using ASP = System.Web.UI.WebControls;
using Ra.Brix.Types;

namespace UsersModules
{
    [ActiveModule]
    public class CreateNewUsers : System.Web.UI.UserControl, IModule
    {
        protected global::Ra.Widgets.TextBox noUsers;

        protected void save_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < int.Parse(noUsers.Text); i++)
            {
                User user = new User {Username = "username" + i};
                user.Save();
            }
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
