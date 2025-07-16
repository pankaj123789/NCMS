using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace F1Solutions.Naati.Common.Wiise.NativeModels
{
    [DataContract]
    [JsonObject(ItemNullValueHandling = NullValueHandling.Ignore)]
    public class PurchaseInvoice : BaseModel
    {
        [JsonProperty("@odata.etag")]
        public string OdataEtag { get; set; }
        public Guid? id { get; set; }
        public string number { get; set; }
        [JsonProperty("invoiceDate", DefaultValueHandling = DefaultValueHandling.Ignore)]
        [JsonConverter(typeof(DateFormatConverter), "yyyy-MM-dd")]
        public DateTime? InvoiceDate { get; set; }
        [JsonProperty("postingDate", DefaultValueHandling = DefaultValueHandling.Ignore)]
        [JsonConverter(typeof(DateFormatConverter), "yyyy-MM-dd")]
        public DateTime? PostingDate { get; set; }
        [JsonProperty("dueDate", DefaultValueHandling = DefaultValueHandling.Ignore)]
        [JsonConverter(typeof(DateFormatConverter), "yyyy-MM-dd")]
        public DateTime? DueDate { get; set; }
        public string vendorInvoiceNumber { get; set; }
        public Guid vendorId { get; set; }
        public string vendorNumber { get; set; }
        public string vendorName { get; set; }
        public string payToName { get; set; }
        public string payToContact { get; set; }
        public string payToVendorId { get; set; }
        public string payToVendorNumber { get; set; }
        public string shipToName { get; set; }
        public string shipToContact { get; set; }
        public string buyFromAddressLine1 { get; set; }
        public string buyFromAddressLine2 { get; set; }
        public string buyFromCity { get; set; }
        public string buyFromCountry { get; set; }
        public string buyFromState { get; set; }
        public string buyFromPostCode { get; set; }
        public string shipToAddressLine1 { get; set; }
        public string shipToAddressLine2 { get; set; }
        public string shipToCity { get; set; }
        public string shipToCountry { get; set; }
        public string shipToState { get; set; }
        public string shipToPostCode { get; set; }
        public string payToAddressLine1 { get; set; }
        public string payToAddressLine2 { get; set; }
        public string payToCity { get; set; }
        public string payToCountry { get; set; }
        public string payToState { get; set; }
        public string payToPostCode { get; set; }
        public string currencyId { get; set; }
        public string currencyCode { get; set; }
        public bool pricesIncludeTax { get; set; }
        public double? discountAmount { get; set; }
        //public bool discountAppliedBeforeTax { get; set; }
        public double? totalAmountExcludingTax { get; set; }
        public double? totalTaxAmount { get; set; }
        //public double? totalAmountIncludingTax { get; set; }
        public string status { get; set; }
        [JsonProperty("lastModifiedDateTime", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public DateTime? LastModifiedDateTime { get; set; }
        public List<LineItem> purchaseInvoiceLines { get; set; }

        public PurchaseInvoice() { }
        public PurchaseInvoice(BaseModel baseModel)
        {
            HasValidationErrors = baseModel.HasValidationErrors;
            ValidationErrors = baseModel.ValidationErrors;
        }
    }
}



