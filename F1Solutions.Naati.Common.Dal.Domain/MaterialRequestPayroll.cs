using System;

namespace F1Solutions.Naati.Common.Dal.Domain
{
    public class MaterialRequestPayroll : EntityBase
    {
        public virtual  MaterialRequestPanelMembership MaterialRequestPanelMembership { get; set; }
        public virtual  User ApprovedByUser { get; set; }
        public virtual  DateTime? ApprovedDate { get; set; }
        public virtual User PaidByUser { get; set; }
        public virtual  DateTime? PaymentDate { get; set; }
        public virtual  string PaymentReference { get; set; }
        public virtual  decimal? Amount { get; set; }
    }
}
