using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;

namespace SchedulingBlocks.Email
{
    public class Emailer
    {
        private string SmtpHost { get; set; }

        private int SmtpPort { get; set; }

        private string NotificationEmail { get; set; }

        private string NotificationEmailPassword { get; set; }

        private bool SmtpSsl { get; set; }

        public Emailer()
        {
            SmtpHost = ConfigurationManager.AppSettings["SmtpHost"];
            SmtpPort = Int32.Parse(ConfigurationManager.AppSettings["SmtpPort"]);
            NotificationEmail = ConfigurationManager.AppSettings["NotificationEmail"];
            NotificationEmailPassword = ConfigurationManager.AppSettings["NotificationEmailPassword"];
            SmtpSsl = ConfigurationManager.AppSettings["SmtpSsl"] == "true";
        }

        public void SendEmail(string toAddress, string toName, string subject, string htmlBody)
        {
            try
            {
                var smtp = new SmtpClient(SmtpHost, SmtpPort);
                smtp.UseDefaultCredentials = false;
                smtp.Credentials = new NetworkCredential(NotificationEmail, NotificationEmailPassword);
                smtp.EnableSsl = SmtpSsl;
                smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
                var from = new MailAddress("noreply@moorebattingcage.com", "Moore Batting Cage");
                var to = new MailAddress(toAddress, toName);
                var email = new MailMessage(from, to);
                email.Subject = subject;
                email.Body = htmlBody;
                email.IsBodyHtml = true;
                smtp.Send(email);
            }
            catch (Exception emailEx)
            {
                //TODO: Log or something
            }
        }
    }
}