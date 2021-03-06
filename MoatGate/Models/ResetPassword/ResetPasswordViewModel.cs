﻿using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace MoatGate.Models.ResetPassword
{
    public class ResetPasswordViewModel
    {
        public Guid UserId { set; get; }

        [DisplayName("New Password")]
        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [DisplayName("Confirm New Password")]
        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }

        public string Code { get; set; }
    }
}
