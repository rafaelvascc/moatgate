using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MoatGate.Services;
using MoatGate.Models.AspNetIIdentityCore.EntityFramework;
using Microsoft.AspNetCore.Authorization;
using System.Numerics;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using SixLabors.Primitives;
using SixLabors.Shapes;
using SixLabors.ImageSharp.Formats.Png;
using System.Security.Claims;

namespace MoatGate.Controllers
{
    [ApiExplorerSettings(IgnoreApi = true)]
    [Authorize]
    [Produces("application/json")]
    [Route("api/internal/profile")]
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

        [HttpGet("avatar")]
        public async Task<IActionResult> GetUserAvatar()
        {
            var id = _manager.GetUserId(User);
            var systemUser = await _manager.FindByIdAsync(id);
            var userClaims = await _manager.GetClaimsAsync(systemUser);

            if (userClaims.Any(c => c.Type == "picture"))
            {
                var picClaim = userClaims.FirstOrDefault(c => c.Type == "picture");
                var bytes = Convert.FromBase64String(picClaim.Value.Substring("data:image/png;base64,".Length));
                return File(bytes, @"image/png", $"avatar_{id}.png");
            }
            else
            {
                return NotFound();
            }
        }

        [Authorize]
        [HttpGet("avatar/{userId}")]
        public async Task<IActionResult> GetUserAvatar(string userId)
        {
            var systemUser = await _manager.FindByIdAsync(userId);
            var userClaims = await _manager.GetClaimsAsync(systemUser);

            if (userClaims.Any(c => c.Type == "picture"))
            {
                var picClaim = userClaims.FirstOrDefault(c => c.Type == "picture");
                var bytes = Convert.FromBase64String(picClaim.Value.Substring("data:image/png;base64,".Length));
                return File(bytes, @"image/png", $"avatar_{userId}.png");
            }
            else
            {
                return NotFound();
            }
        }

        [HttpPost("avatar")]
        public IActionResult ProcessProfileAvatar(IFormFile avatar)
        {
            var encodedImage = string.Empty;
            var id = _manager.GetUserId(User);
            using (var imageStream = avatar.OpenReadStream())
            {
                using (Image<Rgba32> image = Image.Load(imageStream))
                {
                    var originalWidth = image.Width;
                    var originalHeight = image.Height;
                    var rec = new Rectangle(0, 0, originalWidth, originalHeight);

                    //If it is not a square, make it a square
                    if (originalWidth != originalHeight)
                    {
                        if (originalWidth > originalHeight)
                        {
                            var offsetWidth = (originalWidth - originalHeight) / 2;
                            rec.X = offsetWidth;
                            rec.Y = 0;
                            rec.Width = originalHeight;
                            rec.Height = originalHeight;
                        }
                        else
                        {
                            var offsetHeight = (originalHeight - originalWidth) / 2;
                            rec.X = 0;
                            rec.Y = offsetHeight;
                            rec.Width = originalWidth;
                            rec.Height = originalWidth;
                        }

                        image.Mutate(x => x.Crop(rec));
                    }

                    image.Mutate(x => x.AutoOrient().Resize(160, 160));
                    encodedImage = image.ToBase64String(PngFormat.Instance);
                }
            }

            return Ok(encodedImage);
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

        #region Avater processing methods
        #endregion
    }
}