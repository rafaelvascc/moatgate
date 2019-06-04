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
using IdentityModel;
using IdentityServer4.EntityFramework.Mappers;
using ApiResource = IdentityServer4.EntityFramework.Entities.ApiResource;

namespace MoatGate.Pages.Resources.Api
{
    public class CreateEditModel : PageModel
    {
        private readonly ConfigurationDbContext _context;

        [BindProperty]
        public ApiResource ApiResource { get; set; } = new ApiResource
        {
            Scopes = new List<ApiScope>(),
            Secrets = new List<ApiSecret>(),
            UserClaims = new List<ApiResourceClaim>()
        };

        [BindProperty]
        public List<string> UserClaims { set; get; } = new List<string>();

        [BindProperty]
        public Dictionary<string, List<string>> ScopeUserClaims { set; get; } = new Dictionary<string, List<string>>();

        public CreateEditModel(ConfigurationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> OnGet(int? id)
        {
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

            var userClaimsOptions = new List<SelectListItem>();
            var allIdentityResources = (await _context.IdentityResources.Include(c => c.UserClaims).AsNoTracking().ToListAsync()).Select(i => i.ToModel());
            var allClaimsInIdentityResources = allIdentityResources.SelectMany(c => c.UserClaims).Distinct();
            var allPossibleClaimTypes = typeof(JwtClaimTypes).GetFields().Select(t => t.GetValue(null).ToString()).ToList();
            var missingClaimTypesOnDatabase = allPossibleClaimTypes.Except(allClaimsInIdentityResources).OrderBy(v => v).ToList();
            var claimTypesByIdentityResource = allIdentityResources
                .OrderBy(r => r.Name)
                .ToDictionary(r => r.Name, r => r.UserClaims.OrderBy(c => c).Select(c => c));

            foreach (var claimType in missingClaimTypesOnDatabase)
            {
                var item = new SelectListItem(claimType, claimType, false, false);
                userClaimsOptions.Add(item);
            }

            foreach (var g in claimTypesByIdentityResource)
            {
                var group = new SelectListGroup
                {
                    Name = g.Key
                };
                foreach (var claim in g.Value)
                {
                    var item = new SelectListItem(claim, claim, false, false)
                    {
                        Group = group
                    };
                    userClaimsOptions.Add(item);
                }
            }

            ViewData["UserClaimsOptions"] = userClaimsOptions;

            if (ApiResource.UserClaims != null && ApiResource.UserClaims.Any())
            {
                foreach (var userClaim in ApiResource.UserClaims.Select(c => c.Type))
                {
                    UserClaims.Add(userClaim);
                }
            }

            if (ApiResource.Scopes != null && ApiResource.Scopes.Any())
            {
                foreach (var scope in ApiResource.Scopes)
                {
                    if (scope.UserClaims != null && scope.UserClaims.Any())
                    {
                        ScopeUserClaims.Add(scope.Id.ToString(), scope.UserClaims.Select(c => c.Type).ToList());
                    }
                    else
                    {
                        ScopeUserClaims.Add(scope.Id.ToString(), new List<string>());
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

                    if (ScopeUserClaims.ContainsKey(scope.Id.ToString()))
                    {
                        var claims = ScopeUserClaims[scope.Id.ToString()] as List<string>;
                        if (claims != null && claims.Any())
                        {
                            scope.UserClaims = claims.Select(c => new ApiScopeClaim() { Type = c }).ToList();
                        }
                    }
                    else
                    {
                        var key = (ApiResource.Scopes.IndexOf(scope) + 1) * -1;
                        var claims = ScopeUserClaims[key.ToString()] as List<string>;
                        if (claims != null && claims.Any())
                        {
                            scope.UserClaims = claims.Select(c => new ApiScopeClaim() { Type = c }).ToList();
                        }
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