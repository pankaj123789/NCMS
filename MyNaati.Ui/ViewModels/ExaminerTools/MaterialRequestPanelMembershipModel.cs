using System.Collections.Generic;

namespace MyNaati.Ui.ViewModels.ExaminerTools
{
    public class MaterialRequestPanelMembershipModel
    {
        public int Id { get; set; }

        public int PanelMemberShipId { get; set; }
        public int NaatiNumber { get; set; }
        public int EntityId { get; set; }

        public IList<MaterialRequestTaskModel> Tasks { get; set; }
        public int MemberTypeId { get; set; }

        public string GivenName { get; set; }
        public string PrimaryEmail { get; set; }
    }
}