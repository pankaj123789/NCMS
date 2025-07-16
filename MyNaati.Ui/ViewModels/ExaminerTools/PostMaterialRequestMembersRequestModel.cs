using System.Collections.Generic;

namespace MyNaati.Ui.ViewModels.ExaminerTools
{
    public class PostMaterialRequestMembersRequestModel
    {
        public int MaterialRequestId { get; set; }
        public int RoundId { get; set; }
        public IEnumerable<MaterialRequestPanelMembershipModel> Members { get; set; }
    }
}