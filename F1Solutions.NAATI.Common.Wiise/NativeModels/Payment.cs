using Newtonsoft.Json;
using System;

namespace F1Solutions.Naati.Common.Wiise.NativeModels
{
    public class Payment : BaseModel
    {
        [JsonProperty("customerNumber")]
        public string CustomerNumber { get; set; }
        [JsonProperty("postingDate", DefaultValueHandling = DefaultValueHandling.Ignore)]
        [JsonConverter(typeof(DateFormatConverter), "yyyy-MM-dd")]
        public DateTime? PostingDate { get; set; }
        [JsonProperty("amount")]
        public double Amount { get; set; }
        [JsonProperty("number")]
        public string Number { get; set; }
        [JsonProperty("appliesToInvoiceNumber")]
        public string AppliesToInvoiceNumber { get; set; }
        [JsonProperty("appliesToCreditNoteNumber")]
        public string AppliesToCreditNoteNumber { get; set; }
        [JsonProperty("description")]
        public string Description { get; set; }
        [JsonProperty("comment")]
        public string Comment { get; set; }
        [JsonProperty("invoiceId", NullValueHandling = NullValueHandling.Ignore)]
        public Guid InvoiceId { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public Guid CreditNoteId { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Reference { get; set; }
        [JsonProperty("lastModifiedDateTime", NullValueHandling = NullValueHandling.Ignore)]
        public string LastModifiedDateTime { get; set; }

        public Payment(BaseModel baseModel)
        {
            HasValidationErrors = baseModel.HasValidationErrors;
            ValidationErrors = baseModel.ValidationErrors; 
        }

        public Payment() { }
    }
}
