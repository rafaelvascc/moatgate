using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MoatGate.Models.Client
{
    public class ClientListItemViewModel
    {
        public int Id { set; get; }
        public string ClientName { set; get; }
        public string ClientId { set; get; }
        public string Description { set; get; }
        public bool Enabled { set; get; }
    }
}
