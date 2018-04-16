using Microsoft.Extensions.Options;
using SendGrid;
using SendGrid.Helpers.Mail;
using System;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace MoatGate.Services
{
    public class SendGridEmailSender : IEmailSender
    {
        public SendGridOptions Options { get; } //set only via Secret Manager

        public SendGridEmailSender(IOptions<SendGridOptions> optionsAccessor)
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

        public async Task SendEmailPasswordResetAsync(string sendTo, string callbackUrl)
        {
            await SendEmailAsync("admin@moatgate.com", "admin", sendTo, "Moatgate Password Reset", ComposeResetPasswordEmail(callbackUrl));
        }

        public async Task SendEmailConfirmationEmailAsync(string sendTo, string callbackUrl)
        {
            await SendEmailAsync("admin@moatgate.com", "admin", sendTo, "Moatgate Email Confirmation", ComposeEmailConfirmationEmail(callbackUrl));
        }

        public async Task SendEmailChangeAlertEmailAsync(string sendTo)
        {
            await SendEmailAsync("admin@moatgate.com", "admin", sendTo, "Moatgate Email Change By Operator", ComposeEmailChangeAlertEmail());
        }

        public async Task SendPhoneNumberChangeAlertEmailAsync(string sendTo)
        {
            await SendEmailAsync("admin@moatgate.com", "admin", sendTo, "Moatgate Phone Number Change By Operator", ComposePhoneNumberChangeAlertEmail());
        }

        public async Task SendProfileChangeAlertEmailAsync(string sendTo)
        {
            await SendEmailAsync("admin@moatgate.com", "admin", sendTo, "Moatgate Profile Change By Operator", ComposeProfileChangeAlertEmail());
        }

        private string ComposeResetPasswordEmail(string callbackUrl)
        {
            return $"Please reset your password by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.";
        }

        private string ComposeEmailConfirmationEmail(string callbackUrl)
        {
            return $"Please confirm your email by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.";
        }

        private string ComposeEmailChangeAlertEmail()
        {
            return $"A Moatgate operator changed your account email. If this operation was not authorized by you, contact us as soon as possible.";
        }

        private string ComposePhoneNumberChangeAlertEmail()
        {
            return $"A Moatgate operator changed your account phone number. If this operation was not authorized by you, contact us as soon as possible.";
        }

        private string ComposeProfileChangeAlertEmail()
        {
            return $"A Moatgate operator changed your account profile. If this operation was not authorized by you, contact us as soon as possible.";
        }
    }
}