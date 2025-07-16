using System;

namespace F1Solutions.Naati.Common.Wiise.PublicModels
{
    public class Payment : BaseModel
    {
        public DateTime? Date { get; set; }
        public double Amount { get; set; }
        public string InvoiceNumber { get; set; }
        public string CreditNoteNumber { get; set; }
        public string Reference { get; set; }
        public string Account { get; set; }
        public Guid InvoiceId { get; set; }
        public StatusEnum StatusEnum { get; set; }
        public Guid CreditNoteId { get; set; }
        public int? OperationId { get; set; }

        public Payment() { }
        public Payment(BaseModel baseModel)
        {
            HasValidationErrors = baseModel.HasValidationErrors;
            ValidationErrors = baseModel.ValidationErrors;
        }
    }
}
