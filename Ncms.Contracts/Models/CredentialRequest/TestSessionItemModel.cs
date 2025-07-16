using System;
using System.Collections.Generic;
using F1Solutions.Naati.Common.Contracts.Dal.DTO;

namespace Ncms.Contracts.Models.CredentialRequest
{
    public class TestSessionItemModel
    {
        public int Id { get; set; }
        public string Session { get; set; }
        public string Venue { get; set; }
        public DateTime TestDate { get; set; }
        public int Capacity { get; set; }
        public int? PreparationTime { get; set; }
        public int? SessionDuration { get; set; }
        public int TotalApplicants { get; set; }
        public string Notes { get; set; }
        public bool AllowSelfAssign { get; set; }
        public List<SkillDto> Skills { get; set; }
    }
}
