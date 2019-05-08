#region Using Directives
using System;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Options;

using NewEraFlowerStore.Data;
#endregion Using Directives

namespace NewEraFlowerStore.Services
{
    public class EmailSender : IEmailSender
    {
        private readonly EmailSettings _emailSettings;

        public EmailSender(IOptions<EmailSettings> emailSettings)
        {
            _emailSettings = emailSettings.Value;
        } // end constructor EmailSender

        public Task SendEmailAsync(string email, string subject, string message)
        {
            try
            {
                var networkCredential = new NetworkCredential(_emailSettings.Sender, _emailSettings.Password);
                var mailMessage = new MailMessage()
                {
                    From = new MailAddress(_emailSettings.Sender, _emailSettings.SenderName),
                    Subject = subject,
                    Body = message,
                    IsBodyHtml = true
                }; // write a mail

                mailMessage.To.Add(new MailAddress(email));

                var smtpClient = new SmtpClient()
                {
                    Port = _emailSettings.MailPort,
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    UseDefaultCredentials = false,
                    Host = _emailSettings.MailServer,
                    EnableSsl = true,
                    Credentials = networkCredential
                };

                smtpClient.Send(mailMessage);
            }
            catch (Exception e)
            {
                throw new InvalidOperationException(e.Message);
            } // end try...catch

            return Task.CompletedTask;
        } // end method SendEmailAsync
    } // end class EmailSender
} // end namespace NewEraFlowerStore.Services