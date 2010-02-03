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
using LanguageRecords;
using TipTheAuthorsRecords;
using HelperGlobals;
using UserRecords;
using Ra.Brix.Data;
using Ra.Brix.Types;

namespace TipsTheAuthorsController
{
    [ActiveController]
    public class TipsTheAuthorsController
    {
        [ActiveEvent(Name = "ApplicationStartup")]
        protected static void ApplicationStartup(object sender, ActiveEventArgs e)
        {
            Language.Instance.SetDefaultValue("ButtonTipsTheAuthors", "Submit Tip");
            Language.Instance.SetDefaultValue("ButtonViewArticleTips", "View Tips");
        }

        [ActiveEvent(Name = "GetMenuItems")]
        protected void GetMenuItems(object sender, ActiveEventArgs e)
        {
            e.Params["ButtonTipsTheAuthors"].Value = "Menu-TipTheAuthors";
            e.Params["ButtonViewArticleTips"].Value = "Menu-ViewArticleTips";
        }

        [ActiveEvent(Name = "Menu-TipTheAuthors")]
        protected void TipTheAuthors(object sender, ActiveEventArgs e)
        {
            ActiveEvents.Instance.RaiseLoadControl(
                "TipTheAuthorsModules.TipAuthors",
                "dynMid");
        }

        [ActiveEvent(Name = "Menu-ViewArticleTips")]
        protected void ViewArticleTips(object sender, ActiveEventArgs e)
        {
            Node node = new Node();
            int idxNo = 0;
            foreach (ArticleTip idx in ArticleTip.Select(Criteria.Sort("TipDate", false)))
            {
                node["ModuleSettings"]["Tips"]["Tip" + idxNo]["URL"].Value = idx.URL;
                node["ModuleSettings"]["Tips"]["Tip" + idxNo]["Date"].Value = DateFormatter.FormatDate(idx.TipDate);
                node["ModuleSettings"]["Tips"]["Tip" + idxNo]["User"].Value = idx.SubmittedBy == null ? "Anonymous Coward" : idx.SubmittedBy.Username;
                idxNo += 1;
                if (idxNo > 100)
                    break;
            }
            ActiveEvents.Instance.RaiseLoadControl(
                "TipTheAuthorsModules.ViewTips",
                "dynMid",
                node);
        }

        [ActiveEvent(Name = "NewArticleTipSubmitted")]
        protected void NewArticleTipSubmitted(object sender, ActiveEventArgs e)
        {
            string url = e.Params["URL"].Get<string>();
            if (url.IndexOf("http") != 0)
                url = "http://" + url;
            ArticleTip t = new ArticleTip();
            t.URL = url;
            if (!string.IsNullOrEmpty(Users.LoggedInUserName))
            {
                t.SubmittedBy = User.SelectFirst(Criteria.Eq("Username", Users.LoggedInUserName));
            }
            t.Save();
        }
    }
}
