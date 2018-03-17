using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.Models;
using Microsoft.EntityFrameworkCore;

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

        public void OnGet(int? id)
        {
            if (id.HasValue)
            {
                Client = _context.Clients
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
            }
            else
            {
                Client = new IdentityServer4.EntityFramework.Entities.Client();
            }
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            //Hash secrets
            foreach (var secret in Client.ClientSecrets)
            {
                secret.Value = secret.Value.Sha256();
            }

            //Convert grant types
            var grantTypes = (IList<string>)typeof(GrantTypes).GetProperties().Where(p => p.Name == GrantTypes).Single().GetValue(null);

            Client.AllowedGrantTypes = grantTypes.Select(g => new IdentityServer4.EntityFramework.Entities.ClientGrantType
            {
                GrantType = g
            }).ToList();

            _context.Clients.Add(Client);

            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}