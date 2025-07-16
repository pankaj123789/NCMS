namespace F1Solutions.Naati.Common.Dal.Domain
{
    public class Recertification : EntityBase
    {
        public virtual CredentialApplication CredentialApplication { get; set; }
        public virtual CertificationPeriod CertificationPeriod { get; set; }
    }
}
