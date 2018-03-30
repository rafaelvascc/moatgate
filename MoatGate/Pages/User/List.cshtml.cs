using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using MoatGate.Models.AspNetIIdentityCore.EntityFramework;

namespace MoatGate.Pages.User
{
    public class ListModel : PageModel
    {
        private readonly MoatGateIdentityDbContext _context;
        private readonly UserManager<MoatGateIdentityUser> _manager;

        public ListModel(MoatGateIdentityDbContext context, UserManager<MoatGateIdentityUser> manager)
        {
            _context = context;
            _manager = manager;
        }

        public IList<MoatGateIdentityUser> MoatGateIdentityUsers { get;set; }

        public async Task OnGetAsync()
        {
            MoatGateIdentityUsers = await _context.Users.ToListAsync();
        }

        public async Task<IActionResult> OnPostDeleteAsync(int id)
        {
            await _manager.DeleteAsync(await _manager.FindByIdAsync(id.ToString()));

            return RedirectToPage();
        }
    }
}
