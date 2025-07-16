using System;

namespace F1Solutions.Naati.Common.Contracts.Dal
{
    public class TestContract
    {
        public int JobID { get; set; }
        public int TestSittingId { get; set; }
        public int TestResultID { get; set; }
        public string SkillDisplayName { get; set; }
        public string CredentialTypeExternalName { get; set; }
        public DateTime TestDate { get; set; }
        public DateTime? DueDate { get; set; }
        public int MaterialID { get; set; }
        public string Status { get; set; }
        public string Examiner { get; set; }
        public DateTime DateAllocated { get; set; }
        public string Description { get; set; }
        public bool HasAssets { get; set; }
        public string Applicant { get; set; }
        public int NaatiNumber { get; set; }
        public bool Supplementary { get; set; }
        public bool HasDefaultSpecification { get; set; }
        public int TestMarkingTypeId { get; set; }
        public string CredentialTypeInternalName { get; set; }
    }
}