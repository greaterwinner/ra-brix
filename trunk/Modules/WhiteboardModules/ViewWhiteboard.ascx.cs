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

namespace WhiteboardModules
{
    [ActiveModule]
    public class ViewWhiteboard : System.Web.UI.UserControl, IModule
    {
        protected global::Components.Grid grd;
        protected global::Ra.Widgets.Panel pnlWrp;

        protected void grid_RowDeleted(object sender, Grid.GridActionEventArgs e)
        {
            Node node = new Node();
            node["RowID"].Value = e.ID;
            node["WhiteboardID"].Value = WhiteboardID;
            ActiveEvents.Instance.RaiseActiveEvent(
                this,
                "DeleteWhiteboardRow",
                node);
        }

        protected void grid_Action(object sender, Grid.GridActionEventArgs e)
        {
        }

        protected void grid_CellEdited(object sender, Grid.GridEditEventArgs e)
        {
            Node node = new Node();
            node["WhiteboardID"].Value = WhiteboardID;
            node["RowID"].Value = e.ID;
            node["ColumnName"].Value = e.Key;
            node["Value"].Value = e.NewValue;
            ActiveEvents.Instance.RaiseActiveEvent(
                this,
                "WhiteboardRowValueEdited",
                node);
        }

        protected void addRow_Click(object sender, EventArgs e)
        {
            Node node = new Node();
            node["WhiteboardID"].Value = WhiteboardID;
            ActiveEvents.Instance.RaiseActiveEvent(
                this,
                "AddRowToWhiteboard",
                node);
            grd.DataSource = node["Whiteboard"];
            grd.PageToEnd();
            grd.Rebind();
        }

        private string WhiteboardID
        {
            get { return (string)ViewState["WhiteboardID"]; }
            set { ViewState["WhiteboardID"] = value; }
        }

        public void InitialLoading(Node node)
        {
            Load +=
                delegate
                {
                    grd.EnableHeaders = node["Whiteboard"]["EnableHeaders"].Get<bool>();
                    grd.EnableFilter = node["Whiteboard"]["EnableFiltering"].Get<bool>();
                    grd.PageSize = node["Whiteboard"]["PageSize"].Get<int>();
                    grd.EnableDeletion = node["Whiteboard"]["EnableDeletion"].Get<bool>();
                    grd.DataSource = node["Whiteboard"];
                    WhiteboardID = node["Whiteboard"]["ID"].Get<string>();
                };
        }

        [ActiveEvent("GetHelpContents")]
        protected static void GetHelpContents(object sender, ActiveEventArgs e)
        {
            e.Params[Language.Instance["WhiteboardHelpLabel", null, "About the Whiteboards"]].Value = "Help-AboutTheWhiteboards";
        }

        [ActiveEvent("Help-AboutTheWhiteboards")]
        protected static void Help_AboutTheWhiteboards(object sender, ActiveEventArgs e)
        {
            const string whiteBoardHelp = @"
<p>
The Whiteboard module is a computer synthesiz of a traditional whiteboard where you would normally 
keep track of all the different tasks associated with employee or something similar. But the Whiteboard
module can mostly be used for keeping track of any two dimensional array of data you wish to keep track of.
</p>
<p>
The Whiteboard will also automatically refresh itself every n'th second (configurable) with any new data
from the server, if available. And also all columns and such are configurable.
</p>
<p>
Some pre-defined column types exists which you can use for tracking your data, but the types of the whiteboard
are also plugable like everything else in this portal.
</p>
";
            e.Params["Text"].Value = Language.Instance["WhiteboardHelp", null, whiteBoardHelp];
        }
    }
}
















