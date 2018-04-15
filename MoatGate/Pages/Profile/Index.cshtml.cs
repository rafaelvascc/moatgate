using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MoatGate.Models.AspNetIIdentityCore.EntityFramework;
using MoatGate.Models.Profile;

namespace MoatGate.Pages.Profile
{
    public class IndexModel : PageModel
    {
        private readonly UserManager<MoatGateIdentityUser> _manager;

        public IndexModel(UserManager<MoatGateIdentityUser> manager)
        {
            _manager = manager;
        }

        public string Id { set; get; }

        public string Username { set; get; }

        public UserProfileViewModel UserProfileViewModel { get; set; }

        public IList<string> Roles { set; get; } = new List<string>();

        public async Task OnGetAsync()
        {
            var id = _manager.GetUserId(User);
            var user = await _manager.FindByIdAsync(id);
            Id = id.ToString();
            Username = user.UserName;
            Roles = await _manager.GetRolesAsync(user);
            UserProfileViewModel = new UserProfileViewModel(User.Claims);
        }
    }
}
