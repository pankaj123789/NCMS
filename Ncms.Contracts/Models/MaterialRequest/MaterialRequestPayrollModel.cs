using System;

namespace Ncms.Contracts.Models.MaterialRequest
{
    public class MaterialRequestPayrollModel 
    {
        public int Id { get; set; }
        public int RoundPanelMemberId { get; set; }
        public int? ApprovedByUserId { get; set; }
        public DateTime? ApprovedDate { get; set; }
        public int? PaidByUserId { get; set; }
        public DateTime? PaymentDate { get; set; }
        public string PaymentReference { get; set; }
        public decimal? Amount { get; set; }
    }
}
