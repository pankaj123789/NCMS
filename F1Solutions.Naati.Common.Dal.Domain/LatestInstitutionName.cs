namespace F1Solutions.Naati.Common.Dal.Domain
{
    public class LatestInstitutionName : EntityBase
    {
        public virtual Institution Institution { get; set; }
        public virtual InstitutionName InstitutionName { get; set; }
    }
}
