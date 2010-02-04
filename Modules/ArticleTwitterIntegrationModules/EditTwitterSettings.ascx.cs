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
using Ra.Effects;
using Ra.Widgets;

namespace ArticleTwitterIntegrationModules
{
    [ActiveModule]
    public class EditTwitterSettings : System.Web.UI.UserControl, IModule
    {
        protected global::Ra.Widgets.Panel pnl2;
        protected global::Ra.Widgets.Panel pnl;
        protected global::Ra.Widgets.TextBox username;
        protected global::Ra.Widgets.TextBox password;
        protected global::Ra.Extensions.Widgets.ExtButton submit;

        public void InitialLoading(Node node)
        {
            Load +=
                delegate
                {
                    username.Text = node["Username"].Get<string>();
                    password.Text = node["Password"].Get<string>();
                    username.Select();
                    username.Focus();
                    submit.DataBind();
                };
        }

        protected void submit_Click(object sender, EventArgs e)
        {
            Node node = new Node();
            node["Username"].Value = username.Text;
            node["Password"].Value = password.Text;
            ActiveEvents.Instance.RaiseActiveEvent(
                this,
                "SaveTwitterUsernameAndPassword",
                node);
            pnl2.Visible = true;
            pnl2.Style[Styles.display] = "none";
            new EffectFadeOut(pnl, 500)
                .ChainThese(
                    new EffectFadeIn(pnl2, 500))
                .Render();
        }
    }
}