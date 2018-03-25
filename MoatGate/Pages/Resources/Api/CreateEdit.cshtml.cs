using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using MoatGate.Models.AspNetIIdentityCore.EntityFramework;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using IdentityServer4.EntityFramework.Entities;
using IdentityServer4.Stores;
using IdentityServer4.EntityFramework.DbContexts;
using Microsoft.EntityFrameworkCore;
using IdentityServer4.Models;
using AutoMapper;
using MoatGate.Helpers;

namespace MoatGate.Pages.Resources.Api
{
    public class ApiResourceScopeChecksModel
    {
        public string Required { set; get; }
        public string Emphasize { set; get; }
        public string ShowInDiscoveryDocument { set; get; }
    }

    public class CreateEditModel : PageModel
    {
        private readonly ConfigurationDbContext _context;

        [BindProperty]
        public IdentityServer4.EntityFramework.Entities.ApiResource ApiResource { get; set; } = new IdentityServer4.EntityFramework.Entities.ApiResource
        {
            Scopes = new List<ApiScope>(),
            Secrets = new List<ApiSecret>(),
            UserClaims = new List<ApiResourceClaim>()
        };

        [BindProperty]
        public List<ApiResourceScopeChecksModel> ScopeChecks { set; get; } = new List<ApiResourceScopeChecksModel>();

        public CreateEditModel(ConfigurationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> OnGet(int? id)
        {
            if (id.HasValue)
            {
                ViewData["Title"] = "Edit Api Resource";
                ApiResource = await _context.ApiResources
                    .AsNoTracking()
                    .Include(a => a.Scopes)
                    .Include(a => a.Secrets)
                    .Include(a => a.UserClaims)
                    .SingleOrDefaultAsync(r => r.Id == id.Value);
            }
            else
            {
                ViewData["Title"] = "Create ApiResource";
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            if (ApiResource.Id == 0)
            {
                //Hash secrets
                if (ApiResource?.Secrets?.Count > 0)
                {
                    foreach (var secret in ApiResource.Secrets)
                    {
                        secret.Value = secret.Value.Sha256();
                    }
                }

                _context.ApiResources.Add(ApiResource);
                await _context.SaveChangesAsync();
            }
            else
            {
                var CurrentApiResource = _context.ApiResources
                       .Include(c => c.Scopes)
                       .Include(c => c.Secrets)
                       .Include(c => c.UserClaims)
                       .SingleOrDefault(c => c.Id == ApiResource.Id);

                Mapper.Map(ApiResource, CurrentApiResource);
                CurrentApiResource.Scopes.ReflectEntityFrameworkState(ApiResource.Scopes, _context);
                CurrentApiResource.Secrets.ReflectEntityFrameworkState(ApiResource.Secrets, _context);
                CurrentApiResource.UserClaims.ReflectEntityFrameworkState(ApiResource.UserClaims, _context);

                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }
    }
}