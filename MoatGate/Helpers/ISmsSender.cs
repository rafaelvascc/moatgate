using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MoatGate.Helpers
{
    public interface ISmsSender
    {
        Task SendSMSAsync(string number, string message);
        Task SendPhoneNumberChangeConfirmationSMSAsync(string number, string callbackUrl);
    }
}
