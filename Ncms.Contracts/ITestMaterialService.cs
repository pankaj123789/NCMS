using System;
using System.Collections.Generic;
using F1Solutions.Naati.Common.Contracts.Bl.Attributes;
using F1Solutions.Naati.Common.Contracts.Bl.DTO;
using F1Solutions.Naati.Common.Contracts.Dal.Enum;
using F1Solutions.Naati.Common.Contracts.Dal.Request;
using F1Solutions.Naati.Common.Contracts.Dal.Response;
using Ncms.Contracts.Models.MaterialRequest;
using Ncms.Contracts.Models.TestMaterial;

namespace Ncms.Contracts
{
    public interface ITestMaterialService
    {
        // IList<TestMaterialSearchResponseModel> List(string request);
        AttachmentResponse CreateOrUpdateTestMaterialAttachment(TestMaterialAttachmentRequest model);
        CreateOrUpdateResponse CreateOrUpdateTestMaterial(TestMaterialRequest model);
        IEnumerable<TestMaterialResponse> GetTestMaterialsByAttendees(IList<int> ids, bool showAll = false);

        IEnumerable<TestMaterialResponse> GetTestMaterialsByAttendee(int id);

        IList<int> GetExistingTestMaterialIdsByAttendees(IList<int> ids);

        TestMaterialResponse GetTestMaterials(int id);
        GenericResponse<IEnumerable<string>> GetDocumentTypesForTestMaterialType();
        GenericResponse<IEnumerable<TestMaterialAttachMentResponse>> GetTestMaterialAttachment(int testMaterialId);
        void DeleteAttachment(int storedFileId);
        void AssignMaterial(AssignTestMaterialModel model);
        void RemoveTestMaterials(AssignTestMaterialModel model);

        BusinessServiceResponse BulkDownloadTestMaterial(int testSessionId, bool includeExaminer);
        GenericResponse<IEnumerable<PersonTestTask>> GetTestTasksPendingToAssign(int testSessionId);



        void DeleteMaterialById(int id);

        GenericResponse<IEnumerable<TestMaterialResponse>> GetTestMaterialsFromTestTask(int testComponentId, int? skillId, bool? includeSystemValueSkillTypes);

        GetTestSpecificationResponse GetTestSpecificationDetails(TestMaterialBulkAssignmentRequest request);
        GenericResponse<IEnumerable<object>> ValidateTestSpecification(TestMaterialBulkAssignmentRequest request);
        TestSpecificationSkillsResponse GetTestSpecificationSkills(TestMaterialBulkAssignmentRequest request);
        GenericResponse<IEnumerable<object>> ValidateSkill(TestMaterialBulkAssignmentRequest request);
        GetTestMaterialsResponse GetTestMaterials(PagedTestMaterialBulkAssignmentRequest request);
        TestTaskResponse GetTestTasks(TestMaterialBulkAssignmentRequest request);
        GenericResponse<IEnumerable<object>> ValidateTestMaterials(TestMaterialBulkAssignmentRequest request);

        TestMaterialSummaryResponse GetTestMaterialsSummary(TestMaterialBulkAssignmentRequest request);
        GenericResponse<IEnumerable<object>> ValidateTestMaterialSummary(TestMaterialBulkAssignmentRequest request);

        GenericResponse<string> GetTestMaterialUriFromBackgroundOperation(int notificationId);

        GetSupplemeantaryTestResponse GetSupplementaryTests(TestMaterialBulkAssignmentRequest request);

        TestTaskResponse GetTestSpecificationComponents(int testSepecificationId);

        GenericResponse<IEnumerable<ApplicationBriefModel>> GetPendingCandidateBriefsToSend(PendingBriefRequest request);
        GetExaminersAndRolePlayersResponse GetExaminersAndRolePlayers(TestMaterialBulkAssignmentRequest request);
        GenericResponse<IEnumerable<TestMaterialSearchModel>> SearchTestMaterials(SearchRequest request);
        GenericResponse<List<string>> GetIncludeSystemValueSkillNames();
    }

    public class ApplicationBriefModel
    {public int CredentialRequestId { get; set; }
        public int CredentialApplicationId { get; set; }
        public IEnumerable<CandidateBriefFileInfoModel> Briefs { get; set; }

    }

    public class TestMaterialSearchModel
    {
        public int Id { get; set; }
        public string Title { get; set; }
        [LookupDisplay(LookupType.TestMaterialDomain, "Domain")]
        public int TestMaterialDomainId { get; set; }
        public string LanguageOrSkill { get; set; }
        public int? LanguageId { get; set; }
        public string CredentialType { get; set; }
        public int CredentialTypeId { get; set; }
        public string TaskType { get; set; }
        public bool HasFile { get; set; }
        public bool Available { get; set; }
        public int TestComponentTypeId { get; set; }
        public int? SkillId { get; set; }
        public int TestSpecificationId { get; set; }
        public bool TestSpecificationActive { get; set; }
        public int? SourceMaterialId { get; set; }
        public string SourceMaterialTitle { get; set; }

        public int StatusId { get; set; }
        public DateTime? LastUsedDate { get; set; }
    }


    public class CandidateBriefFileInfoModel
    {
        public int? CandidateBriefId { get; set; }

        public int StorageFileId { get; set; }

        public string TaskLabel { get; set; }
        public string TaskTypeLabel { get; set; }
        public int TestMaterialId { get; set; }
        public int TestMaterialAttachmentId { get; set; }
    }

