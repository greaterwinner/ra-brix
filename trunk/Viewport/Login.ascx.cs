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
using Ra.Widgets;
using System.Web.UI;
using Ra.Brix.Loader;
using Ra.Extensions.Widgets;
using Ra.Selector;
using Ra.Effects;

namespace Viewport
{
    [ActiveModule]
    public class Login : UserControl
    {
        protected global::Ra.Extensions.Widgets.Window wndTop;
        protected global::Ra.Extensions.Widgets.Window wndMid;
        protected global::Ra.Extensions.Widgets.Window wndRight;
        protected global::Ra.Widgets.Panel pnlLeft;
        protected global::Ra.Widgets.Panel pnlMid;
        protected global::Ra.Widgets.Panel pnlRight;

        [ActiveEvent(Name = "LoadControl")]
        protected void LoadControl(object sender, ActiveEventArgs e)
        {
            Dynamic dyn = Selector.FindControl<Dynamic>(this, e.Params["Position"].Value.ToString());
            ClearControls(dyn);
            dyn.LoadControls(e.Params["Name"].Value.ToString());
            IModule module = dyn.Controls[0] as IModule;
            if (module != null)
            {
                module.InitialLoading(e.Params["Parameters"]);
            }
            /*dyn.Style[Styles.opacity] = "0";
            new EffectFadeIn(dyn, 400)
                .Render();*/
        }

        [ActiveEvent(Name = "ClearControls")]
        protected void ClearControls(object sender, ActiveEventArgs e)
        {
            Dynamic dyn = Selector.FindControl<Dynamic>(this, e.Params["Position"].Get<string>());
            ClearControls(dyn);
        }

        private static void ClearControls(Dynamic dyn)
        {
            if (dyn.Controls.Count <= 0)
                return;
            Control tmp = dyn.Controls[0];
            ActiveEvents.Instance.RemoveListener(tmp);
            dyn.ClearControls();
        }

        protected void res_Resized(object sender, ResizeHandler.ResizedEventArgs e)
        {
            // Calculating width/height - "extras"...
            int width = Math.Max(e.Width - 20, 500);
            int height = Math.Max(e.Height - 20, 400);

            // Creating effects to animate window...
            new EffectSize(wndTop, 200, -1, width)
                .ChainThese(
                    new EffectSize(pnlLeft, 200, height - 150, -1),
                    new EffectSize(pnlMid, 200, height - 150, -1),
                    new EffectSize(pnlRight, 200, height - 150, -1),
                    new EffectMove(wndRight, 200, width - 210, int.Parse(wndRight.Style[Styles.top].Replace("px", ""))),
                    new EffectSize(wndMid, 200, -1, width - 430)
                    )
                .Render();

            // Setting top window size
            wndTop.Style["width"] = string.Format("{0}px", width);

            // Setting left window size
            pnlLeft.Style["height"] = string.Format("{0}px", height - 150);

            // Setting center window size
            pnlMid.Style["height"] = string.Format("{0}px", height - 150);
            wndMid.Style["width"] = string.Format("{0}px", width - 430 /* 215x2 width of left and right combined */);

            // Setting right window position
            pnlRight.Style["height"] = string.Format("{0}px", height - 150);
            wndRight.Style["left"] = string.Format("{0}px", width - 210);
        }

        protected void dynamic_LoadControls(object sender, Dynamic.ReloadEventArgs e)
        {
            Dynamic dynamic = sender as Dynamic;
            if (dynamic == null)
                return;
            Control ctrl = PluginLoader.Instance.LoadControl(e.Key);
            dynamic.Controls.Add(ctrl);
        }
    }
}



