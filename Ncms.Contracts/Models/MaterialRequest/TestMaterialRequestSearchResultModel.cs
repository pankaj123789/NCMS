using System;
using F1Solutions.Naati.Common.Contracts.Bl.Attributes;
using F1Solutions.Naati.Common.Contracts.Dal.Enum;

namespace Ncms.Contracts.Models.MaterialRequest
{
    public class TestMaterialRequestSearchResultModel
    {
        public int Id { get; set; }
        public string CredentialType { get; set; }
        public string TestTaskType { get; set; }
        public string Language { get; set; }
        public string Panel { get; set; }
        public string Round { get; set; }
        [LookupDisplay(LookupType.MaterialRequestRoundStatus, "RoundStatus")]
        public int RoundStatusTypeId { get; set; }
        public string Coordinator { get; set; }
        [LookupDisplay(LookupType.MaterialRequestStatus, "RequestStatus")]
        public int RequestStatusTypeId  { get; set; }
        public string RequestTitle { get; set; }
        public string RequestType { get; set; }
        public string RelationType { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime DueDate { get; set; }
        public int SourceMaterialId { get; set; }
        public int OutputMaterialId { get; set; }
    }
}