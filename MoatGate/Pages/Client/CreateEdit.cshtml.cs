using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.Models;
using Microsoft.EntityFrameworkCore;
using AutoMapper;
using MoatGate.Services;
using System;
using Microsoft.AspNetCore.Mvc.Rendering;
using IdentityServer4.EntityFramework.Mappers;
using IdentityServer4.EntityFramework.Entities;

namespace MoatGate.Pages.Client
{
    public class CreateEditModel : PageModel
    {
        private readonly ConfigurationDbContext _context;

        public CreateEditModel(ConfigurationDbContext context)
        {
            _context = context; 
        }

        [BindProperty]
        public IdentityServer4.EntityFramework.Entities.Client Client { set; get; }

        [BindProperty]
        public string GrantTypes { set; get; } = "ClientCredentials";

        [BindProperty]
        public List<string> SelectedAllowedScopes { set; get; } = new List<string>();

        public async Task<IActionResult> OnGet(int? id)
        {            
            if (id.HasValue)
            {
                ViewData["Editing"] = true;
                ViewData["Title"] = "Edit Client";
                Client = _context.Clients.AsNoTracking()
                    .Include(c => c.AllowedCorsOrigins)
                    .Include(c => c.AllowedGrantTypes)
                    .Include(c => c.AllowedScopes)
                    .Include(c => c.Claims)
                    .Include(c => c.ClientSecrets)
                    .Include(c => c.IdentityProviderRestrictions)
                    .Include(c => c.PostLogoutRedirectUris)
                    .Include(c => c.Properties)
                    .Include(c => c.RedirectUris)
                    .SingleOrDefault(c => c.Id == id);

                //Convert grant types
                GrantTypes = typeof(GrantTypes).GetProperties()
                    .Where(p => Client.AllowedGrantTypes.Count == ((IList<string>)p.GetValue(null)).Count &&
                    ((IList<string>)p.GetValue(null)).All(g => Client.AllowedGrantTypes.Select(gt => gt.GrantType).Contains(g))).SingleOrDefault()?.Name ?? "ClientCredentials";
            }
            else
            {
                ViewData["Editing"] = false;
                ViewData["Title"] = "Create Client";
                Client = new IdentityServer4.EntityFramework.Entities.Client
                {
                    ClientSecrets = new List<IdentityServer4.EntityFramework.Entities.ClientSecret>(),
                    RedirectUris = new List<IdentityServer4.EntityFramework.Entities.ClientRedirectUri>(),
                    AllowedScopes = new List<IdentityServer4.EntityFramework.Entities.ClientScope>(),
                    Properties = new List<IdentityServer4.EntityFramework.Entities.ClientProperty>(),
                    PostLogoutRedirectUris = new List<IdentityServer4.EntityFramework.Entities.ClientPostLogoutRedirectUri>(),
                    IdentityProviderRestrictions = new List<IdentityServer4.EntityFramework.Entities.ClientIdPRestriction>(),
                    AllowedCorsOrigins = new List<IdentityServer4.EntityFramework.Entities.ClientCorsOrigin>(),
                    Claims = new List<IdentityServer4.EntityFramework.Entities.ClientClaim>()
                };
            }

            var allowedIdentityScopeOptions = new List<SelectListItem>();
            var allowedApiScopeOptions = new List<SelectListItem>();

            var identityResourceOptionsGroup = new SelectListGroup()
            {
                Name = "Identity Resource"
            };

            var apiResourceOptionsGroup = new SelectListGroup()
            {
                Name = "Api Resource"
            };

            var userClaimsOptions = new List<SelectListItem>();
            var allApiResources = (await _context.ApiResources.Include(r => r.Scopes).AsNoTracking().ToListAsync()).Select(i => i.ToModel());
            var allIdentityResources = await _context.IdentityResources.Select(r => r.Name).AsNoTracking().ToListAsync();
            allIdentityResources.Add("offline_access");

            if (allApiResources.Any() && allApiResources.SelectMany(r => r.Scopes).Any())
            {
                allowedApiScopeOptions = allApiResources.SelectMany(c => c.Scopes).Distinct().Select(s =>
                {
                    var scopeOf = string.Join(", ", allApiResources.Where(r => r.Scopes.Any(rs => rs.Name == s.Name)).Select(r => r.Name).OrderBy(n => n).ToList());
                    var optionText = $"{s.Name} ({scopeOf})";
                    return new SelectListItem(optionText, s.Name, false, false)
                    {
                        Group = apiResourceOptionsGroup
                    };
                }).ToList();
            }

            if (allIdentityResources.Any())
            {
                allowedIdentityScopeOptions = allIdentityResources.Distinct().OrderBy(n => n).Select(s =>
                {
                    return new SelectListItem(s, s, false, false)
                    {
                        Group = identityResourceOptionsGroup
                    };
                }).ToList();
            }

            ViewData["AllowedScopeOptions"] = allowedApiScopeOptions.Union(allowedIdentityScopeOptions).ToList();

            if (Client.AllowedScopes != null && Client.AllowedScopes.Any())
            {
                SelectedAllowedScopes = Client.AllowedScopes.Select(s => s.Scope).ToList();
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            //Convert grant types
            var grantTypes = (IList<string>)typeof(GrantTypes).GetProperties().Where(p => p.Name == GrantTypes).Single().GetValue(null);

            Client.AllowedGrantTypes = grantTypes.Select(g => new IdentityServer4.EntityFramework.Entities.ClientGrantType
            {
                GrantType = g
            }).ToList();

            if (Client.Id == 0)
            {
                //Hash secrets
                if (Client.ClientSecrets?.Count > 0)
                {
                    foreach (var secret in Client.ClientSecrets)
                    {
                        secret.Value = secret.Value.Sha256();
                    }
                }

                Client.AllowedScopes = SelectedAllowedScopes.Select(s => new ClientScope() { Scope = s }).ToList();

                _context.Clients.Add(Client);
                await _context.SaveChangesAsync();
            }
            else
            {
                var CurrentClient = _context.Clients
                    .Include(c => c.AllowedCorsOrigins)
                    .Include(c => c.AllowedGrantTypes)
                    .Include(c => c.AllowedScopes)
                    .Include(c => c.Claims)
                    .Include(c => c.ClientSecrets)
                    .Include(c => c.IdentityProviderRestrictions)
                    .Include(c => c.PostLogoutRedirectUris)
                    .Include(c => c.Properties)
                    .Include(c => c.RedirectUris)
                    .SingleOrDefault(c => c.Id == Client.Id);

                Client.AllowedScopes = SelectedAllowedScopes.Select(s => new ClientScope() { Scope = s }).ToList();

                Mapper.Map(Client, CurrentClient);
                CurrentClient.AllowedCorsOrigins.ReflectToEntityFrameworkState(Client.AllowedCorsOrigins, _context);
                CurrentClient.AllowedGrantTypes.ReflectToEntityFrameworkState(Client.AllowedGrantTypes, _context);
                CurrentClient.AllowedScopes.ReflectToEntityFrameworkState(Client.AllowedScopes, _context);
                CurrentClient.Claims.ReflectToEntityFrameworkState(Client.Claims, _context);
                CurrentClient.ClientSecrets.ReflectToEntityFrameworkState(Client.ClientSecrets, _context);
                CurrentClient.IdentityProviderRestrictions.ReflectToEntityFrameworkState(Client.IdentityProviderRestrictions, _context);
                CurrentClient.PostLogoutRedirectUris.ReflectToEntityFrameworkState(Client.PostLogoutRedirectUris, _context);
                CurrentClient.Properties.ReflectToEntityFrameworkState(Client.Properties, _context);
                CurrentClient.RedirectUris.ReflectToEntityFrameworkState(Client.RedirectUris, _context);

                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./List");
        }
    }
}