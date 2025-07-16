namespace F1Solutions.Naati.Common.Dal.Domain
{
    public class ProfessionalDevelopmentCredentialApplication : EntityBase
    {
        public virtual ProfessionalDevelopmentActivity ProfessionalDevelopmentActivity { get; set; }
        public virtual CredentialApplication CredentialApplication { get; set; }
    }
}