namespace F1Solutions.Naati.Common.Dal.Domain
{
    public class ProfessionalDevelopmentCategoryRequirement : EntityBase
    {
        public virtual int Points { get; set; }
        public virtual ProfessionalDevelopmentCategory ProfessionalDevelopmentCategory { get; set; }
        public virtual ProfessionalDevelopmentRequirement ProfessionalDevelopmentRequirement { get; set; }

    }
}
