using System;

namespace F1Solutions.Naati.Common.Contracts.Dal.DTO
{
    public class PanelDto
    {
        public int PanelId { get; set; }
        public int? LanguageId { get; set; }
        public string Language { get; set; }
        public string Name { get; set; }
        public string Note { get; set; }
        public int PanelTypeId { get; set; }
        public string PanelType { get; set; }
        public DateTime ComissionedDate { get; set; }
        public bool HasCurrentMembers { get; set; }
        public bool HasExaminersAllocated { get; set; }
        public bool VisibleInEportal { get; set; }
    }
}