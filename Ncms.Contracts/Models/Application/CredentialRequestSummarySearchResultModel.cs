using System;
using System.Collections.Generic;
using Ncms.Contracts.Models.CredentialRequest;

namespace Ncms.Contracts.Models.Application
{
    public class CredentialRequestSummarySearchResultModel
    {
        public string CompositeId { get; set; }
        public double DaysSinceSubmissionInt { get; set; }
        public int CredentialApplicationTypeId { get; set; }
        public int CredentialTypeId { get; set; }
        public int SkillId { get; set; }
        public int CredentialRequestStatusTypeId { get; set; }
        public string DaysSinceSubmission { get; set; }
        public string ApplicationTypeName { get; set; }
        public string CredentialTypeName { get; set; }
        public string Skill { get; set; }
        public string PreferredTestLocation { get; set; }
        public int NumberOfApplicants { get; set; }
        public string RequestStatus { get; set; }
        public bool ShowAllocateTestSession { get; set; }
        public int TestLocationId { get; set; }

        public DateTime EarliestApplication { get; set; }
        public DateTime LastApplication { get; set; }
    }

    public class CredentialRequestSummaryItemModel
    {
        public string CompositeId { get; set; }
        public double DaysSinceSubmissionInt { get; set; }
        public int CredentialApplicationTypeId { get; set; }
        public int CredentialTypeId { get; set; }
        public int SkillId { get; set; }
        public int CredentialRequestStatusTypeId { get; set; }
        public string DaysSinceSubmission { get; set; }
        public string ApplicationTypeName { get; set; }
        public string CredentialTypeName { get; set; }
        public string Skill { get; set; }
        public string PreferredTestLocation { get; set; }
        public string RequestStatus { get; set; }
        public bool ShowAllocateTestSession { get; set; }
        public int TestLocationId { get; set; }

        public DateTime EarliestApplication { get; set; }
        public DateTime LastApplication { get; set; }

        public IEnumerable<CredentialRequestApplicantModel> Applicants { get; set; }
    }

}
