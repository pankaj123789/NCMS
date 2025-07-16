namespace Ncms.Contracts
{
    public class CredentialRequestBulkActionRequest
    {
        public int CredentialApplicationTypeId { get; set; }
        public int CredentialTypeId { get; set; }
        public int SkillId { get; set; }
        public int CredentialRequestStatusTypeId { get; set; }
        public int TestLocationId { get; set; }
        public int TestSessionId { get; set; }
        public int Action { get; set; }

        public int[] CredentialRequestIds { get; set; }
    }
}
