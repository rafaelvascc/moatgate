using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using MoatGate.Models.AspNetIIdentityCore.EntityFramework;

namespace MoatGate.Pages.Roles
{
    public class DeleteModel : PageModel
    {
        private readonly MoatGate.Models.AspNetIIdentityCore.EntityFramework.MoatGateIdentityDbContext _context;

        public DeleteModel(MoatGate.Models.AspNetIIdentityCore.EntityFramework.MoatGateIdentityDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public MoatGateIdentityRole MoatGateIdentityRole { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            MoatGateIdentityRole = await _context.Roles.SingleOrDefaultAsync(m => m.Id == id);

            if (MoatGateIdentityRole == null)
            {
                return NotFound();
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            MoatGateIdentityRole = await _context.Roles.FindAsync(id);

            if (MoatGateIdentityRole != null)
            {
                _context.Roles.Remove(MoatGateIdentityRole);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }
    }
}
