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
using Ra.Brix.Types;
using Components;

namespace InstalledAppsModules
{
    [ActiveModule]
    public class ViewAllApps : System.Web.UI.UserControl, IModule
    {
        protected global::Components.Grid grd;

        protected void grd_Action(object sender, Grid.GridActionEventArgs e)
        {
            Node node = new Node();
            node["AppName"].Value = e.ID;
            ActiveEvents.Instance.RaiseActiveEvent(
                this,
                "InstalledApps-ViewDetailsOfApp",
                node);
        }

        public void InitialLoading(Node node)
        {
            Load +=
                delegate
                {
                    grd.DataSource = node["Grid"];
                };
        }

        public string GetCaption()
        {
            return "";
        }
    }
}

