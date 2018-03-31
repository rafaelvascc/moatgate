using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using MoatGate.Models.AspNetIIdentityCore.EntityFramework;
using MoatGate.Models.Profile;
using System.Security.Principal;

namespace MoatGate.Pages.Profile
{
    public class IndexModel : PageModel
    {
        private readonly UserManager<MoatGateIdentityUser> _manager;

        public IndexModel(UserManager<MoatGateIdentityUser> manager)
        {
            _manager = manager;
        }

        [BindProperty]
        public UserProfileViewModel UserProfileViewModel { get; set; }

        public void OnGet()
        {
            UserProfileViewModel = new UserProfileViewModel(User.Claims);
        }
    }
}
