using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using MoatGate.Models.AspNetIIdentityCore.EntityFramework;
using MoatGate.Models.ChangePassword;

namespace MoatGate.Pages.Account
{
    public class ChangePasswordModel : PageModel
    {

        private readonly UserManager<MoatGateIdentityUser> _userManager;
        private readonly SignInManager<MoatGateIdentityUser> _signInManager;
        private readonly ILogger<ChangePasswordModel> _logger;


        [BindProperty]
        public ChangePasswordViewModel ChangePasswordViewModel { get; set; }

        [TempData]
        public string StatusMessage { get; set; }

        public ChangePasswordModel(
            UserManager<MoatGateIdentityUser> userManager,
            SignInManager<MoatGateIdentityUser> signInManager,
            ILogger<ChangePasswordModel> logger)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
        }

        
        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                throw new ApplicationException($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            //Will this really happen?
            //var hasPassword = await _userManager.HasPasswordAsync(user);
            //if (!hasPassword)
            //{
            //    return RedirectToPage("./SetPassword");
            //}

            return Page();
        }


        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                throw new ApplicationException($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            var changePasswordResult = await _userManager.ChangePasswordAsync(user, ChangePasswordViewModel.OldPassword, ChangePasswordViewModel.NewPassword);
            if (!changePasswordResult.Succeeded)
            {
                foreach (var error in changePasswordResult.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
                return Page();
            }

            await _signInManager.SignInAsync(user, isPersistent: false);
            _logger.LogInformation("User changed their password successfully.");
            StatusMessage = "Your password has been changed.";

            return RedirectToPage("/Profile/Index", new { PasswordChanged = true });
        }
    }
}