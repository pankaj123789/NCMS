using System;
using System.Collections.Generic;

namespace MyNaati.Ui.ViewModels.ExaminerTools
{
    public class TestMaterialRequestModel
    {
        public int MaterialRequestId { get; set; }
        public int TestMaterialId { get; set; }
        public int PanelId { get; set; }
        public int CredentialTypeId { get; set; }
        public string Title { get; set; }
        public string CredentialTypeExternalName { get; set; }
        public string SkillDisplayName { get; set; }
        public string TaskTypeName { get; set; }
        public string RequestTypeName { get; set; }
        public string RequestStatusName { get; set; }
        public int RoundId { get; set; }
        public int RoundNumber { get; set; }
        public string Round { get; set; }
        public int RoundStatusId { get; set; }
        public string RoundStatusName { get; set; }
        public DateTime DueDate { get; set; }
        public decimal ProductSpecificationCostPerUnit { get; set; }
        public IEnumerable<KeyValuePair<int, string>> AvailableDocumentTypes { get; set; }
        public MaterialRequestPanelMembershipModel[] Members { get; set; }
        public string MaterialRequestCoordinatorLoadingPercentage { get; set; }
        public IEnumerable<KeyValuePair<int, string>> MaterialRequestTaskTypes { get; set; }
        public string LatestRoundNotes { get; set; }
        public double MaxBillableHours { get; set; }
        public int TestMaterialDomainId { get; set; }
    }

    public class MaterialRequestSearchResultModel
    {
        public int MaterialRequestId { get; set; }
        public int TestMaterialId { get; set; }
        public string Title { get; set; }
        public string CredentialTypeExternalName { get; set; }
        public string SkillDisplayName { get; set; }
        public string TaskTypeName { get; set; }
        public string RequestTypeName { get; set; }
        public string RequestStatusName { get; set; }
        public int RoundId { get; set; }
        public int RoundNumber { get; set; }
      
        public int RoundStatusId { get; set; }
        public string RoundStatusName { get; set; }
        public DateTime DueDate { get; set; }

        public bool IsCoordinator { get; set; }
        public bool IsEditable { get; set; }
    }
}