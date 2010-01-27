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
using System.Web.UI;
using Ra.Brix.Loader;
using Ra.Effects;
using System.Web.UI.WebControls;
using Ra.Brix.Types;

namespace ActiveDirectoryLoginModules
{
    [ActiveModule]
    public class Login : UserControl, IModule
    {
        protected global::Ra.Widgets.TextBox domainAndUserName;
        protected global::Ra.Widgets.TextBox passWord;
        protected global::Ra.Widgets.Label lbl;
        protected global::System.Web.UI.HtmlControls.HtmlTable tbl;
        
        protected void Page_Load(object sender, EventArgs e)
        {
            tbl.DataBind();

            if (!IsPostBack)
            {
                Literal lit = new Literal();
                string url = Request.Url.ToString();
                if (url.Contains("?"))
                    url = url.Substring(0, url.IndexOf("?"));
                lit.Text = string.Format(@"<link rel=""openid.server"" href=""{0}"" />", url);
                Page.Header.Controls.Add(lit); 
            }
        }

        protected void btnLogin_Click(object sender, EventArgs e)
        {
            Node node = new Node();
            node["Username"].Value = domainAndUserName.Text;

            node["Password"].Value = passWord.Text;

            ActiveEvents.Instance.RaiseActiveEvent(
                this, 
                "UserLoggedIn", 
                node);

            if (!node["Authenticated"].Get<bool>())
            {
                // failure
                new EffectFadeIn(lbl, 1000).Render();

                lbl.CssClass = "errorLbl";
                lbl.Text = Language.Instance["ADLoginLabelFailed",null,"Username or password incorrect"];
                domainAndUserName.Select();
                domainAndUserName.Focus();
            }
        }

        public void InitialLoading(Node node)
        {
            domainAndUserName.Focus();
        }
    }
}



