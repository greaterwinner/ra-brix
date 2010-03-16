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
using Ra.Selector;
using Ra.Effects;
using Ra;
using Ra.Brix.Types;
using SettingsRecords;
using UserSettingsRecords;
using Ra.Extensions.Widgets;

namespace Viewport
{
    [ActiveModule]
    public class Content : UserControl
    {
        protected global::Ra.Extensions.Widgets.Window popupWindow2;
        protected global::Ra.Widgets.Dynamic dynPopup2;
        protected global::Ra.Widgets.Image zoomImage;
        protected global::Ra.Widgets.Label informationLabel;
        protected global::Ra.Extensions.Widgets.Window informationPanel;
        protected global::Ra.Extensions.Widgets.ExtButton handleInformationEvt;
        protected global::Ra.Widgets.Panel customBreadParent;
        protected global::Ra.Extensions.Widgets.Window popupWindow;
        protected global::Ra.Widgets.Dynamic dynPopup;
        protected global::Ra.Widgets.Dynamic dynTop;
        protected global::Ra.Widgets.Dynamic dynLeft;
        protected global::Ra.Widgets.Dynamic dynMid;
        protected global::System.Web.UI.HtmlControls.HtmlGenericControl headerBackground;
        private Node _initializingParameter;

        protected void Page_Load(object sender, EventArgs e)
        {
            zoomImage.DataBind();
            handleInformationEvt.Text = Language.Instance["InfoHandleButton", null, "Handle..."];
            informationPanel.DataBind();
        }

        private LinkButton _pinButton;

        protected void popupWindow2_CreateTitleBarControls(object sender, Window.CreateTitleBarControlsEventArgs e)
        {
            LinkButton btn = new LinkButton();
            btn.ID = "pinWnd";
            btn.CssClass = "window_pin";
            btn.Text = "&nbsp;";
            btn.Click +=
                delegate(object sender2, EventArgs e2)
                {
                    LinkButton b = sender2 as LinkButton;
                    b.CssClass = b.CssClass == "window_pin" ? "window_pinned" : "window_pin";
                    if (b.CssClass == "window_pinned")
                        PinPopupWindow();
                };
            e.Caption.Controls.Add(btn);
            _pinButton = btn;
        }

        protected void timer_Tick(object sender, EventArgs e)
        {
            // Completely dummy, just to make sure users are kept logged in
            // to their sessions while browser is open...
        }

        protected string GetCssRootFolder()
        {
            string retVal = ApplicationRoot.Root + "media/skins/";
            string portalDefault = Settings.Instance.Get(
                "CssRootFolder",
                "Gold");
            if (string.IsNullOrEmpty(Users.LoggedInUserName))
            {
                retVal += portalDefault;
            }
            else
            {
                retVal += UserSettings.Instance.Get(
                    "CssRootFolder",
                    Users.LoggedInUserName,
                    portalDefault);
            }
            return retVal;
        }

        protected void popupWindow_Closed(object sender, EventArgs e)
        {
            dynPopup.ClearControls();
        }

        protected void zoomImage_MouseOver(object sender, EventArgs e)
        {
            PinPopupWindow();
        }

        private void PinPopupWindow()
        {
            if (popupWindow2.CssClass == "window")
                return;
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
            if (_pinButton.CssClass == "window_pinned")
                return;
            UnPinPopupWindow();
        }

        private void UnPinPopupWindow()
        {
            if (dynPopup2.Style["display"] != "none")
            {
                popupWindow2.CssClass = "window hideCaption";
                zoomImage.Style[Styles.display] = "none";
                zoomImage.Style[Styles.position] = "";
                dynPopup2.Style[Styles.zIndex] = "99";
                zoomImage.Style[Styles.zIndex] = "100";
                new EffectSize(popupWindow2, 200, -1, 90)
                    .ChainThese(
                        new EffectSize(dynPopup2, 200, 50, -1),
                        new EffectFadeOut(dynPopup2, 50),
                        new EffectFadeIn(zoomImage, 50))
                    .Render();
            }
        }

