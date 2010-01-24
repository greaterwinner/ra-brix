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
using System;
using Ra.Extensions.Widgets;
using Ra.Widgets;

namespace ArticlePublisherModules
{
    [ActiveModule]
    public class ShowTags : System.Web.UI.UserControl, IModule
    {
        protected global::System.Web.UI.WebControls.Repeater rep;
        protected global::Ra.Widgets.Panel pnl;

        public void InitialLoading(Node node)
        {
            Page.Load +=
                delegate
                {
                    if (node["AlignBottomRight"].Value != null && node["AlignBottomRight"].Get<bool>())
                    {
                        pnl.Style[Styles.position] = "absolute";
                        pnl.Style[Styles.right] = "45px";
                        pnl.Style[Styles.bottom] = "0";
                    }
                    else if (node["CenterAlign"].Value != null && node["CenterAlign"].Get<bool>())
                    {
                        pnl.Style[Styles.marginLeft] = "auto";
                        pnl.Style[Styles.marginRight] = "auto";
                        pnl.Style[Styles.width] = "464px";
                    }
                };
            rep.DataSource = node["Tags"];
            rep.DataBind();
        }

        public string GetCaption()
        {
            return "";
        }
    }
}