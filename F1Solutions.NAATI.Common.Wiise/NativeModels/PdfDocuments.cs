using Newtonsoft.Json;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace F1Solutions.Naati.Common.Wiise.NativeModels
{
    [DataContract]
    public class PdfDocuments
    {
        [JsonProperty("@odata.context")]
        public string OdataContext { get; set; }
        [JsonProperty("value")]
        public List<PdfDocument> _Documents { get; set; }
    }
}
