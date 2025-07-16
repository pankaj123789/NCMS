using System;
using System.Collections.Generic;

namespace F1Solutions.Naati.Common.Contracts.Dal.DTO
{
    public class WorkPracticeCredentialDto : CredentialsDetailsDto
    {
        public bool Calculated { get; set; }
        public decimal Points { get; set; }
        public DateTime RecertificationActivitesStartDate { get; set; }
        public double Requirement { get; set; }
        public IEnumerable<int> IncludedWorkPracticeIds { get; set; }
    }
}