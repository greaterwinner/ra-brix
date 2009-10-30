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

namespace HistoryModules
{
    [ActiveModule]
    public class History : System.Web.UI.UserControl, IModule
    {
        protected global::Components.Grid grd;

        protected void grid_Action(object sender, Grid.GridActionEventArgs e)
        {
            Node node = new Node();
            node["ID"].Value = e.ID;
            ActiveEvents.Instance.RaiseActiveEvent(
                this,
                "OpenHistoryItem",
                node);
        }

        [ActiveEvent(Name = "HistoryWasUpdated")]
        protected void HistoryWasUpdated(object sender, ActiveEventArgs e)
        {
            grd.DataSource = e.Params["Grid"];
            grd.Rebind();
        }

        public void InitialLoading(Node node)
        {
            grd.DataSource = node["Grid"];
        }

        public string GetCaption()
        {
            return "";
        }
    }
}