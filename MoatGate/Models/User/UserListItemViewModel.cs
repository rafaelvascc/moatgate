using System.Collections.Generic;
using System.Linq;

namespace MoatGate.Models.User
{
    public class UserListItemViewModel
    {
        public string Name => $"{(Claims.Any(c => c.Key == "given_name") ? Claims.Single(c => c.Key == "given_name").Value + " " : string.Empty)}" +
                              $"{(Claims.Any(c => c.Key == "middle_name") ? Claims.Single(c => c.Key == "middle_name").Value + " " : string.Empty)}" +
                              $"{(Claims.Any(c => c.Key == "family_name") ? Claims.Single(c => c.Key == "family_name").Value : string.Empty)}";
        public UserViewModel User { set; get; }
        public Dictionary<string, string> Claims { set; get; }
        public List<string> Roles { set; get; }
    }
}
