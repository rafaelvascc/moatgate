using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IdentityServer4.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MoatGate.Models.AspNetIIdentityCore.EntityFramework;
using MoatGate.Models.Login;

namespace MoatGate.Pages.Account
{
    public class LoginModel : PageModel
    {
        [BindProperty]
        public LoginViewmodel LoginData { set; get; } = new LoginViewmodel();

        private readonly SignInManager<MoatGateIdentityUser> _signInManager;
        private readonly UserManager<MoatGateIdentityUser> _userManager;
        private readonly IIdentityServerInteractionService _interaction;

        public LoginModel(IIdentityServerInteractionService interaction, UserManager<MoatGateIdentityUser> userManager, SignInManager<MoatGateIdentityUser> signInManager)
        {
            _signInManager = signInManager;
            _interaction = interaction;
            _userManager = userManager;
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
    }
}