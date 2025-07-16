using Newtonsoft.Json;
using System;

namespace F1Solutions.Naati.Common.Wiise.NativeModels
{
    internal class Vendor:BaseModel
    {
        [JsonProperty("@odata.etag")]
        public string OdataEtag { get; set; }
        public Guid id { get; set; }
        public string number { get; set; }
        public string displayName { get; set; }
        public string addressLine1 { get; set; }
        public string addressLine2 { get; set; }
        public string city { get; set; }
        public string state { get; set; }
        public string country { get; set; }
        public string postalCode { get; set; }
        public string phoneNumber { get; set; }
        public string email { get; set; }
        public string website { get; set; }
        public string taxRegistrationNumber { get; set; }
        public string currencyId { get; set; }
        public string currencyCode { get; set; }
        public string irs1099Code { get; set; }
        public string paymentTermsId { get; set; }
        public string paymentMethodId { get; set; }
        public bool taxLiable { get; set; }
        public string blocked { get; set; }
        public double? balance { get; set; }
        [JsonProperty("lastModifiedDateTime", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public DateTime? LastModifiedDateTime { get; set; }
    }
}
