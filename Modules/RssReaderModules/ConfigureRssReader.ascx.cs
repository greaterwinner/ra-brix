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
using Components;

namespace RssReaderModules
{
    [ActiveModule]
    public class ConfigureRssReader : System.Web.UI.UserControl, IModule
    {
        protected global::Components.Grid grd;

        protected void grid_CellEdited(object sender, Grid.GridEditEventArgs e)
        {
            Node node = new Node();
            node["ID"].Value = e.ID;
            node["URL"].Value = e.NewValue.ToString();
            ActiveEvents.Instance.RaiseActiveEvent(
                this,
                "EditRSSItemURL",
                node);
        }

        protected void grid_RowDeleted(object sender, Grid.GridActionEventArgs e)
        {
            Node node = new Node();
            node["ID"].Value = e.ID;
            ActiveEvents.Instance.RaiseActiveEvent(
                this,
                "DeleteRSSItem",
                node);
        }

        protected void addColumn_Click(object sender, EventArgs e)
        {
            Node node = new Node();
            ActiveEvents.Instance.RaiseActiveEvent(
                this,
                "AddNewRSSItem",
                node);
            grd.DataSource = node["Grid"];
            grd.Rebind();
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
            return Language.Instance["ConfigureRssModule", null, "Configure RSS module"];
        }
    }
}




