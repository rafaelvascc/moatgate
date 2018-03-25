using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using MoatGate.Models.AspNetIIdentityCore.EntityFramework;

namespace MoatGate.Pages.Resources.Identity
{
    public class IndexModel : PageModel
    {
        private readonly MoatGate.Models.AspNetIIdentityCore.EntityFramework.MoatGateIdentityDbContext _context;

        public IndexModel(MoatGate.Models.AspNetIIdentityCore.EntityFramework.MoatGateIdentityDbContext context)
        {
            _context = context;
        }

        public IList<MoatGateIdentityUser> MoatGateIdentityUsers { get;set; }

        public async Task OnGetAsync()
        {
            MoatGateIdentityUsers = await _context.Users.ToListAsync();
        }
    }
}
