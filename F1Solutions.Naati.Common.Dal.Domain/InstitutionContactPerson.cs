namespace F1Solutions.Naati.Common.Dal.Domain
{
    public class InstitutionContactPerson : EntityBase
    {
        public virtual Institution Institution { get; set; }
        public virtual Person Person { get; set; }
    }
}
