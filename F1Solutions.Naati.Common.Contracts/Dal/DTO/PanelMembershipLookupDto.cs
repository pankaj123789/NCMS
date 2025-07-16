namespace F1Solutions.Naati.Common.Contracts.Dal.DTO
{
    public class PanelMembershipLookupDto : LookupTypeDto
    {
        public int NaatiNumber { get; set; }
        public string Email { get; set; }
        public bool? IsCoordinatorCredentialType { get; set; }
    }
}
