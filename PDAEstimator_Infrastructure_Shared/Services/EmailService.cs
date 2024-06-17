using MailKit.Net.Smtp;
using MimeKit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDAEstimator_Infrastructure_Shared.Services
{
    public class EmailSender : IEmailSender
    {
        private readonly EmailConfiguration _emailConfig;
        public EmailSender(EmailConfiguration emailConfig)
        {
            _emailConfig = emailConfig;
        }
        public void SendEmail(Message message)
        {
            var emailMessage = CreateEmailMessage(message);
            Send(emailMessage);
        }

        private MimeMessage CreateEmailMessage(Message message)
        {
            if (message.FromCompany == "FromSamsara")
            {
                var emailMessage = new MimeMessage();
                emailMessage.From.Add(new MailboxAddress("email ", _emailConfig.FromSamsara));
                emailMessage.To.AddRange(message.To);
                emailMessage.Subject = message.Subject;
                //var bodyBuilder = new BodyBuilder();
                //bodyBuilder.HtmlBody = "<b>This is some html text</b>";
                //bodyBuilder.TextBody = "This is some plain text";
                emailMessage.Body = new TextPart(MimeKit.Text.TextFormat.Html) { Text = message.Content };
                return emailMessage;
            }
            else if (message.FromCompany == "FromMerchant")
            {
                var emailMessage = new MimeMessage();
                emailMessage.From.Add(new MailboxAddress("email ", _emailConfig.FromMerchant));
                emailMessage.To.AddRange(message.To);
                emailMessage.Subject = message.Subject;
                emailMessage.Body = new TextPart(MimeKit.Text.TextFormat.Html) { Text = message.Content };
                return emailMessage;
            }
            else 
            {
                var emailMessage = new MimeMessage();
                emailMessage.From.Add(new MailboxAddress("email ", _emailConfig.From));
                emailMessage.To.AddRange(message.To);
                emailMessage.Subject = message.Subject;
                emailMessage.Body = new TextPart(MimeKit.Text.TextFormat.Html) { Text = message.Content };
                return emailMessage;
            }
        }

        private void Send(MimeMessage mailMessage)
        {
            using (var client = new SmtpClient())
            {
                try
                {
                    client.Connect(_emailConfig.SmtpServer, _emailConfig.Port, false);
                    client.AuthenticationMechanisms.Remove("XOAUTH2");
                    client.Authenticate(_emailConfig.UserName, _emailConfig.Password);
                    client.Send(mailMessage);
                }
                catch
                {
                    //log an error message or throw an exception or both.
                    throw;
                }
                finally
                {
                    client.Disconnect(true);
                    client.Dispose();
                }
            }
        }
    }
}
