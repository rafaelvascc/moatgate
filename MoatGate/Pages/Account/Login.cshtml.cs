using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MoatGate.Models.AspNetIIdentityCore.EntityFramework;
using MoatGate.Models.Login;

namespace MoatGate.Pages.Account
{
    public class LoginModel : PageModel
    {
        private readonly SignInManager<MoatGateIdentityUser> _signInManager;

        [BindProperty]
        public LoginViewmodel LoginData { set; get; } = new LoginViewmodel();

        public LoginModel(SignInManager<MoatGateIdentityUser> signInManager)
        {
            _signInManager = signInManager;
        }

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl)
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var result = await _signInManager.PasswordSignInAsync(LoginData.UserName, LoginData.Password, LoginData.RememberMe, true);
            if (result.Succeeded)
            {
                if (string.IsNullOrEmpty(returnUrl))
                {
                    return RedirectToPage("/Welcome");
                }
                else
                {
                    return Redirect(returnUrl);
                }
            }
            if (result.RequiresTwoFactor)
            {
                //return RedirectToAction(nameof(SendCode), new { ReturnUrl = returnUrl, RememberMe = model.RememberMe });
                //TODO: implement two factor auth
            }
            if (result.IsLockedOut)
            {
                ModelState.AddModelError(string.Empty, "Account Lockout");
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Invalid login attempt.");
            }

            return Page();
        }

        public async Task<IActionResult> OnPostLogoutAsync(string returnUrl)
        {
            await _signInManager.SignOutAsync();
            return RedirectToPage("/Account/Login");
        }
    }
}