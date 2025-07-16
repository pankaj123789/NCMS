using System;

namespace F1Solutions.Naati.Common.Contracts.Dal.DTO
{
    public class TestSittingDto
    {
        public int TestSittingId { get; set; }
        public bool Rejected { get; set; }
        public int CredentialRequestId { get; set; }
        public int CredentialRequestStatusTypeId { get; set; }
    }

    public class TestSittingHistoryItemDto
    {
        public int TestSittingId { get; set; }
        public bool Rejected { get; set; }
        public int CredentialRequestId { get; set; }
        public int CredentialRequestStatusTypeId { get; set; }
        public int TestSessionId { get; set; }
        public DateTime TestDateTime { get; set; }
        public bool Sat { get; set; }
        public DateTime AllocatedDate { get; set; }
        public DateTime? RejectedDate { get; set; }
    }
}