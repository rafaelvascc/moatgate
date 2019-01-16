using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MoatGate.Models.User
{
    public class UserCreateApiViewModel
    {
        [DisplayName("Username")]
        public string UserName { set; get; }

        [Required]
        [DisplayName("Email")]
        [DataType(DataType.EmailAddress)]
        public string Email { set; get; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { set; get; }

        [Required]
        [DataType(DataType.Password)]
        [Compare("Password")]
        [DisplayName("Confirm Password")]
        public string ConfirmPassword { set; get; }

        public IList<string> Roles { set; get; } = new List<string>();
    }
}
