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
using ASP = System.Web.UI.WebControls;
using Ra.Brix.Types;
using Ra.Effects;

namespace RolesModules
{
    [ActiveModule]
    public class CreateNewRole : System.Web.UI.UserControl, IModule
    {
        protected global::Ra.Widgets.TextBox roleTxt;
        protected global::Ra.Extensions.Widgets.ExtButton btnSubmit;
        protected global::Ra.Widgets.Panel pnlWrp;

        protected void Page_Load(object sender, EventArgs e)
        {
            btnSubmit.Text = Language.Instance["RolesSaveButton", null, "Save"];
            pnlWrp.DataBind();
        }

        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            Node node = new Node();
            node["RoleName"].Value = roleTxt.Text;
            ActiveEvents.Instance.RaiseActiveEvent(
                this, 
                "CreateNewRole", 
                node);
        }

        protected void roleTxt_EscPressed(object sender, EventArgs e)
        {
            ActiveEvents.Instance.RaiseClearControls("dynPopup");
        }

        public void InitialLoading(Node init)
        {
            roleTxt.Text = Language.Instance["RolesRoleNameText", null, "Role name..."];
            Load +=
                delegate
                    {
                        new EffectTimeout(200)
                            .ChainThese(new EffectFocusAndSelect(roleTxt))
                            .Render();
                    };
        }
    }
}