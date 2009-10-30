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
using ChatRecords;
using HelperGlobals;
using LanguageRecords;
using Ra.Brix.Loader;
using System.Web;
using Ra.Brix.Types;
using Ra.Brix.Data;
using System.Collections.Generic;
using SettingsRecords;

namespace ChatController
{
    [ActiveController]
    public class ChatController
    {
        private bool NewMessage
        {
            get
            {
                return HttpContext.Current.Application["InitializeController.ChatController.NewMessage" + Users.LoggedInUserName] == null ?
                    false :
                    (bool)HttpContext.Current.Application["InitializeController.ChatController.NewMessage" + Users.LoggedInUserName];
            }
            set
            {
                HttpContext.Current.Application["InitializeController.ChatController.NewMessage" + Users.LoggedInUserName] = value ? true : false;
            }
        }

        private static void SetNewMessageForUser(string username)
        {
            HttpContext.Current.Application["InitializeController.ChatController.NewMessage" + username] = true;
        }

        [ActiveEvent(Name = "ApplicationStartup")]
        protected static void ApplicationStartup(object sender, ActiveEventArgs e)
        {
            Language.Instance.SetDefaultValue("ButtonChat", "Chat");
            Language.Instance.SetDefaultValue("ButtonAvailableUsers", "Users");
            Language.Instance.SetDefaultValue("ButtonViewHistory", "View Log");
        }

        [ActiveEvent(Name = "GetMenuItems")]
        protected void GetMenuItems(object sender, ActiveEventArgs e)
        {
            e.Params["ButtonAppl"].Value = "Menu-Applications";
            e.Params["ButtonAppl"]["ButtonChat"].Value = "Menu-Applications-Chat";
            e.Params["ButtonAppl"]["ButtonChat"]["ButtonAvailableUsers"].Value = "Menu-OpenChat";
            e.Params["ButtonAppl"]["ButtonChat"]["ButtonViewHistory"].Value = "Menu-OpenChatHistory";
        }

        [ActiveEvent(Name = "Menu-OpenChatHistory")]
        protected void OpenChatHistory(object sender, ActiveEventArgs e)
        {
            Node init = new Node();
            init["TabCaption"].Value = Language.Instance["ChatHistoryCaption", null, "Chat History"];
            List<ChatMessage> chats = new List<ChatMessage>();
            foreach (ChatMessage idx in
                ActiveRecord<ChatMessage>.Select(
                    Criteria.Eq("SentToUsername", Users.LoggedInUserName)))
            {
                chats.Add(idx);
            }
            foreach (ChatMessage idx in
                ActiveRecord<ChatMessage>.Select(
                    Criteria.Eq("SentByUsername", Users.LoggedInUserName)))
            {
                chats.Add(idx);
            }
            chats.Sort(
                delegate(ChatMessage left, ChatMessage right)
                    {
                        return right.When.CompareTo(left.When);
                    });

            init["ModuleSettings"]["Grid"]["Columns"]["SentBy"]["Caption"].Value = Language.Instance["ChatSentByCaption", null, "Sent By"];
            init["ModuleSettings"]["Grid"]["Columns"]["SentBy"]["ControlType"].Value = "Label";

            init["ModuleSettings"]["Grid"]["Columns"]["When"]["Caption"].Value = Language.Instance["ChatWhenCaption", null, "When"];
            init["ModuleSettings"]["Grid"]["Columns"]["When"]["ControlType"].Value = "Label";

            init["ModuleSettings"]["Grid"]["Columns"]["Message"]["Caption"].Value = Language.Instance["ChatMessageCaption", null, "Message"];
            init["ModuleSettings"]["Grid"]["Columns"]["Message"]["ControlType"].Value = "Label";

            int idxNo = 0;
            foreach (ChatMessage idx in chats)
            {
                init["ModuleSettings"]["Grid"]["Rows"]["Row" + idxNo]["ID"].Value = idx.ID;
                init["ModuleSettings"]["Grid"]["Rows"]["Row" + idxNo]["SentBy"].Value = idx.SentByUsername;
                init["ModuleSettings"]["Grid"]["Rows"]["Row" + idxNo]["When"].Value = idx.When;
                init["ModuleSettings"]["Grid"]["Rows"]["Row" + idxNo]["When"]["Format"].Value = "d MMM yy : HH:mm";
                init["ModuleSettings"]["Grid"]["Rows"]["Row" + idxNo]["Message"].Value = idx.Message;
                idxNo += 1;
            }
            init["ModuleSettings"]["LoggedInUser"].Value = Users.LoggedInUserName;

            ActiveEvents.Instance.RaiseLoadControl(
                "ChatModules.ChatHistory", 
                "dynMid",
                init);
        }

        [ActiveEvent(Name = "LoadComplete")]
        protected void LoadComplete(object sender, ActiveEventArgs e)
        {
            if (NewMessage)
            {
                NewMessage = false;
                Node node = new Node();
                node["Message"].Value = 
                    Language.Instance["ChatYouGotNewChatsInfo", null, @"You've got new chats waiting in the chat room..."];
                node["Duration"].Value = 5000;
                node["EventToFire"].Value = "OpenChatClient";
                ActiveEvents.Instance.RaiseActiveEvent(
                    this, 
                    "ShowInformationMessage", 
                    node);
            }
        }

