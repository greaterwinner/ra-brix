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
using System.Text;
using System.Security.Cryptography;
using HelperGlobals;
using System.Configuration;

namespace StackedModules
{
    [ActiveModule]
    public class ViewPosts : System.Web.UI.UserControl, IModule
    {
        protected global::System.Web.UI.WebControls.Repeater rep;
        protected global::Ra.Extensions.Widgets.ExtButton ask;

        public void InitialLoading(Node node)
        {
            Load +=
                delegate
                {
                    rep.DataSource = node["Questions"];
                    rep.DataBind();
                    ask.DataBind();
                };
        }

        protected string GetGravatar(object emailObj)
        {
            string email = emailObj as string;
            email = email == null ? "" : email;

            StringBuilder emailHash = new StringBuilder();
            MD5 md5 = MD5.Create();
            byte[] emailBuffer = Encoding.ASCII.GetBytes(email);
            byte[] hash = md5.ComputeHash(emailBuffer);

            foreach (byte hashByte in hash)
                emailHash.Append(hashByte.ToString("x2"));

            return string.Format("http://www.gravatar.com/avatar/{0}?s=32&d=identicon", emailHash.ToString());
        }

        protected string GetUsernameLink(object usernameObj)
        {
            string username = usernameObj as string;
            return ApplicationRoot.Root +
                "authors/" + username.Replace(".", "--") +
                ConfigurationManager.AppSettings["DefaultPageExtension"];
        }

        protected void ask_Click(object sender, EventArgs e)
        {
            ActiveEvents.Instance.RaiseActiveEvent(
                this,
                "AskStackedQuestion");
        }
    }
}




