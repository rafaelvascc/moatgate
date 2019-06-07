using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MoatGate
{
    public static class EnvironmentVariablesKeys
    {
        public static string ApplicationUrl => "MOATGATE_APPLICATION_URL";

        public static string DbConnectionString => "MOATGATE_DB_CONNECTION_STRING";

        public static string UseRedisTicketStore => "MOATGATE_USE_REDIS_TICKET_STORE";
        public static string RedisConnectionString => "MOATGATE_REDIS_CONNECTION_STRING";

        public static string UseSendGrid => "MOATGATE_USE_SENDGRID";
        public static string SendGridUser => "MOATGATE_SENDGRID_USER";
        public static string SendGridKey => "MOATGATE_SENDGRID_KEY";

        public static string UseTwilio => "MOATGATE_USE_TWILLIO";
        public static string TwilioAccountSid => "MOATGATE_TWILLIO_ACCOUNT_SID";
        public static string TwilioAccountPassword => "MOATGATE_TWILLIO_ACCOUNT_PASSWORD";
        public static string TwilioAccountFromNumber => "MOATGATE_TWILLIO_FROM_NUMBER";
        public static string TwilioTestAccountSid => "MOATGATE_TWILLIO_TEST_ACCOUNT_SID";
        public static string TwilioTestAccountPassword => "MOATGATE_TWILLIO_TEST_ACCOUNT_PASSWORD";
        public static string TwilioTestAccountFromNumber => "MOATGATE_TWILLIO_TEST_FROM_NUMBER";
    }
}
