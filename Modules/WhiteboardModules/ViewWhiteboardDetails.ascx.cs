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
using System;
using Ra.Widgets;

namespace WhiteboardModules
{
    [ActiveModule]
    public class ViewWhiteboardDetails : System.Web.UI.UserControl, IModule
    {
        protected global::Ra.Widgets.Panel wrpPnl;
        protected global::Ra.Widgets.Label headerLbl;
        private Node _cells;

        protected void Page_Load(object sender, EventArgs e)
        {
            if(_cells != null)
            {
                Cells = _cells["Cells"];
            }
            CreateControls(Cells);
        }

        private Node Cells
        {
            get { return ViewState["Cells"] as Node; }
            set { ViewState["Cells"] = value; }
        }

        private void CreateControls(Node node)
        {
            foreach(Node idx in node)
            {
                string caption = idx["Caption"].Get<string>();
                string value = idx["Value"].Get<string>();
                string type = idx["Type"].Get<string>();
                int position = idx["Position"].Get<int>();

                Panel pnl = new Panel();
                pnl.CssClass = "bordered-panel";

                Label cpt = new Label();
                cpt.CssClass = "label-caption";
                cpt.Text = caption;
                pnl.Controls.Add(cpt);

                Label val = new Label();
                val.CssClass = "label-value";
                val.Text = value;
                pnl.Controls.Add(val);

                // Rooting...
                wrpPnl.Controls.Add(pnl);
            }
        }

        public void InitialLoading(Node node)
        {
            _cells = node;
            headerLbl.Text = node["Header"].Get<string>();
        }

        public string GetCaption()
        {
            return "";
        }
    }
}
















