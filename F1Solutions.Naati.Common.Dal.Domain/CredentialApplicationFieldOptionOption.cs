namespace F1Solutions.Naati.Common.Dal.Domain
{
    public class CredentialApplicationFieldOptionOption : EntityBase
    {
        public virtual CredentialApplicationField CredentialApplicationField { get; set; }
        public virtual CredentialApplicationFieldOption CredentialApplicationFieldOption { get; set; }
        public override string ToString()
        {
            return Id.ToString();
        }
    }
}
