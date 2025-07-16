using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace F1Solutions.Naati.Common.Wiise.NativeModels
{
    public class CreditNote : BaseModel
    {
        [JsonProperty("@odata.etag")]
        public string OdataEtag { get; set; }
        [JsonProperty("id", NullValueHandling = NullValueHandling.Ignore)]
        public Guid? Id { get; set; }
        [JsonProperty("number")]
        public string Number { get; set; }
        [JsonProperty("externalDocumentNumber", NullValueHandling = NullValueHandling.Ignore)]
        public string ExternalDocumentNumber { get; set; }
        [JsonProperty("creditMemoDate", DefaultValueHandling = DefaultValueHandling.Ignore)]
        [JsonConverter(typeof(DateFormatConverter), "yyyy-MM-dd")]
        public DateTime CreditMemoDate { get; set; }
        [JsonProperty("postingDate", DefaultValueHandling = DefaultValueHandling.Ignore)]
        [JsonConverter(typeof(DateFormatConverter), "yyyy-MM-dd")]
        public DateTime PostingDate { get; set; }
        [JsonProperty("dueDate", DefaultValueHandling = DefaultValueHandling.Ignore)]
        [JsonConverter(typeof(DateFormatConverter), "yyyy-MM-dd")]
        public DateTime DueDate { get; set; }
        [JsonProperty("customerId", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public Guid? CustomerId { get; set; }
        [JsonProperty("customerNumber")]
        public string CustomerNumber { get; set; }
        [JsonProperty("customerName")]
        public string CustomerName { get; set; }
        [JsonProperty("billToName")]
        public string BillToName { get; set; }
        public string billToCustomerId { get; set; }
        public string billToCustomerNumber { get; set; }
        public string sellToAddressLine1 { get; set; }
        public string sellToAddressLine2 { get; set; }
        public string sellToCity { get; set; }
        public string sellToCountry { get; set; }
        public string sellToState { get; set; }
        public string sellToPostCode { get; set; }
        public string billToAddressLine1 { get; set; }
        public string billToAddressLine2 { get; set; }
        public string billToCity { get; set; }
        public string billToCountry { get; set; }
        public string billToState { get; set; }
        public string billToPostCode { get; set; }
        public string currencyId { get; set; }
        public string currencyCode { get; set; }
        public string paymentTermsId { get; set; }
        public string shipmentMethodId { get; set; }
        public string salesperson { get; set; }
        [JsonProperty("pricesIncludeTax", NullValueHandling = NullValueHandling.Ignore)]
        public bool? PricesIncludeTax { get; set; }
        public int discountAmount { get; set; }
        [JsonProperty("discountAppliedBeforeTax", NullValueHandling = NullValueHandling.Ignore)]
        public bool? DiscountAppliedBeforeTax { get; set; }
        [JsonProperty("totalAmountExcludingTax", NullValueHandling = NullValueHandling.Ignore)]
        public double? TotalAmountExcludingTax { get; set; }
        [JsonProperty("totalTaxAmount", NullValueHandling = NullValueHandling.Ignore)]
        public double? TotalTaxAmount { get; set; }
        [JsonProperty("totalAmountIncludingTax", NullValueHandling = NullValueHandling.Ignore)]
        public decimal? TotalAmountIncludingTax { get; set; }
        [JsonProperty("status", NullValueHandling = NullValueHandling.Ignore)]
        public string Status { get; set; }
        [JsonProperty("lastModifiedDateTime", NullValueHandling = NullValueHandling.Ignore)]
        public DateTime? LastModifiedDateTime { get; set; }
        [JsonProperty("invoiceId", NullValueHandling = NullValueHandling.Ignore)]
        public Guid? InvoiceId { get; set; }
        [JsonProperty("invoiceNumber", NullValueHandling = NullValueHandling.Ignore)]
        public string InvoiceNumber { get; set; }
        public string phoneNumber { get; set; }
        public string email { get; set; }

        [JsonProperty("salesCreditMemoLines")]
        public IEnumerable<SalesCreditMemoLine> SalesCreditMemoLines { get; set; }

    }

    public class SalesCreditMemoLine
    {
        [JsonProperty("accountId")]
        public Guid AccountId { get; set; }
        [JsonProperty("itemId", NullValueHandling = NullValueHandling.Ignore)]
        public Guid? ItemId { get; set; }
        [JsonProperty("description")]
        public string Description { get; set; }
        [JsonProperty("quantity")]
        public decimal Quantity { get; set; }
        [JsonProperty("unitPrice")]
        public decimal UnitPrice { get; set; }
        [JsonProperty("dimensionSetLines")]
        public IEnumerable<DimensionSetLine> DimensionSetLines { get; set; }
    }
}
