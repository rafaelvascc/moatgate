using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MoatGate.Models.User
{
    public class UserCreateViewModel
    {
        public Guid Id { set; get; } = Guid.Empty;

        [Required]
        [DisplayName("Username")]
        public string UserName { set; get; }

        [Required]
        [DisplayName("Email")]
        [DataType(DataType.EmailAddress)]
        public string Email { set; get; }

        [DisplayName("Phone Number")]
        [DataType(DataType.PhoneNumber)]
        public string PhoneNumber { set; get; }

        [DisplayName("Enable Two Factor Authentication")]
        public bool TwoFactorEnabled { set; get; } = false;

        [DisplayName("Assign Roles")]
        public Dictionary<string, bool> RoleChecks { set; get; } = new Dictionary<string, bool>();

        [Required]
        [DataType(DataType.Password)]
        public string Password { set; get; }

        [Required]
        [DataType(DataType.Password)]
        [Compare("Password")]
        [DisplayName("Confirm Password")]
        public string ConfirmPassword { set; get; }
    }
}
