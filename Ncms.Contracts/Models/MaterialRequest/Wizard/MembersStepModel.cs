namespace Ncms.Contracts.Models.MaterialRequest.Wizard
{
    public class MembersStepModel
    {
        public MaterialRequestMember[] SelectedMembers { get; set; }
    }

    public class MaterialRequestMember
    {
        public int PanelMembershipId { get; set; }
        public int MemberTypeId { get; set; }

        public MaterialRequestTaskModel[] Tasks { get; set; }
    }

 
}

