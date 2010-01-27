/*
 * Ra-Brix - A Modular-based Framework for building 
 * Web Applications Copyright 2009 - Thomas Hansen 
 * thomas@ra-ajax.org. Unless permission is 
 * explicitly given this code is licensed under the 
 * GNU Affero GPL version 3 which can be found in the 
 * license.txt file on disc.
 * 
 */

using Ra.Brix.Loader;
using ASP = System.Web.UI.WebControls;
using Ra.Brix.Types;
using Components;

namespace RolesModules
{
    [ActiveModule]
    public class ViewAllRoles : System.Web.UI.UserControl, IModule
    {
        protected global::Components.Grid grd;

        protected void grid_Deleted(object sender, Grid.GridActionEventArgs e)
        {
            Node node = new Node();
            node["ID"].Value = e.ID;

            ActiveEvents.Instance.RaiseActiveEvent(
                this,
                "DeleteRole",
                node);
        }

        public void InitialLoading(Node node)
        {
            grd.DataSource = node["Grid"];
        }
    }
}