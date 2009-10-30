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

namespace UsersModules
{
    [ActiveModule]
    public class CreateNewUser : System.Web.UI.UserControl, IModule
    {
        protected global::Ra.Widgets.TextBox usernameTxt;
        protected global::Ra.Widgets.Panel pnlWrp;
        protected global::Ra.Extensions.Widgets.ExtButton btnSubmit;

        protected void Page_Load(object sender, EventArgs e)
        {
            btnSubmit.Text = Language.Instance["UsersSaveButton", null, "Save"];
            pnlWrp.DataBind();
        }

        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            Node node = new Node();
            node["Username"].Value = usernameTxt.Text;
            ActiveEvents.Instance.RaiseActiveEvent(
                this,
                "CreateNewUser",
                node);
        }

        protected void usernameTxt_EscPressed(object sender, EventArgs e)
        {
            ActiveEvents.Instance.RaiseActiveEvent(
                this,
                "CloseCreateUserForm",
                null);
        }

        public void InitialLoading(Node node)
        {
            Load +=
                delegate
                {
                    new EffectTimeout(200)
                        .ChainThese(new EffectFocusAndSelect(usernameTxt))
                        .Render();
                };
            usernameTxt.Text = Language.Instance["UsersTypeUserNameText", null, "Type in username of user"];
        }

        public string GetCaption()
        {
            return "";
        }
    }
}





