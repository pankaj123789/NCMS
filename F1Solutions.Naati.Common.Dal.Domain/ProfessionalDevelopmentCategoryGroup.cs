namespace F1Solutions.Naati.Common.Dal.Domain
{
    public class ProfessionalDevelopmentCategoryGroup : EntityBase
    {
        public virtual string Name { get; set; }
        public virtual string Description { get; set; }
        public virtual double RequiredPointsPerYear { get; set; }

    }
}
