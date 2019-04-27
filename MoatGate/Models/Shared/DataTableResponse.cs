using Newtonsoft.Json;
using System;
using System.Collections.Generic;

//https://datatables.net/manual/server-side
namespace MoatGate.Models.Shared
{
    public class DataTableResponse<T>
    {
        [JsonProperty(PropertyName = "draw")]
        public int Draw { set; get; }

        [JsonProperty(PropertyName = "recordsTotal")]
        public int RecordsTotal { set; get; }

        [JsonProperty(PropertyName = "recordsFiltered")]
        public int RecordsFiltered { set; get; }

        [JsonProperty(PropertyName = "data")]
        public List<T> Data { set; get; }

        [JsonProperty(PropertyName = "error")]
        public String Error { set; get; }
    }
}
