using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MoatGate.Models.User
{
    public class MoatGateIdentityUserViewModel
    {
        public int Id { set; get; } = 0;

        [Required]
        public string UserName { set; get; }
        
        [Required]
        [DataType(DataType.EmailAddress)]
        public string Email { set; get; }
        
        [DataType(DataType.PhoneNumber)]
        public string PhoneNumber { set; get; }

        public bool TwoFactorEnabled { set; get; } = false;

        [DisplayName("Assign Roles")]
        public Dictionary<string, bool> RoleChecks { set; get; } = new Dictionary<string, bool>();
    }
}
