using System.Collections.Generic;
using F1Solutions.Naati.Common.Contracts.Dal.DTO;
using F1Solutions.Naati.Common.Contracts.Dal.Request;
using F1Solutions.Naati.Common.Contracts.Dal.Response;

namespace F1Solutions.Naati.Common.Contracts.Dal.QueryServices
{

    public interface ITestSessionQueryService : IQueryService
    {
        
        GetTestSessionDetailsResponse GetTestSessionDetailsById(int testSessionId);

        //GetTestSessionsForTelevicDownloadResponse GetTestSessionsForTelevicDownload(GetTestSessionsForTelevicDownloadRequest request);

        AvailableTestSessionsResponse ApiGetTestSessionAvailability(ApiSessionAvailabilityRequest request);
       
        void MarkAsCompleteTestSession(int testSessionId);

        ServiceResponse<IEnumerable<TestSittingHistoryItemDto>> GetTestSittings(int credentialRequestId);

        void ReopenTestSession(int testSessionId);
        
        GetTestSessionResponse GetTestSessionById(int testSessionId);
        
        GetTestSessionApplicantsResponse GetApplicantsById(int testSessionId, bool includeRejected);
        
        void UpdateTestSession(UpdateTestSessionRequest request);
        
        GetTestSessionsResponse GetActiveTestSessions(GetActiveTestSessionRequest request);

        SetAllowAvailabilityNoticeResponse DisableAllowAvailabilityNotice(HashSet<int> ids);

        CreateOrUpdateTestSessionResponse CreateOrUpdateTestSession(CreateOrUpdateTestSessionRequest request);
        
        TestSessionSearchResponse Search(GetTestSessionSearchRequest request);
        
        GetTestSessionSkillsResponse GetSkillAttendeesCount(int testSessionId);
        
        TestSessionSkillResponse GetTestSessionSkillsForTestSession(int testSessionId);

        bool CheckCapacityTestSession(
            int credentialTypeId,
            int skillId,
            int testSessionId,
            int credentialStatusTypeId,
            int preferredTestLocationId);

        AvailableTestSessionsResponse GetAvailableTestSessionAfterBacklogAssignment(int credentialRequestId);
        
        bool AllTestSittingsAreSat(int testSessionId);
        
        string AllTestSittingsMarkCompleteRequirements(int testSessionId);
        
        GetTestSessionRolePlayerResponse GetTestSessionRolePlayers(GetTestSessionRolePlayerRequest request);
        
        GetRolePlayerResponse GetAvailableRolePlayers(GetRolePlayerRequest request);
        
        IEnumerable<TestSessionSpecificationDetailsDto> GetTestSpecificationDetails(TestSpecificationDetailsRequest request);
        
        IEnumerable<TestSessionSkillDetailsDto> GetTestSessionLanguageDetails(TestSessionLanguageDetailsRequest request);
        
        SkillDetailsDto GetSkillDetails(int skillId);
        
        UpsertRolePlayerResponse UpsertTestSessionRolePlayer(UpsertSessionRolePlayerRequest request);
        
        ServiceResponse<TestSessionRolePlayerDto> GetTestSessionRolePlayer(int testSessionRolePlayerId);
        
        ServiceResponse<IEnumerable<TestSessionRolePlayerDto>> GetTestSessionRolePlayerBySessionId(int testSessionId);
        
        PersonTestSessionRolePlaysResponse GetPersonRolePlays(int naatiNumber);
        
        ServiceResponse<IEnumerable<TestSessionRolePlayerDto>> GetSessionRolePlayers(GetRolePlaySessionRequest request);

        IEnumerable<TestSessionAvailabilityObject> GetTestSessionAvailabilityObjects();

        void UpdateTestSitting(UpdateTestSittingRequest request);

        /// <summary>
        /// return the email recipients from SystemValue for the Material Reminder email
        /// </summary>
        /// <returns>Email recipients</returns>
        ServiceResponse<List<string>> TestSittingsWithoutMaterialReminderEmailAddresses();
        ServiceResponse<int> TestSittingsWithoutMaterialReminderEmailAddresses(string emailAddress);

        bool IsRolePlayerAvailable();

        //IEnumerable<TestSessionSummaryDto> GetTestSittingsWithUsers(int hours);
        //void MarkAsSynced(int testSessionId);
        //IEnumerable<TestSessionSummaryDto> GetTestSittingsWithUsersProj(int hours);
    }
}
