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

namespace CommonModules
{
    [ActiveModule]
    public class MessageBox : System.Web.UI.UserControl, IModule
    {
        protected global::Ra.Widgets.Label txt;
        protected global::Ra.Extensions.Widgets.ExtButton submit;

        protected void submit_Click(object sender, EventArgs e)
        {
            Node node = new Node();
            if(!string.IsNullOrEmpty(EventParamName))
            {
                node[EventParamName].Value = EventParamValue;
            }
            ActiveEvents.Instance.RaiseActiveEvent(
                this,
                EventName,
                node);
            ActiveEvents.Instance.RaiseClearControls("dynPopup");
        }

        private string EventName
        {
            get { return ViewState["EventName"] as string; }
            set { ViewState["EventName"] = value; }
        }

        private string EventParamName
        {
            get { return ViewState["EventParamName"] as string; }
            set { ViewState["EventParamName"] = value; }
        }

        private string EventParamValue
        {
            get { return ViewState["EventParamValue"] as string; }
            set { ViewState["EventParamValue"] = value; }
        }

        public void InitialLoading(Node node)
        {
            submit.DataBind();
            Load +=
                delegate
                    {
                        txt.Text = node["Text"].Get<string>();
                        EventName = node["EventToRaiseOnOK"].Get<string>();
                        EventParamName = node["Params"]["Name"].Get<string>();
                        EventParamValue = node["Params"]["Value"].Get<string>();
                        txt.Focus();
                    };
        }

        public string GetCaption()
        {
            return "";
        }
    }
}