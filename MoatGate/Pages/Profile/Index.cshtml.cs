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

        [BindProperty]
        public IList<string> Roles { set; get; } = new List<string>();

        public async Task OnGetAsync()
        {
            var id = _manager.GetUserId(User);
            var user = await _manager.FindByIdAsync(id);
            Roles = await _manager.GetRolesAsync(user);
            UserProfileViewModel = new UserProfileViewModel(User.Claims);
        }
    }
}
