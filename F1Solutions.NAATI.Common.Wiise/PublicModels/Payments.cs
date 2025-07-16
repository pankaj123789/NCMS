using Newtonsoft.Json;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace F1Solutions.Naati.Common.Wiise.PublicModels
{
    [DataContract]
    public class Payments
    {
        [JsonProperty("@odata.context")]
        public string OdataContext { get; set; }
        [JsonProperty("Value")]
        public List<Payment> _Payments { get; set; }

        public Payments()
        {
            _Payments = new List<Payment>();
        }
    }
}
