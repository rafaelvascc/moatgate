using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.EntityFramework.Entities;
using Microsoft.EntityFrameworkCore;
using AutoMapper;
using MoatGate.Services;
using Microsoft.AspNetCore.Mvc.Rendering;
using IdentityModel;
using System.Linq;
using IdentityServer4.EntityFramework.Mappers;

namespace MoatGate.Pages.Resources.Identity
{
    public class CreateEditModel : PageModel
    {
        private readonly ConfigurationDbContext _context;

        [BindProperty]
        public List<string> SelectedClaims { get; set; } = new List<string>();

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
                    .AsNoTracking()
                    .SingleOrDefaultAsync(i => i.Id == id.Value);

                SelectedClaims = IdentityResource.UserClaims.Select(c => c.Type).ToList(); 
            }
            else
            {
                ViewData["Title"] = "Create Identity Resource";
            }

            var allIdentityResources = (await _context.IdentityResources.Include(c => c.UserClaims).AsNoTracking().ToListAsync()).Select(i => i.ToModel());
            var allClaimsInIdentityResources = allIdentityResources.SelectMany(c => c.UserClaims).Distinct();
            var defaultJwtClaimTypes = typeof(JwtClaimTypes).GetFields().Select(t => t.GetValue(null).ToString()).ToList();
            var userClaimsOptions = defaultJwtClaimTypes.Union(allClaimsInIdentityResources).Distinct().OrderBy(v => v).Select(c => new SelectListItem(c, c, false, false)).ToList();

            ViewData["UserClaimsOptions"] = userClaimsOptions;

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
                if (SelectedClaims.Any())
                {
                    IdentityResource.UserClaims = SelectedClaims.Select(c => new IdentityClaim { Type = c.Trim().ToLower().Replace(" ", "_") }).ToList();
                }

                _context.IdentityResources.Add(IdentityResource);
                await _context.SaveChangesAsync();
            }
            else
            {
                var CurrentIdentityResource = await _context.IdentityResources
                    .Include(r => r.UserClaims)
                    .SingleOrDefaultAsync(i => i.Id == IdentityResource.Id);

                if (SelectedClaims.Any())
                {
                    IdentityResource.UserClaims = SelectedClaims.Select(c => new IdentityClaim { Type = c.Trim().ToLower().Replace(" ", "_") }).ToList();
                }

                Mapper.Map(IdentityResource, CurrentIdentityResource);
                CurrentIdentityResource.UserClaims.ReflectToEntityFrameworkState(IdentityResource.UserClaims, _context);

                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./List");
        }
    }
}