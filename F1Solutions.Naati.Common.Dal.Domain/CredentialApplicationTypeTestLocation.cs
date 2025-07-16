namespace F1Solutions.Naati.Common.Dal.Domain
{
    public class CredentialApplicationTypeTestLocation : EntityBase
    {
        public virtual CredentialApplicationType CredentialApplicationType { get; set; }
        public virtual TestLocation TestLocation { get; set; }
	}
}
