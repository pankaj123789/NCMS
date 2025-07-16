using Newtonsoft.Json;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace F1Solutions.Naati.Common.Wiise.NativeModels
{
    [DataContract]
    public class PurchaseInvoices
    {
        [JsonProperty("@odata.context")]
        public string OdataContext { get; set; }
        [JsonProperty("Value")]
        public List<PurchaseInvoice> _PurchaseInvoices { get; set; }
        public PurchaseInvoices()
        {
            _PurchaseInvoices = new List<PurchaseInvoice>();
        }
    }
}