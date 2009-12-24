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
using System;
using Ra.Brix.Types;
using ASP = System.Web.UI.WebControls;
using Ra.Effects;

namespace ADGroups2RolesModules
{
    [ActiveModule]
    public class CreateNew : System.Web.UI.UserControl, IModule
    {
        protected global::Ra.Widgets.TextBox adGroupName;
        protected global::Ra.Widgets.Panel pnlWrp;

        protected void Page_Load(object sender, EventArgs e)
        {
            pnlWrp.DataBind();
        }

        protected void submit_Click(object sender, EventArgs e)
        {
            Node node = new Node();
            node["ADGroupName"].Value = adGroupName.Text;
            ActiveEvents.Instance.RaiseActiveEvent(
                this,
                "CreateNewAD2RoleMapping",
                node);
        }

        protected void adGroupName_EscPressed(object sender, EventArgs e)
        {
            ActiveEvents.Instance.RaiseClearControls("dynPopup");
        }

        public void InitialLoading(Node node)
        {
            adGroupName.Text = Language.Instance["ADGroups2RolesNameText", null, "Name of AD group"];
            Load +=
                delegate
                    {
                        new EffectTimeout(150).ChainThese(
                            new EffectFocusAndSelect(adGroupName))
                            .Render();
                    };
        }

        public string GetCaption()
        {
            return "";
        }
    }
}