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
using Ra.Effects;
using UserSettingsRecords;

namespace TipOfTodayModules
{
    [ActiveModule]
    public class TipOfToday : System.Web.UI.UserControl, IModule
    {
        protected global::Ra.Widgets.Panel pnl;

        private string _username;

        public void InitialLoading(Node node)
        {
            _username = node["Username"].Get<string>();
            new EffectTimeout(1000)
                .ChainThese(
                    new EffectRollDown(pnl, 500)
                        .JoinThese(new EffectFadeIn()))
                .Render();
        }

        protected string GetTipOfToday()
        {
            Node node = new Node();
            ActiveEvents.Instance.RaiseActiveEvent(
                this,
                "GetTipOfToday", 
                node);
            string current = UserSettings.Instance["CurrentTipOfToday", _username];
            if (string.IsNullOrEmpty(current))
            {
                UserSettings.Instance["CurrentTipOfToday", _username] = "0";
                current = "0";
            }
            int curInt = int.Parse(current);
            if (curInt >= node["Tip"].Count)
            {
                // Resettings back to zero since we've made our way completely over all tips of today...
                curInt = 0;
                UserSettings.Instance["CurrentTipOfToday", _username] = "0";
            }
            else
            {
                // Incrementing our "daily tip counter"...
                UserSettings.Instance["CurrentTipOfToday", _username] = (curInt + 1).ToString();
            }
            return node["Tip"][curInt].Get<string>();
        }

        [ActiveEvent(Name = "GetTipOfToday")]
        protected static void GetTipOfToday(object sender, ActiveEventArgs e)
        {
            const string tmp = @"
Every time you start the portal you will be presented with a 'tip of today' which will give you useful
hints about features and tings you can do within the portal.

These tips will be presented to you every time you log in. Most modules will provide their own
tips which means that thse tips will vary according to which modules you've installed and such.";
            e.Params["Tip"]["TipOfTipOfToday"].Value = Language.Instance["TipOfTodayFromTipOfToday", null, tmp];
        }
    }
}




