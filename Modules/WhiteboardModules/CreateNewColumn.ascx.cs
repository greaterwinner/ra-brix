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
using Ra.Brix.Types;

namespace WhiteboardModules
{
    [ActiveModule]
    public class CreateNewColumn : System.Web.UI.UserControl, IModule
    {
        protected global::Ra.Widgets.Panel wrp;
        protected global::Ra.Widgets.TextBox name;

        protected void submit_Click(object sender, EventArgs e)
        {
            Node node = new Node();
            node["ColumnName"].Value = name.Text;
            node["WhiteboardID"].Value = WhiteboardID;
            ActiveEvents.Instance.RaiseActiveEvent(
                this,
                "CreateNewWhiteboardColumn",
                node);
        }

        private string WhiteboardID
        {
            get { return (string)ViewState["WhiteboardID"]; }
            set { ViewState["WhiteboardID"] = value; }
        }

        public void InitialLoading(Node node)
        {
            wrp.DataBind();
            name.Text = "Name of Column";
            name.Select();
            name.Focus();

            Load +=
                delegate
                {
                    WhiteboardID = node["WhiteboardID"].Get<string>();
                };
        }

        public string GetCaption()
        {
            return "";
        }
    }
}
















