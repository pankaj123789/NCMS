using Newtonsoft.Json;
using System;

namespace F1Solutions.Naati.Common.Wiise.NativeModels
{
    public class CustomerPaymentJournal : BaseModel
    {
        [JsonProperty("id")]
        public string Id { get; set; }
        [JsonProperty("code")]
        public string Code { get; set; }
        [JsonProperty("displayName")]
        public string DisplayName { get; set; }
        [JsonProperty("lastModifiedDateTime", NullValueHandling = NullValueHandling.Ignore)]
        public DateTime? LastModifiedDateTime { get; set; }
        [JsonProperty("balancingAccountId")]
        public string BalancingAccountId { get; set; }
        [JsonProperty("balancingAccountNumber")]
        public string BalancingAccountNumber { get; set; }
    }
}
