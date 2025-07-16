using System;

namespace Ncms.Contracts.Models.Test
{
    public class TestSearchResponseModel
    {
        public int TestResultID { get; set; }
        public int TestID { get; set; }
        public DateTime TestDate { get; set; }
        public DateTime? DueDate { get; set; }
        public int? JobID { get; set; }
        public int? LastReviewJobID { get; set; }
        public int LanguageId { get; set; }
        public string Language { get; set; }
        public int LevelId { get; set; }
        public string Level { get; set; }
        public int CategoryId { get; set; }
        public string Category { get; set; }
        public string Direction { get; set; }
        public string Status { get; set; }
        public int NAATINumber { get; set; }
        public int EntityId { get; set; }
        public string PersonName { get; set; }
        public string Description { get; set; }
        public string Abbreviation { get; set; }
        public bool HasAssets { get; set; }
        public bool HasExaminers { get; set; }
        public bool HasSat { get; set; }
        public int EventId { get; set; }
        public int EventVenueId { get; set; }
        public string Office { get; set; }
        public string ResultStatus { get; set; }
        public int? TestMaterialId { get; set; }
        public bool? IsEportalActive { get; set; }
        public DateTime? PersonBirthDate { get; set; }
        public bool PersonHasPhoto { get; set; }
        public DateTime? PhotoDate { get; set; }
        public DateTime? EportalRegistrationDate { get; set; }
        public bool Supplementary { get; set; }
    }


    public class TestSearchResultModel
    {
        public int AttendanceId { get; set; }
        public int? TestResultId { get; set; }
        public int CredentialRequestId { get; set; }
        public int TestSessionId { get; set; }
        public int SkillId { get; set; }
        public int TestOfficeId { get; set; }
        public int CredentialTypeId { get; set; }
        public int? JobId { get; set; }

        public int StatusTypeId { get; set; }

        public string Skill { get; set; }
        public int NaatiNumber { get; set; }
        public string PersonName { get; set; }
        public string Office { get; set; }
        public DateTime TestDate { get; set; }
        public string CredentialTypeInternalName { get; set; }

        public string Status { get; set; }
        public bool HasAssets { get; set; }

        public bool HasExaminers { get; set; }

        // Todo: Fix this Properties
        public string AccreditationDescription { get; set; } = string.Empty;

        public bool Supplementary { get; set; }

        public int CredentialRequestStatusTypeId { get; set; }

    }
}
