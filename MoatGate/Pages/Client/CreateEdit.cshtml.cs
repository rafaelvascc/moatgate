using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.Models;
using Microsoft.EntityFrameworkCore;
using AutoMapper;
using MoatGate.Helpers;
using System;
using IdentityServer4.Stores;

namespace MoatGate.Pages.Client
{
    public class CreateEditModel : PageModel
    {
        private readonly ConfigurationDbContext _context;

        public CreateEditModel(ConfigurationDbContext context, IResourceStore store)
        {
            _context = context; 
        }

        [BindProperty]
        public IdentityServer4.EntityFramework.Entities.Client Client { set; get; }

        [BindProperty]
        public string GrantTypes { set; get; } = "ClientCredentials";

        public void OnGet(int? id)
        {
            if (id.HasValue)
            {
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
                ViewData["Title"] = "Create Client";
                Client = new IdentityServer4.EntityFramework.Entities.Client();
                Client.ClientSecrets = new List<IdentityServer4.EntityFramework.Entities.ClientSecret>();
                Client.RedirectUris = new List<IdentityServer4.EntityFramework.Entities.ClientRedirectUri>();
                Client.AllowedScopes = new List<IdentityServer4.EntityFramework.Entities.ClientScope>();
                Client.Properties = new List<IdentityServer4.EntityFramework.Entities.ClientProperty>();
                Client.PostLogoutRedirectUris = new List<IdentityServer4.EntityFramework.Entities.ClientPostLogoutRedirectUri>();
                Client.IdentityProviderRestrictions = new List<IdentityServer4.EntityFramework.Entities.ClientIdPRestriction>();
                Client.AllowedCorsOrigins = new List<IdentityServer4.EntityFramework.Entities.ClientCorsOrigin>();
                Client.Claims = new List<IdentityServer4.EntityFramework.Entities.ClientClaim>();
            }
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
                foreach (var secret in Client.ClientSecrets)
                {
                    secret.Value = secret.Value.Sha256();
                }

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
                
                Mapper.Map(Client, CurrentClient);
                CurrentClient.AllowedCorsOrigins.ReflectEntityFrameworkState(Client.AllowedCorsOrigins, _context);
                CurrentClient.AllowedGrantTypes.ReflectEntityFrameworkState(Client.AllowedGrantTypes, _context);
                CurrentClient.AllowedScopes.ReflectEntityFrameworkState(Client.AllowedScopes, _context);
                CurrentClient.Claims.ReflectEntityFrameworkState(Client.Claims, _context);
                CurrentClient.ClientSecrets.ReflectEntityFrameworkState(Client.ClientSecrets, _context);
                CurrentClient.IdentityProviderRestrictions.ReflectEntityFrameworkState(Client.IdentityProviderRestrictions, _context);
                CurrentClient.PostLogoutRedirectUris.ReflectEntityFrameworkState(Client.PostLogoutRedirectUris, _context);
                CurrentClient.Properties.ReflectEntityFrameworkState(Client.Properties, _context);
                CurrentClient.RedirectUris.ReflectEntityFrameworkState(Client.RedirectUris, _context);

                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }
    }
}