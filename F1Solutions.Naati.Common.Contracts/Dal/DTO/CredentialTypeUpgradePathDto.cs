namespace F1Solutions.Naati.Common.Contracts.Dal.DTO
{
    public class CredentialTypeUpgradePathDto
    {
        public int Id { get; set; }
        public int CredentialTypeFromId { get; set; }
        public int CredentialTypeToId { get; set; }
        public bool MatchDirection { get; set; }
    }
}