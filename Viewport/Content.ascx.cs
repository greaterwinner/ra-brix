﻿/*
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
using Ra.Selector;
using Ra.Effects;
using Ra;
using Ra.Brix.Types;
using SettingsRecords;
using UserSettingsRecords;

namespace Viewport
{
    [ActiveModule]
    public class Content : UserControl
    {
        protected global::Ra.Widgets.Label informationLabel;
        protected global::Ra.Widgets.Panel informationPanel;
        protected global::Ra.Extensions.Widgets.ExtButton handleInformationEvt;
        protected global::Ra.Widgets.Panel customBreadParent;
        protected global::Ra.Extensions.Widgets.Window popupWindow;
        protected global::Ra.Widgets.Dynamic dynPopup;
        protected global::Ra.Widgets.Dynamic dynTop;
        protected global::Ra.Widgets.Dynamic dynLeft;
        protected global::Ra.Widgets.Dynamic dynMid;
        private Node _initializingParameter;

        protected void Page_Load(object sender, EventArgs e)
        {
            handleInformationEvt.Text = Language.Instance["InfoHandleButton", null, "Handle..."];
            informationPanel.DataBind();
        }

        protected string GetCssRootFolder()
        {
            string portalDefault = Settings.Instance.Get(
                "CssRootFolder",
                "Light");
            if (string.IsNullOrEmpty(Users.LoggedInUserName))
            {
                return portalDefault;
            }
            return UserSettings.Instance.Get(
                "CssRootFolder",
                Users.LoggedInUserName,
                portalDefault);
        }

        protected void popupWindow_Closed(object sender, EventArgs e)
        {
            dynPopup.ClearControls();
        }

        [ActiveEvent(Name = "RefreshMenu")]
        protected void RefreshMenu(object sender, ActiveEventArgs e)
        {
            customBreadParent.ReRender();
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
                default:
                    dyn = Selector.FindControl<Dynamic>(this, e.Params["Position"].Value.ToString());
                    break;
            }
            if (dyn == null)
            {
                // Defaulting to Center piece of screen...
                dyn = dynMid;
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
                case "dynPopup":
                    dyn = dynPopup;
                    popupWindow.Visible = false;
                    break;
                default:
                    dyn = Selector.FindControl<Dynamic>(this, e.Params["Position"].Value.ToString());
                    break;
            }
            ClearControls(dyn);
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


