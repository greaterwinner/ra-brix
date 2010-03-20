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
using System.Text;
using System.Security.Cryptography;
using HelperGlobals;
using System.Configuration;
using Ra.Effects;

namespace StackedModules
{
    [ActiveModule]
    public class ViewPosts : System.Web.UI.UserControl, IModule
    {
        protected global::System.Web.UI.HtmlControls.HtmlGenericControl header;
        protected global::System.Web.UI.WebControls.Repeater rep;
        protected global::Ra.Extensions.Widgets.ExtButton ask;
        protected global::Ra.Widgets.TextBox filter;
        protected global::Ra.Widgets.Panel repWrp;

        public void InitialLoading(Node node)
        {
            Load +=
                delegate
                {
                    rep.DataSource = node["Questions"];
                    rep.DataBind();
                    ask.DataBind();
                    if (node["Header"] == null)
                    {
                        header.InnerHtml = Language.Instance["QuestionsAndAnswer", null, "Questions and Answers"];
                    }
                    else
                    {
                        header.InnerHtml = node["Header"].Get<string>();
                    }
                    if (node["HideAskButton"].Value != null && node["HideAskButton"].Get<bool>())
                    {
                        ask.Visible = false;
                    }
                    UsernameFilter = node["Username"].Value as string;
                };
        }

        private string UsernameFilter
        {
            get { return ViewState["UsernameFilter"] as string; }
            set { ViewState["UsernameFilter"] = value; }
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

        protected string GetUsernameLink(object usernameObj)
        {
            string username = usernameObj as string;
            return ApplicationRoot.Root +
                "authors/" + username.Replace(".", "--") +
                ConfigurationManager.AppSettings["DefaultPageExtension"];
        }

        protected void searchBtn_Click(object sender, EventArgs e)
        {
            Node node = new Node();
            node["Filter"].Value = filter.Text;
            node["Username"].Value = UsernameFilter;
            ActiveEvents.Instance.RaiseActiveEvent(
                this,
                "FilterStackedQuestions",
                node);
            rep.DataSource = node["Questions"];
            rep.DataBind();
            repWrp.ReRender();
            filter.Focus();
            filter.Select();
            new EffectHighlight(repWrp, 500)
                .Render();
        }

        protected void ask_Click(object sender, EventArgs e)
        {
            ActiveEvents.Instance.RaiseActiveEvent(
                this,
                "AskStackedQuestion");
        }
    }
}




