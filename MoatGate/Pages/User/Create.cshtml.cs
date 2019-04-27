using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MoatGate.Models.AspNetIIdentityCore.EntityFramework;
using Microsoft.AspNetCore.Identity;
using MoatGate.Models.User;
using AutoMapper;
using System.Linq;

namespace MoatGate.Pages.User
{
    public class CreateModel : PageModel
    {
        private readonly UserManager<MoatGateIdentityUser> _userManager;
        private readonly RoleManager<MoatGateIdentityRole> _roleManager;

        [BindProperty]
        public UserCreateViewModel MoatGateIdentityUser { get; set; } = new UserCreateViewModel();

        public CreateModel(UserManager<MoatGateIdentityUser> userManager, RoleManager<MoatGateIdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public IActionResult OnGet()
        {
            ViewData["Title"] = "Create User";

            MoatGateIdentityUser.RoleChecks = _roleManager.Roles.ToDictionary(k => k.Name, v => false);

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            IdentityResult result = null;

            var newUser = new MoatGateIdentityUser();
            Mapper.Map(MoatGateIdentityUser, newUser);
            result = await _userManager.CreateAsync(newUser, MoatGateIdentityUser.Password);

            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(error.Code, error.Description);
                }
                return Page();
            }

            var roleResult = await _userManager.AddToRolesAsync(newUser, MoatGateIdentityUser.RoleChecks.Where(r => r.Value).Select(r => r.Key));

            if (!roleResult.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(error.Code, error.Description);
                }
                return RedirectToPage("Edit", new { errors = "roles" });
            }

            return RedirectToPage("./List");
        }
    }
}