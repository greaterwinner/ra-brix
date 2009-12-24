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
using HelperGlobals;
using LanguageRecords;
using Ra.Widgets;
using System.Web.UI;
using Ra.Brix.Loader;
using Ra.Extensions.Widgets;
using Ra.Selector;
using Ra.Effects;
using System.Collections.Generic;
using Ra;
using Ra.Brix.Types;
using SettingsRecords;
using UserSettingsRecords;

namespace Viewport
{
    [ActiveModule]
    public class Main : UserControl
    {
        protected global::Ra.Widgets.Label informationLabel;
        protected global::Ra.Widgets.Panel informationPanel;
        protected global::Ra.Extensions.Widgets.ExtButton handleInformationEvt;
        protected global::Ra.Widgets.Panel pnlTop;
        protected global::Ra.Widgets.Panel customBreadParent;
        protected global::Ra.Extensions.Widgets.Window popupWindow;
        protected global::Ra.Extensions.Widgets.Window popupWindow2;
        protected global::Ra.Extensions.Widgets.Window wndLeft;
        protected global::Ra.Extensions.Widgets.Window wndMid;
        protected global::Ra.Extensions.Widgets.Window maxiWindow;
        protected global::Ra.Widgets.Dynamic dynPopup;
        protected global::Ra.Widgets.Dynamic dynPopup2;
        protected global::Ra.Widgets.Image zoomImage;
        protected global::Ra.Widgets.Dynamic dynTop;
        protected global::Ra.Widgets.Dynamic dynLeft;
        protected global::Ra.Widgets.Dynamic dynMid;
        protected global::Ra.Widgets.Dynamic dynMaxi;
        protected global::Ra.Widgets.Panel midWrp;
        protected global::Ra.Extensions.Widgets.TabControl tab;
        protected global::Ra.Widgets.LinkButton close;
        private Node _initializingParameter;

        protected void Page_Load(object sender, EventArgs e)
        {
            zoomImage.DataBind();
            close.DataBind();
            handleInformationEvt.Text = Language.Instance["InfoHandleButton", null, "Handle..."];
        }

        protected string GetCssRootFolder()
        {
            string portalDefault = Settings.Instance.Get(
                "CssRootFolder",
                "Gold");
            if (string.IsNullOrEmpty(Users.LoggedInUserName))
            {
                return portalDefault;
            }
            return UserSettings.Instance.Get(
                "CssRootFolder",
                Users.LoggedInUserName,
                portalDefault);
        }

        protected void close_Click(object sender, EventArgs e)
        {
            ActiveEvents.Instance.RaiseClearControls("dynMaxi");
        }

        [ActiveEvent(Name = "RefreshMenu")]
        protected void RefreshMenu(object sender, ActiveEventArgs e)
        {
            customBreadParent.ReRender();
        }

        protected void popupWindow_Closed(object sender, EventArgs e)
        {
            dynPopup.ClearControls();
        }

        protected void popupWindow2_Closed(object sender, EventArgs e)
        {
            dynPopup2.ClearControls();
        }

        protected void maxiWindow_Closed(object sender, EventArgs e)
        {
            dynMaxi.ClearControls();
        }

        [ActiveEvent("GetHelpContents")]
        protected static void GetHelpContents(object sender, ActiveEventArgs e)
        {
            e.Params[Language.Instance["ViewportHelpLabel", null, "About the Viewport"]].Value = "Help-AboutTheViewport";
        }

        [ActiveEvent("Help-AboutTheViewport")]
        protected static void Help_AboutTheViewport(object sender, ActiveEventArgs e)
        {
            const string aboutTheViewportDefault = @"
<p>
The Viewport is the container for your modules and the ""skin"" of your portal. The viewport has
several services it provides to your applications, like for instance that it hosts your modules. 
In addition it gives support for hosting multiple modules at the same time. The menu to the left
for instance is a module, and the thing you're reading right now is another module (the ""Help"" module)
</p>
<p>
Another core feature of the Viewport is that it provides the notifcation services in addition to that it
also provides support for hosting multiple tabs, each with its own set of modules hosted within.
</p>
<p>
Notice that the ""ping sound"", which is a service provided by the Viewport can be turned off by 
changing the setting called; ""UseNotificationSounds"".
</p>
";
            e.Params["Text"].Value = Language.Instance["ViewPortHelp", null, aboutTheViewportDefault];
        }

