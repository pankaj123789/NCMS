using Newtonsoft.Json;
using System;

namespace F1Solutions.Naati.Common.Wiise.NativeModels
{
    public class Journal
    {
        [JsonProperty("@odata.etag")]
        public string OdataEtag { get; set; }
        [JsonProperty("id")]
        public string Id { get; set; }
        [JsonProperty("code")]
        public string Code { get; set; }
        [JsonProperty("displayName")]
        public string DisplayName { get; set; }
        [JsonProperty("lastModifiedDateTime", NullValueHandling = NullValueHandling.Ignore)]
        public DateTime? LastModifiedDateTime { get; set; }
        [JsonProperty("balancingAccountId")]
        public Guid? BalancingAccountId { get; set; }
        [JsonProperty("balancingAccountNumber")]
        public string BalancingAccountNumber { get; set; }
    }
}
