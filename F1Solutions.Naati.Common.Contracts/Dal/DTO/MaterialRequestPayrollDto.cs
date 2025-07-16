using System;

namespace F1Solutions.Naati.Common.Contracts.Dal.DTO
{
    public class MaterialRequestPayrollDto
    {
        public int Id { get; set; }
        public int RoundPanelMemberId { get; set; }
        public  int? ApprovedByUserId { get; set; }
        public  DateTime? ApprovedDate { get; set; }
        public  int? PaidByUserId { get; set; }
        public  DateTime? PaymentDate { get; set; }
        public  string PaymentReference { get; set; }
        public  decimal? Amount { get; set; }
    }
}