        [ActiveEvent(Name = "LoadControl")]
        protected void LoadControl(object sender, ActiveEventArgs e)
        {
            Dynamic dyn;
            switch (e.Params["Position"].Value.ToString())
            {
                case "dynPopup":
                    dyn = dynPopup;
                    popupWindow.Visible = true;
                    popupWindow.Caption = e.Params["Parameters"]["TabCaption"].Get<string>();
                    popupWindow.Style[Styles.width] =
                        e.Params["Parameters"]["Width"].Get(350) + "px";
                    dynPopup.Style[Styles.height] =
                        e.Params["Parameters"]["Height"].Get(150) + "px";
                    break;
                case "dynPopup2":
                    dyn = dynPopup2;
                    popupWindow2.Visible = true;
                    popupWindow2.CssClass = "window hideCaption";
                    popupWindow2.Style[Styles.display] = "none";
                    new EffectFadeIn(popupWindow2, 200)
                        .Render();
                    popupWindow2.Caption = e.Params["Parameters"]["TabCaption"].Get<string>();
                    popupWindow2.Xtra =
                        e.Params["Parameters"]["Width"].Get(350) +
                        "x" +
                        e.Params["Parameters"]["Height"].Get(250);
                    popupWindow2.Style[Styles.width] = "66px";
                    dynPopup2.Style[Styles.height] = "50px";
                    dynPopup2.Style[Styles.display] = "none";
                    zoomImage.Style[Styles.display] = "none";
                    new EffectFadeIn(zoomImage, 200).Render();
                    zoomImage.Style[Styles.height] = "50px";
                    zoomImage.Style[Styles.position] = "";
                    break;
                case "dynMaxi":
                    dyn = dynMaxi;
                    maxiWindow.Visible = true;
                    maxiWindow.Style[Styles.display] = "none";
                    new EffectFadeIn(maxiWindow, 200)
                        .Render();
                    maxiWindow.Caption = e.Params["Parameters"]["TabCaption"].Get<string>();
                    break;
                case "dynMid":
                    if (e.Params["Parameters"]["Index"].Value == null)
                    {
                        dyn = Selector.SelectFirst<Dynamic>(tab.ActiveTabView);
                        if (e.Params["Parameters"]["TabCaption"].Value != null)
                        {
                            string caption = e.Params["Parameters"]["TabCaption"].Value.ToString();
                            tab.ActiveTabView.Caption = caption;
                        }
                    }
                    else
                    {
                        string caption = e.Params["Parameters"]["TabCaption"].Value.ToString();
                        TabView view = new List<TabView>(tab.Views)[(int)e.Params["Parameters"]["Index"].Value];
                        view.Visible = true;
                        view.Caption = caption;
                        if (true.Equals(e.Params["Parameters"]["Active"].Value))
                        {
                            tab.SetActiveTabViewIndex((int)e.Params["Parameters"]["Index"].Value);
                        }
                        dyn = Selector.SelectFirst<Dynamic>(view);
                    }
                    dyn.Style[Styles.opacity] = "0.1";
                    new EffectFadeIn(dyn, 200)
                        .Render();
                    break;
                default:
                    dyn = Selector.FindControl<Dynamic>(this, e.Params["Position"].Value.ToString());
                    break;
            }
            ClearControls(dyn);
            _initializingParameter = e.Params["Parameters"]["ModuleSettings"];
            dyn.LoadControls(e.Params["Name"].Value.ToString());
        }

        protected void handleInformationEvt_Click(object sender, EventArgs e)
        {
            RaWebControl ctrl = sender as RaWebControl;
            if (ctrl == null)
            {
                return;
            }
            string evtToFire = ctrl.Xtra;
            ActiveEvents.Instance.RaiseActiveEvent(this, evtToFire);
        }

        protected void zoomImage_MouseOver(object sender, EventArgs e)
        {
            popupWindow2.CssClass = "window";
            dynPopup2.Style[Styles.display] = "none";
            int width = int.Parse(popupWindow2.Xtra.Split('x')[0]);
            int height = int.Parse(popupWindow2.Xtra.Split('x')[1]);
            dynPopup2.Style[Styles.zIndex] = "100";
            zoomImage.Style[Styles.zIndex] = "99";
            dynPopup2.Style[Styles.height] = "50px";
            zoomImage.Style[Styles.position] = "absolute";
            zoomImage.Style[Styles.top] = "0px";
            zoomImage.Style[Styles.left] = "0px";
            new EffectFadeIn(dynPopup2, 50)
                .ChainThese(
                    new EffectFadeOut(zoomImage, 200),
                    new EffectSize(popupWindow2, 200, -1, width),
                    new EffectSize(dynPopup2, 200, height, -1))
                .Render();
        }

        protected void popupWindow2_MouseOut(object sender, EventArgs e)
        {
            popupWindow2.CssClass = "window hideCaption";
            zoomImage.Style[Styles.display] = "none";
            zoomImage.Style[Styles.position] = "";
            dynPopup2.Style[Styles.zIndex] = "99";
            zoomImage.Style[Styles.zIndex] = "100";
            new EffectSize(popupWindow2, 200, -1, 66)
                .ChainThese(
                    new EffectSize(dynPopup2, 200, 50, -1),
                    new EffectFadeOut(dynPopup2, 50),
                    new EffectFadeIn(zoomImage, 50))
                .Render();
        }

