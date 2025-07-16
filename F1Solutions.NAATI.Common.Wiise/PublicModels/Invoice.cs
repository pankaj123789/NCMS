using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace F1Solutions.Naati.Common.Wiise.PublicModels
{
    public class Invoice : BaseModel
    {
        public Invoice()
        {
            Contact = new Contact();
            LineItems = new List<LineItem>();
        }

        public Invoice(BaseModel baseModel)
        {
            base.HasValidationErrors = baseModel.HasValidationErrors;
            base.ValidationErrors = baseModel.ValidationErrors; 
        }

        [DataMember(Name = "CISDeduction", EmitDefaultValue = false)]
        public decimal? CISDeduction { get; }
        [DataMember(Name = "TotalTax", EmitDefaultValue = false)]
        public decimal? TotalTax { get; set; }
        [DataMember(Name = "Total", EmitDefaultValue = false)]
        public decimal? Total { get; set; }
        [DataMember(Name = "TotalDiscount", EmitDefaultValue = false)]
        public decimal? TotalDiscount { get; }
        [DataMember(Name = "InvoiceID", EmitDefaultValue = false)]
        public Guid? InvoiceID { get; set; }
        [DataMember(Name = "InvoiceType", EmitDefaultValue = false)]
        public InvoiceType InvoiceType { get; set; }
        [DataMember(Name = "HasAttachments", EmitDefaultValue = false)]
        public bool? HasAttachments { get; }
        [DataMember(Name = "IsDiscounted", EmitDefaultValue = false)]
        public bool? IsDiscounted { get; }
        [DataMember(Name = "AmountDue", EmitDefaultValue = false)]
        public decimal? AmountDue { get; set; }
        [DataMember(Name = "AmountPaid", EmitDefaultValue = false)]
        public decimal? AmountPaid { get; }
        [DataMember(Name = "FullyPaidOnDate", EmitDefaultValue = false)]
        public DateTime? FullyPaidOnDate { get; }
        [DataMember(Name = "AmountCredited", EmitDefaultValue = false)]
        public decimal? AmountCredited { get; }
        [DataMember(Name = "UpdatedDateUTC", EmitDefaultValue = false)]
        public DateTime? UpdatedDateUTC { get; }
        [DataMember(Name = "HasErrors", EmitDefaultValue = false)]
        public bool? HasErrors { get; set; }
        [DataMember(Name = "SubTotal", EmitDefaultValue = false)]
        public decimal? SubTotal { get; }
        [DataMember(Name = "Warnings", EmitDefaultValue = false)]
        public List<ValidationError> Warnings { get; set; }
        [DataMember(Name = "PlannedPaymentDate", EmitDefaultValue = false)]
        public DateTime? PlannedPaymentDate { get; set; }
        [DataMember(Name = "ExpectedPaymentDate", EmitDefaultValue = false)]
        public DateTime? ExpectedPaymentDate { get; set; }
        [DataMember(Name = "Type", EmitDefaultValue = false)]
        public TypeEnum Type { get; set; }
        [DataMember(Name = "LineAmountTypes", EmitDefaultValue = false)]
        public LineAmountTypes LineAmountTypes { get; set; }
        [DataMember(Name = "Status", EmitDefaultValue = false)]
        public StatusEnum? Status { get; set; }
        [DataMember(Name = "StatusAttributeString", EmitDefaultValue = false)]
        public string StatusAttributeString { get; set; }
        [DataMember(Name = "LineItems", EmitDefaultValue = false)]
        public List<LineItem> LineItems { get; set; }
        [DataMember(Name = "Payments", EmitDefaultValue = false)]
        public List<Payment> Payments { get; set; }
        [DataMember(Name = "Date", EmitDefaultValue = false)]
        public DateTime? Date { get; set; }
        [DataMember(Name = "Contact", EmitDefaultValue = false)]
        public Contact Contact { get; set; }
        [DataMember(Name = "InvoiceNumber", EmitDefaultValue = false)]
        public string InvoiceNumber { get; set; }
        [DataMember(Name = "Reference", EmitDefaultValue = false)]
        public string Reference { get; set; }
        [DataMember(Name = "BrandingThemeID", EmitDefaultValue = false)]
        public Guid? BrandingThemeID { get; set; }
        [DataMember(Name = "Url", EmitDefaultValue = false)]
        public string Url { get; set; }
        [DataMember(Name = "CurrencyRate", EmitDefaultValue = false)]
        public decimal? CurrencyRate { get; set; }
        [DataMember(Name = "SentToContact", EmitDefaultValue = false)]
        public bool? SentToContact { get; set; }
        [DataMember(Name = "DueDate", EmitDefaultValue = false)]
        public DateTime? DueDate { get; set; }
        [DataMember(Name = "ValidationErrors", EmitDefaultValue = false)]
        public Guid? PaymentMethodId { get; set; }
        public int? OperationId { get; set; }


        [JsonConverter(typeof(StringEnumConverter))]
        public enum TypeEnum
        {
            ACCPAY = 1,
            ACCPAYCREDIT = 2,
            APOVERPAYMENT = 3,
            APPREPAYMENT = 4,
            ACCREC = 5,
            ACCRECCREDIT = 6,
            AROVERPAYMENT = 7,
            ARPREPAYMENT = 8
        }
        [JsonConverter(typeof(StringEnumConverter))]
        public enum StatusEnum
        {
            DRAFT,
            OPEN,
            CANCELED,
            PAID
        }
    }
}
