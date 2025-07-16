using System;

namespace F1Solutions.Naati.Common.Contracts.Dal.DTO
{
    public class TestSessionApplicantDto
    {
        public int CredentialRequestId { get; set; }
        public int ApplicationId { get; set; }
        public int CustomerNo { get; set; }
        public int PersonId { get; set; }
        public string Name { get; set; }
        public string ApplicationReference { get; set; }
        public string Status { get; set; }
        public int StatusId { get; set; }
        public DateTime ApplicationSubmittedDate { get; set; }
        public bool Rejected { get; set; }
        public bool Sat { get; set; }
        public DateTime StatusModifiedDate { get; set; }
        public string Language1 { get; set; }
        public string Language2 { get; set; }
        public string DirectionDisplayName { get; set; }
    }
}