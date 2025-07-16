using System;
using System.Collections.Generic;

namespace F1Solutions.Naati.Common.Contracts.Dal.DTO
{
    public class TestSessionApplicantsDto
    {
        public int TestSittingId { get; set; }
        public string ApplicationReference { get; set; }
        public string Name { get; set; }
        public string CustomerNo { get; set; }
        public string Gender { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Skill { get; set; }
        public int SkillId { get; set; }
        public int Status { get; set; }
        public string StatusDisplayName { get; set; }
        public DateTime StatusModifiedDate { get; set; }
        public int ApplicationId { get; set; }
        public int CredentialRequestId { get; set; }
        public bool Rejected { get; set; }
        public int ApplicationStatusId { get; set; }
        public int CredentialTypeId { get; set; }
        public int ApplicationTypeId { get; set; }
        public string AttendanceId { get; set; }
        public IEnumerable<TestTaskDto> TestTasks { get; set; }
        public bool Supplementary { get; set; }
        public bool Sat { get; set; }
        public bool SupplementaryCredentialRequest { get; set; }
        public bool HasDefaultSpecification { get; set; }
        public bool HasTestMaterials { get; set; }
        public string LanguageCharacterType { get; set; }
    }
}