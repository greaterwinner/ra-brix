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
using LanguageRecords;
using Ra.Brix.Loader;
using Ra.Effects;
using ASP = System.Web.UI.WebControls;
using Ra.Brix.Types;

namespace ChatModules
{
    [ActiveModule]
    public class ChatChannel : System.Web.UI.UserControl, IModule
    {
        protected global::Ra.Widgets.TextBox txtNewChat;
        protected global::Ra.Widgets.Label chatOutput;
        protected global::Ra.Widgets.Panel chatWrp;

        private string ChattingWith
        {
            get { return ViewState["ChattingWith"].ToString(); }
            set { ViewState["ChattingWith"] = value; }
        }

        protected void btnSubmitNewChat_Click(object sender, EventArgs e)
        {
            Node node = new Node();
            node["ChattingWith"].Value = ChattingWith;
            node["Message"].Value = txtNewChat.Text;
            ActiveEvents.Instance.RaiseActiveEvent(
                this, 
                "NewChatAdded", 
                node);
            txtNewChat.Text = Language.Instance["TextNewChat", null, "Type your message here..."];
            txtNewChat.Select();
            txtNewChat.Focus();
            UpdateChatOutput();
        }

        protected void chatTimer_Tick(object sender, EventArgs e)
        {
            UpdateChatOutput();
        }

        private DateTime LastChat
        {
            get
            {
                if (ViewState["LastChat"] == null)
                    return DateTime.MinValue;
                return (DateTime)ViewState["LastChat"];
            }
            set { ViewState["LastChat"] = value; }
        }

        private void UpdateChatOutput()
        {
            Node msgs = new Node();
            msgs["ChattingWith"].Value = ChattingWith;
            ActiveEvents.Instance.RaiseActiveEvent(
                this,
                "GetLatestChatMessages",
                msgs);
            string lblText = "";
            bool hasChanges = false;
            foreach (Node idx in msgs["Chats"])
            {
                string dateString;
                DateTime when = idx["When"].Get<DateTime>();
                if (when > LastChat)
                {
                    hasChanges = true;
                    LastChat = when;
                }
                TimeSpan span = DateTime.Now - when;
                if (span.TotalSeconds < 30)
                    dateString = "10s";
                else if (span.TotalSeconds < 60)
                    dateString = "40s";
                else if (span.TotalSeconds < 120)
                    dateString = "1m";
                else if (span.TotalSeconds < 180)
                    dateString = "2m";
                else
                    dateString = "3m";
                lblText += string.Format(
@"<li><span class=""time"">[{2}]</span> - <span class=""username"">{0}</span><span class=""content"">{1}</span></li>",
                    idx["ByWhom"].Value,
                    idx["Message"].Value,
                    dateString);
            }
            if (hasChanges)
            {
                chatOutput.Text = "<ul class=\"chatOutput\">" + lblText + "</ul>";
                if (Request.Browser.Browser != "AppleMAC-Safari")
                {
                    // Highlight doesn't work with Chrome for some reasons ... :(
                    new EffectHighlight(chatOutput, 500)
                        .Render();
                }
            }
        }

        private string _chattingWith;
        public void InitialLoading(Node node)
        {
            _chattingWith = node["WhichUser"].Value.ToString();
            ChattingWith = _chattingWith;
            if (string.IsNullOrEmpty(_chattingWith))
                throw new ApplicationException("Cannot chat with 'no user'");
            UpdateChatOutput();
            txtNewChat.Text = Language.Instance["TextNewChat", null, "Type your message here..."];
            txtNewChat.Select();
            txtNewChat.Focus();
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            chatWrp.DataBind();

            if (!string.IsNullOrEmpty(_chattingWith))
                ChattingWith = _chattingWith;
        }
    }
}