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
using System.Net.Mail;
using SettingsRecords;
using System.Net;
using System.Web;

namespace CMSCommonPlugins
{
    [ActiveController]
    public class CMSCommonPlugins
    {
        [ActiveEvent(Name = "CMSGetPluginTypes")]
        protected void CMSGetPluginTypes(object sender, ActiveEventArgs e)
        {
            e.Params["ContactUs"]["Name"].Value = Language.Instance["ContactUs", null, "Contact us"];
            e.Params["ContactUs"]["Value"].Value = "CMSCommonPluginModules.ContactUs";
        }

        [ActiveEvent(Name = "CMSCommonPluginsSendEmail")]
        protected void CMSCommonPluginsSendEmail(object sender, ActiveEventArgs e)
        {
            string smtpServer = Settings.Instance["SMTPServer"];
            string smtpServerUsername = Settings.Instance["SMTPServerUsername"];
            string smtpServerPassword = Settings.Instance["SMTPServerPassword"];
            string smtpServerUseSsl = Settings.Instance["SMTPServerUseSsl"];
            string adminEmail = Settings.Instance["AdminEmail"];
            MailMessage msg = new MailMessage();
            msg.To.Add(adminEmail);
            msg.From = new MailAddress("noreply@noreply.com");
            msg.Subject = Language.Instance["EmailDefaultSubjectLine", null, "Comment from: "] + e.Params["Name"].Get<string>();
            msg.Body = 
                Language.Instance["ContactCredentials", null, "Contact credentials: "] + 
                e.Params["Email"].Get<string>() + 
                ". " + 
                e.Params["Comment"].Get<string>()
                + @"

Promo Code: " + HttpContext.Current.Session["PromoCode"];
            msg.IsBodyHtml = false;
            SmtpClient smtp = new SmtpClient();
            smtp.Host = smtpServer;
            smtp.Credentials = new NetworkCredential(smtpServerUsername, smtpServerPassword);
            smtp.EnableSsl = smtpServerUseSsl == "True";
            smtp.Send(msg);
        }
    }
}
