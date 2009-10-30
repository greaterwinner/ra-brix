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

namespace WhiteboardModules
{
    [ActiveModule]
    public class ViewAllWhiteboards : System.Web.UI.UserControl, IModule
    {
        protected global::Components.Grid grd;

        protected void grid_Action(object sender, Grid.GridActionEventArgs e)
        {
            Node node = new Node();
            node["ID"].Value = e.ID;
            ActiveEvents.Instance.RaiseActiveEvent(
                this,
                "WhiteboardSelectedForViewing",
                node);
        }

        protected void grid_RowDeleted(object sender, Grid.GridActionEventArgs e)
        {
            Node node = new Node();
            node["ID"].Value = e.ID;
            ActiveEvents.Instance.RaiseActiveEvent(
                this,
                "DeleteWhiteboard",
                node);
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
















