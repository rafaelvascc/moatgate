using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MoatGate.Models.Shared
{
    public class ResourceListItemViewModel
    {
        public int Id { set; get; }
        public string Name { set; get; }
        public string DisplayName { set; get; }
        public string Description { set; get; }
        public bool Enabled { set; get; }
    }
}
