using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MoatGate.Models.User
{
    public class MoatGateIdentityUserEditViewModel : MoatGateIdentityUserViewModel
    {
        [DisplayName("Assign Roles")]
        public bool SendResetPasswordEmail { set; get; }
    }
}