        [ActiveEvent(Name = "ShowInformationMessage")]
        protected void ShowInformationMessage(object sender, ActiveEventArgs e)
        {
            informationLabel.Text = e.Params["Message"].Value.ToString();
            int duration = e.Params["Duration"].Get<int>();
            informationPanel.Style[Styles.display] = "none";
            string evtToFire = e.Params["EventToFire"].Value as string;
            if (string.IsNullOrEmpty(evtToFire))
            {
                handleInformationEvt.Visible = false;
            }
            else
            {
                handleInformationEvt.Visible = true;
                handleInformationEvt.Xtra = evtToFire;
            }
            new EffectFadeIn(informationPanel, 500)
                .JoinThese(new EffectRollDown())
                .ChainThese(
                    new EffectTimeout(duration),
                    new EffectFadeOut(informationPanel, 500)
                        .JoinThese(
                            new EffectRollUp()))
                .Render();
            if (Settings.Instance.Get("UseNotificationSounds", true))
            {
                if (Request.Browser.Browser == "Firefox")
                    AjaxManager.Instance.WriterAtBack.Write("Ra.$('pingSound').play();");
            }
        }

        [ActiveEvent(Name = "ClearControls")]
        protected void ClearControls(object sender, ActiveEventArgs e)
        {
            Dynamic dyn;
            switch (e.Params["Position"].Value.ToString())
            {
                case "dynMid":
                    dyn = Selector.SelectFirst<Dynamic>(tab.ActiveTabView);
                    break;
                case "dynPopup":
                    dyn = dynPopup;
                    popupWindow.Visible = false;
                    break;
                case "dynPopup2":
                    dyn = dynPopup2;
                    popupWindow2.Visible = false;
                    break;
                case "dynMaxi":
                    dyn = dynMaxi;
                    maxiWindow.Visible = false;
                    break;
                default:
                    dyn = Selector.FindControl<Dynamic>(this, e.Params["Position"].Value.ToString());
                    break;
            }
            ClearControls(dyn);
        }

        [ActiveEvent(Name = "UnloadAllApplications")]
        protected void UnloadAllApplications(object sender, ActiveEventArgs e)
        {
            foreach (TabView idx in tab.Views)
            {
                ClearControls(Selector.SelectFirst<Dynamic>(idx));
                idx.Caption = "";
            }
        }

        private static void ClearControls(Dynamic dyn)
        {
            if (dyn.Controls.Count > 0)
            {
                Control tmp = dyn.Controls[0];
                ActiveEvents.Instance.RemoveListener(tmp);
                dyn.ClearControls();
            }
        }

        protected void timer_Tick(object sender, EventArgs e)
        {
            // This is just a dummy timer to make sure we periodically check the server for
            // new stuff that might have come in since last refresh...
        }

        protected void res_Resized(object sender, ResizeHandler.ResizedEventArgs e)
        {
            // Calculating width/height - "extras"...
            int width = Math.Max(e.Width - 20, 500);
            int height = Math.Max(e.Height - 20, 400);

            // Creating effects to animate window...
            new EffectSize(dynLeft, 200, height - 155, -1)
                .ChainThese(
                    new EffectSize(midWrp, 200, height - 155, -1),
                    new EffectSize(midWrp, 200, -1, width - 235),
                    new EffectSize(tab.ActiveTabView, 200, height - 180, -1)
                    )
                .Render();
            foreach (TabView idx in tab.Views)
            {
                if (idx.ID != tab.ActiveTabView.ID)
                {
                    idx.Style[Styles.height] = (height - 180) + "px";
                }
            }
            maxiWindow.Style[Styles.width] = (e.Width - 10) + "px";
            dynMaxi.Style[Styles.height] = (e.Height - 45) + "px";
            ActiveEvents.Instance.RaiseActiveEvent(
                this,
                "MainViewportResized");
        }

        protected void dynamic_LoadControls(object sender, Dynamic.ReloadEventArgs e)
        {
            Dynamic dynamic = sender as Dynamic;
            if (dynamic == null)
                return;
            Control ctrl = PluginLoader.Instance.LoadControl(e.Key);
            if (e.FirstReload)
            {
                ctrl.Init +=
                    delegate
                        {
                            IModule module = ctrl as IModule;
                            if (module != null)
                            {
                                module.InitialLoading(_initializingParameter);
                            }
                        };
            }
            dynamic.Controls.Add(ctrl);
        }
    }
}



