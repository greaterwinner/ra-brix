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
using Ra.Brix.Loader;
using LanguageRecords;
using System.Web;
using Ra.Brix.Types;
using StackedRecords;
using Ra.Brix.Data;

namespace StackedController
{
    [ActiveController]
    public class StackedController
    {
        [ActiveEvent(Name = "ApplicationStartup")]
        protected static void ApplicationStartup(object sender, ActiveEventArgs e)
        {
            Language.Instance.SetDefaultValue("ButtonStacked", "Forums");
        }

        [ActiveEvent(Name = "GetMenuItems")]
        protected static void GetMenuItems(object sender, ActiveEventArgs e)
        {
            e.Params["ButtonStacked"].Value = "url:~/stacked";
        }

        [ActiveEvent(Name = "Page_Init_InitialLoading")]
        protected void InitialLoadingOfPage(object sender, ActiveEventArgs e)
        {
            string contentId = HttpContext.Current.Request.Params["ContentID"];
            if (contentId != null)
            {
                contentId = contentId.Trim('/');
                if (contentId.Trim('/') == "stacked")
                {
                    ShowQuestionsLandingPage();
                }
                else if (contentId.Contains("stacked/"))
                {
                    ShowQuestion(contentId);
                }
            }
        }

        private void ShowQuestion(string contentId)
        {
            Question q = Question.FindArticle(contentId);
            Node node = new Node();
            node["ModuleSettings"]["Header"].Value = q.Header;
            node["ModuleSettings"]["Body"].Value = q.Body;
            ActiveEvents.Instance.RaiseLoadControl(
                "StackedModules.ViewQuestion",
                "dynMid",
                node);
        }

        private static void ShowQuestionsLandingPage()
        {
            // Viewing all questions...
            Node node = new Node();
            foreach (Question idx in Question.Select(Criteria.Sort("Asked", false)))
            {
                node["ModuleSettings"]["Questions"]["Q" + idx.ID]["Header"].Value = idx.Header;
                node["ModuleSettings"]["Questions"]["Q" + idx.ID]["Body"].Value = idx.Body;
                node["ModuleSettings"]["Questions"]["Q" + idx.ID]["URL"].Value = idx.URL;
                node["ModuleSettings"]["Questions"]["Q" + idx.ID]["User"].Value = idx.Author;
                node["ModuleSettings"]["Questions"]["Q" + idx.ID]["Asked"].Value = idx.Asked;
                if (node.Count >= 50)
                    break;
            }
            ActiveEvents.Instance.RaiseLoadControl(
                "StackedModules.ViewPosts",
                "dynMid",
                node);
        }

        [ActiveEvent(Name = "AskStackedQuestion")]
        protected void AskStackedQuestion(object sender, ActiveEventArgs e)
        {
            ActiveEvents.Instance.RaiseLoadControl(
                "StackedModules.AskQuestion",
                "dynMid");
        }

        [ActiveEvent(Name = "RequestSavingOfStackedQuestion")]
        protected void RequestSavingOfStackedQuestion(object sender, ActiveEventArgs e)
        {
            Question q = new Question();
            q.Body = e.Params["Body"].Get<string>();
            q.Header = e.Params["Header"].Get<string>();
            q.Save();
            ShowQuestionsLandingPage();
        }
    }
}
