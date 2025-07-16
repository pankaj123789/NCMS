using System;

namespace F1Solutions.Naati.Common.Contracts.Dal.Request
{
    public class GetAllCredentialTestsRequest
    {
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
        public int[] PreferredTestLocation { get; set; }
        public int[] TestVenue { get; set; }
        public int[] Credential { get; set; }
        public int[] CredentialSkill { get; set; }
        public bool IsActive { get; set; } = true;
    }
}