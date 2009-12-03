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
using Components;

namespace WhiteboardModules
{
    [ActiveModule]
    public class ViewAllWhiteboards : System.Web.UI.UserControl, IModule
    {
        protected global::Components.Grid grd;

        protected void grid_Action(object sender, Grid.GridActionEventArgs e)
        {
            if (e.ColumnName == "View")
            {
                Node node = new Node();
                node["ID"].Value = e.ID;
                ActiveEvents.Instance.RaiseActiveEvent(
                    this,
                    "WhiteboardSelectedForViewing",
                    node);
            }
            else if (e.ColumnName == "Edit")
            {
                Node node = new Node();
                node["ID"].Value = e.ID;
                ActiveEvents.Instance.RaiseActiveEvent(
                    this,
                    "WhiteboardSelectedForEditing",
                    node);
            }
        }

        protected void grid_RowDeleted(object sender, Grid.GridActionEventArgs e)
        {
            Node init = new Node();
            init["TabCaption"].Value = Language.Instance["AreYouSure", null, "Are you sure?"];
            init["Width"].Value = 350;
            init["Height"].Value = 130;
            init["ModuleSettings"]["Text"].Value =
                Language.Instance["ReallyDeleteWhiteBoard", null,
                @"Do you really want to delete the selected whiteboard?"];
            init["ModuleSettings"]["EventToRaiseOnOK"].Value = "WhiteBoardConfirmedDeleted";
            init["ModuleSettings"]["Params"]["Name"].Value = "WhiteboardID";
            init["ModuleSettings"]["Params"]["Value"].Value = e.ID;
            ActiveEvents.Instance.RaiseLoadControl(
                "CommonModules.MessageBox",
                "dynPopup",
                init);
        }

        [ActiveEvent(Name = "WhiteBoardConfirmedDeleted")]
        protected void WhiteBoardDeleted(object sender, ActiveEventArgs e)
        {
            Node node = new Node();
            node["ID"].Value = e.Params["WhiteboardID"].Get<string>();
            ActiveEvents.Instance.RaiseActiveEvent(
                this,
                "DeleteWhiteboard",
                node);

            // Just forwarding to our menu event ...
            // To make sure this control is RE-loaded...!!
            ActiveEvents.Instance.RaiseActiveEvent(
                this,
                "Menu-ViewAllWhiteboards");
        }

        public void InitialLoading(Node node)
        {
            grd.DataSource = node["Grid"];
        }

        [ActiveEvent(Name = "WhiteboardWasCreated")]
        [ActiveEvent(Name = "WhiteboardWasDeleted")]
        protected void WhiteboardWasCreated(object sender, ActiveEventArgs e)
        {
            // Just forwarding to our menu event ...
            // To make sure this control is RE-loaded...!!
            ActiveEvents.Instance.RaiseActiveEvent(
                this,
                "Menu-ViewAllWhiteboards");
        }

        public string GetCaption()
        {
            return "";
        }
    }
}
