        protected void popupWindow2_Closed(object sender, EventArgs e)
        {
            dynPopup2.ClearControls();
        }

        [ActiveEvent(Name = "RefreshMenu")]
        protected void RefreshMenu(object sender, ActiveEventArgs e)
        {
            customBreadParent.ReRender();
        }

        [ActiveEvent(Name = "AnimateLeftWindowContainer")]
        protected void AnimateLeftWindowContainer(object sender, ActiveEventArgs e)
        {
            int height = e.Params["Height"].Get<int>();
            new EffectSize(dynLeft, 250, height, -1)
                .Render();
        }

        [ActiveEvent(Name = "LoadControl")]
        protected void LoadControl(object sender, ActiveEventArgs e)
        {
            Dynamic dyn;
            bool clearOldControls = true;
            if (e.Params["Parameters"]["AddToExistingCollection"].Value != null &&
                e.Params["Parameters"]["AddToExistingCollection"].Get<bool>() == true)
                clearOldControls = false;
            switch (e.Params["Position"].Value.ToString())
            {
                case "dynPopup":
                    dyn = dynPopup;
                    popupWindow.Visible = true;
                    popupWindow.Caption = e.Params["Parameters"]["TabCaption"].Get<string>();
                    if (!(e.Params["Parameters"]["NoResize"].Value != null && 
                        e.Params["Parameters"]["NoResize"].Get<bool>() == true))
                    {
                        popupWindow.Style[Styles.width] =
                            e.Params["Parameters"]["Width"].Get(350) + "px";
                        dynPopup.Style[Styles.height] =
                            e.Params["Parameters"]["Height"].Get(150) + "px";
                    }
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
                    popupWindow2.Style[Styles.width] = "90px";
                    dynPopup2.Style[Styles.height] = "50px";
                    dynPopup2.Style[Styles.display] = "none";
                    zoomImage.Style[Styles.display] = "none";
                    new EffectFadeIn(zoomImage, 200)
                        .Render();
                    zoomImage.Style[Styles.height] = "50px";
                    zoomImage.Style[Styles.position] = "";
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
            if (clearOldControls)
                ClearControls(dyn);

            _initializingParameter = e.Params["Parameters"]["ModuleSettings"];
            if (clearOldControls)
                dyn.LoadControls(e.Params["Name"].Value.ToString());
            else
                dyn.AppendControl(e.Params["Name"].Value.ToString());
        }

        protected void dynamic_LoadControls(object sender, Dynamic.ReloadEventArgs e)
        {
            Dynamic dynamic = sender as Dynamic;
            if (dynamic == null)
                return;
            string key = e.Key;
            if (key.Contains("|"))
            {
                if (e.FirstReload)
                {
                    string[] keys = key.Split('|');
                    key = keys[keys.Length - 1];
                    LoadOneControl(e, dynamic, key);
                }
                else
                {
                    // Need to loop through all keys...
                    string[] keys = key.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
                    foreach (string idxKey in keys)
                    {
                        LoadOneControl(e, dynamic, idxKey);
                    }
                }
            }
            else
            {
                LoadOneControl(e, dynamic, key);
            }
        }

        private void LoadOneControl(Dynamic.ReloadEventArgs e, Dynamic dynamic, string key)
        {
            Control ctrl = PluginLoader.Instance.LoadControl(key);
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
            informationPanel.Visible = true;
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
                    AjaxManager.Instance.WriterAtBack.Write("Ra.$('ctl03_informationPanel_pingSound').play();");
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

        protected string GetMostWantedResponseUrl()
        {
            return Settings.Instance["MostWantedResponseUrl"];
        }

        protected string GetMostWantedResponseTooltip()
        {
            return Settings.Instance["MostWantedResponseTooltip"];
        }
    }
}



