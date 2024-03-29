﻿/*
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
        protected global::Ra.Widgets.Label biography;

        public void InitialLoading(Node node)
        {
            header.InnerHtml = node["Username"].Get<string>() + 
                " - " +
                GetRank(node["Score"].Get<int>(), node["IsAdmin"].Get<bool>());
            summaryView.InnerHtml = string.Format(Language.Instance["UserInformationText", null, @"
User has posted {0} articles and {1} comments. And was last seen {2}. User's email addresse is <a href=""mailto:{3}"">{3}</a> and if you want 
to check out if he passes the Turing Test you can call him at {4}. User has {6} points. Oh yeah, and user belongs to these role(s); {5}...
"],
                node["ArticleCount"].Value,
                node["CommentCount"].Value,
                DateFormatter.FormatDate((DateTime)node["LastLoggedIn"].Value),
                node["Email"].Value,
                node["Phone"].Value,
                node["Roles"].Value,
                node["Score"].Value);
            gravatar.Alt = "The facial expressions of the humanoid declared to own the OpenID of " + 
                node["Username"].Get<string>();
            gravatar.Src = string.Format(
                "http://www.gravatar.com/avatar/{0}?s=80&d=identicon",
                MD5Hash(node["Email"].Get<string>()));
            biography.Text = node["Biography"].Get<string>();
        }

        private string GetRank(int score, bool isAdmin)
        {
            string retVal = "";
            if (score < 25)
                retVal = "Virgin";
            else if (score < 50)
                retVal = "Noob";
            else if (score < 100)
                retVal = "Mediocre";
            else if (score < 200)
                retVal = "Student";
            else if (score < 400)
                retVal = "Graduate";
            else if (score < 800)
                retVal = "Captain";
            else if (score < 1600)
                retVal = "Master";
            else if (score < 3200)
                retVal = "Artist";
            else if (score < 6400)
                retVal = "Guru";
            else if (score < 12400)
                retVal = "Prophet";
            else
                retVal = "Living God...!";
            if (isAdmin)
                retVal += " [Admin]";
            return retVal;
        }

        private static string MD5Hash(string email)
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