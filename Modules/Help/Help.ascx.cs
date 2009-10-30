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
using Ra.Widgets;
using Ra.Brix.Loader;
using Ra.Effects;
using Ra.Brix.Types;
using Ra.Selector;

namespace HelpModules
{
    [ActiveModule]
    public class Help : System.Web.UI.UserControl, IModule
    {
        protected global::System.Web.UI.WebControls.Repeater rep;
        protected global::System.Web.UI.HtmlControls.HtmlGenericControl headers;

        protected void Page_Load(object sender, EventArgs e)
        {
            headers.DataBind();
        }

        private Node HelpContents
        {
            get { return Session["HelpModules.Help.HelpContents"] as Node; }
            set { Session["HelpModules.Help.HelpContents"] = value; }
        }

        [ActiveEvent(Name = "GetTipOfToday")]
        protected static void GetTipOfToday(object sender, ActiveEventArgs e)
        {
            const string tmp = @"
If you're stuck and you need help for some features of the portal, it is highly likely
that the Help module will give you advice in regards to how to use some specific feature
of the portal.

The Help module can be easily accessed through the Applications menu, and will stay open until
you explicitly close it or open up another application which uses the same Viewport Container
as the Help module.
";
            e.Params["Tip"]["TipOfHelp"].Value = Language.Instance["TipOfHelp", null, tmp];
        }

        public void InitialLoading(Node node)
        {
            Node nodeInit = new Node();
            ActiveEvents.Instance.RaiseActiveEvent(
                this,
                "GetHelpContents",
                nodeInit);
            HelpContents = nodeInit;
            DataBindHelpContents();
        }

        private void DataBindHelpContents()
        {
            rep.DataSource = HelpContents;
            rep.DataBind();
        }

        protected void ViewContents(object sender, EventArgs e)
        {
            LinkButton btn = sender as LinkButton;
            if (btn == null)
                return;
            Label txtLbl = Selector.SelectFirst<Label>(btn.Parent);
            Panel pnl = Selector.SelectFirst<Panel>(btn.Parent);
            if (pnl.Xtra != "true")
            {
                pnl.Xtra = "true";
                string eventName = btn.Xtra;
                Node nodeInit = new Node();
                ActiveEvents.Instance.RaiseActiveEvent(
                    this,
                    eventName,
                    nodeInit);

                // Finding Label to display contents within...
                pnl.Style[Styles.display] = "none";
                txtLbl.Text = nodeInit["Text"].Get<string>();
                new EffectRollDown(pnl, 400)
                    .JoinThese(new EffectFadeIn())
                    .Render();
            }
            else
            {
                pnl.Xtra = "false";
                new EffectRollUp(pnl, 400)
                    .JoinThese(new EffectFadeOut())
                    .Render();
            }
        }

        public string GetCaption()
        {
            return "";
        }
    }
}