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
using System.Configuration;

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
            e.Params["ButtonStacked"].Value = "url:~/stacked" + ConfigurationManager.AppSettings["DefaultPageExtension"];
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
            User user = null;
            if (!string.IsNullOrEmpty(Users.LoggedInUserName))
                user = User.SelectFirst(Criteria.Eq("Username", Users.LoggedInUserName));
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
                return;
            }
            else
            {
                Answer a = Answer.SelectByID(id);
                if (a.Author == user)
                {
                    Node nodeMessage = new Node();
                    nodeMessage["Message"].Value = Language.Instance[
                        "CannotVoteOwnAnswers",
                        null,
                        "You cannot vote for your own answers..."];
                    nodeMessage["Duration"].Value = 2000;
                    ActiveEvents.Instance.RaiseActiveEvent(
                        this,
                        "ShowInformationMessage",
                        nodeMessage);
                    return;
                }
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
                a.Author.Score += (points * 2);
                a.Author.Save();
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
                Answer.Vote xVote = idx.Voters.Find(
                    delegate(Answer.Vote idxVote)
                    {
                        return idxVote.User == user;
                    });
                node["Answers"]["A" + idx.ID]["CurrentVote"].Value = xVote == null ? 0 : xVote.Points;
                node["Answers"]["A" + idx.ID]["ID"].Value = idx.ID;
            }
            ActiveEvents.Instance.RaiseActiveEvent(
                this,
                "UpdateStackedAnswers",
                node);
        }

        [ActiveEvent(Name = "FilterStackedQuestions")]
        protected void FilterStackedQuestions(object sender, ActiveEventArgs e)
        {
            // Viewing all questions...
            List<Criteria> crits = new List<Criteria>();
            crits.Add(Criteria.Sort("LastAnswer", false));
            crits.AddRange(DataHelper.CreateSearchFilter("Header", e.Params["Filter"].Get<string>()));

            if (e.Params["Username"].Value != null)
                crits.Add(Criteria.Eq("Author.Username", e.Params["Username"].Get<string>()));
            if (!string.IsNullOrEmpty(Users.LoggedInUserName))
            {
                User user = User.SelectFirst(Criteria.Eq("Username", Users.LoggedInUserName));
                e.Params["CanDelete"].Value = user.InRole("Administrator");
            }
            foreach (Question idx in Question.Select(crits.ToArray()))
            {
                e.Params["Questions"]["Q" + idx.ID]["Header"].Value = idx.Header;
                e.Params["Questions"]["Q" + idx.ID]["Body"].Value = idx.Body;
                e.Params["Questions"]["Q" + idx.ID]["URL"].Value = idx.URL;
                e.Params["Questions"]["Q" + idx.ID]["Username"].Value = idx.Author.Username;
                e.Params["Questions"]["Q" + idx.ID]["Email"].Value = idx.Author.Email;
                e.Params["Questions"]["Q" + idx.ID]["Score"].Value = idx.Author.Score;
                e.Params["Questions"]["Q" + idx.ID]["Asked"].Value = idx.Asked;
                e.Params["Questions"]["Q" + idx.ID]["Answers"].Value = idx.Answers.Count;
                e.Params["Questions"]["Q" + idx.ID]["LastAnswer"].Value = idx.LastAnswer;
                e.Params["Questions"]["Q" + idx.ID]["Viewed"].Value = idx.Viewed;
                e.Params["Questions"]["Q" + idx.ID]["ID"].Value = idx.ID;
                if (e.Params["Questions"].Count >= 50)
                    break;
            }
        }

        [ActiveEvent(Name = "ShowArticlesUserDetailsFooter")]
        protected void ShowArticlesUserDetailsFooter(object sender, ActiveEventArgs e)
        {
            string username = e.Params["Username"].Get<string>();
            Node node = new Node();
            foreach (Question idx in Question.Select(
                Criteria.Sort("LastAnswer", false),
                Criteria.Eq("Author.Username", username)))
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
                node["ModuleSettings"]["Questions"]["Q" + idx.ID]["Viewed"].Value = idx.Viewed;
                node["ModuleSettings"]["Questions"]["Q" + idx.ID]["ID"].Value = idx.ID;
                if (node.Count >= 50)
                    break;
            }
            if (node["ModuleSettings"]["Questions"].Count > 0)
            {
                if (!string.IsNullOrEmpty(Users.LoggedInUserName))
                {
                    User user = User.SelectFirst(Criteria.Eq("Username", Users.LoggedInUserName));
                    e.Params["ModuleSettings"]["CanDelete"].Value = user.InRole("Administrator");
                }
                node["AddToExistingCollection"].Value = true;
                node["ModuleSettings"]["Header"].Value =
                    Language.Instance["QuestionsFrom", null, "Questions from "] + username;
                node["ModuleSettings"]["HideAskButton"].Value = true;
                node["ModuleSettings"]["Username"].Value = username;
                ActiveEvents.Instance.RaiseLoadControl(
                    "StackedModules.ViewPosts",
                    "dynMid",
                    node);
            }
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
            User user = User.SelectFirst(Criteria.Eq("Username", Users.LoggedInUserName));
            foreach (Answer idx in answers)
            {
                node["Answers"]["A" + idx.ID]["Body"].Value = idx.Body;
                node["Answers"]["A" + idx.ID]["Username"].Value = idx.Author.Username;
                node["Answers"]["A" + idx.ID]["Email"].Value = idx.Author.Email;
                node["Answers"]["A" + idx.ID]["Score"].Value = idx.Author.Score;
                node["Answers"]["A" + idx.ID]["Asked"].Value = idx.Asked;
                node["Answers"]["A" + idx.ID]["Votes"].Value = idx.Votes;
                Answer.Vote xVote = idx.Voters.Find(
                    delegate(Answer.Vote idxVote)
                    {
                        return idxVote.User == user;
                    });
                node["Answers"]["A" + idx.ID]["CurrentVote"].Value = xVote == null ? 0 : xVote.Points;
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
            q.Viewed += 1;
            q.Save();
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
            User user = null;
            if (!string.IsNullOrEmpty(Users.LoggedInUserName))
                user = User.SelectFirst(Criteria.Eq("Username", Users.LoggedInUserName));
            foreach (Answer idx in answers)
            {
                node["ModuleSettings"]["Answers"]["A" + idx.ID]["Body"].Value = idx.Body;
                node["ModuleSettings"]["Answers"]["A" + idx.ID]["Username"].Value = idx.Author.Username;
                node["ModuleSettings"]["Answers"]["A" + idx.ID]["Email"].Value = idx.Author.Email;
                node["ModuleSettings"]["Answers"]["A" + idx.ID]["Score"].Value = idx.Author.Score;
                node["ModuleSettings"]["Answers"]["A" + idx.ID]["Asked"].Value = idx.Asked;
                node["ModuleSettings"]["Answers"]["A" + idx.ID]["Votes"].Value = idx.Votes;
                node["ModuleSettings"]["Answers"]["A" + idx.ID]["ID"].Value = idx.ID;
                if (user != null)
                {
                    Answer.Vote xVote = idx.Voters.Find(
                        delegate(Answer.Vote idxVote)
                        {
                            return idxVote.User == user;
                        });
                    node["ModuleSettings"]["Answers"]["A" + idx.ID]["CurrentVote"].Value = xVote == null ? 0 : xVote.Points;
                }
                else
                    node["ModuleSettings"]["Answers"]["A" + idx.ID]["CurrentVote"].Value = 0;
                node["ModuleSettings"]["Answers"]["A" + idx.ID]["ID"].Value = idx.ID;
            }

            // Defaulting to false on deletion...
            node["ModuleSettings"]["CanDelete"].Value = false;
            if (user != null)
            {
                if (user.InRole("Administrator"))
                {
                    // User *can* delete answers...
                    node["ModuleSettings"]["CanDelete"].Value = true;
                }
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
            // Settings title of page
            ((System.Web.UI.Page)HttpContext.Current.CurrentHandler).Title =
                Settings.Instance.Get<string>("StackedLandingPageTitle", "Welcome to Stacked, and Open Source Q&A/Forum system");

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
                node["ModuleSettings"]["Questions"]["Q" + idx.ID]["Viewed"].Value = idx.Viewed;
                node["ModuleSettings"]["Questions"]["Q" + idx.ID]["ID"].Value = idx.ID;
                if (node.Count >= 50)
                    break;
            }
            if (!string.IsNullOrEmpty(Users.LoggedInUserName))
            {
                User user = User.SelectFirst(Criteria.Eq("Username", Users.LoggedInUserName));
                node["ModuleSettings"]["CanDelete"].Value = user.InRole("Administrator");
            }
            node["ModuleSettings"]["HideAskButton"].Value = Users.LoggedInUserName == null;
            ActiveEvents.Instance.RaiseLoadControl(
                "StackedModules.ViewPosts",
                "dynMid",
                node);
        }

        [ActiveEvent(Name = "DeleteStackedQuestion")]
        protected void DeleteStackedQuestion(object sender, ActiveEventArgs e)
        {
            int id = e.Params["ID"].Get<int>();
            Question q = Question.SelectByID(id);

            // Penaltizing author of question...
            q.Author.Score -= 50;
            q.Author.Save();

            // Deleting question
            q.Delete();

            // Showing information message...
            Node nodeMessage = new Node();
            nodeMessage["Message"].Value = Language.Instance[
                "StackedQuestionWasDeleted",
                null,
                "The question was deleted. Notice that user posting questions was penaltized with -50 points."];
            nodeMessage["Duration"].Value = 2000;
            ActiveEvents.Instance.RaiseActiveEvent(
                this,
                "ShowInformationMessage",
                nodeMessage);

            // For simplicity, we're just reloading the questions...
            ShowQuestionsLandingPage();
        }

        [ActiveEvent(Name = "DeleteStackedAnswer")]
        protected void DeleteStackedAnswer(object sender, ActiveEventArgs e)
        {
            int id = e.Params["ID"].Get<int>();
            Answer a = Answer.SelectByID(id);

            // Penaltizing author of answer...
            a.Author.Score -= 50;
            a.Author.Save();

            // Deleting answer
            a.Delete();

            // Showing information message...
            Node nodeMessage = new Node();
            nodeMessage["Message"].Value = Language.Instance[
                "StackedAnswerWasDeleted",
                null,
                "The answer was deleted. Notice that user posting the answer was penaltized with -50 points."];
            nodeMessage["Duration"].Value = 2000;
            ActiveEvents.Instance.RaiseActiveEvent(
                this,
                "ShowInformationMessage",
                nodeMessage);

            User user = null;
            if (!string.IsNullOrEmpty(Users.LoggedInUserName))
                user = User.SelectFirst(Criteria.Eq("Username", Users.LoggedInUserName));

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
                Answer.Vote xVote = idx.Voters.Find(
                    delegate(Answer.Vote idxVote)
                    {
                        return idxVote.User == user;
                    });
                node["Answers"]["A" + idx.ID]["CurrentVote"].Value = xVote == null ? 0 : xVote.Points;
                node["Answers"]["A" + idx.ID]["ID"].Value = idx.ID;
            }
            ActiveEvents.Instance.RaiseActiveEvent(
                this,
                "UpdateStackedAnswers",
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