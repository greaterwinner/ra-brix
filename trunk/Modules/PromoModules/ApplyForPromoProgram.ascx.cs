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
using Ra.Effects;
using Ra.Widgets;

namespace PromoModules
{
    [ActiveModule]
    public class ApplyForPromoProgram : System.Web.UI.UserControl, IModule
    {
        protected global::Ra.Widgets.TextBox wantedCode;
        protected global::Ra.Widgets.TextBox emailAdr;
        protected global::Ra.Extensions.Widgets.ExtButton submit;
        protected global::Ra.Widgets.Label lblErr;
        protected global::Ra.Widgets.Panel pnlPromo;
        protected global::Ra.Widgets.Panel pnlSuccess;
        protected global::Ra.Widgets.RadioButton rdo1;
        protected global::Ra.Widgets.RadioButton rdo2;
        protected global::Ra.Widgets.RadioButton rdo3;
        protected global::Ra.Widgets.RadioButton rdo4;

        protected void submit_Click(object sender, EventArgs e)
        {
            Node node = new Node();
            node["Code"].Value = wantedCode.Text;
            node["Email"].Value = emailAdr.Text;
            if (rdo1.Checked)
            {
                node["Cause"].Value = "WikiPedia";
            }
            else if (rdo2.Checked)
            {
                node["Cause"].Value = "OLPC";
            }
            else if (rdo3.Checked)
            {
                node["Cause"].Value = "The Rainforest Foundation";
            }
            else if (rdo4.Checked)
            {
                node["Cause"].Value = "KIVA";
            }
            ActiveEvents.Instance.RaiseActiveEvent(
                this,
                "ApplyForPromoCode",
                node);
            if (node["Success"].Get<bool>())
            {
                pnlSuccess.Visible = true;
                pnlSuccess.Style[Styles.display] = "none";
                new EffectFadeOut(pnlPromo, 250)
                    .ChainThese(new EffectFadeIn(pnlSuccess, 250))
                    .Render();
            }
            else
            {
                lblErr.Text = node["Error"].Get<string>();
                lblErr.Style[Styles.display] = "none";
                new EffectRollDown(lblErr, 250)
                    .JoinThese(new EffectFadeIn(lblErr, 250))
                    .Render();
                emailAdr.Focus();
                emailAdr.Select();
            }
        }

        public void InitialLoading(Node node)
        {
            Load +=
                delegate
                {
                    emailAdr.Select();
                    emailAdr.Focus();
                    emailAdr.Text = LanguageRecords.Language.Instance["YourEmailAddress", null, "Your email"];
                    wantedCode.Text = LanguageRecords.Language.Instance["WantedPromoCode2", null, "Promo code desire"];
                    submit.Text = Language.Instance["Submit", null, "Submit"];
                };
        }
    }
}