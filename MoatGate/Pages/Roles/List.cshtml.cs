using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using MoatGate.Models.AspNetIIdentityCore.EntityFramework;

namespace MoatGate.Pages.Roles
{
    public class ListModel : PageModel
    {
        private readonly MoatGateIdentityDbContext _context;
        private readonly RoleManager<MoatGateIdentityRole> _manager;

        public ListModel(MoatGateIdentityDbContext context, RoleManager<MoatGateIdentityRole> manager)
        {
            _context = context;
            _manager = manager;
        }

        [BindProperty]
        public IList<MoatGateIdentityRole> MoatGateIdentityRoles { get;set; }

        public async Task OnGetAsync()
        {
            MoatGateIdentityRoles = await _context.Roles.ToListAsync();
        }

        public async Task<IActionResult> OnPostDeleteAsync(int id)
        {
            await _manager.DeleteAsync(await _manager.FindByIdAsync(id.ToString()));

            return RedirectToPage();
        }
    }
}
