using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using MoatGate.Models.AspNetIIdentityCore.EntityFramework;

namespace MoatGate.Pages.Roles
{
    public class CreateModel : PageModel
    {
        private readonly MoatGate.Models.AspNetIIdentityCore.EntityFramework.MoatGateIdentityDbContext _context;

        public CreateModel(MoatGate.Models.AspNetIIdentityCore.EntityFramework.MoatGateIdentityDbContext context)
        {
            _context = context;
        }

        public IActionResult OnGet()
        {
            return Page();
        }

        [BindProperty]
        public MoatGateIdentityRole MoatGateIdentityRole { get; set; }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.Roles.Add(MoatGateIdentityRole);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}