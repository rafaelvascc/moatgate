using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using MoatGate.Models.AspNetIIdentityCore.EntityFramework;

namespace MoatGate.Pages.User
{
    public class DeleteModel : PageModel
    {
        private readonly MoatGate.Models.AspNetIIdentityCore.EntityFramework.MoatGateIdentityDbContext _context;

        public DeleteModel(MoatGate.Models.AspNetIIdentityCore.EntityFramework.MoatGateIdentityDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public MoatGateIdentityUser MoatGateIdentityUser { get; set; }

        public async Task<IActionResult> OnGetAsync(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            MoatGateIdentityUser = await _context.Users.SingleOrDefaultAsync(m => m.Id == id);

            if (MoatGateIdentityUser == null)
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

            MoatGateIdentityUser = await _context.Users.FindAsync(id);

            if (MoatGateIdentityUser != null)
            {
                _context.Users.Remove(MoatGateIdentityUser);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }
    }
}
