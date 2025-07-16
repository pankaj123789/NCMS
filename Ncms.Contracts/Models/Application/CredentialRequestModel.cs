using System;
using System.Collections.Generic;

namespace Ncms.Contracts.Models.Application
{
    public class CredentialRequestModel
    {
        public int Id { get; set; }
        public string Category { get; set; }
        public int CategoryId { get; set; }
        public string CredentialName { get; set; }
        public string ExternalCredentialName { get; set; }
        public int DirectionTypeId { get; set; }
        public string Direction { get; set; }
        public string Status { get; set; }
        public bool? AutoCreated { get; set; }
        public string ApplicationTypeDisplayName { get; set; }
        public int StatusTypeId { get; set; }
        public int SkillId { get; set; }
        public DateTime StatusChangeDate { get; set; }
        public string ModifiedBy { get; set; }
        public int StatusChangeUserId { get; set; }
        public int CredentialTypeId { get; set; }
        public int? CredentialId { get; set; }

        public int? ConcededFromCredentialRequestId { get; set; }

        public bool Supplementary { get; set; }


        public int CredentialRequestPathTypeId { get; set; }
        public CredentialTypeModel CredentialType { get; set; }
        public SkillModel Skill { get; set; }
        public IList<CredentialRequestFieldModel> Fields { get; set; }
        public IList<CredentialModel> Credentials { get; set; }
        public IList<dynamic> Actions { get; set; }
        public IList<CredentialWorkflowFeeModel> CredentialWorkflowFees { get; set; }

        public IList<CredentialRequestTestSessionModel> TestSessions { get; set; }

        public IList<WorkPracticeDataModel> WorkPractices { get; set; }

        public IList<CandidateBriefModel> Briefs { get; set; }

        public IList<RefundModel> RefundRequests { get; set; } = new List<RefundModel>();
    }
}
