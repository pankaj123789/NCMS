using System;

namespace F1Solutions.Naati.Common.Contracts.Dal.Request
{
    public class CreateOrUpdatePanelRequest
    {
        public int? PanelId { get; set; }
        public int? LanguageId { get; set; }
        public string Name { get; set; }
        public string Note { get; set; }
        public int PanelTypeId { get; set; }
        public DateTime ComissionedDate { get; set; }
        public bool VisibleInEportal { get; set; }
    }
}