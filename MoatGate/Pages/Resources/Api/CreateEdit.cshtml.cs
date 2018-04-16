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
using MoatGate.Services;

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
        
        public CreateEditModel(ConfigurationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> OnGet(int? id)
        {
            if (id.HasValue)
            {
                ViewData["Editing"] = true;
                ViewData["Title"] = "Edit Api Resource";
                ApiResource = await _context.ApiResources
                    .AsNoTracking()
                    .Include(a => a.Scopes).ThenInclude(s => s.UserClaims)
                    .Include(a => a.Secrets)
                    .Include(a => a.UserClaims)
                    .SingleOrDefaultAsync(r => r.Id == id.Value);
            }
            else
            {
                ViewData["Editing"] = false;
                ViewData["Title"] = "Create Api Resource";
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
                       .Include(c => c.Scopes).ThenInclude(s => s.UserClaims)
                       .Include(c => c.Secrets)
                       .Include(c => c.UserClaims)
                       .SingleOrDefault(c => c.Id == ApiResource.Id);

                foreach (var scope in CurrentApiResource.Scopes)
                {
                    if (scope.UserClaims == null)
                    {
                        scope.UserClaims = new List<ApiScopeClaim>();
                    }
                }

                foreach (var scope in ApiResource.Scopes)
                {
                    if (scope.UserClaims == null)
                    {
                        scope.UserClaims = new List<ApiScopeClaim>();
                    }
                }

                Mapper.Map(ApiResource, CurrentApiResource);
                CurrentApiResource.Scopes.ReflectToEntityFrameworkState(ApiResource.Scopes, _context);
                CurrentApiResource.Secrets.ReflectToEntityFrameworkState(ApiResource.Secrets, _context);
                CurrentApiResource.UserClaims.ReflectToEntityFrameworkState(ApiResource.UserClaims, _context);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./List");
        }
    }
}