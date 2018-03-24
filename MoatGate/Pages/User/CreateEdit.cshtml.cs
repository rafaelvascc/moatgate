using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using MoatGate.Models.AspNetIIdentityCore.EntityFramework;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace MoatGate.Pages.User
{
    public class CreateEditModel : PageModel
    {
        private readonly UserManager<MoatGateIdentityUser> _userManager;

        [BindProperty]
        public MoatGateIdentityUser MoatGateIdentityUser { get; set; } = new MoatGateIdentityUser();

        [Required]
        [DataType(DataType.Password)]
        [BindProperty]
        public string Password { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Compare("Password")]
        [BindProperty]
        public string ConfirmPassword { get; set; }

        public CreateEditModel(UserManager<MoatGateIdentityUser> userManager)
        {
            _userManager = userManager;
        }


        public async Task<IActionResult> OnGet(Guid? id)
        {
            if (id.HasValue)
            {
                ViewData["Title"] = "Edit User";
                MoatGateIdentityUser = await _userManager.FindByIdAsync(id.ToString());
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

            if (MoatGateIdentityUser.Id == 0)
            {
                var result = await _userManager.CreateAsync(MoatGateIdentityUser, Password);

                if (!result.Succeeded)
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(error.Code, error.Description);
                    }
                    return Page();
                }
            }
            else
            {
                var result = _userManager.UpdateAsync(MoatGateIdentityUser);
            }

            return RedirectToPage("./Index");
        }
    }
}