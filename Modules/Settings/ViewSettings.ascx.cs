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
using ASP = System.Web.UI.WebControls;
using Ra.Brix.Types;
using Components;

namespace SettingsModules
{
    [ActiveModule]
    public class ViewSettings : System.Web.UI.UserControl, IModule
    {
        protected global::Components.Grid grd;

        protected void grid_CellEdited(object sender, Grid.GridEditEventArgs e)
        {
            Node node = new Node();
            node["ID"].Value = e.ID;
            node["Value"].Value = e.NewValue.ToString();
            ActiveEvents.Instance.RaiseActiveEvent(
                this,
                EditEventName,
                node);
        }

        protected void grid_RowDeleted(object sender, Grid.GridActionEventArgs e)
        {
            Node node = new Node();
            node["ID"].Value = e.ID;
            ActiveEvents.Instance.RaiseActiveEvent(
                this,
                DeleteEventName,
                node);
        }

        private string DeleteEventName
        {
            get { return ViewState["DeleteEventName"] as string; }
            set { ViewState["DeleteEventName"] = value; }
        }

        private string EditEventName
        {
            get { return ViewState["EditEventName"] as string; }
            set { ViewState["EditEventName"] = value; }
        }

        public void InitialLoading(Node node)
        {
            grd.DataSource = node["Grid"];

            Load +=
                delegate
                {
                    DeleteEventName = node["DeleteEventName"].Get<string>();
                    EditEventName = node["EditEventName"].Get<string>();
                };
        }
    }
}




