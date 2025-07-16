using System;

namespace F1Solutions.Naati.Common.Contracts.Dal.DTO
{
    public class CredentialRequestApplicantDto
    {
        public int CredentialRequestId { get; set; }
        public int ApplicationId { get; set; }
        public int CustomerNo { get; set; }
        public int PersonId { get; set; }
        public string Name { get; set; }
        public string ApplicationReference { get; set; }
        public string Status { get; set; }
        public int StatusId { get; set; }
        public bool? AutoCreated { get; set; }
        public int SkillId { get; set; }
        public DateTime ApplicationSubmittedDate { get; set; }
        public string Language1 { get; set; }
        public string Language2 { get; set; }
        public string DirectionDisplayName { get; set; }
    }
}