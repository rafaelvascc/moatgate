using System.Collections.Generic;

namespace MoatGate.Models.Consent
{
    public class ConsentViewModel
    {
        public bool RememberConsent { get; set; }
        public string ReturnUrl { get; set; }
        public string ClientName { get; set; }
        public string ClientUrl { get; set; }
        public string ClientLogoUrl { get; set; }
        public bool AllowRememberConsent { get; set; }

        public IEnumerable<ScopeViewModel> IdentityScopes { get; set; } = new List<ScopeViewModel>();
        public IEnumerable<ScopeViewModel> ResourceScopes { get; set; } = new List<ScopeViewModel>();
    }
}
