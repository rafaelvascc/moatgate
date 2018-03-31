using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MoatGate.Models.AspNetIIdentityCore.EntityFramework;
using MoatGate.Models.ResetPassword;

namespace MoatGate.Pages.Account
{
    public class ResetPasswordModel : PageModel
    {
        private readonly UserManager<MoatGateIdentityUser> _userManager;

        [BindProperty]
        public ResetPasswordViewModel ResetPasswordViewModel { get; set; } = new ResetPasswordViewModel();

        public ResetPasswordModel(UserManager<MoatGateIdentityUser> userManager)
        {
            _userManager = userManager;
        }

        public IActionResult OnGet(string code = null)
        {
            if (code == null)
            {
                throw new ApplicationException("A code must be supplied for password reset.");
            }
            else
            {
                ResetPasswordViewModel.Code = code;
                return Page();
            }
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var user = await _userManager.FindByEmailAsync(ResetPasswordViewModel.Email);
            if (user == null)
            {
                return RedirectToPage("Login", new { passwordReseted = true });
            }

            var result = await _userManager.ResetPasswordAsync(user, ResetPasswordViewModel.Code, ResetPasswordViewModel.Password);
            if (result.Succeeded)
            {
                return RedirectToPage("Login", new { passwordReseted = true });
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
            return Page();
        }
    }
}