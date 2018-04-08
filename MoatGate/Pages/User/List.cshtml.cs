using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using MoatGate.Models.AspNetIIdentityCore.EntityFramework;
using MoatGate.Models.User;

namespace MoatGate.Pages.User
{
    public class ListModel : PageModel
    {
        private readonly MoatGateIdentityDbContext _context;
        [BindProperty]
        public List<string> Roles { set; get; } = new List<string>();

        public ListModel(MoatGateIdentityDbContext context)
        {
            _context = context;
        }

        public async Task OnGetAsync()
        {
            Roles = await _context.Roles.AsNoTracking().Select(r => r.Name).OrderBy(r => r).ToListAsync();
        }
    }
}
