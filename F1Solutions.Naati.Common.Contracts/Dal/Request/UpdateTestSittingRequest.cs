using System;

namespace F1Solutions.Naati.Common.Contracts.Dal.Request
{
    public class UpdateTestSittingRequest
    {
        public int CredentialRequestId { get; set; }
        public int TestSessionId { get; set; }
        public int CredentialRequestStatusTypeId { get; set; }
        public bool IsRejected { get; set; }
        public DateTime AllocatedDate { get; set; }
        public bool Supplementary { get; set; }
        public int TestSpecificationId { get; set; }
    }
}
