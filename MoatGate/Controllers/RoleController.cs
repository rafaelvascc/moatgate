using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MoatGate.Models.AspNetIIdentityCore.EntityFramework;
using MoatGate.Models.Role;
using MoatGate.Models.Shared;

namespace MoatGate.Controllers
{
    [ApiExplorerSettings(IgnoreApi = true)]
    [Authorize(Roles = "IdentityAdmin")]
    [Produces("application/json")]
    [Route("api/internal/roles")]
    public class RoleController : Controller
    {
        private readonly MoatGateIdentityDbContext _context;
        private readonly RoleManager<MoatGateIdentityRole> _manager;

        public RoleController(MoatGateIdentityDbContext context, RoleManager<MoatGateIdentityRole> manager)
        {
            _context = context;
            _manager = manager;
        }

        [HttpPost("search")]
        public DataTableResponse<RoleListItemViewModel> SearchByDatatables(DataTableRequest datatableRequest)
        {
            var baseQuery = from r in _context.Roles.AsNoTracking()
                            select new RoleListItemViewModel
                            {
                                Id = r.Id,
                                Name = r.Name
                            };

            if (!string.IsNullOrEmpty(datatableRequest.Search?.Value))
            {
                baseQuery = baseQuery.Where(r => r.Name.Contains(datatableRequest.Search.Value));
            }

            var filterTotal = baseQuery.Count();

            if (!string.IsNullOrEmpty(datatableRequest.Order?.FirstOrDefault()?.Dir))
            {

                if (datatableRequest.Order.FirstOrDefault().Dir.Equals("asc", StringComparison.InvariantCultureIgnoreCase))
                {
                    baseQuery = baseQuery.OrderBy(o => o.Name);
                }
                if (datatableRequest.Order.FirstOrDefault().Dir.Equals("desc", StringComparison.InvariantCultureIgnoreCase))
                {
                    baseQuery = baseQuery.OrderByDescending(o => o.Name);
                }
            }

            var queryResult = baseQuery.Skip(datatableRequest.Start)
                            .Take(datatableRequest.Length)
                            .ToList();

            return new DataTableResponse<RoleListItemViewModel>
            {
                Draw = datatableRequest.Draw,
                RecordsTotal = _context.Roles.Count(),
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
            var user = await _manager.FindByIdAsync(model.Id.ToString());

            if (user == null)
                return NotFound();

            var result = await _manager.DeleteAsync(user);

            if (!result.Succeeded)
            {
                return StatusCode(500);
            }

            return Ok();
        }
    }
}