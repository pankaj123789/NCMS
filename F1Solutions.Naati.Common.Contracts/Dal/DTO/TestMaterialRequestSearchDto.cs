using System;

namespace F1Solutions.Naati.Common.Contracts.Dal.DTO
{
    public class TestMaterialRequestSearchDto
    {
        public int Id { get; set; }
        public string CredentialType { get; set; }
        public string TestTaskType { get; set; }
        public string Language { get; set; }
        public string Panel { get; set; }
        public string PanelLanguageName { get; set; }
        public int Round { get; set; }
        public int LatestRoundId { get; set; }
        public int CoordinatorNaatiNumber { get; set; }
        public int RoundStatusTypeId { get; set; }
        public string Coordinator { get; set; }
        public int RequestStatusTypeId { get; set; }
        public string RequestTitle { get; set; }
        public string RequestType { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime DueDate { get; set; }
        public int SourceMaterialId { get; set; }
        public int OutputMaterialId { get; set; }
    }
}
