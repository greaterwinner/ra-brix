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
using ASP = System.Web.UI.WebControls;
using Ra.Brix.Types;
using Ra.Extensions.Widgets;
using System.Collections.Generic;
using Doxygen.NET;
using System.IO;
using Ra.Widgets;

namespace DoxygentDotNetViewDocsModules
{
    [ActiveModule]
    public class ViewTutorial : System.Web.UI.UserControl, IModule
    {
        protected global::System.Web.UI.HtmlControls.HtmlGenericControl cnt;
        protected global::System.Web.UI.HtmlControls.HtmlGenericControl header;

        public void InitialLoading(Node node)
        {
            cnt.InnerHtml = node["Text"].Get<string>();
            header.InnerHtml = node["Header"].Get<string>();
        }
    }
}