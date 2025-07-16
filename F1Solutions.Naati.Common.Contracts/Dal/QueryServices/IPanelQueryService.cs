using System.Collections.Generic;
using F1Solutions.Naati.Common.Contracts.Dal.DTO;
using F1Solutions.Naati.Common.Contracts.Dal.Request;
using F1Solutions.Naati.Common.Contracts.Dal.Response;
using F1Solutions.Naati.Common.Contracts.Dal.Services;

namespace F1Solutions.Naati.Common.Contracts.Dal.QueryServices
{
    
    public interface IPanelQueryService : IQueryService
    {
        
        GetPanelsResponse GetPanels(GetPanelsRequest request);

        
        ServiceResponse<int> CreateOrUpdatePanel(CreateOrUpdatePanelRequest request);

        
        DeletePanelResponse DeletePanel(DeletePanelRequest request);

        
        GetMembershipsResponse GetMemberships(GetMembershipsRequest request);
        
        
        PanelMembershipLookupTypeResponse GetPanelMembershipLookUp(GetPanelMemberLookupRequest request);

        
        AddOrUpdateMembershipResponse AddOrUpdateMembership(AddOrUpdateMembershipRequest request);

        
        DeleteMembershipResponse DeleteMembership(DeleteMembershipRequest request);

        
        ReappointMembersResponse ReappointMembers(ReappointMembersRequest request);

        
        ValidateExaminerSecurityCodeResponse ValidateExaminerSecurityCode(ValidateExaminerSecurityCodeRequest request);

        
        GetUnavailabilityResponse GetUnavailability(GetUnavailabilityRequest getUnavailabilityRequest);

        
        GetMarkingRequestsResponse GetMarkingRequests(GetMarkingRequestsRequest getMarkingRequestsRequest);

        
        GetMaterialRequestsResponse GetMaterialRequests(GetMaterialRequestsRequest getMaterialRequestsRequest);
        
        
        bool HasPersonEmailAddress(int personId);

        
        bool HasOverlappingMembership(OverlappingMembershipRequestItem[] checkOverlappingMembershipRequest);

        
        bool HasRolePlayerRatingLocation(int personId);

        
        GetPanelTypesResponse GetPanelTypes();

        
        GetPanelResponse GetPanel(int panelId);

        
        ServiceResponse<IEnumerable<PanelMembershipInfoDto>> GetPanelMembershipInfo(PanelMembershipInfoRequest request);
    }
}
