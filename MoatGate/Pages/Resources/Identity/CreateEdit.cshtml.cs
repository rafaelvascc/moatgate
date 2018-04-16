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
using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.EntityFramework.Entities;
using Microsoft.EntityFrameworkCore;
using AutoMapper;
using MoatGate.Services;

namespace MoatGate.Pages.Resources.Identity
{
    public class CreateEditModel : PageModel
    {
        private readonly ConfigurationDbContext _context;

        [BindProperty]
        public IdentityResource IdentityResource { get; set; } = new IdentityResource() { UserClaims = new List<IdentityClaim>() };
        
        public CreateEditModel(ConfigurationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> OnGet(int? id)
        {
            if (id.HasValue)
            {
                ViewData["Title"] = "Edit Identity Resource";
                IdentityResource = await _context.IdentityResources
                    .Include(r => r.UserClaims)
                    .SingleOrDefaultAsync(i => i.Id == id.Value);
            }
            else
            {
                ViewData["Title"] = "Create Identity Resource";
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            if (IdentityResource.Id == 0)
            {
                _context.IdentityResources.Add(IdentityResource);
                await _context.SaveChangesAsync();
            }
            else
            {
                var CurrentIdentityResource = await _context.IdentityResources
                    .Include(r => r.UserClaims)
                    .SingleOrDefaultAsync(i => i.Id == IdentityResource.Id);

                Mapper.Map(IdentityResource, CurrentIdentityResource);
                CurrentIdentityResource.UserClaims.ReflectToEntityFrameworkState(IdentityResource.UserClaims, _context);

                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./List");
        }
    }
}