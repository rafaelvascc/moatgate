using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MoatGate.Services
{
    public interface IEmailSender
    {
        Task SendEmailAsync(string senderEmail, string sendName, string sendTo, string subject, string message);
        Task SendEmailPasswordResetAsync(string sendTo, string callbackUrl);
        Task SendEmailConfirmationEmailAsync(string sendTo, string callbackUrl);
        Task SendEmailChangeAlertEmailAsync(string sendTo);
        Task SendProfileChangeAlertEmailAsync(string sendTo);
        Task SendPhoneNumberChangeAlertEmailAsync(string sendTo);
    }
}
