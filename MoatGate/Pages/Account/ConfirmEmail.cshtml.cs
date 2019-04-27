using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MoatGate.Models.AspNetIIdentityCore.EntityFramework;

namespace MoatGate.Pages.Account
{
    public class ConfirmEmailModel : PageModel
    {
        private readonly UserManager<MoatGateIdentityUser> _userManager;
        private readonly SignInManager<MoatGateIdentityUser> _signInManager;

        public ConfirmEmailModel(UserManager<MoatGateIdentityUser> userManager, SignInManager<MoatGateIdentityUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        public async Task<IActionResult> OnGetAsync(string userId, string token, string newEmail)
        {
            if (userId == null || token == null)
            {
                return RedirectToPage("Login");
            }

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                throw new ApplicationException($"Unable to load user with ID '{userId}'.");
            }

            if (string.IsNullOrWhiteSpace(newEmail))
            {
                var result = await _userManager.ConfirmEmailAsync(user, token);
                if (!result.Succeeded)
                {
                    throw new ApplicationException($"Error confirming email for user with ID '{userId}':");
                }

                if (User.Identity.IsAuthenticated)
                {
                    //TODO: find a way to get the current authentication if it is persitent or not
                    await _signInManager.SignInAsync(user, false);
                }
            }
            else
            {
                var result = await _userManager.ChangeEmailAsync(user, newEmail, token);
                if (result.Succeeded)
                {
                    var confirmationToken = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    var resultConfirm = await _userManager.ConfirmEmailAsync(user, confirmationToken);
                    if (resultConfirm.Succeeded)
                    {
                        if (User.Identity.IsAuthenticated)
                        {
                            //TODO: find a way to get the current authentication if it is persitent or not
                            await _signInManager.SignInAsync(user, false);
                        }
                    }
                    else
                    {
                        throw new ApplicationException($"Error confirming email for user with ID '{userId}':");
                    }
                }
                else
                {
                    throw new ApplicationException($"Error changing email for user with ID '{userId}':");
                }
            }

            return Page();
        }
    }
}