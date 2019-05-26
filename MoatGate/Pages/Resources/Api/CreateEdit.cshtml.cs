using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using IdentityServer4.EntityFramework.Entities;
using IdentityServer4.EntityFramework.DbContexts;
using Microsoft.EntityFrameworkCore;
using IdentityServer4.Models;
using AutoMapper;
using MoatGate.Services;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace MoatGate.Pages.Resources.Api
{
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

        public List<SelectListItem> UserClaimsOptions { set; get; } = new List<SelectListItem>();

        [BindProperty]
        public List<string> UserClaims { set; get; } = new List<string>();

        public CreateEditModel(ConfigurationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> OnGet(int? id)
        {
            var claimByIdentityResource = _context.IdentityResources.Include(c => c.UserClaims).AsNoTracking();
            if (id.HasValue)
            {
                ViewData["Editing"] = true;
                ApiResource = await _context.ApiResources
                    .AsNoTracking()
                    .Include(a => a.Scopes).ThenInclude(s => s.UserClaims)
                    .Include(a => a.Secrets)
                    .Include(a => a.UserClaims)
                    .SingleOrDefaultAsync(r => r.Id == id.Value);
                ViewData["Title"] = "New Api Resource";
            }
            else
            {
                ViewData["Editing"] = false;
                ViewData["Title"] = "Create Api Resource";
            }

            var userClaimsChecks = await claimByIdentityResource
                .OrderBy(r => r.Name)
                .ToDictionaryAsync(r => r.Name, r => r.UserClaims.OrderBy(c => c.Type).Select(c => (Selected: ApiResource.UserClaims.Any(a => a.Type == c.Type), c.Type)));

            foreach (var g in userClaimsChecks)
            {
                var group = new SelectListGroup
                {
                    Name = g.Key
                };
                foreach (var (Selected, Type) in g.Value)
                {
                    var item = new SelectListItem(Type, Type, Selected, false)
                    {
                        Group = group
                    };
                    UserClaimsOptions.Add(item);
                    if (Selected)
                    {
                        UserClaims.Add(Type);
                    }
                }
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
                var currentApiResource = _context.ApiResources
                       .Include(c => c.Scopes).ThenInclude(s => s.UserClaims)
                       .Include(c => c.Secrets)
                       .Include(c => c.UserClaims)
                       .SingleOrDefault(c => c.Id == ApiResource.Id);

                if (currentApiResource.Scopes == null)
                {
                    currentApiResource.Scopes = new List<ApiScope>();
                }

                foreach (var scope in currentApiResource.Scopes)
                {
                    if (scope.UserClaims == null)
                    {
                        scope.UserClaims = new List<ApiScopeClaim>();
                    }
                }

                if (ApiResource.Scopes == null)
                {
                    ApiResource.Scopes = new List<ApiScope>();
                }

                foreach (var scope in ApiResource.Scopes)
                {
                    if (scope.UserClaims == null)
                    {
                        scope.UserClaims = new List<ApiScopeClaim>();
                    }
                }

                Mapper.Map(ApiResource, currentApiResource);
                currentApiResource.Scopes.ReflectToEntityFrameworkState(ApiResource.Scopes, _context);
                currentApiResource.Secrets.ReflectToEntityFrameworkState(ApiResource.Secrets, _context);
                currentApiResource.UserClaims.ReflectToEntityFrameworkState(UserClaims?.Select(c => new ApiResourceClaim() { Type = c }).ToList(), _context);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./List");
        }
    }
}