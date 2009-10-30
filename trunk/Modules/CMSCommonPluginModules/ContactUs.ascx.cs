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

namespace CMSCommonPluginModules
{
    [ActiveModule]
    public class ContactUs : System.Web.UI.UserControl, IModule
    {
        protected global::Ra.Widgets.Panel wrp;
        protected global::Ra.Widgets.TextBox email;
        protected global::Ra.Widgets.TextBox header;
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
Something went wrong while trying to send email, message from server was: {0}", err.Message);
                node["Duration"].Value = 5000;
                ActiveEvents.Instance.RaiseActiveEvent(
                    this,
                    "ShowInformationMessage",
                    node);
            }
        }

        private void SendEmail()
        {
            string smtpServer = Settings.Instance["SMTPServer"];
            string smtpServerUsername = Settings.Instance["SMTPServerUsername"];
            string smtpServerPassword = Settings.Instance["SMTPServerPassword"];
            string smtpServerUseSsl = Settings.Instance["SMTPServerUseSsl"];
            string adminEmail = Settings.Instance["AdminEmail"];
            MailMessage msg = new MailMessage();
            msg.To.Add(adminEmail);
            msg.From = new MailAddress(email.Text);
            msg.Subject = header.Text;
            msg.Body = body.Text;
            msg.IsBodyHtml = false;
            SmtpClient smtp = new SmtpClient();
            smtp.Host = smtpServer;
            smtp.Credentials = new NetworkCredential(smtpServerUsername, smtpServerPassword);
            smtp.EnableSsl = smtpServerUseSsl == "True";
            smtp.Send(msg);
        }

        public void InitialLoading(Node node)
        {
        }

        public string GetCaption()
        {
            return "";
        }
    }
}