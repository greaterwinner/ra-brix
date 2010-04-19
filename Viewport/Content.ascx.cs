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
        protected global::Ra.Widgets.Label informationLabel;
        protected global::Ra.Extensions.Widgets.Window informationPanel;
        protected global::Ra.Extensions.Widgets.ExtButton handleInformationEvt;
        protected global::Ra.Widgets.Panel customBreadParent;
        protected global::Ra.Extensions.Widgets.Window popupWindow;
        protected global::Ra.Widgets.Dynamic dynPopup;
        protected global::Ra.Widgets.Dynamic dynTop;
        protected global::Ra.Widgets.Dynamic dynLeft;
        protected global::Ra.Widgets.Dynamic dynMid;
        protected global::Ra.Widgets.Dynamic dynFooter;
        protected global::System.Web.UI.HtmlControls.HtmlGenericControl headerBackground;
        protected global::System.Web.UI.HtmlControls.HtmlGenericControl pingSound;
        private Node _initializingParameter;

        protected void Page_Load(object sender, EventArgs e)
        {
            handleInformationEvt.Text = Language.Instance["InfoHandleButton", null, "Handle..."];
            informationPanel.DataBind();
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
                    popupWindow2.Style[Styles.display] = "none";
                    new EffectFadeIn(popupWindow2, 200)
                        .Render();
                    popupWindow2.Caption = e.Params["Parameters"]["TabCaption"].Get<string>();
                    popupWindow2.Style[Styles.width] = e.Params["Parameters"]["Width"].Get(350).ToString() + "px";
                    dynPopup2.Style[Styles.height] = e.Params["Parameters"]["Height"].Get(250).ToString() + "px";
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
                    AjaxManager.Instance.WriterAtBack.Write("Ra.$('" + pingSound.ClientID + "').play();");
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
            return Settings.Instance.Get<string>("MostWantedResponseUrl", "http://ra-brix.org");
        }

        protected string GetMostWantedResponseTooltip()
        {
            return Settings.Instance.Get<string>("MostWantedResponseTooltip", "Download Ra-Brix");
        }
    }
}



