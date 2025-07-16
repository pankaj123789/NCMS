using System;
using System.Collections.Generic;

namespace Ncms.Contracts.Models.Test
{
    public class TestRubricModel
    {
        public int AttendanceId { get; set; }
        public int Id { get; set; }
        public string ExaminerName { get; set; }
        public DateTime? DateSubmitted { get; set; }
        public DateTime? ReceivedDate { get; set; }
        public string CredentialType { get; set; }
        public string Skill { get; set; }
        public List<TestComponentModel> TestComponents { get; set; }
        public int NaatiNumber { get; set; }
        public string ApplicationReference { get; set; }
        public int ApplicationId { get; set; }
        public string ApplicationType { get; set; }
        public DateTime TestDate { get; set; }
        public string TestStatus { get; set; }
        public int TestStatusTypeId { get; set; }
        public int? OriginalTestResultStatusTypeId { get; set; }
        public int? TestResultStatusTypeId { get; set; }
        public string TestResultStatus { get; set; }
        public string TestLocation { get; set; }
        public string Venue { get; set; }
        public IEnumerable<int> TestMaterialIds { get; set; }
        public int MinCommentsLength { get; set; }
        public bool Supplementary { get; set; }
        public bool ComputedEligibleForPass { get; set; }
        public bool ComputedEligibleForConcededPass { get; set; }
        public bool ComputedEligibleForSupplementary { get; set; }
        public bool ResultAutoCalculation { get; set; }
        public string Feedback { get; set; }
    }
}