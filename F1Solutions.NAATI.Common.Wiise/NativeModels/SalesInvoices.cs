using Newtonsoft.Json;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace F1Solutions.Naati.Common.Wiise.NativeModels
{
    [DataContract]
    public class SalesInvoices
    {
        [JsonProperty("@odata.context")]
        public string OdataContext { get; set; }
        [JsonProperty("Value")]
        public List<SalesInvoice> _SalesInvoices { get; set; }

        public SalesInvoices()
        {
            _SalesInvoices = new List<SalesInvoice>();
        }
    }
}