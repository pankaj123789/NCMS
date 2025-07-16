using System;

namespace F1Solutions.Naati.Common.Contracts.Dal.Request
{
    public class CredentialPointsRequest
    {
        public int CredentialId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
    }
}