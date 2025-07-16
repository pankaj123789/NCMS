namespace F1Solutions.Naati.Common.Dal.Domain
{
    public class ProfessionalDevelopmentSectionCategory:EntityBase
    {
        public  virtual  ProfessionalDevelopmentSection ProfessionalDevelopmentSection { get; set; }
        public  virtual ProfessionalDevelopmentCategory ProfessionalDevelopmentCategory { get; set; }
        public virtual int? PointsLimit { get; set; }
        public virtual PdPointsLimitType PdPointsLimitType { get; set; }
    }
}
