using System.Collections.Generic;
using F1Solutions.Naati.Common.Contracts.Bl.DTO;
using Ncms.Contracts.Models;
using Ncms.Contracts.Models.Application;
using Ncms.Contracts.Models.Panel;

namespace Ncms.Contracts
{
    public interface IPanelService
    {
        IList<PanelModel> List(string request);
        IList<PanelMembershipModel> ListMembership(string request);
        int CreateOrUpdate(PanelModel model);
        int AddOrUpdateMember(PanelMembershipModel model);
        PanelModel Delete(int panelId);
        PanelMembershipModel RemoveMember(int panelMembershipId);
        int[] ReappointMembers(ReappointMembersModel reappointMembersModel);
        IList<ExaminerUnavailability> GetUnavailability(int panelMembershipId);
        IList<MarkingRequest> GetMarkingRequests(int panelMembershipId);
        IList<MaterialRequest> GetMaterialRequests(int panelMembershipId);
        GenericResponse<IEnumerable<PanelMembershipLookupModel>> GetPanelMembershipLookUp(GetPanelMemberLookupRequestModel request);
        GenericResponse<IEnumerable<LookupTypeModel>> GetAvailableMembershipCredentialTypes(int panelId, int membershipId);
        GenericResponse<IEnumerable<PanelTypeModel>> GetPanelTypes();
        bool HasPersonEmailAddress(int personId);
        bool HasRolePlayerRatingLocation(int personId);
        bool HasOverlappingMembership(OverlappingMembershipModel[] model);
        GenericResponse<PanelModel> GetPanel(int panelId);

        GenericResponse<IEnumerable<PanelMembershipInfoModel>> GetPanelMembershipInfo(IEnumerable<int> panelMembershipIds);
    }

    public class PanelTypeModel
    {
        public  int Id { get; set; }
        public  string DisplayName { get; set; }
        public  bool AllowCredentialTypeLink { get; set; }
    }

    public class GetPanelMemberLookupRequestModel
    {
        public int[] PanelIds { get; set; }

        public bool ActiveMembersOnly { get; set; }

        public int?  CredentialTypeId { get; set; }
    }
}
