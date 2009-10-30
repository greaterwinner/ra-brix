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

namespace LoginOpenIDModules
{
    [ActiveModule]
    public class ReportAbuse : System.Web.UI.UserControl, IModule
    {
        protected global::System.Web.UI.HtmlControls.HtmlGenericControl header;

        protected void Page_Load(object sender, EventArgs e)
        {
            header.DataBind();
        }

        private string Username
        {
            get { return (string)ViewState["Username"]; }
            set { ViewState["Username"] = value; }
        }

        public string GetCaption()
        {
            return Language.Instance["OpenIDCaption" , null, "Report abuse..."];
        }

        public void InitialLoading(Node node)
        {
            Load +=
                delegate
                {
                    Username = node["Username"].Value.ToString();
                };
        }

        protected void blockBtn_Click(object sender, EventArgs e)
        {
            Node node = new Node();
            node["Username"].Value = Username;
            ActiveEvents.Instance.RaiseActiveEvent(
                this, 
                "BlockUser", 
                node);
        }
    }
}