using System;
using System.Collections.Generic;

namespace F1Solutions.Naati.Common.Contracts.Dal.DTO
{
    public class SpecificationSkillDto
    {
        public int Id { get; set; }
        public string Skill { get; set; }
        public IEnumerable<DateTime> TestDates { get; set; }
        public int ApplicantsWithoutMaterials { get; set; }
        public int TotalApplicants { get; set; }
    }
}