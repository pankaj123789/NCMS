using System.Collections.Generic;

namespace F1Solutions.Naati.Common.Contracts.Dal.Request
{
    public class GetPanelMemberLookupRequest
    {
        public IEnumerable<int> PanelIds { get; set; }

        public bool ActiveMembersOnly { get; set; }

        public int? CredentialTypeId { get; set; }
    }
}