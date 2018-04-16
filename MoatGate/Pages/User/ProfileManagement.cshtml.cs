using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MoatGate.Services;
using MoatGate.Models.AspNetIIdentityCore.EntityFramework;
using MoatGate.Models.Profile;

namespace MoatGate.Pages.User
{
    public class ProfileManagementModel : PageModel
    {
        private readonly UserManager<MoatGateIdentityUser> _userManager;
        private readonly RoleManager<MoatGateIdentityRole> _roleManager;
        private readonly IEmailSender _emailSender;

        public string Id { set; get; }

        public string Username { set; get; }

        public IDictionary<string, string> Cultures { set; get; } = new Dictionary<string, string>();

        public IDictionary<string, string> TimeZones { set; get; } = new Dictionary<string, string>();

        [BindProperty]
        public EdiProfileViewModel EditProfileViewModel { get; set; }

        [BindProperty]
        [DisplayName("Assign Roles")]
        public Dictionary<string, bool> RoleChecks { set; get; } = new Dictionary<string, bool>();

        public ProfileManagementModel(UserManager<MoatGateIdentityUser> userManager, RoleManager<MoatGateIdentityRole> roleManager, IEmailSender emailSender)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _emailSender = emailSender;
        }

        public async Task OnGetAsync(int id)
        {
            var user = await _userManager.FindByIdAsync(id.ToString());
            var userClaims = await _userManager.GetClaimsAsync(user);
            var userRoles = await _userManager.GetRolesAsync(user);
            Id = id.ToString();
            Username = user.UserName;
            EditProfileViewModel = new EdiProfileViewModel(user, userClaims);
            RoleChecks = _roleManager.Roles.ToDictionary(k => k.Name, v => userRoles.Contains(v.Name));
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
            IList<IdentityResult> resultFinal = new List<IdentityResult>();
            var id = _userManager.GetUserId(User);
            var systemUser = await _userManager.FindByIdAsync(id);
            var oldEmail = systemUser.Email;

            //Profile update
            EditProfileViewModel.UpdatedAt = DateTime.Now;
            var newValues = EditProfileViewModel.ToClaims();
            var currentValues = await _userManager.GetClaimsAsync(systemUser);

            var toInsert = newValues.Where(nv => !currentValues.Select(c => c.Type).Distinct().Contains(nv.Type));
            var toUpdate = newValues.Where(nv => currentValues.Select(c => c.Type).Distinct().Contains(nv.Type) &&
                currentValues.Where(c => c.Type == nv.Type).Single().Value != newValues.Where(c => c.Type == nv.Type).Single().Value);
            var toRemove = currentValues.Where(ov => !newValues.Select(c => c.Type).Distinct().Contains(ov.Type) ||
                string.IsNullOrWhiteSpace(newValues.Where(c => c.Type == ov.Type).Single().Value));

            var resultAdd = await _userManager.AddClaimsAsync(systemUser, toInsert);
            var resultRemove = await _userManager.RemoveClaimsAsync(systemUser, toRemove);

            resultFinal.Add(resultAdd);
            resultFinal.Add(resultRemove);

            foreach (var newClaim in toUpdate)
            {
                var oldClaim = User.Claims.SingleOrDefault(c => c.Type == newClaim.Type);
                if (oldClaim != null)
                {
                    resultFinal.Add(await _userManager.ReplaceClaimAsync(systemUser, oldClaim, newClaim));
                }
            }

            //Email update
            if (systemUser.NormalizedEmail != _userManager.KeyNormalizer.Normalize(EditProfileViewModel.Email))
            {
                var token = await _userManager.GenerateChangeEmailTokenAsync(systemUser, EditProfileViewModel.Email);
                var resultEmailChange = await _userManager.ChangeEmailAsync(systemUser, EditProfileViewModel.Email, token);
                resultFinal.Add(resultEmailChange);
                if (resultEmailChange.Succeeded)
                {
                    await _emailSender.SendEmailChangeAlertEmailAsync(systemUser.Email);
                    confirmationEmailSent = true;
                }
            }

            if (systemUser.PhoneNumber != EditProfileViewModel.PhoneNumber)
            {
                var token = await _userManager.GenerateChangePhoneNumberTokenAsync(systemUser, EditProfileViewModel.PhoneNumber);
                var resultPhoneChange = await _userManager.ChangePhoneNumberAsync(systemUser, EditProfileViewModel.PhoneNumber, token);
                resultFinal.Add(resultPhoneChange);
                if (resultPhoneChange.Succeeded)
                {
                    await _emailSender.SendPhoneNumberChangeAlertEmailAsync(systemUser.Email);
                    confirmationEmailSent = true;
                }
            }

            //Role Update
            var userRoles = await _userManager.GetRolesAsync(systemUser);

            var rolesToAdd = RoleChecks.Where(r => r.Value && !userRoles.Contains(r.Key)).Select(r => r.Key).ToList();
            var rolesToRemove = RoleChecks.Where(r => !r.Value && userRoles.Contains(r.Key)).Select(r => r.Key).ToList();

            var roleAddResult = await _userManager.AddToRolesAsync(systemUser, rolesToAdd);
            var roleRemoveResult =await _userManager.RemoveFromRolesAsync(systemUser, rolesToRemove);
            
            resultFinal.Add(roleAddResult);
            resultFinal.Add(roleRemoveResult);

            //Final Check
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
                if (resultFinal.Any(r => r.Succeeded))
                {
                    await _emailSender.SendProfileChangeAlertEmailAsync(oldEmail);
                }

                //scope.Complete();
                return RedirectToPage("List", new { profileUpdated = true, confirmationEmailSent });
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