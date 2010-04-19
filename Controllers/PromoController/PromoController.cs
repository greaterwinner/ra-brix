/*
 * Ra-Brix - A Modular-based Framework for building 
 * Web Applications Copyright 2009 - Thomas Hansen 
 * thomas@ra-ajax.org. Unless permission is 
 * explicitly given this code is licensed under the 
 * GNU Affero GPL version 3 which can be found in the 
 * license.txt file on disc.
 * 
 */

using Ra.Brix.Loader;
using LanguageRecords;
using System.Web;
using PromoRecords;
using Ra.Brix.Data;
using System.Net.Mail;
using System.Net;
using SettingsRecords;
using Ra.Brix.Types;

namespace PromoController
{
    [ActiveController]
    public class PromoController
    {
        [ActiveEvent(Name = "ApplicationStartup")]
        protected static void ApplicationStartup(object sender, ActiveEventArgs e)
        {
            Language.Instance.SetDefaultValue("ButtonApplyForPromo", "Promo Program");
            Language.Instance.SetDefaultValue("ButtonViewPromoters", "View Promoters");
        }

        [ActiveEvent(Name = "GetMenuItems")]
        protected void GetMenuItems(object sender, ActiveEventArgs e)
        {
            e.Params["ButtonApplyForPromo"].Value = "url:~/promo-program";
            e.Params["ButtonViewPromoters"].Value = "View-Promoters";
        }

        [ActiveEvent(Name = "View-Promoters")]
        protected void ViewPromoters(object sender, ActiveEventArgs e)
        {
            Node node = new Node();

            // Columns...
            node["ModuleSettings"]["Grid"]["Columns"]["Email"]["Caption"].Value = Language.Instance["Email", null, "Email"];
            node["ModuleSettings"]["Grid"]["Columns"]["Email"]["ControlType"].Value = "Label";

            node["ModuleSettings"]["Grid"]["Columns"]["Code"]["Caption"].Value = Language.Instance["PromoCodeGridHeader", null, "Promo Code"];
            node["ModuleSettings"]["Grid"]["Columns"]["Code"]["ControlType"].Value = "Label";

            node["ModuleSettings"]["Grid"]["Columns"]["Cause"]["Caption"].Value = Language.Instance["PromoCodeCauseHeader", null, "Cause"];
            node["ModuleSettings"]["Grid"]["Columns"]["Cause"]["ControlType"].Value = "Label";

            node["ModuleSettings"]["Grid"]["Columns"]["Address"]["Caption"].Value = Language.Instance["Address", null, "Address"];
            node["ModuleSettings"]["Grid"]["Columns"]["Address"]["ControlType"].Value = "Label";
            foreach (PromoCode idx in PromoCode.Select())
            {
                node["ModuleSettings"]["Grid"]["Rows"]["Row" + idx.ID]["Email"].Value = idx.Email;
                node["ModuleSettings"]["Grid"]["Rows"]["Row" + idx.ID]["Code"].Value = idx.Code;
                node["ModuleSettings"]["Grid"]["Rows"]["Row" + idx.ID]["Cause"].Value = idx.Cause;
                node["ModuleSettings"]["Grid"]["Rows"]["Row" + idx.ID]["Address"].Value = idx.Address;
            }
            ActiveEvents.Instance.RaiseLoadControl(
                "PromoModules.ViewPromoters",
                "dynMid",
                node);
        }

        [ActiveEvent(Name = "Page_Init_InitialLoading")]
        protected void InitialLoadingOfPage(object sender, ActiveEventArgs e)
        {
            string contentId = HttpContext.Current.Request.Params["ContentID"];
            string promo = HttpContext.Current.Request.Params["promo"];
            if (!string.IsNullOrEmpty(promo))
            {
                HttpContext.Current.Session["PromoCode"] = promo;
            }
            if (contentId != null)
            {
                contentId = contentId.Trim('/');
                if (contentId == "promo-program")
                {
                    // Setting title of page
                    ((System.Web.UI.Page)HttpContext.Current.CurrentHandler).Title =
                        "Ra-Software Promo Program";
                    ActiveEvents.Instance.RaiseLoadControl(
                        "PromoModules.ApplyForPromoProgram",
                        "dynMid");
                }
            }
        }

