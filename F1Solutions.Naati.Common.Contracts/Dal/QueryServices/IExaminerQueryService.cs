using System.Collections.Generic;
using F1Solutions.Naati.Common.Contracts.Dal.DTO;
using F1Solutions.Naati.Common.Contracts.Dal.Request;
using F1Solutions.Naati.Common.Contracts.Dal.Response;
using F1Solutions.Naati.Common.Contracts.Dal.Services;

namespace F1Solutions.Naati.Common.Contracts.Dal.QueryServices
{
    
    public interface IExaminerQueryService : IQueryService
    {
        
        GetExaminersResponse GetExaminers(GetExaminersRequest request);

        
        GetMarksResponse GetMarks(GetMarksRequest request);

        
        SaveMarksResponse SaveMarks(SaveExaminerMarksRequest request);

        
        UpdateCountMarksResponse UpdateCountMarks(UpdateCountMarksRequest request);

        
        GetJobExaminersResponse GetJobExaminers(GetJobExaminersRequest request);
      
        
        GetJobExaminerResponse GetJobExaminerById(GetJobExaminerRequest request);

        
        GetPersonExaminersResponse GetActiveExaminersByLanguageAndCredentialType(GetPersonExaminersByLanguageRequest request);

        
        void RemoveExaminers(RemoveExaminersRequest request);

        
        ServiceResponse<IEnumerable<RolePlayerDto>> GetRolePlayersForTestSitting(int testSittingId);

        
        bool HasPaidReviewExaminers(int credentialRequestId);

        
        bool CanWithdrawApplicationUnderPaidReview(int credentialRequestId);
    }
}
