using F1Solutions.Naati.Common.Contracts.Bl.DTO;
using F1Solutions.Naati.Common.Contracts.Dal.Request;
using F1Solutions.Naati.Common.Contracts.Dal.Response;

namespace F1Solutions.Naati.Common.Contracts.Dal.QueryServices
{
    
    public interface ITestQueryService : IQueryService
    {
        
        TestSearchResponse Search(GetTestSearchRequest request);

        
        void UpdateDueDate(UpdateDueDateRequest request);

        
        bool TestReadyForAssets(int testAttendanceId);

        
        SaveMarksResponse SaveMarks(SaveMarksRequest request);

        
        GetApplicationIdResponse GetApplicationId(GetApplicationIdRequest request);

        
        GetVenueResponse GetVenueById(int venueId);

        
        GetVenuesResponse GetVenues(GetVenuesRequest request);

        
        GetTestSummaryResponse GetTestSummary(GetTestSummaryRequest request);

        
        AddOrUpdateTestExaminersResponse AddTestExaminers(AddTestExaminersRequest request);

        
        AddOrUpdateTestExaminersResponse UpdateJobExaminers(UpdateJobExaminersRequest request);

        
        GetTestSittingResponse GetTestSitting(GetTestSittingRequest request);

        
        GetTestSittingResponse GetTestSittingByCredentailRequestId(GetTestSittingRequest request);

        
		GetTestResultsResponse GetTestResults(GetTestResultsRequest getTestResultsRequest);

        
        GetTestSummaryResponse GetTestSummaryFromTestResultId(int testResultId);
        GetVenuesResponse GetVenuesShowingActive(GetVenuesRequest request);

        GenericResponse<FeedbackDalResponse> GetFeedback(int? testAttendanceId);
    }
}

