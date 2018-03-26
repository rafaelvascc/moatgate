using System.Collections.Generic;
using System.Threading.Tasks;
using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.EntityFramework.Entities;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace MoatGate.Pages.Resources.Identity
{
    public class IndexModel : PageModel
    {
        private readonly ConfigurationDbContext _context;

        public IndexModel(ConfigurationDbContext context)
        {
            _context = context;
        }

        public IList<IdentityResource> IdentityResources { get; set; }

        public async Task OnGetAsync()
        {
            IdentityResources = await _context.IdentityResources.ToListAsync();
        }
    }
}
