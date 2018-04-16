using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Twilio;
using Twilio.Rest.Api.V2010.Account;
using Twilio.Types;

namespace MoatGate.Services
{
    public class TwilioSmsSender : ISmsSender
    {
        private readonly TwilioOptions _options;

        public TwilioSmsSender(IOptions<TwilioOptions> optionsAcessor)
        {
            _options = optionsAcessor.Value;
        }

        public async Task SendSMSAsync(string number, string message)
        {
            // Plug in your SMS service here to send a text message.
            // Your Account SID from twilio.com/console
            var accountSid = _options.TwilioAccountSid;
            // Your Auth Token from twilio.com/console
            var authToken = _options.TwilioAccountPassword;

            TwilioClient.Init(accountSid, authToken);

            await MessageResource.CreateAsync(
              to: new PhoneNumber(number),
              from: new PhoneNumber(_options.TwilioAccountFromNumber),
              body: message);
        }

        public async Task SendPhoneNumberChangeConfirmationSMSAsync(string number, string callbackUrl)
        {
            await SendSMSAsync(number, ComposePhoneNumberConfirmationSms(callbackUrl));
        }
                
        private string ComposePhoneNumberConfirmationSms(string callbackUrl)
        {
            return $"Confir phone URL {callbackUrl}";
        }
    }
}
