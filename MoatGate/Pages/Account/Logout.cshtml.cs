using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IdentityServer4.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MoatGate.Models.AspNetIIdentityCore.EntityFramework;
using MoatGate.Models.Logout;

namespace MoatGate.Pages.Account
{
    public class LogoutModel : PageModel
    {
        [BindProperty]
        public LogoutViewModel LogoutData { set; get; } = new LogoutViewModel();
        private readonly IIdentityServerInteractionService _interaction;
        private readonly SignInManager<MoatGateIdentityUser> _signInManager;

        public LogoutModel(IIdentityServerInteractionService interaction, SignInManager<MoatGateIdentityUser> signInManager)
        {
            _interaction = interaction;
            _signInManager = signInManager;
        }

        public async Task OnGetAsync(string logoutId)
        {
            var logoutContext = await _interaction.GetLogoutContextAsync(logoutId);
            LogoutData.LogoutId = logoutId;
            LogoutData.RedirectUrl = logoutContext.PostLogoutRedirectUri;
        }

        public async Task<IActionResult> OnPostAsync()
        {
            await _signInManager.SignOutAsync();
            if (!string.IsNullOrWhiteSpace(LogoutData.RedirectUrl))
            {
                return Redirect(LogoutData.RedirectUrl);
            }

            return RedirectToPage("Login");
        }
    }
}