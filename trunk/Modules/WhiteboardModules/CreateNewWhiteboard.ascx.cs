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

namespace WhiteboardModules
{
    [ActiveModule]
    public class CreateNewWhiteboard : System.Web.UI.UserControl, IModule
    {
        protected global::Ra.Widgets.Panel wrp;
        protected global::Ra.Widgets.TextBox name;

        protected void submit_Click(object sender, EventArgs e)
        {
            Node node = new Node();
            node["WhiteboardName"].Value = name.Text;
            ActiveEvents.Instance.RaiseActiveEvent(
                this,
                "CreateNewWhiteboard",
                node);
        }

        protected void name_EscPressed(object sender, EventArgs e)
        {
            ActiveEvents.Instance.RaiseClearControls("dynPopup");
        }

        public void InitialLoading(Node node)
        {
            wrp.DataBind();
            name.Text = Language.Instance["ListName", null, "Name of List"];
            name.Select();
            name.Focus();
        }
    }
}
















