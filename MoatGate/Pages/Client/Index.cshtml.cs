using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using IdentityServer4.EntityFramework.DbContexts;
using Microsoft.EntityFrameworkCore;

namespace MoatGate.Pages.Client
{
    public class IndexModel : PageModel
    {
        private readonly ConfigurationDbContext _context;

        public IndexModel(ConfigurationDbContext context)
        {
            _context = context;
        }

        public IList<IdentityServer4.EntityFramework.Entities.Client> Clients { set; get; }

        public async Task OnGetAsync()
        {
            Clients = await _context.Clients.ToListAsync();
        }

        public async Task<IActionResult> OnPostDeleteAsync(int id)
        {
            _context.Clients.Remove(await _context.Clients.SingleOrDefaultAsync(c => c.Id == id));
            await _context.SaveChangesAsync();

            return RedirectToPage();
        }
    }
}