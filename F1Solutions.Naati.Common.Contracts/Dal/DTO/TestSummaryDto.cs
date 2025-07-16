using System;
using System.Collections.Generic;

namespace F1Solutions.Naati.Common.Contracts.Dal.DTO
{
    public class TestSummaryDto
    {
        public int TestAttendanceId { get; set; } 
        public int ApplicationId { get; set; }
        public string ApplicationReference { get; set; }
        public int ApplicationTypeId { get; set; }
        public int CredentialTypeId { get; set; } 
        public int SkillId { get; set; }
        public string ApplicationType { get; set; } 
        public string CredentialTypeInternalName { get; set; }    
        public string State { get; set; } 
        public string CredentialTypeExternalName { get; set; } 
        public string Skill { get; set; }
        public int? ResultId { get; set; }
        public int? CurrentJobId { get; set; }
        public int? LastTestResultStatusTypeId { get; set; }
        public string LastTestResultStatus { get; set; }
        public int? LastReviewTestResultId { get; set; }
        public int Language1Id { get; set; }
        public int Language2Id { get; set; }
        public DateTime TestDate { get; set; }
        public int TestStatusTypeId { get; set; }
        public string TestStatus { get; set; } 
        public int TestLocationId { get; set; }
        public string TestLocation { get; set; }
        public int VenueId { get; set; }
        public string Venue { get; set; } 
        public int PersonId { get; set; }
        public int NaatiNumber { get; set; }
        public bool Supplementary { get; set; }
        public IEnumerable<int> TestMaterialIds { get; set; } 
        public IEnumerable<string> TestMaterialNames { get; set; }
        public int CredentialRequestStatusTypeId { get; set; }
        public int ApplicationStatusTypeId { get; set; }
        public int CredentialRequestId { get; set; }
        public bool AllowSupplementary { get; set; }
        public bool HasDowngradePaths { get; set; }
        public bool SupplementaryCredentialRequest { get; set; }
        public int MarkingSchemaTypeId { get; set; }
        public bool? EligibleForConcededPass { get; set; }
        public bool? EligibleForSupplementary { get; set; }
        public int TestSessionId { get; set; }
        public bool DefaultTestSpecification { get; set; }
        public int TestSpecificationId { get; set; }
        public bool HasFeedback { get; set; }
    }
}