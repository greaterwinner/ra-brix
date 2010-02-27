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
using System.Net.Mail;
using Ra.Brix.Loader;
using Ra.Brix.Types;
using SettingsRecords;
using ASP = System.Web.UI.WebControls;
using System.Net;
using LanguageRecords;
using Ra.Effects;

namespace CMSCommonPluginModules
{
    [ActiveModule]
    public class ContactUs : System.Web.UI.UserControl, IModule
    {
        protected global::Ra.Widgets.Panel wrp;
        protected global::Ra.Widgets.Panel wrp2;
        protected global::Ra.Widgets.TextBox email;
        protected global::Ra.Widgets.TextBox name;
        protected global::Ra.Widgets.TextArea body;

        protected void Page_Load(object sender, EventArgs e)
        {
            wrp.DataBind();
        }

        protected void submit_Click(object sender, EventArgs e)
        {
            try
            {
                SendEmail();
                Node node = new Node();
                node["Message"].Value = Language.Instance["EmailSuccessfullySent", null, @"
Email was successfully sent, thank you for your feedback. Your email will be answered as soon as possible. Have a nice day :)"];
                node["Duration"].Value = 5000;
                ActiveEvents.Instance.RaiseActiveEvent(
                    this,
                    "ShowInformationMessage",
                    node);
            }
            catch (Exception err)
            {
                Node node = new Node();
                node["Message"].Value = string.Format(@"
Something went wrong while trying to send email, message from server was: {0}", err.InnerException.Message);
                node["Duration"].Value = 5000;
                ActiveEvents.Instance.RaiseActiveEvent(
                    this,
                    "ShowInformationMessage",
                    node);
            }
        }

        private void SendEmail()
        {
            Node node = new Node();
            node["Comment"].Value = body.Text;
            node["Email"].Value = email.Text;
            node["Name"].Value = name.Text;
            ActiveEvents.Instance.RaiseActiveEvent(
                this,
                "CMSCommonPluginsSendEmail",
                node);

            // Animating contact field...
            new EffectRollUp(wrp, 200)
                .ChainThese(
                    new EffectRollDown(wrp2, 200))
                .Render();
        }

        public void InitialLoading(Node node)
        {
            Load +=
                delegate
                {
                    body.Text = Language.Instance["SendEmailTemplate", null,
@"My phone number; (xxx)-xxx-xxxx
My name; John Doe
Contact me on phone please [x]
Comment;
"];
                };
        }
    }
}