    public class GetSupplemeantaryTestResponse
    {
        public IEnumerable<SupplementarytTestApplicantModel> Results { get; set; }
    }

    public class GetExaminersAndRolePlayersResponse
    {
        public IEnumerable<TestMaterialExaminerModel> Results { get; set; }
    }

    public class TestMaterialExaminerModel
    {
        public int NaatiNumber { get; set; }
        public string Name { get; set; }
        public IEnumerable<TestMaterialPanelMembershipModel> PanelMemberships { get; set; }
    }

    public class TestMaterialPanelMembershipModel
    {
        public TestMaterialPanelModel Panel { get; set; }
        public string RoleName { get; set; }
        public DateTime From { get; set; }
        public DateTime To { get; set; }
    }

    public class TestMaterialPanelModel
    {
        public int PanelId { get; set; }
        public string Name { get; set; }
    }

    public class SupplementarytTestApplicantModel
    {

        public int NaatiNumber { get; set; }

        public int TestSittingId { get; set; }

        public string ApplicationReference { get; set; }

        public int ApplicationId { get; set; }

        public int TestSessionId { get; set; }

    }
    public class TestMaterialSummaryResponse
    {
        public int TotalNewApplicantsNotSat { get; set; }
        public int TotalNewApplicantsSat { get; set; }
        public int TotalApplicantsToOverrideNotSat { get; set; }
        public int TotalApplicantsToOverrideSat { get; set; }

        public IEnumerable<TestMaterialApplicantModel> ApplicantsAlreadySat { get; set; }

    }

    public class TestMaterialApplicantModel
    {

        public int NaatiNumber { get; set; }

        public int TestSittingId { get; set; }

        public string Name { get; set; }

        public int PreviousTestSessionId { get; set; }

        public string PreviousTestSessionName { get; set; }

        public DateTime PreviousTestSessionDate { get; set; }

        public ICollection<int> ConflictingTestMaterialsIds { get; set; }

    }
    public class TestTaskResponse
    {
        public IEnumerable<TestTaskModel> Results { get; set; }
    }

    public class TestTaskModel
    {
        public int Id { get; set; }
        public int TypeId { get; set; }
        public string TaskLabel { get; set; }
        public string TypeLabel { get; set; }
        public string TaskName { get; set; }
    }

    public class GetTestMaterialsResponse
    {
        public IEnumerable<TestMaterialModel> Results { get; set; }
    }

    public class TestMaterialModel
    {
        public int Id { get; set; }

        public int TypeId { get; set; }

        public string TypeLabel { get; set; }

        public string Title { get; set; }

       
        public int StatusId { get; set; }


        [LookupDisplay(LookupType.TestMaterialType, "TestMaterialTypeName")]
        public int TestMaterialTypeId { get; set; }

        [LookupDisplay(LookupType.CredentialType, "CredentialTypeInternalName")]
        public int CredentialTypeId { get; set; }
        public string CredentialType { get; set; }
        public double DefaultMaterialRequestHours { get; set; }

        public int ApplicantsRangeTypeId { get; set; }

        public string TypeDescription { get; set; }
        public DateTime? LastUsedDate { get; set; }

        public IList<TestMaterialLinkModel> Links { get; set; }

        [LookupDisplay(LookupType.Languages, "LanguageName")]
        public int? LanguageId { get; set; }
        public string Language { get; set; }
       
        public int? SkillId { get; set; }
        public string SkillName { get; set; }
        public bool Available { get; set; }

        [LookupDisplay(LookupType.TestMaterialDomain, "Domain")]
        public int TestMaterialDomainId { get; set; }

    }

    public class PagedTestMaterialBulkAssignmentRequest : TestMaterialBulkAssignmentRequest
    {
        public int Skip { get; set; }
        public int Take { get; set; }
    }

    public class TestMaterialBulkAssignmentRequest
    {
        public int[] TestSessionIds { get; set; }

        public int TestSpecificationId { get; set; }

        public int SkillId { get; set; }

        public IEnumerable<TestMaterialAssignmentModel> TestMaterialAssignments { get; set; }

        //this is for the "Live Scenarios feature NAATI requested
        public bool? ShowIncludedValue { get; set; }

    }

    public class TestMaterialAssignmentModel
    {
        public int TestTaskId { get; set; }
        public int TestMaterialId { get; set; }
    }

    public class TestSpecificationSkillsResponse
    {
        public IEnumerable<SpecificationSkillModel> Results { get; set; }
    }

    public class GetTestSpecificationResponse
    {
        public IEnumerable<TestSpecificationDetailsModel> Results { get; set; }
    }


    public class SpecificationSkillModel
    {
        public int Id { get; set; }

        public string Skill { get; set; }
        public IEnumerable<DateTime> TestDates { get; set; }
        public int ApplicantsWithoutMaterials { get; set; }
    }



    public class TestSpecificationDetailsModel
    {
        public int Id { get; set; }
        public string Description { get; set; }
        public int NumberOfTasks { get; set; }
        public int ApplicantsWithoutMaterials { get; set; }
    }


    public enum TestMaterialWizardSteps
    {
        SupplementaryTestApplicants = 1,
        TestSpecification = 2,
        Skills = 3,
        TestMaterials = 4,
        Applicants = 5,
        ExaminersAndRolePlayers = 6,
    }   
}



