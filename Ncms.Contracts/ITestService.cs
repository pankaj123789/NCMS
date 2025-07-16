using System;
using System.Collections.Generic;
using F1Solutions.Naati.Common.Contracts.Bl.Attributes;
using F1Solutions.Naati.Common.Contracts.Bl.DTO;
using F1Solutions.Naati.Common.Contracts.Dal.Enum;
using Ncms.Contracts.Models;
using Ncms.Contracts.Models.Application;
using Ncms.Contracts.Models.CredentialRequest;
using Ncms.Contracts.Models.Test;

namespace Ncms.Contracts
{
    public interface ITestService
    {
        GenericResponse<IEnumerable<TestSearchResultModel>> List(TestSearchRequest request);
        GenericResponse<IEnumerable<VenueModel>> GetVenuesShowingInactive(int testLocationId, bool activeOnly = false);
        GenericResponse<AddExaminerResponseModel> AddExaminers(AddExaminerRequestModel request);
        GenericResponse<AddExaminerResponseModel> UpdateExaminers(UpdateExaminersRequestModel request);
        bool TestReadyForAssets(int testAttendanceId);
        
        GenericResponse<IEnumerable<VenueModel>> GetVenues(int testLocationId, bool activeOnly = false);
        GenericResponse<VenueModel> GetVenueById(int venueId);

        GenericResponse<IEnumerable<LookupTypeModel>> GetAllVenues();
        GenericResponse<TestSummaryModel> GetTestSummary(int testAttendanceId);
        GenericResponse<TestDocumentsModel> GetTestDocuments(int testId);

        GenericResponse<TestPaidReviewModel> GetTestPaidReview(int testId);
        GenericResponse<TestAssetsModel> GetTestAssets(int testId);
        GenericResponse<TestNotesModel> GetTestNotes(int testId);
        GenericResponse<TestDataModel> GetTestSittingByCredentailRequestId(int credentialRequestId, bool supplementary);

        GenericResponse<TestRubricModel> GetExaminerRubricMarking(int jobExaminerId);
        GenericResponse<TestRubricModel> GetTestResultRubricMarking(int testResultId);

        void SaveExaminerRubricMarking(TestRubricModel model);
        void SaveTestResultRubricMarking(TestRubricModel model);

        GenericResponse<bool> CheckIfAllowSupplementaryTest(int credentialRequestId);
        GenericResponse<bool> CheckIfAllowConcededPass(int credentialRequestId);
        GenericResponse<bool> CheckIfAllowPaidTestReview(int credentialRequestId);

        void ComputeRubricResults(int jobExaminerId);
        GenericResponse<TestRubricModel> ComputeFinalRubric(int testResultId);
        GenericResponse<FeedbackResponseModel> Feedback(int? testAttendanceId);
    }

    public class TestNotesModel { }

    public class TestAssetsModel { }

    public class TestPaidReviewModel { }

  

    public class TestDocumentsModel { }
    public class TestSummaryModel
    {
        public int TestAttendanceId { get; set; }
        public int TestSessionId { get; set; }

        public int ApplicationId { get; set; }
        public string ApplicationReference { get; set; }

        public int ApplicationTypeId { get; set; }
        public int CredentialTypeId { get; set; } 
        public int SkillId { get; set; }

        public string ApplicationType { get; set; } 
        public string CredentialTypeInternalName { get; set; } 
        public string Skill { get; set; }

        public int? ResultId { get; set; }
        public int? CurrentJobId { get; set; }
        public int? LastReviewTestResultId { get; set; }
        public int Language1Id { get; set; }
        public int Language2Id { get; set; }

        public DateTime TestDate { get; set; }

        public int TestStatusTypeId { get; set; }

        public string TestStatus { get; set; } 

        public int TestLocationId { get; set; } 

        public string TestLocation { get; set; } 

        public int VenueId { get; set; } 

        public string Venue { get; set; } 

        public string TestResultStatus{ get; set; }
        public int? TestResultStatusId { get; set; }
        
        public int PersonId { get; set; }
        public int NaatiNumber { get; set; }

        public IEnumerable<int> TestMaterialIds { get; set; }
        public IEnumerable<string> TestMaterialNames { get; set; }

        public  dynamic Actions { get; set; }
        public int CredentialRequestStatusTypeId { get; set; }
        public int ApplicationStatusTypeId { get; set; }
        public int CredentialRequestId { get; set; }

        public bool Supplementary { get; set; }
        public bool AllowSupplementary { get; set; }

        public bool HasDowngradePaths { get; set; }
        public bool SupplementaryCredentialRequest { get; set; }

        public int MarkingSchemaTypeId { get; set; }

        public bool? EligibleForConcededPass { get; set; }
        public bool? EligibleForSupplementary { get; set; }

        public bool DefaultTestSpecification { get; set; }
        public int TestSpecificationId { get; set; }
        public bool HasFeedback { get; set; }

    }
    
    public class TestMaterialResponse
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Language { get; set; }
        public string CredentialType { get; set; }
        public string TaskType { get; set; }
        public bool HasFile { get; set; }
        public bool Available { get; set; }
        public int? LanguageId { get; set; }
        public int CredentialTypeId { get; set; }
        public int TestComponentTypeId { get; set; }
        public int? SkillId { get; set; }
        public string Notes { get; set; }
        public int TestSittingTestMaterialId { get; set; }
        [LookupDisplay(LookupType.TestMaterialDomain, "Domain")]
        public int TestMaterialDomainId { get; set; }
        public int TestSittingId { get; set; }
        public int TestSpecificationId { get; set; }
        public bool TestSpecificationActive { get; set; }
        public bool IsTestMaterialTypeSource { get; set; }
        public int? SourceTestMaterialId { get; set; }

    }

   
    public class TestMaterialAttachMentResponse
    {
        public int Id { get; set; }
        public int StoredFileId { get; set; }
        public string FileName { get; set; }
        public string Description { get; set; }
        public string FilePath { get; set; }
        public string StoragePath { get; set; }
        public string DocumentType { get; set; }
        public int DocumentTypeId { get; set; }
        public string UploadedByName { get; set; }
        public int UploadedByUserId { get; set; }
        public DateTime UploadedDateTime { get; set; }
        public long FileSize { get; set; }
        public string FileType { get; set; }
        public string Type { get; set; }
        public string Title { get; set; }
        public int MaterialId { get; set; }
        public bool EportalDownload { get; set; }
        public bool MergeDocument { get; set; }
        public DateTime? SoftDeleteDate { get; set; }
    }

    public class PersonTestTask
    {
        public int NaatiNumber { get; set; }
        public string PersonName { get; set; }
        public string TaskLabel { get; set; }
    }
}
