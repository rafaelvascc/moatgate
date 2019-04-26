using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Transactions;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MoatGate.Services;
using MoatGate.Models.AspNetIIdentityCore.EntityFramework;
using MoatGate.Models.Profile;

namespace MoatGate.Pages.Profile
{
    public class EditModel : PageModel
    {
        private readonly UserManager<MoatGateIdentityUser> _manager;
        private readonly SignInManager<MoatGateIdentityUser> _signInManager;
        private readonly IEmailSender _emailSender;
        private readonly ISmsSender _smsSender;

        [BindProperty]
        public EdiProfileViewModel EditProfileViewModel { get; set; }

        public IDictionary<string, string> Cultures { set; get; } = new Dictionary<string, string>();

        public IDictionary<string, string> TimeZones { set; get; } = new Dictionary<string, string>();

        [BindProperty]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [BindProperty]
        public bool RefreshSignIn { get; set; } = true;

        [BindProperty]
        public bool RememberMe { get; set; } = false;

        public EditModel(UserManager<MoatGateIdentityUser> manager, SignInManager<MoatGateIdentityUser> signInManager, IEmailSender emailSender, ISmsSender smsSender)
        {
            _manager = manager;
            _signInManager = signInManager;
            _emailSender = emailSender;
            _smsSender = smsSender;
        }

        public async Task OnGetAsync()
        {
            var id = _manager.GetUserId(User);
            var systemUser = await _manager.FindByIdAsync(id);
            var claims = await _manager.GetClaimsAsync(systemUser);
            EditProfileViewModel = new EdiProfileViewModel(systemUser, claims);
            Cultures = CultureInfo.GetCultures(CultureTypes.SpecificCultures).OrderBy(c => c.Name).ToDictionary(c => c.Name, c => $"{c.Name } - {c.NativeName}");
            TimeZones = TimeZoneInfo.GetSystemTimeZones().ToDictionary(t => t.Id, t => t.DisplayName);
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            IList<IdentityResult> resultFinal = new List<IdentityResult>();

            bool confirmationEmailSent = false;
            bool confirmationSmsSent = false;

            using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {
                    var id = _manager.GetUserId(User);
                    var systemUser = await _manager.FindByIdAsync(id);

                    if ((systemUser.NormalizedEmail != _manager.KeyNormalizer.Normalize(EditProfileViewModel.Email) || systemUser.PhoneNumber != EditProfileViewModel.PhoneNumber) &&
                        (string.IsNullOrWhiteSpace(Password) || !await _manager.CheckPasswordAsync(systemUser, Password)))
                    {
                        ModelState.AddModelError(nameof(Password), "Invalid password when changing email or phone number");
                        return Page();
                    }

                    EditProfileViewModel.UpdatedAt = DateTime.Now;
                    var newValues = EditProfileViewModel.ToClaims();
                    var currentValues = await _manager.GetClaimsAsync(systemUser);

                    var toInsert = newValues.Where(nv => !currentValues.Select(c => c.Type).Distinct().Contains(nv.Type));
                    var toUpdate = newValues.Where(nv => currentValues.Select(c => c.Type).Distinct().Contains(nv.Type) &&
                        currentValues.Where(c => c.Type == nv.Type).Single().Value != newValues.Where(c => c.Type == nv.Type).Single().Value);
                    var toRemove = currentValues.Where(ov => !newValues.Select(c => c.Type).Distinct().Contains(ov.Type) ||
                        string.IsNullOrWhiteSpace(newValues.Where(c => c.Type == ov.Type).Single().Value));

                    var resultAdd = await _manager.AddClaimsAsync(systemUser, toInsert);
                    var resultRemove = await _manager.RemoveClaimsAsync(systemUser, toRemove);

                    resultFinal.Add(resultAdd);
                    resultFinal.Add(resultRemove);

                    foreach (var newClaim in toUpdate)
                    {
                        var oldClaim = User.Claims.SingleOrDefault(c => c.Type == newClaim.Type);
                        if (oldClaim != null)
                        {
                            resultFinal.Add(await _manager.ReplaceClaimAsync(systemUser, oldClaim, newClaim));
                        }
                    }

                    if (resultFinal.Any(r => !r.Succeeded))
                    {
                        var errors = resultFinal.Where(r => !r.Succeeded);
                        foreach (var error in errors.SelectMany(e => e.Errors))
                        {
                            ModelState.AddModelError(error.Code, error.Description);
                        }
                        scope.Dispose();

                        return Page();
                    }
                    else
                    {
                        if (RefreshSignIn)
                            await _signInManager.SignInAsync(systemUser, RememberMe);

                        scope.Complete();

                        if (systemUser.NormalizedEmail != _manager.KeyNormalizer.Normalize(EditProfileViewModel.Email))
                        {
                            var token = await _manager.GenerateChangeEmailTokenAsync(systemUser, EditProfileViewModel.Email);
                            var callbackUrl = Url.Page("/Account/ConfirmEmail", pageHandler: null, values: new { userId = id, token, newEmail = EditProfileViewModel.Email }, protocol: Request.Scheme);
                            await _emailSender.SendEmailConfirmationEmailAsync(EditProfileViewModel.Email, callbackUrl);
                            confirmationEmailSent = true;
                        }

                        if (systemUser.PhoneNumber != EditProfileViewModel.PhoneNumber)
                        {
                            var token = await _manager.GenerateChangePhoneNumberTokenAsync(systemUser, EditProfileViewModel.PhoneNumber);
                            var callbackUrl = Url.Page("/Account/ConfirmPhoneNumber", pageHandler: null, values: new { userId = id, token, newNumber = EditProfileViewModel.PhoneNumber }, protocol: Request.Scheme);
                            await _smsSender.SendPhoneNumberChangeConfirmationSMSAsync(EditProfileViewModel.PhoneNumber, callbackUrl);
                            confirmationSmsSent = true;
                        }

                        return RedirectToPage("Index", new { profileUpdated = true, confirmationEmailSent, confirmationSmsSent });
                    }
                }
                catch (Exception ex)
                {
                    scope.Dispose();
                    throw;
                }
            }
        }
    }
}