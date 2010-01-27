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
using HelperGlobals;
using System;
using LanguageRecords;
using System.Text;
using System.Security.Cryptography;

namespace ArticlePublisherModules
{
    [ActiveModule]
    public class ViewUser : System.Web.UI.UserControl, IModule
    {
        protected global::System.Web.UI.HtmlControls.HtmlGenericControl header;
        protected global::System.Web.UI.HtmlControls.HtmlGenericControl summaryView;
        protected global::System.Web.UI.HtmlControls.HtmlImage gravatar;

        public void InitialLoading(Node node)
        {
            header.InnerHtml = node["Username"].Get<string>();
            summaryView.InnerHtml = string.Format(Language.Instance["UserInformationText", null, @"
User has posted {0} articles and {1} comments. And was last seen {2}. User's email addresse is <a href=""mailto:{3}"">{3}</a> and if you want 
to check out if he passes the Turing Test you can call him at {4}. Oh yeah, and user belongs to these role(s); {5}...
"],
                node["ArticleCount"].Value,
                node["CommentCount"].Value,
                DateFormatter.FormatDate((DateTime)node["LastLoggedIn"].Value),
                node["Email"].Value,
                node["Phone"].Value,
                node["Roles"].Value);
            gravatar.Alt = "The facial expressions of the humanoid declared to own the OpenID of " + 
                node["Username"].Get<string>();
            gravatar.Src = string.Format(
                "http://www.gravatar.com/avatar/{0}?s=64&d=identicon",
                MD5Hash(node["Email"].Get<string>()));
        }

        public static string MD5Hash(string email)
        {
            if (string.IsNullOrEmpty(email))
                return string.Empty;

            StringBuilder emailHash = new StringBuilder();
            MD5 md5 = MD5.Create();
            byte[] emailBuffer = Encoding.ASCII.GetBytes(email);
            byte[] hash = md5.ComputeHash(emailBuffer);

            foreach (byte hashByte in hash)
                emailHash.Append(hashByte.ToString("x2"));

            return emailHash.ToString();
        }
    }
}