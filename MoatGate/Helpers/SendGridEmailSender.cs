using Microsoft.Extensions.Options;
using SendGrid;
using SendGrid.Helpers.Mail;
using System;
using System.Threading.Tasks;

namespace MoatGate.Helpers
{
    public interface IEmailSender
    {
        Task SendEmailAsync(string senderEmail, string sendName, string email, string subject, string message);
    }

    public class SendGridEmailSender : IEmailSender
    {
        public AuthMessageSenderOptions Options { get; } //set only via Secret Manager

        public SendGridEmailSender(IOptions<AuthMessageSenderOptions> optionsAccessor)
        {
            Options = optionsAccessor.Value;
        }

        public Task SendEmailAsync(string senderEmail, string senderName, string email, string subject, string message)
        {
            var client = new SendGridClient(Options.SendGridKey);
            var msg = new SendGridMessage()
            {
                From = new EmailAddress(senderEmail, senderName),
                Subject = subject,
                PlainTextContent = message,
                HtmlContent = message
            };
            msg.AddTo(new EmailAddress(email));
            return client.SendEmailAsync(msg);
        }
    }
}