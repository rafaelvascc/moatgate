using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using MoatGate.Models.AspNetIIdentityCore.EntityFramework;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace MoatGate.Pages.User
{
    public class CreateModel : PageModel
    {
        private readonly UserManager<MoatGateIdentityUser> _userManager;

        public CreateModel(UserManager<MoatGateIdentityUser> userManager)
        {
            _userManager = userManager;
        }

        public IActionResult OnGet()
        {
            return Page();
        }

        [BindProperty]
        public MoatGateIdentityUser MoatGateIdentityUser { get; set; }
        
        [Required]
        [DataType(DataType.Password)]
        [BindProperty]
        public string Password { get; set; }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var result = await _userManager.CreateAsync(MoatGateIdentityUser, Password);

            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(error.Code, error.Description);
                }
                return Page();
            }

            return RedirectToPage("./Index");
        }
    }
}