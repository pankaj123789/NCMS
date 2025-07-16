using System;
using System.Collections.Generic;
using F1Solutions.Naati.Common.Contracts.Bl.DTO;
using F1Solutions.Naati.Common.Contracts.Dal.Response;
using Ncms.Contracts.Models;
using Ncms.Contracts.Models.Common;
using Ncms.Contracts.Models.File;
using Ncms.Contracts.Models.Job;
using Ncms.Contracts.Models.TestResult;

namespace Ncms.Contracts
{
    public interface ITestResultService
    {
        GenericResponse<int> UpdateTestResult(TestResultModel testResult);
        SpecificationResponseModel Specification(int testResultId, bool userOriginalMark);
        List<Dictionary<string, object>> CalculateTestComponentResult(List<Dictionary<string, object>> dataSet);
        GetMarksResponseModel GetMarks(GetMarksRequestModel request);
        GetMarksResponseModel GetMarks(int credentialRequestId);
        void SaveMarks(SaveMarksRequestModel request);
        GenericResponse<TestResultModel> GetTestResult(int testResultId);
        GenericResponse<IEnumerable<TestSittingDocumentModel>> GetTestDocuments(int testSittingId);
        GenericResponse<IEnumerable<ExaminerDocumentModel>> GetExaminerDocuments(int testSittingId);
        GenericResponse<IEnumerable<TestSittingDocumentModel>> GetAssetDocuments(int testSittingId);
        void DeleteDocument(int testSittingDocumentId);
        FileModel GetDocumentsAsZip(int testResultId);
        FileResponseModel GetDocument(int testSittingDocumentId);
        int CreateOrUpdateDocument(CreateOrReplaceTestSittingDocumentModel request);

        void UpdateDueDate(UpdateDueDateRequestModel request);

        GenericResponse<IEnumerable<string>> ValidateTestResult(TestResultModel testResult);

        GenericResponse<bool> UpdateAutomaticIssuingExaminer(int testSittingId, bool? automaticIssuingExaminer);
        bool? GetAutomaticIssuingExaminer(int testSittingId);
        bool GetTestSpecificationAutomaticIssuingByTestSittingId(int testSittingId);
    }

    public class TestResultModel
    {
        public int TestResultId { get; set; }
        public int CurrentJobId { get; set; }

        public int ResultTypeId { get; set; }
        public bool ResultChecked { get; set; }
        public bool AllowCalculate { get; set; }
        public bool IncludePreviousMarks { get; set; }
        public string Comments1 { get; set; }
        public string Comments2 { get; set; }
        public string CommentsEthics { get; set; }
        public string CommentsGeneral { get; set; }
        public DateTime? ProcessedDate { get; set; }
        public DateTime? SatDate { get; set; }
        public bool ThirdExaminerRequired { get; set; }
        public IEnumerable<int> FailureReasonIds { get; set; }
        public bool AllowIssue { get; set; }

        public DateTime? DueDate { get; set; }

        public bool EligibleForConcededPass { get; set; }
        public bool EligibleForSupplementary { get; set; }
        public bool AutomaticIssuingExaminer { get; set; }
    }
}
