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
    public class DetailsModel : PageModel
    {
        private readonly MoatGate.Models.AspNetIIdentityCore.EntityFramework.MoatGateIdentityDbContext _context;

        public DetailsModel(MoatGate.Models.AspNetIIdentityCore.EntityFramework.MoatGateIdentityDbContext context)
        {
            _context = context;
        }

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
    }
}
