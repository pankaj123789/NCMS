using System;
using System.Collections.Generic;

namespace F1Solutions.Naati.Common.Contracts.Dal.DTO
{
    public class TestSessionSummaryDto
    {
        public int TestSessionId { get; set; }
        public string TestSessionName { get; set; }
        public DateTime TestSessionDate { get; set; }
        public int CredentialTypeId { get; set; }
        public string CredentialTypeName { get; set; }
        public int SkillId { get; set; }
        public string SkillName { get; set; }
        public string GivenName { get; set; }
        public string Surname { get; set; }
        public string OtherNames { get; set; }
        public string Email { get; set; }
        public DateTime? BirthDate { get; set; }
        public int NaatiNumber { get; set; }
    }
}
