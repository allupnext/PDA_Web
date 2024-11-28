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
        public void SendEmail(Message message, string filepath = "")
        {
            var emailMessage = CreateEmailMessage(message, filepath);
            Send(emailMessage);
        }

        private MimeMessage CreateEmailMessage(Message message, string filepath="")
        {
            var emailMessage = new MimeMessage();
            if (message.FromCompany == "FromSamsara")
            {
                emailMessage.From.Add(new MailboxAddress(_emailConfig.FromSamsara, _emailConfig.FromSamsara));
            }
            else if (message.FromCompany == "FromMerchant")
            {
                emailMessage.From.Add(new MailboxAddress(_emailConfig.FromMerchant, _emailConfig.FromMerchant));
            }
            else
            {
                if (!string.IsNullOrEmpty(message.FromCompany))
                    emailMessage.From.Add(new MailboxAddress(message.FromCompany, message.FromCompany));
                else
                    emailMessage.From.Add(new MailboxAddress(_emailConfig.FromMerchant, _emailConfig.FromMerchant));
            }
            emailMessage.To.AddRange(message.To);
            emailMessage.Cc.AddRange(message.Cc);
            emailMessage.Subject = message.Subject;

            
            BodyBuilder emailBodyBuilder = new BodyBuilder();
            emailBodyBuilder.HtmlBody = message.Content;
            if(filepath != "")      
                emailBodyBuilder.Attachments.Add(filepath);
            emailMessage.Body = emailBodyBuilder.ToMessageBody();
            return emailMessage;
        }

        private void Send(MimeMessage mailMessage)
        {
            using (var client = new SmtpClient())
            {
                try
                {
                    client.Connect(_emailConfig.SmtpServer, _emailConfig.Port, false);
                    client.AuthenticationMechanisms.Remove("XOAUTH2");
                    //client.Authenticate(_emailConfig.UserName, _emailConfig.Password);
                    client.Send(mailMessage);
                }
                catch
                {
                    //log an error message or throw an exception or both.
                    return;
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
