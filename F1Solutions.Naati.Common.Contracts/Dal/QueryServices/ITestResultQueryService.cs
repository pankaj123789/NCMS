using System.Collections.Generic;
using F1Solutions.Naati.Common.Contracts.Bl.DTO;
using F1Solutions.Naati.Common.Contracts.Dal.DTO;
using F1Solutions.Naati.Common.Contracts.Dal.Request;
using F1Solutions.Naati.Common.Contracts.Dal.Response;

namespace F1Solutions.Naati.Common.Contracts.Dal.QueryServices
{
    
    public interface ITestResultQueryService : IQueryService
    {
        
        GetTestResultResponse GetTestResultById(int testResultId);

        
        SaveTestResultResponse UpdateTestResult(UpdateTestResultRequest request);

        
        GetDocumentsResponse GetDocuments(GetDocumentsRequest request);

        
        DeleteDocumentResponse DeleteDocument(DeleteDocumentRequest request);

        
        GetTestSittingDocumentResponse GetDocument(GetDocumentRequest request);

        
        CreateOrUpdateDocumentResponse CreateOrUpdateDocument(CreateOrUpdateDocumentRequest request);

        
        JobExaminerMarkingDto GetJobExaminerMarkingResult(JobExaminerMarkingResultRequest request);

        
        JobExaminerMarkingDto GetExaminerMarkingResult(int naatiNumber, int testResultId);

        
        TestResultMarkingDto GetTestResultMarkingResult(TestMarkingResultRequest request);

        
		SaveExaminerMarkingResponse SaveJobExaminerMarkingResult(SaveExaminerMarkingRequest request);

         
         SaveTestResultResponse SaveJobExaminerMarkingResultWithNaatiNumber(SaveExaminerMarkingRequest request);

        
		SaveTestMarkingResponse SaveTestMarkingResult(SaveTestMarkingRequest request);

        
        ServiceResponse<RubricPassRulesDto> GetRubricRules(int testSpecificationId);

        
        ServiceResponse<IEnumerable<TestResultInfoDto>> GetPendingTestsToIssueResult();

        
        ServiceResponse<RubricPassRulesDto> GetRubricRulesForTestSitting(int testSittingId);

        
        ServiceResponse<IEnumerable<JobExaminerMarkingDto>> GetAllExaminerMarkingResults(int testResultId, bool includedOnly);
        ServiceResponse<bool> UpdateTestResultAutomaticIssuingExaminer(int testSittingId, bool? automaticIssuingExaminer);
        bool? GetAutomaticIssuingExaminer(int testSittingId);
        bool GetTestSpecificationAutomaticIssuingByTestSittingId(int testSittingId);
        /// <summary>
        /// Checks whether the credential request for the given id is a ccl practice test or not
        /// </summary>
        /// <param name="credentialRequestId"></param>
        /// <returns>true if the credential type is ccl practice, false if not</returns>
        GenericResponse<bool> IsCclPracticeTest(int credentialRequestId);
        /// <summary>
        /// Updates the test result for the given ID so that it is valid for the issue practice results action
        /// </summary>
        /// <param name="testResultId"></param>
        /// <returns>returned bool is a placeholder as no data is required to be passed back, just a response.</returns>
        GenericResponse<bool> UpdateTestResultToIssuePracticeResult(int testResultId);
    }
}
