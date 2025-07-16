namespace Ncms.Contracts.Models.Application
{
    public class CredentialTypeUpgradePathModel
    {
        public int Id { get; set; }
        public int CredentialTypeFromId { get; set; }
        public int CredentialTypeToId { get; set; }
        public bool MatchDirection { get; set; }
    }
}
