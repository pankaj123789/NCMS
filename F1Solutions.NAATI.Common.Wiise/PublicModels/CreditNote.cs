using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using static F1Solutions.Naati.Common.Wiise.PublicModels.Invoice;

namespace F1Solutions.Naati.Common.Wiise.PublicModels
{
    public class CreditNote:BaseModel
    {
        public TypeEnum CreditNoteType { get; set; }
        [DataMember(Name = "UpdatedDateUTC", EmitDefaultValue = false)]
        public DateTime? UpdatedDateUTC { get; }
        [DataMember(Name = "StatusAttributeString", EmitDefaultValue = false)]
        public string StatusAttributeString { get; set; }
        [DataMember(Name = "BrandingThemeID", EmitDefaultValue = false)]
        public Guid? BrandingThemeID { get; set; }
        [DataMember(Name = "Payments", EmitDefaultValue = false)]
        public List<Payment> Payments { get; set; }
        [DataMember(Name = "AppliedAmount", EmitDefaultValue = false)]
        public decimal? AppliedAmount { get; set; }
        [DataMember(Name = "RemainingCredit", EmitDefaultValue = false)]
        public decimal? RemainingCredit { get; set; }
        [DataMember(Name = "CurrencyRate", EmitDefaultValue = false)]
        public decimal? CurrencyRate { get; set; }
        [DataMember(Name = "SentToContact", EmitDefaultValue = false)]
        public bool? SentToContact { get; }
        [DataMember(Name = "Reference", EmitDefaultValue = false)]
        public string Reference { get; set; }
        [DataMember(Name = "CreditNoteNumber", EmitDefaultValue = false)]
        public string CreditNoteNumber { get; set; }
        [DataMember(Name = "CreditNoteID", EmitDefaultValue = false)]
        public Guid? CreditNoteID { get; set; }
        [DataMember(Name = "FullyPaidOnDate", EmitDefaultValue = false)]
        public DateTime? FullyPaidOnDate { get; set; }
        [DataMember(Name = "Total", EmitDefaultValue = false)]
        public decimal? Total { get; set; }
        [DataMember(Name = "TotalTax", EmitDefaultValue = false)]
        public decimal? TotalTax { get; set; }
        [DataMember(Name = "SubTotal", EmitDefaultValue = false)]
        public decimal? SubTotal { get; set; }
        [DataMember(Name = "LineItems", EmitDefaultValue = false)]
        public List<LineItem> LineItems { get; set; }
        [DataMember(Name = "Date", EmitDefaultValue = false)]
        public DateTime? Date { get; set; }
        [DataMember(Name = "Contact", EmitDefaultValue = false)]
        public Contact Contact { get; set; }
        [DataMember(Name = "LineAmountTypes", EmitDefaultValue = false)]
        public LineAmountTypes LineAmountTypes { get; set; }
        [DataMember(Name = "Status", EmitDefaultValue = false)]
        public StatusEnum Status { get; set; }
        [DataMember(Name = "Type", EmitDefaultValue = false)]
        public TypeEnum Type { get; set; }
        [DataMember(Name = "HasAttachments", EmitDefaultValue = false)]
        public bool? HasAttachments { get; set; }
        [DataMember(Name = "HasErrors", EmitDefaultValue = false)]
        public bool? HasErrors { get; set; }
        public DateTime DueDate { get; set; }
        //purpose of this is to post immediately the Credit Note instead of creating a draft
        //the UI will be able to process the payment correctly.
        public bool PostCreditNoteOnCreate { get; set; }
        public int? OperationId { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        public enum TypeEnum
        {
            ACCPAYCREDIT = 1,
            ACCRECCREDIT = 2
        }
        [JsonConverter(typeof(StringEnumConverter))]
        public enum StatusEnum
        {
            DRAFT,
            OPEN,
            PAID,
            CANCELED
        }



    }
}
