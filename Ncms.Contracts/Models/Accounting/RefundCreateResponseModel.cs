using System;

namespace Ncms.Contracts.Models.Accounting
{
    public class RefundCreateResponseModel
    {
        public Guid CreditNoteId { get; set; }
        public string CreditNoteNumber { get; set; }
        public string Reference { get; set; }
        public int? OperationId { get; set; }
        public bool Error { get; set; }
        public string ErrorMessage { get; set; }
        public bool RateLimitExceeded { get; set; }
    }
}
