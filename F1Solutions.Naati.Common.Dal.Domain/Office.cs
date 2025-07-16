namespace F1Solutions.Naati.Common.Dal.Domain
{
    public class Office : LegacyEntityBase
    {
        public Office(int id) : base(id)
        {
        }

        public Office()
        {
        }

        protected override string AuditName => nameof(Office);
        public override IAuditObject RootAuditObject => Institution;

        public virtual Institution Institution { get; set; }
        public virtual State State { get; set; }
        public virtual bool Regional { get; set; }
        public virtual string MYOBCustomerName { get; set; }
        public virtual string CashChequeCode { get; set; }
        public virtual string EFTCode { get; set; }
        public virtual string MergeLetterOutputFolder { get; set; }

        public override string ToString()
        {
            return base.ToString();
        }
    }
}
