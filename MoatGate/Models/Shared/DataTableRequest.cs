using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

//https://datatables.net/manual/server-side
namespace MoatGate.Models.Shared
{
    public class DataTableRequest
    {
        // properties are not capital due to json mapping
        public int Draw { get; set; }
        public int Start { get; set; }
        public int Length { get; set; }
        public DatatableSearch Search { get; set; } = new DatatableSearch();
        public List<DatabaleColumn> Columns { get; set; } = new List<DatabaleColumn>();
        public List<DatatableOrder> Order { get; set; } = new List<DatatableOrder>();
    }

    public class DatabaleColumn
    {
        public string Data { get; set; }
        public string Name { get; set; }
        public bool Searchable { get; set; }
        public bool Orderable { get; set; }
        public DatatableSearch Search { get; set; } = new DatatableSearch();
    }

    public class DatatableSearch
    {
        public string Value { get; set; }
        public string Regex { get; set; }
    }

    public class DatatableOrder
    {
        public int Column { get; set; }
        public string Dir { get; set; }
    }
}
