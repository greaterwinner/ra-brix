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
using UserRecords;
using HelperGlobals;
using SettingsRecords;
using System.Collections.Generic;

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

        [ActiveEvent(Name = "VoteStackedAnswer")]
        protected void VoteStackedAnswer(object sender, ActiveEventArgs e)
        {
            int id = e.Params["ID"].Get<int>();
            int points = e.Params["Points"].Get<int>();
            User user = User.SelectFirst(Criteria.Eq("Username", Users.LoggedInUserName));
            if(user == null)
            {
                Node nodeMessage = new Node();
                nodeMessage["Message"].Value = Language.Instance[
                    "CannotVoteUnlessLoggedIn", 
                    null, 
                    "You cannot vote unless you're logged in. Please login first..."];
                nodeMessage["Duration"].Value = 2000;
                ActiveEvents.Instance.RaiseActiveEvent(
                    this,
                    "ShowInformationMessage",
                    nodeMessage);
            }
            else
            {
                Answer a = Answer.SelectByID(id);
                Answer.Vote old = a.Voters.Find(
                    delegate(Answer.Vote idx)
                    {
                        return user == idx.User;
                    });
                if (old != null)
                {
                    a.Votes -= old.Points;
                }
                else
                {
                    old = new Answer.Vote();
                    old.User = user;
                    a.Voters.Add(old);
                }
                old.Points = points;
                a.Votes += points;
                a.Save();
            }
            Question q = Question.SelectByID(e.Params["QuestionID"].Get<int>());
            Node node = new Node();
            node["QuestionID"].Value = id;
            node["IsVote"].Value = true;
            List<Answer> answers = new List<Answer>(q.Answers);
            answers.Sort(
                delegate(Answer left, Answer right)
                {
                    return right.Votes.CompareTo(left.Votes);
                });
            foreach (Answer idx in answers)
            {
                node["Answers"]["A" + idx.ID]["Body"].Value = idx.Body;
                node["Answers"]["A" + idx.ID]["Username"].Value = idx.Author.Username;
                node["Answers"]["A" + idx.ID]["Email"].Value = idx.Author.Email;
                node["Answers"]["A" + idx.ID]["Score"].Value = idx.Author.Score;
                node["Answers"]["A" + idx.ID]["Asked"].Value = idx.Asked;
                node["Answers"]["A" + idx.ID]["Votes"].Value = idx.Votes;
                node["Answers"]["A" + idx.ID]["ID"].Value = idx.ID;
            }
            ActiveEvents.Instance.RaiseActiveEvent(
                this,
                "UpdateStackedAnswers",
                node);
        }

        [ActiveEvent(Name = "RequestSavingOfStackedAnswer")]
        protected void RequestSavingOfStackedAnswer(object sender, ActiveEventArgs e)
        {
            // Saving Answer...
            int id = e.Params["QuestionID"].Get<int>();
            Question q = Question.SelectByID(id);
            Answer a = new Answer();
            a.Body = e.Params["Body"].Get<string>();
            a.Asked = DateTime.Now;
            a.Author = User.SelectFirst(Criteria.Eq("Username", Users.LoggedInUserName));
            q.Answers.Add(a);
            q.LastAnswer = a.Asked;
            q.Save();

            // Adding points to users score
            a.Author.Score += Settings.Instance.Get<int>("UserScoreForNewAnswer", 2);
            a.Author.Save();

            // Updating answers on form
            Node node = new Node();
            node["QuestionID"].Value = id;
            List<Answer> answers = new List<Answer>(q.Answers);
            answers.Sort(
                delegate(Answer left, Answer right)
                {
                    return right.Votes.CompareTo(left.Votes);
                });
            foreach (Answer idx in answers)
            {
                node["Answers"]["A" + idx.ID]["Body"].Value = idx.Body;
                node["Answers"]["A" + idx.ID]["Username"].Value = idx.Author.Username;
                node["Answers"]["A" + idx.ID]["Email"].Value = idx.Author.Email;
                node["Answers"]["A" + idx.ID]["Score"].Value = idx.Author.Score;
                node["Answers"]["A" + idx.ID]["Asked"].Value = idx.Asked;
                node["Answers"]["A" + idx.ID]["Votes"].Value = idx.Votes;
                node["Answers"]["A" + idx.ID]["ID"].Value = idx.ID;
            }
            ActiveEvents.Instance.RaiseActiveEvent(
                this,
                "UpdateStackedAnswers",
                node);

            // Showing info message
            Node nodeMessage = new Node();
            nodeMessage["Message"].Value = Language.Instance["ThankYouForAnswer", null, @"
Thank you for your answer"];
            nodeMessage["Duration"].Value = 2000;
            ActiveEvents.Instance.RaiseActiveEvent(
                this,
                "ShowInformationMessage",
                nodeMessage);
            e.Params["Success"].Value = true;
        }

        private void ShowQuestion(string contentId)
        {
            Question q = Question.FindArticle(contentId);
            ShowQuestion(q);
            ShowAnswers(q);
            if (!string.IsNullOrEmpty(Users.LoggedInUserName))
                ShowAnswerForm(q);
        }

        private static void ShowQuestion(Question q)
        {
            Node node = new Node();
            node["ModuleSettings"]["Header"].Value = q.Header;
            node["ModuleSettings"]["Body"].Value = q.Body;
            node["ModuleSettings"]["Username"].Value = q.Author.Username;
            node["ModuleSettings"]["Email"].Value = q.Author.Email;
            node["ModuleSettings"]["Score"].Value = q.Author.Score;
            ActiveEvents.Instance.RaiseLoadControl(
                "StackedModules.ViewQuestion",
                "dynMid",
                node);
        }

        private void ShowAnswers(Question q)
        {
            Node node = new Node();
            node["AddToExistingCollection"].Value = true;
            node["ModuleSettings"]["QuestionID"].Value = q.ID;
            List<Answer> answers = new List<Answer>(q.Answers);
            answers.Sort(
                delegate(Answer left, Answer right)
                {
                    return right.Votes.CompareTo(left.Votes);
                });
            foreach (Answer idx in answers)
            {
                node["ModuleSettings"]["Answers"]["A" + idx.ID]["Body"].Value = idx.Body;
                node["ModuleSettings"]["Answers"]["A" + idx.ID]["Username"].Value = idx.Author.Username;
                node["ModuleSettings"]["Answers"]["A" + idx.ID]["Email"].Value = idx.Author.Email;
                node["ModuleSettings"]["Answers"]["A" + idx.ID]["Score"].Value = idx.Author.Score;
                node["ModuleSettings"]["Answers"]["A" + idx.ID]["Asked"].Value = idx.Asked;
                node["ModuleSettings"]["Answers"]["A" + idx.ID]["Votes"].Value = idx.Votes;
                node["ModuleSettings"]["Answers"]["A" + idx.ID]["ID"].Value = idx.ID;
            }
            ActiveEvents.Instance.RaiseLoadControl(
                "StackedModules.ViewAnswers",
                "dynMid",
                node);
        }

        private void ShowAnswerForm(Question q)
        {
            Node node = new Node();
            node["ModuleSettings"]["QuestionID"].Value = q.ID;
            node["AddToExistingCollection"].Value = true;
            ActiveEvents.Instance.RaiseLoadControl(
                "StackedModules.AnswerQuestion",
                "dynMid",
                node);
        }

        private static void ShowQuestionsLandingPage()
        {
            // Viewing all questions...
            Node node = new Node();
            foreach (Question idx in Question.Select(Criteria.Sort("LastAnswer", false)))
            {
                node["ModuleSettings"]["Questions"]["Q" + idx.ID]["Header"].Value = idx.Header;
                node["ModuleSettings"]["Questions"]["Q" + idx.ID]["Body"].Value = idx.Body;
                node["ModuleSettings"]["Questions"]["Q" + idx.ID]["URL"].Value = idx.URL;
                node["ModuleSettings"]["Questions"]["Q" + idx.ID]["Username"].Value = idx.Author.Username;
                node["ModuleSettings"]["Questions"]["Q" + idx.ID]["Email"].Value = idx.Author.Email;
                node["ModuleSettings"]["Questions"]["Q" + idx.ID]["Score"].Value = idx.Author.Score;
                node["ModuleSettings"]["Questions"]["Q" + idx.ID]["Asked"].Value = idx.Asked;
                node["ModuleSettings"]["Questions"]["Q" + idx.ID]["Answers"].Value = idx.Answers.Count;
                node["ModuleSettings"]["Questions"]["Q" + idx.ID]["LastAnswer"].Value = idx.LastAnswer;
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
            // Saving new question...
            Question q = new Question();
            q.Body = e.Params["Body"].Get<string>();
            q.Header = e.Params["Header"].Get<string>();
            q.Save();

            // Updating User's score with points for question
            User u = User.SelectFirst(Criteria.Eq("Username", Users.LoggedInUserName));
            u.Score += Settings.Instance.Get<int>("UserScoreForNewQuestion", 6);
            u.Save();
            ShowQuestionsLandingPage();
        }
    }
}
