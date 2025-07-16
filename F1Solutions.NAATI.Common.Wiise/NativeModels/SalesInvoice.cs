using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace F1Solutions.Naati.Common.Wiise.NativeModels
{
    [DataContract]
    [JsonObject(ItemNullValueHandling = NullValueHandling.Ignore)]
    public class SalesInvoice : BaseModel
    {
        [JsonProperty("@odata.etag")]
        public string OdataEtag { get; set; }
        [JsonProperty("id", NullValueHandling = NullValueHandling.Ignore)]
        public Guid? Id { get; set; }
        [JsonProperty("Number")]
        public string Number { get; set; }
        public Guid? customerId { get; set; }
        public string customerNumber { get; set; }
        public string customerName { get; set; }
        [JsonProperty("customerPurchaseOrderReference")]
        public string CustomerPurchaseOrderReference { get; set; }
        [JsonProperty("email")]
        public string Email { get; set; }
        [JsonProperty("invoiceDate", DefaultValueHandling = DefaultValueHandling.Ignore)]
        [JsonConverter(typeof(DateFormatConverter), "yyyy-MM-dd")]
        public DateTime? InvoiceDate { get; set; }
        [JsonProperty("postingDate", DefaultValueHandling = DefaultValueHandling.Ignore)]
        [JsonConverter(typeof(DateFormatConverter), "yyyy-MM-dd")]
        public DateTime? PostingDate { get; set; }
        [JsonProperty("dueDate", DefaultValueHandling = DefaultValueHandling.Ignore)]
        [JsonConverter(typeof(DateFormatConverter), "yyyy-MM-dd")]
        public DateTime? DueDate { get; set; }
        [JsonProperty("pricesIncludeTax", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public bool? PricesIncludeTax { get; set; }
        public string currencyCode { get; set; }
        public decimal? totalAmountExcludingTax { get; set; }
        [JsonProperty("totalTaxAmount")]
        public decimal? TotalTaxAmount { get; set; }
        [JsonProperty("totalAmountIncludingTax")]
        public decimal? TotalAmountIncludingTax { get; set; }
        [JsonProperty("status")]
        public string Status { get; set; }
        [JsonProperty("remainingAmount")]
        public decimal? RemainingAmount { get; set; }
        [JsonProperty("billingPostalAddress")]
        public BillingPostalAddress BillingPostalAddress { get; set; }
        [JsonProperty("sellingPostalAddress")]
        public BillingPostalAddress SellingPostalAddress { get; set; }
        [JsonProperty("shippingPostalAddress")]
        public BillingPostalAddress ShippingPostalAddress { get; set; }
        [JsonProperty("lastModifiedDateTime", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public DateTime? LastModifiedDateTime { get; set; }
        [JsonProperty("externalDocumentNumber")]
        public string ExternalDocumentNumber { get; set; }
        [JsonProperty("salesInvoiceLines")]
        public List<LineItem> SalesInvoiceLines { get; set; }
        public SalesInvoice() { }
        public SalesInvoice(BaseModel baseModel)
        {
            HasValidationErrors = baseModel.HasValidationErrors;
            ValidationErrors = baseModel.ValidationErrors;
        }

        [JsonConverter(typeof(StringEnumConverter))]
        public enum StatusEnum
        {
            Draft,
            Open,
            Paid,
            Canceled
        }
    }

    public class BillingPostalAddress
    {
        public string street { get; set; }
        public string city { get; set; }
        public string state { get; set; }
        public string countryLetterCode { get; set; }
        public string postalCode { get; set; }
    }



    public class DateFormatConverter : IsoDateTimeConverter
    {
        public DateFormatConverter(string format)
        {
            DateTimeFormat = format;
        }
    }


}
