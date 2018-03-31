using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Options;
using MoatGate.Helpers;
using MoatGate.Models.AspNetIIdentityCore.EntityFramework;

namespace MoatGate.Pages.Account
{
    public class ForgotPasswordModel : PageModel
    {
        private readonly UserManager<MoatGateIdentityUser> _userManager;
        private readonly IEmailSender _emailSender;
        private readonly IOptions<SignInOptions> _signInOptionsAcessor;

        public ForgotPasswordModel(UserManager<MoatGateIdentityUser> userManager, IEmailSender emailSender, IOptions<SignInOptions> signInOptionsAcessor)
        {
            _userManager = userManager;
            _emailSender = emailSender;
            _signInOptionsAcessor = signInOptionsAcessor;
        }

        [Required]
        [EmailAddress]
        [BindProperty]
        public string ResetPasswordEmail { set; get; }

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(ResetPasswordEmail);
                if (user == null || (_signInOptionsAcessor.Value.RequireConfirmedEmail && !(await _userManager.IsEmailConfirmedAsync(user))))
                {
                    // Don't reveal that the user does not exist or is not confirmed
                    return RedirectToPage("Login", new { resetPasswordEmailSent = true });
                }

                // For more information on how to enable account confirmation and password reset please 
                // visit https://go.microsoft.com/fwlink/?LinkID=532713
                var code = await _userManager.GeneratePasswordResetTokenAsync(user);
                var callbackUrl = Url.Page("ResetPassword", pageHandler: null, values: new { userId = user.Id, code }, protocol: Request.Scheme);
                var emailBody = ComposeResetPasswordEmail(callbackUrl);
                await _emailSender.SendEmailAsync("admin@moatgate.com", "admin", ResetPasswordEmail, "Moatgate Password Reset", emailBody);
                return RedirectToPage("Login", new { resetPasswordEmailSent = true });
            }

            return Page();
        }

        private string ComposeResetPasswordEmail(string callbackUrl)
        {
            return $"Please reset your password by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.";
        }
    }
}