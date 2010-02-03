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
using Ra.Brix.Types;
using System;
using Ra.Effects;

namespace TipTheAuthorsModules
{
    [ActiveModule]
    public class TipAuthors : System.Web.UI.UserControl, IModule
    {
        protected global::Ra.Widgets.Label information;
        protected global::Ra.Widgets.TextBox tip;
        protected global::Ra.Extensions.Widgets.ExtButton submit;
        protected global::Ra.Widgets.Label thankYou;

        public void InitialLoading(Node node)
        {
            Load +=
                delegate
                {
                    submit.DataBind();
                    string infoText = @"
<p>
Here you can submit tip to the authors in such a way that whenever an author logs into the system he can see
these tips and click on the link you submit to read the story. This is useful for people who don't want to write 
an article themselves, but still would like to see an article created around a specific subject.
</p>
<p>
So if you have read a press release about something you think TheLightBringer.org should write about, or maybe
you have your own press release you wish someone could write something up about, then this is the place you
want to submit that tip.
</p>
";
                    information.Text = Language.Instance["SubmitTipInfoText2", null, infoText];
                    thankYou.DataBind();
                };
            tip.Focus();
            tip.Select();
        }

        protected void submit_Click(object sender, EventArgs e)
        {
            // Notifying controller
            Node node = new Node();
            node["URL"].Value = tip.Text;
            ActiveEvents.Instance.RaiseActiveEvent(
                this,
                "NewArticleTipSubmitted",
                node);

            // UI stuff...
            thankYou.Visible = true;
            new EffectFadeIn(thankYou, 500)
                .JoinThese(new EffectRollDown())
                .ChainThese(
                    new EffectTimeout(2500),
                    new EffectFadeOut(thankYou, 500)
                        .JoinThese(new EffectRollUp()))
                .Render();
            tip.Text = "http://";
            tip.Focus();
            tip.Select();
        }
    }
}




