using System;

namespace F1Solutions.Naati.Common.Contracts.Dal.Request
{
    public class GetPanelsRequest
    {
        public int[] NAATINumber { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int[] RoleCategoryIds { get; set; }
        public int[] LanguageId { get; set; }
        public int[] MaterialRequestCredentialTypeId { get; set; }
        public int[] MaterialRequestTaskTypeId { get; set; }
        public string MaterialRequestTitle { get; set; }
        public int[] MaterialRequestStatusId { get; set; }
        public int? PanelId { get; set; }
        public bool? Chair { get; set; }
        public bool? IsVisibleInEportal { get; set; }
    }
}