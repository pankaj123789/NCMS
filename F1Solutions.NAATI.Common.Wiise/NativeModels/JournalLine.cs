using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace F1Solutions.Naati.Common.Wiise.NativeModels
{
    public class JournalLine : BaseModel
    {
        [JsonProperty("@odata.etag")]
        public string OdataEtag { get; set; }
        public string id { get; set; }
        public string journalId { get; set; }
        public string journalDisplayName { get; set; }
        public int lineNumber { get; set; }
        public string accountType { get; set; }
        public string accountId { get; set; }
        public string accountNumber { get; set; }
        [JsonProperty("postingDate", NullValueHandling = NullValueHandling.Ignore)]
        [JsonConverter(typeof(DateFormatConverter), "yyyy-MM-dd")]
        public DateTime? PostingDate { get; set; }
        public string documentNumber { get; set; }
        public string externalDocumentNumber { get; set; }
        public double amount { get; set; }
        public string description { get; set; }
        public string comment { get; set; }
        public string taxCode { get; set; }
        public string balanceAccountType { get; set; }
        public string balancingAccountId { get; set; }
        public string balancingAccountNumber { get; set; }
        [JsonProperty("lastModifiedDateTime", NullValueHandling = NullValueHandling.Ignore)]
        public DateTime? LastModifiedDateTime { get; set; }
        public List<DimensionSetLine> dimensionSetLines { get; set; }

        public JournalLine()
        {
            dimensionSetLines = new List<DimensionSetLine>();
        }
    }
}
