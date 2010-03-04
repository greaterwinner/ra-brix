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
using System.Configuration;

namespace ArticlePublisherModules
{
    [ActiveModule]
    public class Status : System.Web.UI.UserControl, IModule
    {
        protected global::Ra.Widgets.Label status;
        protected global::System.Web.UI.HtmlControls.HtmlAnchor linkToUsers;
        protected global::Ra.Widgets.Label status2;

        public void InitialLoading(Node node)
        {
            status.Text = string.Format(Language.Instance["StatusInfoText", null, @"
There are {0} articles, {1} comments and {2} "],
                  node["ArticleCount"].Value,
                  node["CommentCount"].Value,
                  node["UserCount"].Value);
            linkToUsers.InnerHtml = Language.Instance["RegisteredUsers", null, "registered users"];
            linkToUsers.HRef = ApplicationRoot.Root + "authors/all" + ConfigurationManager.AppSettings["DefaultPageExtension"];
            status2.Text = Language.Instance["StatusInfoText2", null, " around here"];
        }
    }
}