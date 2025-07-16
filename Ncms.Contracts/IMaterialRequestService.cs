using System.Collections.Generic;
using F1Solutions.Naati.Common.Contracts.Bl;
using F1Solutions.Naati.Common.Contracts.Bl.DTO;
using Ncms.Contracts.Models;
using Ncms.Contracts.Models.Application;
using Ncms.Contracts.Models.MaterialRequest;

namespace Ncms.Contracts
{
    public interface IMaterialRequestService
    {
        GenericResponse<IEnumerable<EmailMessageModel>> GetEmailPreview(MaterialRequestWizardModel wizardModel);
        GenericResponse<UpsertMaterialRequestResultModel> PerformAction(MaterialRequestWizardModel wizardModel);
        GenericResponse<UpsertMaterialRequestResultModel> UpsertMaterialRequestActionData(MaterialRequestActionModel model);
        GenericResponse<IEnumerable<TestMaterialSearchModel>> SearchTestMaterials(string searchFilter, bool availableOnly);
        GenericResponse<IEnumerable<TestMaterialRequestSearchResultModel>> SearchTestMaterialRequests(SearchRequest request);
        GenericResponse<IEnumerable<TestMaterialRequestSearchResultModel>> GetTestMaterialRelations(int materialId);
        GenericResponse<IEnumerable<TestMaterialRequestSearchResultModel>> GetPanelMaterialRequests(int panelId);
        GenericResponse<IEnumerable<TestMaterialRequestSearchResultModel>> GetActiveCoordinatorRequests(int coordinatorId);
        GenericResponse<MaterialRequestInfoModel> GetMaterialRequest(int materialRequestId);
        GenericResponse<object> GetNewRoundDetails(int materialRequestId, int actionId, int testComponentTypeId);
        GenericResponse<IEnumerable<LookupTypeDetailedModel>> GetAvailableDocumentTypes(int actionId, int testmaterialTypeId);
        GenericResponse<IEnumerable<object>> GetExistingDocuments(int materialId, int sourceTestMaterialId, int actionId, int testMaterialTypeId);
        GenericResponse<bool> CanShowTotalCost(int materialRequestId);
        GenericResponse<bool> IsMaterialDomainRequired(int materialRequestId);
        IEnumerable<MaterialRequestRoundAttachmentModel> ListAttachments(ListAttachmentsRequestModel request);

        int CreateOrReplaceAttachment(MaterialRequestRoundAttachmentModel request);

        void DeleteAttachment(int materialRequestRoundAttachmentId);

        GenericResponse<IEnumerable<LookupTypeModel>> GetRoundLookup(int materialRequestId);

        GenericResponse<MaterialRequestRoundModel> GetMaterialRequestRound(int materialRequestRoundId);

        GenericResponse<IEnumerable<SkillLookupTypeModel>> GetSkillsForCredentialType(int credentialTypeId,int panelId);
        MaterialRequestActionModel GetEmptyActionModel();

        GenericResponse<IEnumerable<ValidationResultModel>> ValidateActionPreconditions(MaterialRequestWizardModel wizardModel);

        GenericResponse<bool> HasActiveMaterialRequest(int outputTestMaterialId);

        GenericResponse<bool> IsUsedAsSourceMaterial(int testMaterialId);

        GenericResponse<bool> PayMaterialRequestMembers(IEnumerable<MaterialRequestMemberGroupingModel> items);
        GenericResponse<bool> ApproveRequests(IEnumerable<MaterialRequestPayrollUserGroupingModel> items);
        GenericResponse<bool> UnApproveRequests(IEnumerable<MaterialRequestMemberGroupingModel> items);

        GenericResponse<TestComponentTypeModel> GetTestComponentType(int testComponenTypeId);

        GenericResponse<IEnumerable<MaterialRequestPayrollUserGroupingModel>> GetPendingItemsToApprove();
        GenericResponse<IEnumerable<MaterialRequestMemberGroupingModel>> GetPendingItemsToPay();

        GenericResponse<IEnumerable<MaterialRequestMemberGroupingModel>> GetPendingItemsToPay(int materialRequestId);
        GenericResponse<IEnumerable<MaterialRequestPayrollUserGroupingModel>> GetPendingItemsToApprove(int materialRequestId);

        GenericResponse<IEnumerable<LookupTypeDetailedModel>> GetAllAvailableDocumentTypes();

        GenericResponse<IEnumerable<LookupTypeModel>> GetTestMaterialDomains(int credentialTypeId);
        GenericResponse<bool> DeleteLink(int materialRequestRoundLinkId);
        GenericResponse<int> AddLink(int materialRequestRoundId, MaterialRequestRoundLinkModel link);
    }

    public enum MaterialRequestWizardStep
    {
        TestMaterial = 1,
        TestMaterialSource = 2,
        RoundDetails = 3,
        DocumentsUpload = 4,
        Members = 5,
        Notes = 6,
        SendEmailCheckOption = 7,
        EmailPreview = 8,
        ExistingDocuments = 9,
        Coordinator = 10,
        RoundLinks = 11,
        Panel = 12
    }
}
