using Newtonsoft.Json;
using System;

namespace F1Solutions.Naati.Common.Wiise.NativeModels
{
    public class Account
    {
        [JsonProperty("@odata.etag")]
        public string OdataEtag { get; set; }
        [JsonProperty("id")]
        public string Id { get; set; }
        [JsonProperty("number")]
        public string Number { get; set; }
        [JsonProperty("displayName")]
        public string DisplayName { get; set; }
        [JsonProperty("category")]
        public string Category { get; set; }
        [JsonProperty("subCategory")]
        public string SubCategory { get; set; }
        [JsonProperty("blocked")]
        public bool Blocked { get; set; }
        [JsonProperty("accountType")]
        public string AccountType { get; set; }
        [JsonProperty("directPosting")]
        public bool DirectPosting { get; set; }
        [JsonProperty("lastModifiedDateTime")]
        public DateTime LastModifiedDateTime { get; set; }
    }
}
