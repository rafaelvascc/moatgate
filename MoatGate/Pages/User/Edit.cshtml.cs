using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MoatGate.Models.AspNetIIdentityCore.EntityFramework;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using MoatGate.Models.User;
using AutoMapper;
using System.Linq;

namespace MoatGate.Pages.User
{
    public class EditModel : PageModel
    {
        private readonly UserManager<MoatGateIdentityUser> _userManager;
        private readonly RoleManager<MoatGateIdentityRole> _roleManager;

        [BindProperty]
        public MoatGateIdentityUserEditViewModel MoatGateIdentityUser { get; set; } = new MoatGateIdentityUserEditViewModel() { Id = 0 };

        public EditModel(UserManager<MoatGateIdentityUser> userManager, RoleManager<MoatGateIdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task<IActionResult> OnGet(int id)
        {
            ViewData["Title"] = "Edit User";
            var currentUser = await _userManager.FindByIdAsync(id.ToString());
            var userRoles = await _userManager.GetRolesAsync(currentUser);
            var allRoles = _roleManager.Roles.ToDictionary(k => k.Name, v => userRoles.Contains(v.Name));

            Mapper.Map(currentUser, MoatGateIdentityUser);
            MoatGateIdentityUser.RoleChecks = allRoles;

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            //IdentityResult result = null;

            //TODO: Do not hard update user profile for now, only update roles
            var currentUser = await _userManager.FindByIdAsync(MoatGateIdentityUser.Id.ToString());
            //Mapper.Map(MoatGateIdentityUser, currentUser);
            //result = await _userManager.UpdateAsync(currentUser);

            //if (!result.Succeeded)
            //{
            //    foreach (var error in result.Errors)
            //    {
            //        ModelState.AddModelError(error.Code, error.Description);
            //    }
            //    return Page();
            //}

            var userRoles = await _userManager.GetRolesAsync(currentUser);

            var rolesToAdd = MoatGateIdentityUser.RoleChecks.Where(r => r.Value).Where(r => !userRoles.Contains(r.Key)).Select(r => r.Key).ToList();
            var rolesToRemove = MoatGateIdentityUser.RoleChecks.Where(r => !r.Value).Where(r => userRoles.Contains(r.Key)).Select(r => r.Key).ToList();

            await _userManager.AddToRolesAsync(currentUser, rolesToAdd);
            await _userManager.RemoveFromRolesAsync(currentUser, rolesToRemove);

            return RedirectToPage("./List");
        }
    }
}