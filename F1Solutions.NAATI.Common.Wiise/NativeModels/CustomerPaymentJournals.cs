using Newtonsoft.Json;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace F1Solutions.Naati.Common.Wiise.NativeModels
{
    [DataContract]
    public class CustomerPaymentJournals
    {
        public CustomerPaymentJournals()
        {
            _CustomerPaymentJournals = new List<CustomerPaymentJournal>();
        }
        [JsonProperty("@odata.context")]
        public string OdataContext { get; set; }
        [JsonProperty("Value")]
        public List<CustomerPaymentJournal> _CustomerPaymentJournals { get; set; }
    }
}
