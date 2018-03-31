using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MoatGate.Models.AspNetIIdentityCore.EntityFramework;
using MoatGate.Models.Profile;

namespace MoatGate.Pages.Profile
{
    public class EditModel : PageModel
    {
        private readonly UserManager<MoatGateIdentityUser> _manager;
        private readonly SignInManager<MoatGateIdentityUser> _signInManager;

        [BindProperty]
        public EdiProfileViewModel EditProfileViewModel { get; set; }

        public EditModel(UserManager<MoatGateIdentityUser> manager, SignInManager<MoatGateIdentityUser> signInManager)
        {
            _manager = manager;
            _signInManager = signInManager;
        }

        public void OnGet()
        {
            EditProfileViewModel = new EdiProfileViewModel(User.Claims);
        }

        public async Task<IActionResult> OnPostAsync()
        {
            EditProfileViewModel.UpdatedAt = DateTime.Now;
            var newValues = EditProfileViewModel.ToClaims();
            var currentValues = User.Claims;

            var toInsert = newValues.Where(nv => !currentValues.Select(c => c.Type).Distinct().Contains(nv.Type));
            var toUpdate = newValues.Where(nv => currentValues.Select(c => c.Type).Distinct().Contains(nv.Type) &&
                currentValues.Where(c => c.Type == nv.Type).Single().Value != newValues.Where(c => c.Type == nv.Type).Single().Value);
            var toRemove = currentValues.Where(ov => !newValues.Select(c => c.Type).Distinct().Contains(ov.Type) ||
                string.IsNullOrWhiteSpace(newValues.Where(c => c.Type == ov.Type).Single().Value));

            var id = _manager.GetUserId(User);
            var systemUser = await _manager.FindByIdAsync(id);

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

                return Page();
            }
            else
            {
                //Signin again to update claim
                //TODO: look how ot recover current remember me
                await _signInManager.SignInAsync(systemUser, true);
                return RedirectToPage("Index");
            }
        }
    }
}