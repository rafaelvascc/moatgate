using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using MoatGate.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MoatGate
{
    public class EnvironmentConfigLoader
    {
        private readonly IHostingEnvironment _env;
        private readonly IConfiguration _configuration;

        public EnvironmentConfigLoader(IHostingEnvironment env, IConfiguration configuration)
        {
            _env = env;
            _configuration = configuration;
        }

        public string GetDbConnectionString()
        {
            var identityServerConnectionString = Environment.GetEnvironmentVariable(EnvironmentVariablesKeys.DbConnectionString);

            if (_env.IsDevelopment() || string.IsNullOrEmpty(identityServerConnectionString))
            {
                identityServerConnectionString = _configuration.GetConnectionString("MoatGateIdentityServerConnectionString");
            }

            return identityServerConnectionString;
        }

        public (bool, string) GetRedisOptions()
        {
            var useRedisTicketStoreConfig = Environment.GetEnvironmentVariable(EnvironmentVariablesKeys.UseRedisTicketStore);

            if (_env.IsDevelopment() || string.IsNullOrEmpty(useRedisTicketStoreConfig) || !bool.TryParse(useRedisTicketStoreConfig, out var useRedisTicketStore))
            {
                useRedisTicketStore = _configuration.GetValue<bool>("UseRedisTicketStore");
            }

            var redisConnectionString = Environment.GetEnvironmentVariable(EnvironmentVariablesKeys.RedisConnectionString);

            if (_env.IsDevelopment() || string.IsNullOrEmpty(redisConnectionString))
            {
                redisConnectionString = _configuration.GetConnectionString("RedisConnectionString");
            }

            return (useRedisTicketStore, redisConnectionString);
        }

        public (bool, SendGridOptions) GetSendGridOptions()
        {
            var useSendgridConfig = Environment.GetEnvironmentVariable(EnvironmentVariablesKeys.UseSendGrid);

            if (_env.IsDevelopment() || string.IsNullOrEmpty(useSendgridConfig) || !bool.TryParse(useSendgridConfig, out var useSendgrid))
            {
                useSendgrid = _configuration.GetValue<bool>("UseSendGrid");
            }

            var sendgridUser = Environment.GetEnvironmentVariable(EnvironmentVariablesKeys.SendGridUser);

            if (_env.IsDevelopment() || string.IsNullOrEmpty(sendgridUser))
            {
                sendgridUser = _configuration.GetValue<string>("SendGridUser");
            }

            var sendgridKey = Environment.GetEnvironmentVariable(EnvironmentVariablesKeys.SendGridKey);

            if (_env.IsDevelopment() || string.IsNullOrEmpty(sendgridKey))
            {
                sendgridKey = _configuration.GetValue<string>("SendGridKey");
            }

            return (useSendgrid, new SendGridOptions
            {
                SendGridKey = sendgridKey,
                SendGridUser = sendgridUser
            });
        }

        public (bool, TwilioOptions) GetTwillioOptions()
        {
            var useTwilioConfig = Environment.GetEnvironmentVariable(EnvironmentVariablesKeys.UseTwilio);

            if (_env.IsDevelopment() || string.IsNullOrEmpty(useTwilioConfig) || !bool.TryParse(useTwilioConfig, out var useTwilio))
            {
                useTwilio = _configuration.GetValue<bool>("UseTwilio");
            }

            var twilioAccountSid = Environment.GetEnvironmentVariable(EnvironmentVariablesKeys.TwilioAccountSid);

            if (_env.IsDevelopment() || string.IsNullOrEmpty(twilioAccountSid))
            {
                twilioAccountSid = _configuration.GetValue<string>("TwilioAccountSid");
            }

            var twilioAccountPassword = Environment.GetEnvironmentVariable(EnvironmentVariablesKeys.TwilioAccountPassword);

            if (_env.IsDevelopment() || string.IsNullOrEmpty(twilioAccountPassword))
            {
                twilioAccountPassword = _configuration.GetValue<string>("TwilioAccountPassword");
            }

            var twilioAccountFromNumber = Environment.GetEnvironmentVariable(EnvironmentVariablesKeys.TwilioAccountFromNumber);

            if (_env.IsDevelopment() || string.IsNullOrEmpty(twilioAccountFromNumber))
            {
                twilioAccountFromNumber = _configuration.GetValue<string>("TwilioAccountFromNumber");
            }

            var twilioTestAccountSid = Environment.GetEnvironmentVariable(EnvironmentVariablesKeys.TwilioTestAccountSid);

            if (_env.IsDevelopment() || string.IsNullOrEmpty(twilioTestAccountSid))
            {
                twilioTestAccountSid = _configuration.GetValue<string>("TwilioTestAccountSid");
            }

            var twilioTestAccountPassword = Environment.GetEnvironmentVariable(EnvironmentVariablesKeys.TwilioTestAccountPassword);

            if (_env.IsDevelopment() || string.IsNullOrEmpty(twilioTestAccountPassword))
            {
                twilioTestAccountPassword = _configuration.GetValue<string>("TwilioTestAccountPassword");
            }

            var twilioTestAccountFromNumber = Environment.GetEnvironmentVariable(EnvironmentVariablesKeys.TwilioTestAccountFromNumber);

            if (_env.IsDevelopment() || string.IsNullOrEmpty(twilioTestAccountFromNumber))
            {
                twilioTestAccountFromNumber = _configuration.GetValue<string>("TwilioTestAccountFromNumber");
            }

            return (useTwilio, new TwilioOptions
            {
                TwilioAccountSid = twilioAccountSid,
                TwilioAccountPassword = twilioAccountPassword,
                TwilioAccountFromNumber = twilioAccountFromNumber,
                TwilioTestAccountSid = twilioTestAccountSid,
                TwilioTestAccountPassword = twilioTestAccountPassword,
                TwilioTestAccountFromNumber = twilioTestAccountFromNumber
            });
        }

        public string GetApplicationUrl()
        {
            var applicationUrl = Environment.GetEnvironmentVariable(EnvironmentVariablesKeys.ApplicationUrl);

            if (_env.IsDevelopment() || string.IsNullOrEmpty(applicationUrl))
            {
                applicationUrl = _configuration.GetValue<string>("ApplicationUrl");
            }

            return applicationUrl;
        }
    }
}
