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
using System;
using Ra.Widgets;
using Ra.Extensions.Widgets;
using Ra.Effects;
using System.Security.Cryptography;
using System.Text;
using HelperGlobals;
using System.Configuration;

namespace StackedModules
{
    [ActiveModule]
    public class ViewAnswers : System.Web.UI.UserControl, IModule
    {
        protected global::System.Web.UI.WebControls.Repeater rep;
        protected global::Ra.Widgets.Panel repWrp;

        protected bool CanDelete
        {
            get { return (bool)ViewState["CanDelete"]; }
            set { ViewState["CanDelete"] = value; }
        }

        protected void DeletePost(object sender, EventArgs e)
        {
            LinkButton lb = sender as LinkButton;
            int id = int.Parse(lb.Xtra);
            Node node = new Node();
            node["ID"].Value = id;
            node["QuestionID"].Value = QuestionID;
            ActiveEvents.Instance.RaiseActiveEvent(
                this,
                "DeleteStackedAnswer",
                node);
        }

        [ActiveEvent(Name = "UpdateStackedAnswers")]
        protected void UpdateStackedAnswers(object sender, ActiveEventArgs e)
        {
            rep.DataSource = e.Params["Answers"];
            rep.DataBind();
            repWrp.ReRender();
            if (e.Params["IsVote"].Value != null && 
                e.Params["IsVote"].Get<bool>())
            {
                // This is a "voting update", which means we can run 
                // some fancy animations to make it all look better ...
                new EffectRollUp(repWrp, 250)
                    .ChainThese(new EffectRollDown(repWrp, 750))
                    .Render();
            }
            else
            {
                new EffectHighlight(repWrp, 500)
                    .Render();
            }
        }

        protected string GetGravatar(object emailObj)
        {
            string email = emailObj as string;
            email = email == null ? "" : email;

            StringBuilder emailHash = new StringBuilder();
            MD5 md5 = MD5.Create();
            byte[] emailBuffer = Encoding.ASCII.GetBytes(email);
            byte[] hash = md5.ComputeHash(emailBuffer);

            foreach (byte hashByte in hash)
                emailHash.Append(hashByte.ToString("x2"));

            return string.Format("http://www.gravatar.com/avatar/{0}?s=32&d=identicon", emailHash.ToString());
        }

        protected string GetCssClassForVoteIcon(object firstObj, object secondObj)
        {
            string first = firstObj as string;
            int second = (int)secondObj;
            string retVal = first;
            switch (second)
            {
                case -1:
                    if (first == "answerVoteMinus")
                        retVal += " activeMinus";
                    break;
                case 1:
                    if (first == "answerVotePlus")
                        retVal += " activePlus";
                    break;
            }
            return retVal;
        }

        protected string GetCssClassForVotes(object scoreObj)
        {
            int score = (int)scoreObj;
            if (score < 0)
            {
                return "votes bad";
            }
            if (score == 0)
            {
                return "votes neutral";
            }
            else
            {
                return "votes good";
            }
        }

        private int QuestionID
        {
            get { return (int)ViewState["QuestionID"]; }
            set { ViewState["QuestionID"] = value; }
        }

        protected void VoteUp(object sender, EventArgs e)
        {
            LinkButton btn = sender as LinkButton;
            int id = int.Parse(btn.Xtra);
            Node node = new Node();
            node["ID"].Value = id;
            node["Points"].Value = 1;
            node["QuestionID"].Value = QuestionID;
            ActiveEvents.Instance.RaiseActiveEvent(
                this,
                "VoteStackedAnswer",
                node);
        }

        protected void VoteDown(object sender, EventArgs e)
        {
            LinkButton btn = sender as LinkButton;
            int id = int.Parse(btn.Xtra);
            Node node = new Node();
            node["ID"].Value = id;
            node["Points"].Value = -1;
            node["QuestionID"].Value = QuestionID;
            ActiveEvents.Instance.RaiseActiveEvent(
                this,
                "VoteStackedAnswer",
                node);
        }

        protected string GetUsernameLink(object usernameObj)
        {
            string username = usernameObj as string;
            return ApplicationRoot.Root + 
                "authors/" + username.Replace(".", "--") + 
                ConfigurationManager.AppSettings["DefaultPageExtension"];
        }

        public void InitialLoading(Node node)
        {
            Load +=
                delegate
                {
                    CanDelete = node["CanDelete"].Get<bool>();
                    QuestionID = node["QuestionID"].Get<int>();
                    rep.DataSource = node["Answers"];
                    rep.DataBind();
                };
        }
    }
}




