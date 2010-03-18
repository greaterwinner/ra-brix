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
using Ra.Widgets;
using Ra.Extensions.Widgets;
using System.Text;
using System.Security.Cryptography;
using System.Configuration;
using HelperGlobals;

namespace StackedModules
{
    [ActiveModule]
    public class ViewQuestion : System.Web.UI.UserControl, IModule
    {
        protected global::System.Web.UI.HtmlControls.HtmlGenericControl header;
        protected global::System.Web.UI.HtmlControls.HtmlGenericControl body;

        protected string GetGravatar()
        {
            string email = Email;
            email = email == null ? "" : email;

            StringBuilder emailHash = new StringBuilder();
            MD5 md5 = MD5.Create();
            byte[] emailBuffer = Encoding.ASCII.GetBytes(email);
            byte[] hash = md5.ComputeHash(emailBuffer);

            foreach (byte hashByte in hash)
                emailHash.Append(hashByte.ToString("x2"));

            return string.Format("http://www.gravatar.com/avatar/{0}?s=48&d=identicon", emailHash.ToString());
        }

        protected string GetUsernameLink()
        {
            return ApplicationRoot.Root +
                "authors/" + Username.Replace(".", "--") +
                ConfigurationManager.AppSettings["DefaultPageExtension"];
        }

        protected string GetUsername()
        {
            return Username;
        }

        private string Username
        {
            get { return ViewState["Username"] as string; }
            set { ViewState["Username"] = value; }
        }

        private string Email
        {
            get { return ViewState["Email"] as string; }
            set { ViewState["Email"] = value; }
        }

        public void InitialLoading(Node node)
        {
            Load +=
                delegate
                {
                    Username = node["Username"].Get<string>();
                    Email = node["Email"].Get<string>();
                    header.InnerHtml = node["Header"].Get<string>();
                    body.InnerHtml = node["Body"].Get<string>();
                };
        }
    }
}




