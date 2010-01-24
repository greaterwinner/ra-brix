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
using Ra.Effects;

namespace ArticlePublisherModules
{
    [ActiveModule]
    public class Favorites : System.Web.UI.UserControl, IModule
    {
        protected global::Ra.Extensions.Widgets.Window bookmarksPanel;
        protected global::Ra.Widgets.Panel wndWrp;
        protected global::Components.Grid grd;

        protected void bookmarks_Click(object sender, EventArgs e)
        {
            wndWrp.Visible = true;
            wndWrp.Style[Styles.display] = "none";
            new EffectFadeIn(wndWrp, 500)
                .Render();
            Node node = new Node();
            ActiveEvents.Instance.RaiseActiveEvent(
                this,
                "GetArticleBookmarks",
                node);
            grd.DataSource = node["Grid"];
            grd.Rebind();
        }

        protected void wndWrp_MouseOut(object sender, EventArgs e)
        {
            new EffectFadeOut(wndWrp, 500)
                .Render();
        }

        void IModule.InitialLoading(Node node)
        {
        }

        string IModule.GetCaption()
        {
            return "";
        }
    }
}