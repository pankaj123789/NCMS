namespace F1Solutions.Naati.Common.Dal.Domain
{
    public class PanelMembershipCoordinatorCredentialType : EntityBase
    {
        public virtual PanelMembership PanelMembership { get; set; }
        public virtual CredentialType CredentialType { get; set; }
    }
}
