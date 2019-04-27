using System;
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

        public IActionResult OnGet(Guid? userId = null, string code = null)
        {
            if (userId == null)
            {
                throw new ApplicationException("An user id must be supplied for password reset.");
            }
            if (code == null)
            {
                throw new ApplicationException("A code must be supplied for password reset.");
            }
            else
            {
                ResetPasswordViewModel.UserId = userId.Value;
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

            var user = await _userManager.FindByIdAsync(ResetPasswordViewModel.UserId.ToString());
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