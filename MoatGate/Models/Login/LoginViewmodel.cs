using System.ComponentModel.DataAnnotations;

namespace MoatGate.Models.Login
{
    public class LoginViewmodel
    {
        [Required]
        public string UserName { set; get; }
        [Required]
        [DataType(DataType.Password)]
        public string Password { set; get; }
        public bool RememberMe { set; get; } = false;
    }
}
