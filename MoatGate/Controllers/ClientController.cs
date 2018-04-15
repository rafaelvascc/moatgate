using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IdentityServer4.EntityFramework.DbContexts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MoatGate.Models.Client;
using MoatGate.Models.Shared;

namespace MoatGate.Controllers
{
    [Produces("application/json")]
    [Route("api/clients")]
    public class ClientController : Controller
    {
        private readonly ConfigurationDbContext _context;

        public ClientController(ConfigurationDbContext context)
        {
            _context = context;
        }

        [HttpPost("search")]
        public DataTableResponse<ClientListItemViewModel> SearchByDatatables(DataTableRequest datatableRequest)
        {
            var baseQuery = from c in _context.Clients.AsNoTracking()
                            select new ClientListItemViewModel
                            {
                                Id = c.Id,
                                ClientId = c.ClientId,
                                ClientName = c.ClientName,
                                Description = c.Description,
                                Enabled = c.Enabled
                            };

            baseQuery = ApplyFilterToQuery(baseQuery, datatableRequest);

            var filterTotal = baseQuery.Count();

            baseQuery = ApplySortToQuery(baseQuery, datatableRequest.Order);

            var queryResult = baseQuery.Skip(datatableRequest.Start)
                            .Take(datatableRequest.Length)
                            .ToList();

            return new DataTableResponse<ClientListItemViewModel>
            {
                Draw = datatableRequest.Draw,
                RecordsTotal = _context.Clients.Count(),
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
            _context.Clients.Remove(await _context.Clients.SingleOrDefaultAsync(c => c.Id == model.Id));
            await _context.SaveChangesAsync();

            return Ok();
        }

        #region Helper Methods
        private IQueryable<ClientListItemViewModel> ApplyFilterToQuery(IQueryable<ClientListItemViewModel> baseQuery, DataTableRequest queryParams)
        {
            foreach (var f in queryParams.Columns)
            {
                if (!f.Searchable || string.IsNullOrWhiteSpace(f.Search?.Value?.ToString()))
                {
                    continue;
                }

                switch (f.Name)
                {
                    case "ClientName":
                        {
                            baseQuery = baseQuery.Where(o => o.ClientId.Contains(f.Search.Value));
                            break;
                        }
                    case "ClientId":
                        {
                            baseQuery = baseQuery.Where(o => o.ClientName.Contains(f.Search.Value));
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

        private IQueryable<ClientListItemViewModel> ApplySortToQuery(IQueryable<ClientListItemViewModel> baseQuery, List<DatatableOrder> orders)
        {
            foreach (var f in orders)
            {
                switch (f.Column)
                {
                    case 0:
                        {
                            if (f.Dir.Equals("asc", StringComparison.InvariantCultureIgnoreCase))
                            {
                                baseQuery = baseQuery.OrderBy(o => o.ClientName);
                            }
                            if (f.Dir.Equals("desc", StringComparison.InvariantCultureIgnoreCase))
                            {
                                baseQuery = baseQuery.OrderByDescending(o => o.ClientName);
                            }
                            break;
                        }
                    case 1:
                        {
                            if (f.Dir.Equals("asc", StringComparison.InvariantCultureIgnoreCase))
                            {
                                baseQuery = baseQuery.OrderBy(o => o.ClientId);
                            }
                            if (f.Dir.Equals("desc", StringComparison.InvariantCultureIgnoreCase))
                            {
                                baseQuery = baseQuery.OrderByDescending(o => o.ClientId);
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