using System;
using F1Solutions.Naati.Common.Contracts.Dal.Enum;

namespace F1Solutions.Naati.Common.Contracts.Dal.Request
{
    public class PendingBriefRequest
    {
        public DateTime SendDate { get; set; }
        public CredentialRequestStatusTypeName CredentialRequestStatus { get; set; }
        public int? TestSittingId { get; set; }
    }
}