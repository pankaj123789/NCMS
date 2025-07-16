using System;

namespace F1Solutions.Naati.Common.Contracts.Dal.DTO
{
    public class CredentialQrCodeDto
    {
        public Guid QrCodeGuid { get; set; }
        public DateTime IssueDate { get; set; }
        public CredentialDto CredentialDto { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public string Title { get; set; }
        public string PractitionerNumber { get; set; }
        public SkillDto SkillDto { get; set; }
        public string CredentialTypeName { get; set; }
        public DateTime ModifiedDate { get; set; }
    }
}