        [ActiveEvent(Name = "ApplyForPromoCode")]
        protected void ApplyForPromoCode(object sender, ActiveEventArgs e)
        {
            e.Params["Success"].Value = false;
            string code = e.Params["Code"].Get<string>().Trim();
            string email = e.Params["Email"].Get<string>().Trim();
            string cause = e.Params["Cause"].Get<string>().Trim();
            if (code == Language.Instance["WantedPromoCode2", null, "Promo code desire"])
            {
                e.Params["Error"].Value = Language.Instance["NeedUniquePromoCode", null, "You need to create your own unique code. Type in e.g. your name or something. Don't use spaces in your code!"];
                return;
            }
            if (code.IndexOfAny(new char[] { ' ', '\t', '\r', '\n', '/', '&', '/', '\\', '%', '@' }) != -1)
            {
                e.Params["Error"].Value = Language.Instance["OnlyPromoCharacter", null, "Please use only alpha-numeric characters in your code!"];
                return;
            }
            if (code.Length < 3)
            {
                e.Params["Error"].Value = Language.Instance["PromoCodeNeedsThreeLetter", null, "You must choose a code with more than 3 letters."];
                return;
            }
            if (!email.Contains("@"))
            {
                e.Params["Error"].Value = Language.Instance["PromoCodeNeedValidEmail", null, "Unless you give us a valid email address, we won't be able to send you your money!"];
                return;
            }
            if (!email.Contains("."))
            {
                e.Params["Error"].Value = Language.Instance["PromoCodeNeedValidEmail", null, "Unless you give us a valid email address, we won't be able to send you your money!"];
                return;
            }
            if (PromoCode.CountWhere(Criteria.Eq("Code", code)) > 0)
            {
                e.Params["Error"].Value = Language.Instance["PromoCodeTake", null, "That promo code is unfortunately taken from before. Please choose another code"];
                return;
            }
            if (PromoCode.CountWhere(Criteria.Eq("Email", email)) > 0)
            {
                PromoCode previ = PromoCode.SelectFirst(Criteria.Eq("Email", email));
                e.Params["Error"].Value = Language.Instance["PromoCodeEmailRegisterd", null, "That email is already taken. The code for that email is: "] + previ.Code;
                return;
            }
            PromoCode promo = new PromoCode();
            promo.Code = code;
            promo.Email = email;
            promo.Cause = cause;
            promo.Save();
            e.Params["Success"].Value = true;

            // Sending confirmation email...
            string smtpServer = Settings.Instance["SMTPServer"];
            string smtpServerUsername = Settings.Instance["SMTPServerUsername"];
            string smtpServerPassword = Settings.Instance["SMTPServerPassword"];
            string smtpServerUseSsl = Settings.Instance["SMTPServerUseSsl"];
            string adminEmail = Settings.Instance["AdminEmail"];
            MailMessage msg = new MailMessage();
            msg.To.Add(email);
            msg.From = new MailAddress(adminEmail);
            msg.Subject = Language.Instance["PromoCodeProgramAccepted", null, "Welcome to our Promo Program"];
            msg.Body = string.Format(Language.Instance["PromoAcceptedMailBody", null, @"
This email was sent due to someone applying for a promo code at http://rasoftwarefactory.com/promo-program
If this person was not you, then please ignore this email.

Welcome to the Ra-Software Promo program. Your promo code is ""{0}"". When someone buys a website from http://rasoftwarefactory.com/products/websites with your promo code we will contact you on email and ask where you want us to send your cheque.

We will also donate $100 to {1} when someone uses your promo code to purchase our website product.

Make sure your friend, colleague, family member, uncle, partner, whatever punches in the *correct* promo code whenever they send us an email requesting a inquiry about a new website, since otherwise we have no way to determine who to send the $200 to.

All our ""contact us"" forms at http://rasoftwarefactory.com contains a field where people can type in a promo code. If they contact us for any reasons, and then later purchases our website product, you will get your money, and your cause will get its money.

If you want to have a link that directly will associate any ""contact us"" request to your promo code you can use this link:

http://rasoftwarefactory.com?promo={0}

If you give people the above link, all ""contact us"" request they send in after clicking that link will automatically associate any purchase with you.

We ask you to not send unsolicited emails [spam] with your promo code.

If you have questions, you can reply to this address.


Have a nice day :)

Thomas Hansen CEO at Ra-Software, Inc."], code, cause);
            msg.IsBodyHtml = false;
            SmtpClient smtp = new SmtpClient();
            smtp.Host = smtpServer;
            smtp.Credentials = new NetworkCredential(smtpServerUsername, smtpServerPassword);
            smtp.EnableSsl = smtpServerUseSsl == "True";
            smtp.Send(msg);
        }
    }
}
