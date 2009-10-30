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
using Ra.Brix.Types;
using Ra.Widgets;
using Ra.Effects;
using Ra.Selector;

namespace RssReaderModules
{
    [ActiveModule]
    public class ViewRSSItems : System.Web.UI.UserControl, IModule
    {
        protected global::System.Web.UI.WebControls.Repeater rep;

        protected void ExpandRSS(object sender, EventArgs e)
        {
            LinkButton btn = sender as LinkButton;
            if (btn == null)
            {
                return;
            }
            Panel toAnimate = Selector.SelectFirst<Panel>(btn.Parent);
            if (toAnimate.Style[Styles.display] == "none")
            {
                new EffectRollDown(toAnimate, 400)
                    .Render();
            }
            else
            {
                new EffectRollUp(toAnimate, 400)
                    .Render();
            }
        }

        public void InitialLoading(Node node)
        {
            rep.DataSource = node["Items"];
            rep.DataBind();
        }

        public string GetCaption()
        {
            return Language.Instance["ConfigureRssModule", null, "News form the world..."];
        }
    }
}




