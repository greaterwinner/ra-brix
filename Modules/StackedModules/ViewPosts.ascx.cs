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

        protected void ask_Click(object sender, EventArgs e)
        {
            ActiveEvents.Instance.RaiseActiveEvent(
                this,
                "AskStackedQuestion");
        }
    }
}




