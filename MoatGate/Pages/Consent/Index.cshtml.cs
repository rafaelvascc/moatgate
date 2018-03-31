using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IdentityServer4.Models;
using IdentityServer4.Services;
using IdentityServer4.Stores;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using MoatGate.Models.Consent;

namespace MoatGate.Pages.Consent
{
    public class IndexModel : PageModel
    {
        private readonly IIdentityServerInteractionService _interaction;
        private readonly IClientStore _clientStore;
        private readonly IResourceStore _resourceStore;
        private readonly ILogger<IndexModel> _logger;

        [BindProperty]
        public ConsentViewModel ConsentData { set; get; } = new ConsentViewModel();

        public IndexModel(IIdentityServerInteractionService interaction, IClientStore clientStore, IResourceStore resourceStore, ILogger<IndexModel> logger)
        {
            _interaction = interaction;
            _clientStore = clientStore;
            _resourceStore = resourceStore;
            _logger = logger;
        }

        public async Task OnGetAsync(string returnUrl)
        {
            await BuildViewModelAsync(returnUrl);
        }

        public async Task<IActionResult> OnPostAllowAsync()
        {
            return await ProcessPostRequest(true);
        }

        public async Task<IActionResult> OnPostDenyAsync()
        {
            return await ProcessPostRequest(false);
        }

        private async Task<IActionResult> ProcessPostRequest(bool consetAllowed)
        {
            var scopesConsented = ConsentData.IdentityScopes.Union(ConsentData.ResourceScopes).Where(c => c.Checked).Select(c => c.Name).ToList();

            var result = await ProcessConsent(consetAllowed, scopesConsented);

            if (result.IsRedirect)
            {
                return Redirect(result.RedirectUri);
            }

            if (result.HasValidationError)
            {
                ModelState.AddModelError("", result.ValidationError);
            }

            return Page();
        }

        private async Task<ProcessConsentResult> ProcessConsent(bool consetAllowed, IList<string> consentedScopes)
        {
            var result = new ProcessConsentResult();

            ConsentResponse grantedConsent = null;

            if (consetAllowed)
            {
                if (consentedScopes?.Count > 0)
                {
                    if (ConsentOptions.EnableOfflineAccess == false)
                    {
                        consentedScopes = consentedScopes.Where(s => s != IdentityServer4.IdentityServerConstants.StandardScopes.OfflineAccess).ToList();
                    }

                    grantedConsent = new ConsentResponse
                    {
                        RememberConsent = ConsentData.RememberConsent,
                        ScopesConsented = consentedScopes
                    };
                }
                else
                {
                    result.ValidationError = ConsentOptions.MustChooseOneErrorMessage;
                }
            }
            else
            {
                grantedConsent = ConsentResponse.Denied;
            }

            if (grantedConsent != null)
            {
                // validate return url is still valid
                var request = await _interaction.GetAuthorizationContextAsync(ConsentData.ReturnUrl);
                if (request == null) return result;

                // communicate outcome of consent back to identityserver
                await _interaction.GrantConsentAsync(request, grantedConsent);

                // indicate that's it ok to redirect back to authorization endpoint
                result.RedirectUri = ConsentData.ReturnUrl;
            }
            else
            {
                // we need to redisplay the consent UI
                await BuildViewModelAsync(ConsentData.ReturnUrl, consentedScopes);
            }

            return result;
        }

        private async Task BuildViewModelAsync(string returnUrl, IList<string> consentedScopes = null)
        {
            var request = await _interaction.GetAuthorizationContextAsync(returnUrl);
            if (request != null)
            {
                var client = await _clientStore.FindEnabledClientByIdAsync(request.ClientId);
                if (client != null)
                {
                    var resources = await _resourceStore.FindEnabledResourcesByScopeAsync(request.ScopesRequested);
                    if (resources != null && (resources.IdentityResources.Any() || resources.ApiResources.Any()))
                    {
                        CreateConsentViewModel(returnUrl, request, client, resources, consentedScopes);
                    }
                    else
                    {
                        _logger.LogError("No scopes matching: {0}", request.ScopesRequested.Aggregate((x, y) => x + ", " + y));
                    }
                }
                else
                {
                    _logger.LogError("Invalid client id: {0}", request.ClientId);
                }
            }
            else
            {
                _logger.LogError("No consent request matching request: {0}", returnUrl);
            }
        }

        private void CreateConsentViewModel(string returnUrl, AuthorizationRequest request, IdentityServer4.Models.Client client, 
            IdentityServer4.Models.Resources resources, IList<string> consentedScopes)
        {
            ConsentData.ReturnUrl = returnUrl;
            ConsentData.ClientName = client.ClientName ?? client.ClientId;
            ConsentData.ClientUrl = client.ClientUri;
            ConsentData.ClientLogoUrl = client.LogoUri;
            ConsentData.AllowRememberConsent = client.AllowRememberConsent;

            ConsentData.IdentityScopes = resources.IdentityResources.Select(x => CreateScopeViewModel(x, consentedScopes == null || consentedScopes.Contains(x.Name))).ToArray();
            ConsentData.ResourceScopes = resources.ApiResources.SelectMany(x => x.Scopes).Select(x => CreateScopeViewModel(x, consentedScopes == null || consentedScopes.Contains(x.Name))).ToArray();
            if (ConsentOptions.EnableOfflineAccess && resources.OfflineAccess)
            {
                ConsentData.ResourceScopes = ConsentData.ResourceScopes.Union(new ScopeViewModel[] {
                    GetOfflineAccessScope(consentedScopes == null || consentedScopes.Contains(IdentityServer4.IdentityServerConstants.StandardScopes.OfflineAccess))
                });
            }
        }

        private ScopeViewModel CreateScopeViewModel(IdentityResource identity, bool check)
        {
            return new ScopeViewModel
            {
                Name = identity.Name,
                DisplayName = identity.DisplayName,
                Description = identity.Description,
                Emphasize = identity.Emphasize,
                Required = identity.Required,
                Checked = check || identity.Required
            };
        }

        public ScopeViewModel CreateScopeViewModel(Scope scope, bool check)
        {
            return new ScopeViewModel
            {
                Name = scope.Name,
                DisplayName = scope.DisplayName,
                Description = scope.Description,
                Emphasize = scope.Emphasize,
                Required = scope.Required,
                Checked = check || scope.Required
            };
        }

        private ScopeViewModel GetOfflineAccessScope(bool check)
        {
            return new ScopeViewModel
            {
                Name = IdentityServer4.IdentityServerConstants.StandardScopes.OfflineAccess,
                DisplayName = ConsentOptions.OfflineAccessDisplayName,
                Description = ConsentOptions.OfflineAccessDescription,
                Emphasize = true,
                Checked = check
            };
        }
    }
}
