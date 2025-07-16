namespace F1Solutions.Naati.Common.Dal.Domain
{
    public class TestSittingNote : EntityBase
    {
        public virtual TestSitting TestSitting { get; set; }
        public virtual Note Note { get; set; }
        public override IAuditObject RootAuditObject => TestSitting;

    }
}
