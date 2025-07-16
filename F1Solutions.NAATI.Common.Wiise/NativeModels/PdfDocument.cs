using Newtonsoft.Json;
using System;

namespace F1Solutions.Naati.Common.Wiise.NativeModels
{
    public class PdfDocument
    {
        [JsonProperty("id")]
        public string Id { get; set; }
        [JsonProperty("pdfDocumentContent@odata.mediaReadLink")]
        public Uri MediaLink { get; set; }
    }
}
