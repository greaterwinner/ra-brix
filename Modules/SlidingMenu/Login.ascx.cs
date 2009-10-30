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
using Ra.Extensions;
using Ra.Widgets;
using System.Configuration;
using Ra.Brix.Data;
using System.Web.UI;
using Ra.Brix.Loader;
using Ra.Extensions.Widgets;
using Ra.Selector;
using Ra.Effects;
using System.Web.UI.WebControls;
using Ra.Brix.Types;

namespace ActiveDirectoryLogin
{
    [ActiveModule]
    public partial class Login : UserControl, IModule
    {
        protected global::Ra.Widgets.TextBox domainAndUserName;
        protected global::Ra.Widgets.TextBox passWord;

        protected void Page_Load(object sender, EventArgs e)
        {
            Literal lit = new Literal();
            string url = Request.Url.ToString();
            if (url.Contains("?"))
                url = url.Substring(0, url.IndexOf("?"));
            lit.Text = string.Format(@"<link rel=""openid.server"" href=""{0}"" />", url);
            Page.Header.Controls.Add(lit);
        }

        protected void btnLogin_Click(object sender, EventArgs e)
        {
            string [] domainAndUserNameArray = domainAndUserName.Text.Split('\\');
            
            string domain = domainAndUserNameArray[0];
            string userName = domainAndUserNameArray[1];
            string pwd = passWord.Text;

            string[] args = new string[] { domain, userName, pwd };
            Node node = new Node("UserLoggedIn");
            node.Add(new Node("Domain", domain));
            node.Add(new Node("Username", userName));
            node.Add(new Node("Password", pwd));

            ActiveEvents.Instance.RaiseActiveEvent(this, "UserLoggedIn", node);
        }

        public void InitialLoading(Node node)
        {
            domainAndUserName.Focus();
        }

        public string GetCaption()
        {
            return "";
        }
    }
}



