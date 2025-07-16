using System.Collections.Generic;
using F1Solutions.Naati.Common.Contracts.Bl.DTO;
using F1Solutions.Naati.Common.Contracts.Dal.DTO;
using F1Solutions.Naati.Common.Contracts.Dal.Request;
using F1Solutions.Naati.Common.Contracts.Dal.Response;
using F1Solutions.Naati.Common.Contracts.Dal.Services;

namespace F1Solutions.Naati.Common.Contracts.Dal.QueryServices
{
    
    public interface ITestMaterialQueryService : IQueryService
    {
        
        ServiceResponse<IEnumerable<DocumentLookupTypeDto>> GetTestMaterialDocuments(int testMaterialId);
        
        AttachmentResponse CreateOrUpdateTestMaterialAttachment(TestMaterialAttachmentRequest model);

        
        CreateOrUpdateResponse CreateOrUpdateTestMaterial(TestMaterialRequest model);

        
        GetTestMaterialsFromTestTaskResponse GetTestMaterialsFromTestTask(GetTestMaterialsFromTestTaskRequest request);

        
        TestMaterialDto GetTestMaterials(int id);

        
        IEnumerable<TestMaterialAttachmentDto> GetTestMaterialAttachment(int testMaterialId, bool eportalDownload = false, bool includeDeleted = false);

        
        IEnumerable<TestMaterialAttachmentDto> GetTestMaterialsAttachments(IList<int> testMaterialIds, bool eportalDownload = false, bool includeDeleted = false);

        
        IEnumerable<AttendeeTestSpecificationAttachmentDto> GetTestSpecificationAttachment(int testSpecificationId, bool eportalDownload = false);

        
        GetDocumentTypesForApplicationTypeResponse GetDocumentTypesForTestMaterialType();

        
        void DeleteAttachment(int storedFileId);

        
        GetTestSessionTestMaterialResponse GetTestMaterialsByAttendees(IList<int> ids, bool showAll);

        
        GetTestSessionTestMaterialResponse GetTestMaterialsByAttendee(int id);

        
        IList<int> GetExistingTestMaterialIdsByAttendees(IList<int> ids);

        
        GetAttendeesTestSpecificationTestMaterialResponse GetAttendeesTestSpecificationTestMaterialList(GetAttendeesTestSpecificationTestMaterialRequest request);

        
        GetAttendeesTestSpecificationTestMaterialResponse GetTestSpecificationTestMaterialsByAttendanceId(int attendanceId, bool eportalDownload = false);

        
        void AssignMaterial(AssignTestMaterialRequest request);

        
        void DeleteMaterialById(int testSittingTestMaterialId);

        
        IEnumerable<string> GetAllCustomerAttendanceIdsList(int testSessionId);

        
        DocumentAdditionalTokenValueDto GetDocumentAdditionalTokens(int attendanceId);

        
        void RemoveTestMaterials(AssignTestMaterialRequest request);

        
        IEnumerable<PersonTestTaskDto> GetTestTasksPendingToAssign(int testSessionId);

        
        IEnumerable<TestSpecificationDetailsDto> GetTestSpecificationDetails(TestSpecificationDetailsRequest request);

        
        IEnumerable<SpecificationSkillDto> GetTestSpecificationSkills(TestSpecificationSkillsRequest request);

        
        IEnumerable<TestMaterialDetailDto> GetTestSpecificationMaterials(TestSpecificationMaterialRequest request);

        
        IEnumerable<TestTaskDetailDto> GetTestSpecificationTasks(TestTaskRequest request);

        
        TestMaterialSummaryDto GetTestMaterialsSummary(TestMaterialsSummaryRequest request);

        
        IEnumerable<SupplementarytTestApplicantDto> GetSupplementaryTestApplicants(SupplementaryTestRequest request);

        
        IEnumerable<TestMaterialApplicantDto> GetApplicantWithAlreadySatMaterialsForTestSession(int testSessionId);

        
        IEnumerable<TestSittingDetailsDto> GetPeopleWithOtherTestSittingAssingnedForTheSameDay(int testSessionId);

        
        IEnumerable<ApplicationBriefDto> GetPendingCandidateBriefsToSend(PendingBriefRequest request);

        
        GetExaminersAndRolePlayersResponse GetExaminersAndRolePlayers(TestMaterialsSummaryRequest request);

        
        TestMaterialSearchResultResponse SearchTestMaterials(TestMaterialSearchRequest request);

        
        LookupTypeResponse GetTestMaterialDomains(int credentialTypeId);

     
        LookupTypeResponse GetCredentialTypeDomains(List<int> credentialTypeIdIntList);

        GetTestMaterialCreationPaymentsResponse GetTestMaterialCreationPayments(GetTestMaterialCreationPaymentsRequest request);
        GenericResponse<List<string>> GetIncludeSystemValueSkillNames();
    }
  

    public enum TestMaterialLinkTypeName
    {
        Child =1,
        Duplicated = 2
    }

    public enum TestMaterialDomainTypeName
    {
        Undefined = 1, 
    }
}