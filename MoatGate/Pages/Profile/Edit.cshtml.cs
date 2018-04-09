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
using MoatGate.Helpers;
using MoatGate.Models.AspNetIIdentityCore.EntityFramework;
using MoatGate.Models.Profile;

namespace MoatGate.Pages.Profile
{
    public class EditModel : PageModel
    {
        private readonly UserManager<MoatGateIdentityUser> _manager;
        private readonly SignInManager<MoatGateIdentityUser> _signInManager;
        private readonly IEmailSender _emailSender;

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

        public EditModel(UserManager<MoatGateIdentityUser> manager, SignInManager<MoatGateIdentityUser> signInManager, IEmailSender emailSender)
        {
            _manager = manager;
            _signInManager = signInManager;
            _emailSender = emailSender;
        }

        public void OnGet()
        {
            EditProfileViewModel = new EdiProfileViewModel(User.Claims);
            Cultures = CultureInfo.GetCultures(CultureTypes.SpecificCultures).OrderBy(c => c.Name).ToDictionary(c => c.Name, c => $"{c.Name } - {c.NativeName}");
            TimeZones = TimeZoneInfo.GetSystemTimeZones().ToDictionary(t => t.Id, t => t.DisplayName);
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            bool confirmationEmailSent = false;

            //TODO: wait for EF Core 2.1 :(
            //using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            //{
            //    try
            //    {
            var id = _manager.GetUserId(User);
            var systemUser = await _manager.FindByIdAsync(id);

            if (systemUser.NormalizedEmail != _manager.KeyNormalizer.Normalize(EditProfileViewModel.Email) &&
                (string.IsNullOrWhiteSpace(Password) || !await _manager.CheckPasswordAsync(systemUser, Password)))
            {
                ModelState.AddModelError(nameof(Password), "Invalid password when changing email");
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

            IList<IdentityResult> resultFinal = new List<IdentityResult>();
            foreach (var newClaim in toUpdate)
            {
                var oldClaim = User.Claims.SingleOrDefault(c => c.Type == newClaim.Type);
                if (oldClaim != null)
                {
                    resultFinal.Add(await _manager.ReplaceClaimAsync(systemUser, oldClaim, newClaim));
                }
            }

            resultFinal.Add(resultAdd);
            resultFinal.Add(resultRemove);

            if (resultFinal.Any(r => !r.Succeeded))
            {
                var errors = resultFinal.Where(r => !r.Succeeded);
                foreach (var error in errors.SelectMany(e => e.Errors))
                {
                    ModelState.AddModelError(error.Code, error.Description);
                }
                //scope.Dispose();

                return Page();
            }
            else
            {
                //scope.Complete();

                if (systemUser.NormalizedEmail != _manager.KeyNormalizer.Normalize(EditProfileViewModel.Email))
                {
                    var token = await _manager.GenerateChangeEmailTokenAsync(systemUser, EditProfileViewModel.Email);
                    var callbackUrl = Url.Page("/Account/ConfirmEmail", pageHandler: null, values: new { userId = id, token, newEmail = EditProfileViewModel.Email }, protocol: Request.Scheme);
                    await _emailSender.SendEmailConfirmationEmailAsync(EditProfileViewModel.Email, callbackUrl);
                    confirmationEmailSent = true;
                }

                if (RefreshSignIn)
                    await _signInManager.SignInAsync(systemUser, RememberMe);

                return RedirectToPage("Index", new { profileUpdated = true, confirmationEmailSent });
            }
            //}
            //catch (Exception ex)
            //{
            //    scope.Dispose();
            //    throw;
            //}
            //}
        }
    }
}