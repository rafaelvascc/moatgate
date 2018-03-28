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
    public class EditModel : PageModel
    {
        private readonly UserManager<MoatGateIdentityUser> _userManager;

        [BindProperty]
        public MoatGateIdentityUserEditViewModel MoatGateIdentityUser { get; set; } = new MoatGateIdentityUserEditViewModel() { Id = 0 };

        public EditModel(UserManager<MoatGateIdentityUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<IActionResult> OnGet(int? id)
        {
            if (id.HasValue)
            {
                ViewData["Title"] = "Edit User";
                var currentUser = await _userManager.FindByIdAsync(id.ToString());
                Mapper.Map(currentUser, MoatGateIdentityUser);
            }
            else
            {
                ViewData["Title"] = "Create User";
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            IdentityResult result = null;

            var currentUser = await _userManager.FindByIdAsync(MoatGateIdentityUser.Id.ToString());
            Mapper.Map(MoatGateIdentityUser, currentUser);
            result = await _userManager.UpdateAsync(currentUser);

            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(error.Code, error.Description);
                }
                return Page();
            }

            return RedirectToPage("./Index");
        }
    }
}