        [ActiveEvent(Name = "OpenChatClient")]
        protected void OpenChatClient(object sender, ActiveEventArgs e)
        {
            OpenViewChatUsers();
        }

        [ActiveEvent(Name = "GetLatestChatMessages")]
        protected void GetLatestChatMessages(object sender, ActiveEventArgs e)
        {
            string chattingWith = e.Params["ChattingWith"].Value.ToString();
            List<ChatMessage> chats = GetLatestChats(chattingWith);
            int idxNo = 0;
            foreach (ChatMessage idx in chats)
            {
                e.Params["Chats"]["chat" + idxNo]["Message"].Value = idx.Message;
                e.Params["Chats"]["chat" + idxNo]["ByWhom"].Value = idx.SentByUsername;
                e.Params["Chats"]["chat" + idxNo]["When"].Value = idx.When;
                idxNo += 1;
            }
        }

        private List<ChatMessage> GetLatestChats(string chattingWith)
        {
            List<ChatMessage>  chats = new List<ChatMessage>();
            foreach (ChatMessage idx in
                ActiveRecord<ChatMessage>.Select(
                    Criteria.Eq("SentByUsername", chattingWith),
                    Criteria.Eq("SentToUsername", Users.LoggedInUserName),
                    Criteria.Mt("When",
                                DateTime.Now.AddMinutes(
                                    -Settings.Instance.Get<int>("MinutesToShowChats")))))
            {
                chats.Add(idx);
            }
            foreach (ChatMessage idx in
                ActiveRecord<ChatMessage>.Select(
                    Criteria.Eq("SentToUsername", chattingWith),
                    Criteria.Eq("SentByUsername", Users.LoggedInUserName),
                    Criteria.Mt("When",
                                DateTime.Now.AddMinutes(
                                    -Settings.Instance.Get<int>("MinutesToShowChats")))))
            {
                chats.Add(idx);
            }
            chats.Sort(
                delegate(ChatMessage left, ChatMessage right)
                    {
                        return left.When.CompareTo(right.When);
                    });
            while (chats.Count > Settings.Instance.Get<int>("NumberOfChatsToShow"))
            {
                chats.RemoveAt(0);
            }
            return chats;
        }

        [ActiveEvent(Name = "NewChatAdded")]
        protected void NewChatAdded(object sender, ActiveEventArgs e)
        {
            string userName = Users.LoggedInUserName;
            string toUserName = e.Params["ChattingWith"].Value.ToString();

            ChatMessage msg = new ChatMessage();
            msg.SentByUsername = userName;
            msg.SentToUsername = toUserName;
            msg.Message = e.Params["Message"].Value.ToString();
            msg.Save();

            SetNewMessageForUser(toUserName);
        }

        [ActiveEvent(Name = "InitiateNewChat")]
        protected void InitiateNewChat(object sender, ActiveEventArgs e)
        {
            string withUser = e.Params["Username"].Get<string>();
            if (withUser == Users.LoggedInUserName)
            {
                Node node = new Node();
                node["Message"].Value =
                    Language.Instance["ChatWithYourselfInfo", null, @"Sorry but you cannot initiate a new chat with yourself ... ;)"];
                node["Duration"].Value = 5000;
                ActiveEvents.Instance.RaiseActiveEvent(this, "ShowInformationMessage", node);
            }
            else
            {
                Node init = new Node();
                init["TabCaption"].Value = Language.Instance["ChatInitiateCaption", null, "Chatting with; "] + withUser;
                init["ModuleSettings"]["WhichUser"].Value = withUser;

                ActiveEvents.Instance.RaiseLoadControl(
                    "ChatModules.ChatChannel",
                    "dynMid",
                    init);
            }
        }

        [ActiveEvent(Name = "Menu-OpenChat")]
        protected void OpenChatApplicationFromMenu(object sender, ActiveEventArgs e)
        {
            OpenViewChatUsers();
        }

        private static void OpenViewChatUsers()
        {
            Node init = new Node();
            init["TabCaption"].Value = Language.Instance["ChatClientCaption", null, "Chat Client"];

            init["ModuleSettings"]["Grid"]["Columns"]["Users"]["Caption"].Value = Language.Instance["ChatButton", null, "Users"];
            init["ModuleSettings"]["Grid"]["Columns"]["Users"]["ControlType"].Value = "LinkButton";

            foreach (string idxUser in Users.Instance)
            {
                int noChats = ActiveRecord<ChatMessage>.CountWhere(
                    Criteria.Eq("SentToUsername", Users.LoggedInUserName),
                    Criteria.Eq("SentByUsername", idxUser),
                    Criteria.Mt("When",
                                DateTime.Now.AddMinutes(
                                    -Settings.Instance.Get<int>("MinutesToShowChats"))));

                init["ModuleSettings"]["Grid"]["Rows"]["Row" + idxUser]["ID"].Value = idxUser;
                init["ModuleSettings"]["Grid"]["Rows"]["Row" + idxUser]["Users"].Value = idxUser + "(" + noChats + ")";
            }

            ActiveEvents.Instance.RaiseLoadControl(
                "ChatModules.ChatViewUsers",
                "dynMid",
                init);
        }
    }
}