using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MoatGate.Helpers
{
    public class TwilioOptions
    {
        public string TwilioAccountSid { get; set; }
        public string TwilioAccountPassword { get; set; }
        public string TwilioAccountFromNumber { get; set; }
        public string TwilioTestAccountSid { get; set; }
        public string TwilioTestAccountPassword { get; set; }
        public string TwilioTestAccountFromNumber { get; set; }
    }
}
