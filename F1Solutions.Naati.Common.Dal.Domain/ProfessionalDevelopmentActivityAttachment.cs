namespace F1Solutions.Naati.Common.Dal.Domain
{
    public class ProfessionalDevelopmentActivityAttachment : EntityBase
    {
        public virtual StoredFile StoredFile { get; set; }
        public virtual string Description { get; set; }
        public virtual ProfessionalDevelopmentActivity ProfessionalDevelopmentActivity { get; set; }
    }
}
