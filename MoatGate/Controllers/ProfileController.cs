using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MoatGate.Helpers;
using MoatGate.Models.AspNetIIdentityCore.EntityFramework;

namespace MoatGate.Controllers
{
    [Produces("application/json")]
    [Route("api/profile")]
    public class ProfileController : Controller
    {
        private readonly UserManager<MoatGateIdentityUser> _manager;
        private readonly SignInManager<MoatGateIdentityUser> _signInManager;
        private readonly IEmailSender _emailSender;
        private readonly ISmsSender _smsSender;

        public ProfileController(UserManager<MoatGateIdentityUser> manager, SignInManager<MoatGateIdentityUser> signInManager, IEmailSender emailSender, ISmsSender smsSender)
        {
            _manager = manager;
            _emailSender = emailSender;
            _smsSender = smsSender;
            _signInManager = signInManager;
        }

        [HttpGet("sendemailconfirm")]
        public async Task<IActionResult> SendEmailConfirmation()
        {
            var id = _manager.GetUserId(User);
            var systemUser = await _manager.FindByIdAsync(id);
            var token = await _manager.GenerateEmailConfirmationTokenAsync(systemUser);
            var callbackUrl = Url.Page("/Account/ConfirmEmail", pageHandler: null, values: new { userId = id, token }, protocol: Request.Scheme);
            await _emailSender.SendEmailConfirmationEmailAsync(systemUser.Email, callbackUrl);
            return Ok();
        }

        [HttpGet("sendphoneconfirm")]
        public async Task<IActionResult> SendPhoneNumberConfirmation()
        {
            var id = _manager.GetUserId(User);
            var systemUser = await _manager.FindByIdAsync(id);
            var token = await _manager.GenerateChangePhoneNumberTokenAsync(systemUser, systemUser.PhoneNumber);
            await _smsSender.SendPhoneNumberChangeConfirmationSMSAsync(systemUser.PhoneNumber, token);
            return Ok();
        }

        [HttpPost("confirmphone/{token}")]
        public async Task<IActionResult> ConfirmPhoneNumber(string token)
        {
            var id = _manager.GetUserId(User);
            var systemUser = await _manager.FindByIdAsync(id);
            var resultTokenValidation = await _manager.VerifyChangePhoneNumberTokenAsync(systemUser, token, systemUser.PhoneNumber);
            if (!resultTokenValidation)
            {
                throw new ApplicationException($"Error confirming phone number for user with ID '{id}':");
            }
            else
            {
                systemUser.PhoneNumberConfirmed = true;
                var resultConfirm = await _manager.UpdateAsync(systemUser);

                if (resultConfirm.Succeeded && User.Identity.IsAuthenticated)
                {
                    //TODO: find a way to get the current authentication if it is persitent or not
                    await _signInManager.SignInAsync(systemUser, false);
                }
            }
            return Ok();
        }
    }
}