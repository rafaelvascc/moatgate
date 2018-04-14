using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Net.Mail;
using System.Text.Encodings.Web;

namespace MoatGate.Helpers
{
    public class DummyMessagingService : IEmailSender, ISmsSender
    {
        public Task SendEmailAsync(string senderEmail, string sendName, string email, string subject, string message)
        {
            MailMessage mail = new MailMessage(senderEmail, email)
            {
                Subject = subject,
                Body = message, 
                IsBodyHtml = true
            };
            SmtpClient client = new SmtpClient
            {
                Port = 25,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                Host = "127.0.0.1"
            };
            client.Send(mail);
            return Task.CompletedTask;
        }

        public async Task SendEmailChangeAlertEmailAsync(string sendTo)
        {
            await SendEmailAsync("admin@moatgate.com", "admin", sendTo, "Moatgate Email Change By Operator", ComposeEmailChangeAlertEmail());
        }

        public async Task SendEmailConfirmationEmailAsync(string sendTo, string callbackUrl)
        {
            await SendEmailAsync("admin@moatgate.com", "admin", sendTo, "Moatgate Email Confirmation", ComposeEmailConfirmationEmail(callbackUrl));
        }

        public async Task SendEmailPasswordResetAsync(string sendTo, string callbackUrl)
        {
            await SendEmailAsync("admin@moatgate.com", "admin", sendTo, "Moatgate Password Reset", ComposeResetPasswordEmail(callbackUrl));
        }

        public async Task SendPhoneNumberChangeAlertEmailAsync(string sendTo)
        {
            await SendEmailAsync("admin@moatgate.com", "admin", sendTo, "Moatgate Phone Number Change By Operator", ComposePhoneNumberChangeAlertEmail());
        }

        public async Task SendProfileChangeAlertEmailAsync(string sendTo)
        {
            await SendEmailAsync("admin@moatgate.com", "admin", sendTo, "Moatgate Profile Change By Operator", ComposeProfileChangeAlertEmail());
        }

        public async Task SendSMSAsync(string number, string message)
        {
            await SendEmailAsync("admin@moatgate.com", "admin", "dummy@dummy.com", $"SMS {number}", message);
        }

        public async Task SendPhoneNumberChangeConfirmationSMSAsync(string number, string callbackUrl)
        {
            await SendSMSAsync(number, ComposePhoneNumberConfirmationSms(callbackUrl));
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

        private string ComposePhoneNumberConfirmationSms(string callbackUrl)
        {
            return $"Please confirm your phone number <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.";
        }
    }
}
