using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IdentityServer4.EntityFramework.DbContexts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MoatGate.Models.Shared;

namespace MoatGate.Controllers
{
    [Authorize(Roles = "IdentityAdmin")]
    [Produces("application/json")]
    [Route("api/resources/api")]
    public class ApiResourceController : Controller
    {
        private readonly ConfigurationDbContext _context;

        public ApiResourceController(ConfigurationDbContext context)
        {
            _context = context;
        }

        [HttpPost("search")]
        public DataTableResponse<ResourceListItemViewModel> SearchByDatatables(DataTableRequest datatableRequest)
        {
            var baseQuery = from r in _context.ApiResources.AsNoTracking()
                            select new ResourceListItemViewModel
                            {
                                Id = r.Id,
                                Name = r.Name,
                                DisplayName = r.DisplayName,
                                Description = r.Description,
                                Enabled = r.Enabled
                            };

            baseQuery = ApplyFilterToQuery(baseQuery, datatableRequest);

            var filterTotal = baseQuery.Count();

            baseQuery = ApplySortToQuery(baseQuery, datatableRequest.Order);

            var queryResult = baseQuery.Skip(datatableRequest.Start)
                            .Take(datatableRequest.Length)
                            .ToList();

            return new DataTableResponse<ResourceListItemViewModel>
            {
                Draw = datatableRequest.Draw,
                RecordsTotal = _context.ApiResources.Count(),
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
        public async Task<IActionResult> Delete([FromBody] DeleteViewModel model)
        {
            _context.ApiResources.Remove(await _context.ApiResources.SingleOrDefaultAsync(c => c.Id == model.Id));
            await _context.SaveChangesAsync();

            return Ok();
        }

        #region Helper Methods
        private IQueryable<ResourceListItemViewModel> ApplyFilterToQuery(IQueryable<ResourceListItemViewModel> baseQuery, DataTableRequest queryParams)
        {
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
                            baseQuery = baseQuery.Where(o => o.Name.Contains(f.Search.Value));
                            break;
                        }
                    case "DisplayName":
                        {
                            baseQuery = baseQuery.Where(o => o.DisplayName.Contains(f.Search.Value));
                            break;
                        }
                    case "Description":
                        {
                            baseQuery = baseQuery.Where(o => o.Description.Contains(f.Search.Value));
                            break;
                        }
                    case "Enabled":
                        {
                            baseQuery = baseQuery.Where(o => o.Enabled == bool.Parse(f.Search.Value));
                            break;
                        }
                }
            }

            return baseQuery;
        }

        private IQueryable<ResourceListItemViewModel> ApplySortToQuery(IQueryable<ResourceListItemViewModel> baseQuery, List<DatatableOrder> orders)
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
                                baseQuery = baseQuery.OrderBy(o => o.DisplayName);
                            }
                            if (f.Dir.Equals("desc", StringComparison.InvariantCultureIgnoreCase))
                            {
                                baseQuery = baseQuery.OrderByDescending(o => o.DisplayName);
                            }
                            break;
                        }
                    case 2:
                        {
                            if (f.Dir.Equals("asc", StringComparison.InvariantCultureIgnoreCase))
                            {
                                baseQuery = baseQuery.OrderBy(o => o.Description);
                            }
                            if (f.Dir.Equals("desc", StringComparison.InvariantCultureIgnoreCase))
                            {
                                baseQuery = baseQuery.OrderByDescending(o => o.Description);
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