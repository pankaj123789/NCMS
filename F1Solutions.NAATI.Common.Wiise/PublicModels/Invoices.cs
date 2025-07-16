using Newtonsoft.Json;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace F1Solutions.Naati.Common.Wiise.PublicModels
{
    [DataContract]
    public class Invoices
    {
        [JsonProperty("@odata.context")]
        public string OdataContext { get; set; }
        [JsonProperty("Value")]
        public List<Invoice> _Invoices { get; set; }

        public Invoices()
        {
            _Invoices = new List<Invoice>();
        }
    }
}