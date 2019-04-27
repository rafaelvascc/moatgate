using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MoatGate.Models.AspNetIIdentityCore.EntityFramework;

namespace MoatGate.Pages.Roles
{
    public class CreateModel : PageModel
    {
        private readonly RoleManager<MoatGateIdentityRole> _manager;

        public CreateModel(RoleManager<MoatGateIdentityRole> manager)
        {
            _manager = manager;
        }

        public async Task<IActionResult> OnGet(int? id)
        {
            if (id.HasValue)
            {
                ViewData["Title"] = "Edit Role";
                MoatGateIdentityRole = await _manager.FindByIdAsync(id.ToString());
            }
            else
            {
                ViewData["Title"] = "Create Role";
            }

            return Page();
        }

        [BindProperty]
        public MoatGateIdentityRole MoatGateIdentityRole { get; set; } = new MoatGateIdentityRole();

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }
            
            var result = await _manager.CreateAsync(MoatGateIdentityRole);

            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(error.Code, error.Description);
                }
                return Page();
            }

            return RedirectToPage("./List");
        }
    }
}