using F1Solutions.Naati.Common.Contracts.Dal.DTO;
using F1Solutions.Naati.Common.Contracts.Dal.Request;
using F1Solutions.Naati.Common.Contracts.Dal.Response;
using F1Solutions.Naati.Common.Contracts.Dal.Services;

namespace F1Solutions.Naati.Common.Contracts.Dal.QueryServices
{
    
    public interface IExaminerToolsService : IQueryService
    {
        
        GetTestsResponse GetTests(GetTestsRequest request);

        
        GetTestsMaterialResponse GetTestsMaterial(GetTestsMaterialRequest request);

        
        TestMaterialContract GetTestMaterial(GetTestMaterialRequest request);

        
        GetTestDetailsResponse GetTestDetails(GetTestDetailsRequest request);

        
        SubmitTestResponse SubmitTest(SubmitTestRequest request);

        
        ListUnavailabilityResponse ListUnavailability(ListUnavailabilityRequest request);

        
        SaveUnavailabilityResponse SaveUnavailability(SaveUnavailabilityRequest request);

        
        DeleteUnavailabilityResponse DeleteUnavailability(DeleteUnavailabilityRequest request);

        
        SaveMaterialResponse SaveMaterial(SaveMaterialRequest request);

        
        SaveAttachmentResponse SaveAttachment(SaveAttachmentRequest request);

        
        DeleteMaterialResponse DeleteMaterial(DeleteMaterialRequest request);

        
        DeleteAttachmentResponse DeleteAttachment(DeleteAttachmentRequest request);

        
        GetMaterialFileResponse GetMaterialFile(GetMaterialFileRequest request);

        
        GetTestAssetsFileResponse GetTestAssetsFile(GetTestAssetsFileRequest request);

        
        GetAttendeesTestSpecificationTestMaterialResponse GetTestMaterialsByAttendaceId(int attendanceId);

        
        GetAttendeesTestSpecificationTestMaterialResponse GetFileStoreTestSpecificationTestMaterialList(GetFileStoreTestSpecificationTestMaterialRequest request);

        
        SubmitMaterialResponse SubmitMaterial(SubmitMaterialRequest request);

        
        GetTestMaterialsFileResponse GetTestMaterialsFile(GetTestMaterialsFileRequest request);

        
        GetTestAttendanceDocumentResponse GetTestAttendanceDocument(GetTestAttendanceDocumentRequest request);

        
        GetDocumentTypesResponse GetDocumentTypes();

        
        DocumentAdditionalTokenValueDto GetDocumentAdditionalTokens(int attendanceId);

        
		void SaveRolePlayerSettings(RolePlayerSettingsRequest serviceRequest);

        
		GetRolePlayerSettingsResponse GetRolePlayerSettings(GetRolePlayerSettingsRequest request);

        
        bool IsValidAttendeeDeleteAttachment(int testAttendanceDocumentId, int naatiNumber);

        
        int? GetTestSittingIdByTestResult(int testResultId);

        
        bool IsValidExaminerForAvailability(int examinerUnavailableId, int naatiNumber);
    }
}
