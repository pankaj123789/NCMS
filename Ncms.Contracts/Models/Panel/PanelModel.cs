using System;

namespace Ncms.Contracts.Models.Panel
{
    public class PanelModel
    {
        public int? PanelId { get; set; }
        public int? LanguageId { get; set; }
        public string Language { get; set; }
        public string Name { get; set; }
        public string Note { get; set; }
        public int PanelTypeId { get; set; }
        public string PanelType { get; set; }
        public DateTime CommissionedDate { get; set; }
        public bool HasCurrentMembers { get; set; }
        public bool VisibleInEportal { get; set; }
        public bool HasExaminersAllocated { get; set; }
     
    }
}
