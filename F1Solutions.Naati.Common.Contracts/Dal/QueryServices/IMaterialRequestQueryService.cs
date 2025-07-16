using System.Collections.Generic;
using F1Solutions.Naati.Common.Contracts.Dal.DTO;
using F1Solutions.Naati.Common.Contracts.Dal.Request;
using F1Solutions.Naati.Common.Contracts.Dal.Response;
using F1Solutions.Naati.Common.Contracts.Dal.Services;

namespace F1Solutions.Naati.Common.Contracts.Dal.QueryServices
{
    
    public interface IMaterialRequestQueryService : IQueryService
    {
        
        UpsertMaterialRequestResultResponse UpsertMaterialRequestActionData(MaterialRequestActionDto actionData);

        
        UpsertMaterialRequestResultResponse UpsertMaterialRequestsActionData(IList<MaterialRequestActionDto> actions);
        
        MaterialRequestActionDto GetMaterialRequestActionData(int materialRequestId);
        
        MaterialRequestInfoDto GetMaterialRequest(int materialRequestId);
        
        
        LookupTypeResponse GetRoundsLookup(int materialRequestId);

        
        ServiceResponse<int> CountTestMaterialRequests(TestMaterialRequestSearchRequest request);

        
        ServiceResponse<IEnumerable<RoundDocumentLookupTypeDto>> GetExistingDocuments(int materialRequestId);

        
        GetMaterialRequestAttachmentsResponse GetAttachments(GetMaterialRequestRoundAttachmentsRequest request);

        
        GetMaterialRequestAttachmentResponse GetAttachment(GetMaterialRequestRoundAttachmentRequest request);

        
        CreateOrReplaceMaterialRequestRoundAttachmentResponse CreateOrReplaceAttachment(CreateOrReplaceMaterialRequestRoundAttachmentRequest request);

        

        DeleteAttachmentResponse DeleteAttachment(DeleteMaterialRequestRoundAttachmentRequest request);

        
        ServiceResponse<MaterialRequestRoundDto> GetMaterialRequestRound(int materialRequestRoundId);

        
        GetSkillsForCredentialTypeResponse GetMaterialRequestSkills(MaterialRequestSkillRequest request);

        
        TestComponentTypeDto GetTestComponentType(int testComponentTypeId);

        
        TestMaterialRequestSearchResultResponse SearchTestMaterialRequests(TestMaterialRequestSearchRequest request);

        
        GetMaterialRequestPayrollResponse GetPendingToApproveMaterialRequestPayRolls(MaterialRequestPayRollRequest request);
        
        GetMaterialRequestPayrollResponse GetPendingToPayMaterialRequestPayRolls(MaterialRequestPayRollRequest request);
        
        UpdateMaterialRequestMembersResponse UpdateMaterialRequestMembers(UpdateMaterialRequestMembersRequest request);
        
        SaveMaterialRequestRoundLinkResponse SaveMaterialRequestRoundLink(SaveMaterialRequestRoundLinkRequest request);
        
        GetMaterialRequestRoundLinkResponse GetMaterialRequestRoundLink(GetMaterialRequestRoundLinkRequest getMaterialRequestRoundLinkRequest);
        
        void DeleteMaterialRequestLink(DeleteMaterialRequestLinkRequest removeMaterialRequestRoundLinkRequest);

   
        LookupTypeResponse GetAvailableMaterialRequestCredentialTypes(int naatiNumber);
    }
}
