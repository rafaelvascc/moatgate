using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MoatGate.Models.AspNetIIdentityCore.EntityFramework;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using MoatGate.Models.User;
using AutoMapper;

namespace MoatGate.Pages.User
{
    public class CreateModel : PageModel
    {
        private readonly UserManager<MoatGateIdentityUser> _userManager;

        [BindProperty]
        public MoatGateIdentityUserCreateViewModel MoatGateIdentityUser { get; set; } = new MoatGateIdentityUserCreateViewModel() { Id = 0 };

        public CreateModel(UserManager<MoatGateIdentityUser> userManager)
        {
            _userManager = userManager;
        }

        public IActionResult OnGet()
        {
            ViewData["Title"] = "Create User";

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