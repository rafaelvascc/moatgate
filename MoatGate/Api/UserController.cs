using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MoatGate.Models.AspNetIIdentityCore.EntityFramework;
using MoatGate.Models.User;

namespace MoatGate.Api
{
    [Produces("application/json")]
    [Route("api/users")]
    public class UserController : Controller
    {
        private readonly UserManager<MoatGateIdentityUser> _userManager;

        public UserController(UserManager<MoatGateIdentityUser> manager)
        {
            _userManager = manager;
        }

        [HttpPost("create")]
        public async Task<ActionResult> CreateNew([FromBody] UserCreateApiViewModel user)
        {
            var newUser = new MoatGateIdentityUser();
            user.UserName = user.UserName ?? user.Email;
            Mapper.Map(user, newUser);
            var result = await _userManager.CreateAsync(newUser, user.Password);

            if (!result.Succeeded)
            {
                return StatusCode(500, result.Errors.Select(e => e.Description));
            }

            if (user.Roles.Any())
            {
                var roleResult = await _userManager.AddToRolesAsync(newUser, user.Roles);

                if (!roleResult.Succeeded)
                {
                    return StatusCode(500, result.Errors.Select(e => e.Description));
                }
            }

            return Ok(new { Message = $"User {user.UserName} created successfully" });
        }
    }
}