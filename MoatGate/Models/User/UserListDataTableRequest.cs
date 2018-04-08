using MoatGate.Models.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MoatGate.Models.User
{
    public class UserListDataTableRequest : DataTableRequest
    {
        public int? Id { set; get; }
        public List<string> Roles { set; get; } = new List<string>();
    }
}
