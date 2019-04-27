using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MoatGate.Models.AspNetIIdentityCore.EntityFramework;

namespace MoatGate.Pages.Account
{
    public class ConfirmPhoneNumberModel : PageModel
    {
        private readonly UserManager<MoatGateIdentityUser> _userManager;
        private readonly SignInManager<MoatGateIdentityUser> _signInManager;

        public ConfirmPhoneNumberModel(UserManager<MoatGateIdentityUser> userManager, SignInManager<MoatGateIdentityUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        public async Task<IActionResult> OnGetAsync(string userId, string token, string newNumber)
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

            //If confirm an existing number
            if (string.IsNullOrWhiteSpace(newNumber))
            {
                var resultTokenValidation = await _userManager.VerifyChangePhoneNumberTokenAsync(user, token, user.PhoneNumber);
                if (!resultTokenValidation)
                {
                    throw new ApplicationException($"Error confirming phone number for user with ID '{userId}':");
                }
                else
                {
                    user.PhoneNumberConfirmed = true;
                    var resultConfirm = await _userManager.UpdateAsync(user);

                    if (resultConfirm.Succeeded && User.Identity.IsAuthenticated)
                    {
                        //TODO: find a way to get the current authentication if it is persitent or not
                        await _signInManager.SignInAsync(user, false);
                    }
                }
            }
            //If confirming a new number
            else
            {
                var resultTokenValidation = await _userManager.VerifyChangePhoneNumberTokenAsync(user, token, newNumber);
                if (!resultTokenValidation)
                {
                    throw new ApplicationException($"Error confirming phone number for user with ID '{userId}':");
                }
                else
                {
                    var resultChange = await _userManager.ChangePhoneNumberAsync(user, newNumber, token);
                    if (resultChange.Succeeded)
                    {
                        user.PhoneNumberConfirmed = true;
                        var resultConfirm = await _userManager.UpdateAsync(user);

                        if (resultConfirm.Succeeded && User.Identity.IsAuthenticated)
                        {
                            //TODO: find a way to get the current authentication if it is persitent or not
                            await _signInManager.SignInAsync(user, false);
                        }
                    }
                }
            }

            return Page();
        }
    }
}