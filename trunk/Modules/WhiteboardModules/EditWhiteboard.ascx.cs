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
using Ra.Extensions.Widgets;
using Ra.Brix.Types;
using Ra.Widgets;
using Components;

namespace WhiteboardModules
{
    [ActiveModule]
    public class EditWhiteboard : System.Web.UI.UserControl, IModule
    {
        protected global::Ra.Extensions.Widgets.InPlaceEdit header;
        protected global::Components.Grid grd;
        protected global::Ra.Extensions.Widgets.InPlaceEdit txtPageSize;
        protected global::Ra.Widgets.CheckBox enableFiltering;
        protected global::Ra.Widgets.Panel settingsHeaders;
        protected global::Ra.Widgets.CheckBox chkEnableHeader;
        protected global::Ra.Widgets.CheckBox chkEnableDeletion;

        protected void Page_Load(object sender, EventArgs e)
        {
            enableFiltering.DataBind();
            txtPageSize.DataBind();
            settingsHeaders.DataBind();
        }

        private string WhiteboardID
        {
            get { return (string)ViewState["WhiteboardID"]; }
            set { ViewState["WhiteboardID"] = value; }
        }

        public void InitialLoading(Node node)
        {
            header.Text = node["Whiteboard"]["Name"].Get<string>();
            Load +=
                delegate
                {
                    WhiteboardID = node["Whiteboard"]["ID"].Get<string>();
                    DataBindGrid(node);
                    enableFiltering.Checked = node["Whiteboard"]["EnableFiltering"].Get<bool>();
                    txtPageSize.Text = node["Whiteboard"]["PageSize"].Get<int>().ToString();
                    chkEnableHeader.Checked = node["Whiteboard"]["EnableHeaders"].Get<bool>();
                    chkEnableDeletion.Checked = node["Whiteboard"]["EnableDeletion"].Get<bool>();
                };
        }

        protected void txtPageSize_TextChanged(object sender, EventArgs e)
        {
            Node node = new Node();
            node["WhiteboardID"].Value = WhiteboardID;
            node["PageSize"].Value = ((InPlaceEdit)sender).Text;
            ActiveEvents.Instance.RaiseActiveEvent(
                this,
                "UpdateWhiteboard",
                node);
        }

        protected void enableFiltering_CheckedChanged(object sender, EventArgs e)
        {
            Node node = new Node();
            node["WhiteboardID"].Value = WhiteboardID;
            node["EnableFiltering"].Value = ((CheckBox)sender).Checked;
            ActiveEvents.Instance.RaiseActiveEvent(
                this,
                "UpdateWhiteboard",
                node);
        }

        private void DataBindGrid(Node node)
        {
            // Creating grid...
            Node gridNode = new Node("Grid");
            gridNode["Columns"]["Name"]["Caption"].Value = Language.Instance["WhiteboardColumnCaption", null, "Column"];
            gridNode["Columns"]["Name"]["ControlType"].Value = "InPlaceEdit";
            gridNode["Columns"]["Type"]["Caption"].Value = Language.Instance["WhiteboardColumnTypeCaption", null, "ColumnType"];
            gridNode["Columns"]["Type"]["ControlType"].Value = "InPlaceEdit";
            gridNode["Columns"]["Position"]["Caption"].Value = Language.Instance["WhiteboardPositionCaption", null, "Position"];
            gridNode["Columns"]["Position"]["ControlType"].Value = "InPlaceEdit";
            int idxNo = 0;
            foreach (Node idx in node["Whiteboard"]["Columns"])
            {
                gridNode["Rows"]["Row" + idxNo]["ID"].Value = idx["ID"].Get<string>();
                gridNode["Rows"]["Row" + idxNo]["Name"].Value = idx["Caption"].Get<string>();
                gridNode["Rows"]["Row" + idxNo]["Type"].Value = idx["Type"].Get<string>();
                gridNode["Rows"]["Row" + idxNo]["Position"].Value = idx["Position"].Get<string>();
                idxNo += 1;
            }
            grd.DataSource = gridNode;
        }

        protected void addColumn_Click(object sender, EventArgs e)
        {
            Node node = new Node();
            node["WhiteboardID"].Value = WhiteboardID;
            ActiveEvents.Instance.RaiseActiveEvent(
                this,
                "AddColumnToEditedWhiteboard",
                node);
        }

        protected void grid_RowDeleted(object sender, Grid.GridActionEventArgs e)
        {
            Node node = new Node();
            node["WhiteboardID"].Value = WhiteboardID;
            node["ColumnID"].Value = e.ID;
            ActiveEvents.Instance.RaiseActiveEvent(
                this,
                "DeleteWhiteboardColumn",
                node);

            // Rebinding grid...
            DataBindGrid(node);
            grd.Rebind();
        }

        [ActiveEvent(Name = "NewWhiteboardColumnAdded")]
        protected void AddNewWhiteboardColumn(object sender, ActiveEventArgs e)
        {
            DataBindGrid(e.Params);
            grd.Rebind();
        }

        protected void grid_CellEdited(object sender, Grid.GridEditEventArgs e)
        {
            Node node = new Node();
            node["WhiteboardID"].Value = WhiteboardID;
            node["ID"].Value = e.ID;
            node["Value"].Value = e.NewValue;
            node["Column"].Value = e.Key;
            ActiveEvents.Instance.RaiseActiveEvent(
                this,
                "WhiteboardColumnEdited",
                node);
        }

        protected void header_TextChanged(object sender, EventArgs e)
        {
            Node node = new Node();
            node["Name"].Value = header.Text;
            node["ID"].Value = WhiteboardID;
            ActiveEvents.Instance.RaiseActiveEvent(
                this,
                "ChangeNameOfSpecificWhiteboard",
                node);
        }

        protected void chkEnableHeader_CheckedChanged(object sender, EventArgs e)
        {
            Node node = new Node();
            node["WhiteboardID"].Value = WhiteboardID;
            node["EnableHeaders"].Value = ((CheckBox)sender).Checked;
            ActiveEvents.Instance.RaiseActiveEvent(
                this,
                "UpdateWhiteboard",
                node);
        }

        protected void chkEnableDeletion_CheckedChanged(object sender, EventArgs e)
        {
            Node node = new Node();
            node["WhiteboardID"].Value = WhiteboardID;
            node["EnableDeletion"].Value = ((CheckBox)sender).Checked;
            ActiveEvents.Instance.RaiseActiveEvent(
                this,
                "UpdateWhiteboard",
                node);
        }

        public string GetCaption()
        {
            return "";
        }

        [ActiveEvent("GetHelpContents")]
        protected static void GetHelpContents(object sender, ActiveEventArgs e)
        {
            e.Params[Language.Instance["WiteboardHelp", null, "About the Whiteboards"]].Value = "Help-AboutTheWhiteboards";
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
















