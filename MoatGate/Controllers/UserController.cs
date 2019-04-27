using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MoatGate.Models.AspNetIIdentityCore.EntityFramework;
using MoatGate.Models.Shared;
using MoatGate.Models.User;

namespace MoatGate.Controllers
{
    [ApiExplorerSettings(IgnoreApi = true)]
    [Authorize(Roles = "IdentityAdmin")]
    [Produces("application/json")]
    [Route("api/internal/users")]
    public class UserController : Controller
    {
        private readonly MoatGateIdentityDbContext _context;
        private readonly UserManager<MoatGateIdentityUser> _userManager;

        public UserController(MoatGateIdentityDbContext context, UserManager<MoatGateIdentityUser> manager)
        {
            _context = context;
            _userManager = manager;
        }

        [HttpPost("search")]
        public DataTableResponse<UserListItemViewModel> SearchByDatatables(UserListDataTableRequest datatableRequest)
        {
            var baseQuery = from u in _context.Users.AsNoTracking()
                            select new UserListItemViewModel
                            {
                                User = new UserViewModel
                                {
                                    Id = u.Id,
                                    UserName = u.UserName,
                                    Email = u.Email,
                                    PhoneNumber = u.PhoneNumber,
                                    EmailConfirmed = u.EmailConfirmed,
                                    PhoneNumberConfirmed = u.PhoneNumberConfirmed,
                                    TwoFactorEnabled = u.TwoFactorEnabled,
                                    LockoutEnabled = u.LockoutEnabled,
                                    AccessFailedCount = u.AccessFailedCount,
                                    LockoutEnd = u.LockoutEnd
                                },
                                Roles = (from ur in _context.UserRoles.AsNoTracking()
                                         join r in _context.Roles.AsNoTracking() on ur.RoleId equals r.Id
                                         where ur.UserId == u.Id
                                         select r.Name).ToList(),
                                Claims = (from c in _context.UserClaims.AsNoTracking()
                                          where c.UserId == u.Id
                                          select c).ToDictionary(c => c.ClaimType, c => c.ClaimValue)
                            };

            baseQuery = ApplyFilterToQuery(baseQuery, datatableRequest);

            var filterTotal = baseQuery.Count();

            baseQuery = ApplySortToQuery(baseQuery, datatableRequest.Order);

            var queryResult = baseQuery.Skip(datatableRequest.Start)
                            .Take(datatableRequest.Length)
                            .ToList();

            return new DataTableResponse<UserListItemViewModel>
            {
                Draw = datatableRequest.Draw,
                RecordsTotal = _context.Users.Count(),
                RecordsFiltered = filterTotal,
                Data = queryResult
            };
        }

        /// <summary>
        /// Deletes user. Using POST method and puting the user id in the body to prevent accidental deletion
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost("delete")]
        public async Task<IActionResult> Delete([FromBody] DeleteViewModelGuid model)
        {
            var user = await _userManager.FindByIdAsync(model.Id.ToString());

            if (user == null)
                return NotFound();

            var result = await _userManager.DeleteAsync(user);

            if (!result.Succeeded)
            {
                return StatusCode(500);
            }

            return Ok();
        }

        #region Helper Methods
        private IQueryable<UserListItemViewModel> ApplyFilterToQuery(IQueryable<UserListItemViewModel> baseQuery, UserListDataTableRequest queryParams)
        {
            if (queryParams.Id.HasValue)
            {
                baseQuery = baseQuery.Where(o => o.User.Id == queryParams.Id);
            }

            foreach (var f in queryParams.Columns)
            {
                if (!f.Searchable || string.IsNullOrWhiteSpace(f.Search?.Value?.ToString()))
                {
                    continue;
                }

                switch (f.Name)
                {
                    case "Name":
                        {
                            baseQuery = baseQuery.Where(o => o.Claims.Any(g => (g.Key == "given_name" || g.Key == "middle_name" || g.Key == "family_name") && g.Value.Contains(f.Search.Value)));
                            break;
                        }
                    case "UserName":
                        {
                            baseQuery = baseQuery.Where(o => o.User.UserName.Contains(f.Search.Value));
                            break;
                        }
                    case "Email":
                        {
                            baseQuery = baseQuery.Where(o => o.User.Email.Contains(f.Search.Value));
                            break;
                        }
                    case "PhoneNumber":
                        {
                            baseQuery = baseQuery.Where(o => o.User.PhoneNumber.Contains(f.Search.Value));
                            break;
                        }
                    case "LockoutEndFrom":
                        {
                            baseQuery = baseQuery.Where(o => o.User.LockoutEnd >= DateTime.Parse(f.Search.Value));
                            break;
                        }
                    case "LockoutEndFromTo":
                        {
                            baseQuery = baseQuery.Where(o => o.User.LockoutEnd <= DateTime.Parse(f.Search.Value));
                            break;
                        }
                    case "LockoutEnabled":
                        {
                            baseQuery = baseQuery.Where(o => o.User.LockoutEnabled == bool.Parse(f.Search.Value));
                            break;
                        }
                    case "AccessFailedCount":
                        {
                            baseQuery = baseQuery.Where(o => o.User.AccessFailedCount == Int32.Parse(f.Search.Value));
                            break;
                        }
                }
            }

            if (queryParams.Roles.Any())
            {
                baseQuery = baseQuery.Where(o => queryParams.Roles.Intersect(o.Roles).Any());
            }

            return baseQuery;
        }

        private IQueryable<UserListItemViewModel> ApplySortToQuery(IQueryable<UserListItemViewModel> baseQuery, List<DatatableOrder> orders)
        {
            foreach (var f in orders)
            {
                switch (f.Column)
                {
                    case 0:
                        {
                            if (f.Dir.Equals("asc", StringComparison.InvariantCultureIgnoreCase))
                            {
                                baseQuery = baseQuery.OrderBy(o => o.Name);
                            }
                            if (f.Dir.Equals("desc", StringComparison.InvariantCultureIgnoreCase))
                            {
                                baseQuery = baseQuery.OrderByDescending(o => o.Name);
                            }
                            break;
                        }
                    case 1:
                        {
                            if (f.Dir.Equals("asc", StringComparison.InvariantCultureIgnoreCase))
                            {
                                baseQuery = baseQuery.OrderBy(o => o.User.UserName);
                            }
                            if (f.Dir.Equals("desc", StringComparison.InvariantCultureIgnoreCase))
                            {
                                baseQuery = baseQuery.OrderByDescending(o => o.User.UserName);
                            }
                            break;
                        }
                    case 2:
                        {
                            if (f.Dir.Equals("asc", StringComparison.InvariantCultureIgnoreCase))
                            {
                                baseQuery = baseQuery.OrderBy(o => o.User.Email);
                            }
                            if (f.Dir.Equals("desc", StringComparison.InvariantCultureIgnoreCase))
                            {
                                baseQuery = baseQuery.OrderByDescending(o => o.User.Email);
                            }
                            break;
                        }
                    case 3:
                        {
                            if (f.Dir.Equals("asc", StringComparison.InvariantCultureIgnoreCase))
                            {
                                baseQuery = baseQuery.OrderBy(o => o.User.PhoneNumber);
                            }
                            if (f.Dir.Equals("desc", StringComparison.InvariantCultureIgnoreCase))
                            {
                                baseQuery = baseQuery.OrderByDescending(o => o.User.PhoneNumber);
                            }
                            break;
                        }
                }
            }

            return baseQuery;
        }
        #endregion
    }
}