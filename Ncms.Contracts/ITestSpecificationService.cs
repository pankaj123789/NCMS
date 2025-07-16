using System;
using System.Collections.Generic;
using F1Solutions.Naati.Common.Contracts.Bl.DTO;
using F1Solutions.Naati.Common.Contracts.Dal.Enum;
using F1Solutions.Naati.Common.Contracts.Dal.Request;
using Ncms.Contracts.Models;

namespace Ncms.Contracts
{
    public interface ITestSpecificationService
    {
        IEnumerable<TestSpecificationModel> Get(int id);
        GenericResponse<string> UpdateTestSpecification(TestSpecificationModel model);
        IEnumerable<TestSpecificationAttachmentModel> GetAttachments(int id);
        int CreateOrReplaceAttachment(TestSpecificationAttachmentModel model);
        void DeleteAttachment(int storeFiledId);
        IEnumerable<TestComponentModel> GetTestComponentsBySpecificationId(int testSpecificationId);
        GenericResponse<bool> CanUpload(int testSpecificationId);
        IEnumerable<string> GetDocumentTypesForTestSpecificationType();
        GetQuestionPassRulesResponseModel GetQuestionPassRules(int testSpecificationId);
        GenericResponse<bool> SaveQuestionPassRules(int testSpecificationId, IEnumerable<RubricQuestionPassRuleModel> request);
        GetTestBandRulesResponseModel GetTestBandRules(int testSpecificationId);
        GenericResponse<bool> SaveTestBandRules(int testSpecificationId, IEnumerable<RubricTestBandRuleModel> request);
        GetTestQuestionRulesResponseModel GetTestQuestionRules(int testSpecificationId);
        void SaveTestQuestionRules(int testSpecificationId, IEnumerable<RubricTestQuestionRuleModel> request);
        GetRubricConfigurationResponseModel GetRubricConfiguration(int testSpecificationId);
        GetRubricMarkingBandResponseModel GetMarkingBand(int rubricMarkingBandId);
        void UpdateMarkingBand(RubricMarkingBandModel rubricMarkingBandModel);

        /// <summary>
        /// Creates a new Test Specification with the supplied title
        /// against a supplied CredentialTypeId
        /// </summary>
        /// <param name="model"></param>
        /// <returns>GenericResponse and id of new TestSpecification</returns>
        GenericResponse<int> AddTestSpecification(AddTestSpecificationRequest model);
    }

    public class TestComponentModel
    {
        public int Id { get; set; }
        public double PassMark { get; set; }
        public string TaskType { get; set; }
        public string TaskTypeDescription { get; set; }
        public string BasedOn { get; set; }
        public int ComponentNumber { get; set; }
        public string Name { get; set; }
        public int TotalMarks { get; set; }
        public int MinNaatiCommentLength { get; set; }
        public int MinExaminerCommentLength { get; set; }
    }

    public class GetQuestionPassRulesResponseModel
    {
        public int TestSpecificationId { get; set; }
        public List<Models.Test.TestComponentModel> TestComponents { get; set; }
        public List<RubricQuestionPassRuleModel> Configurations { get; set; }
    }

    public class GetTestBandRulesResponseModel
    {
        public int TestSpecificationId { get; set; }
        public List<Models.Test.TestComponentModel> TestComponents { get; set; }
        public List<RubricTestBandRuleModel> Configurations { get; set; }
    }

    public class GetRubricConfigurationResponseModel
    {
        public int TestSpecificationId { get; set; }
        public List<Models.Test.TestComponentModel> TestComponents { get; set; }
    }

    public class GetTestQuestionRulesResponseModel
    {
        public int TestSpecificationId { get; set; }
        public List<Models.Test.TestComponentModel> TestComponents { get; set; }
        public List<RubricTestQuestionRuleModel> Configurations { get; set; }
    }

    public class GetRubricMarkingBandResponseModel
    {
        public RubricMarkingBandModel RubricMarkingBand { get; set; }
    }

    public class RubricQuestionPassRuleModel
    {
        public int TestSpecificationId { get; set; }
        public int TestComponentTypeId { get; set; }
        public int RubricMarkingAssessmentCriterionId { get; set; }
        public int MaximumBandLevel { get; set; }
        public string RuleGroup { get; set; }
    }

    public class RubricTestBandRuleModel
    {
        public int TestSpecificationId { get; set; }
        public int TestComponentTypeId { get; set; }
        public int TestResultEligibilityTypeId { get; set; }
        public int NumberOfQuestions { get; set; }
        public int RubricMarkingAssessmentCriterionId { get; set; }
        public int MaximumBandLevel { get; set; }
        public string RuleGroup { get; set; }
    }

    public class RubricTestQuestionRuleModel
    {
        public int TestSpecificationId { get; set; }
        public int? TestComponentTypeId { get; set; }
        public int TestResultEligibilityTypeId { get; set; }
        public int MinimumQuestionsAttempted { get; set; }
        public int MinimumQuestionsPassed { get; set; }
        public string RuleGroup { get; set; }
    }

    public class RubricMarkingBandModel
    {
        public int BandId { get; set; }
        public string Label { get; set; }
        public string Description { get; set; }
        public int Level { get; set; }
        public string CriterionName { get; set; }
        public string CriterionLabel { get; set; }
    }

    public class TestSpecificationAttachmentModel
    {
        public int? Id { get; set; }
        public int TestSpecificationAttachmentId { get; set; }
        public int TestSpecificationId { get; set; }
        public int StoredFileId { get; set; }
        public string FileName { get; set; }
        public string Title { get; set; }
        public string DocumentType { get; set; }
        public string UploadedByName { get; set; }
        public int UploadedByUserId { get; set; }
        public DateTime UploadedDateTime { get; set; }
        public long FileSize { get; set; }
        public string FileType { get; set; }
        public StoredFileType Type { get; set; }
        public string FilePath { get; set; }
        public string StoragePath { get; set; }
        public bool MergeDocument { get; set; }
        public bool EportalDownload { get; set; }
        public int? UpdateStoredFileId { get; set; }
        public DateTime? SoftDeleteDate { get; set; }
    }

    public class TestSpecificationModel
    {
        public int Id { get; set; }
        public bool Active { get; set; }
        public int CredentialTypeId { get; set; }
        public string Description { get; set; }
        public int? OverallPassMark { get; set; }
        public string CredentialType { get; set; }
        public string Reference { get; set; }
        public bool ResultAutoCalculation { get; set; }
        public bool IsRubric { get; set; }
        public bool HasFutureTestSession { get; set; }
    }
}