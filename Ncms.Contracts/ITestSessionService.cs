using System;
using System.Collections.Generic;
using F1Solutions.Naati.Common.Contracts.Bl.Attributes;
using F1Solutions.Naati.Common.Contracts.Bl.DTO;
using F1Solutions.Naati.Common.Contracts.Dal.Enum;
using F1Solutions.Naati.Common.Contracts.Dal.Request;
using F1Solutions.Naati.Common.Contracts.Dal.Response;
using Ncms.Contracts.Models;
using Ncms.Contracts.Models.Application;
using Ncms.Contracts.Models.CredentialRequest;
using Ncms.Contracts.Models.RolePlayer;
using Ncms.Contracts.Models.System;
using Ncms.Contracts.Models.Test;

namespace Ncms.Contracts
{
    public interface ITestSessionService
    {
        GenericResponse<IEnumerable<TestSessionSearchResultModel>> List(TestSessionSearchRequest request);
        GenericResponse<TestSessionDetails> GetTestSessionDetailsById(int testSessionId);
        GenericResponse<string> MarkAsCompleteTestSession(int testSessionId);

        void ReopenTestSession(int testSessionId);
        GenericResponse<IEnumerable<ApplicantModel>> GetApplicantsByTestSessionId(int testSessionId, bool includeRejected);
        GenericResponse<TestSessionModel> GetTestSessionById(int testSessionId);

        void UpdateTestSession(TestSessionModel testSessionModel);

        GenericResponse<IEnumerable<TestSessionItemModel>> GetActiveTestSessions(CredentialRequestBulkActionRequest request);

        GenericResponse<int> CreateOrUpdateTestSession(TestSessionRequestModel testSessionModel);
        GenericResponse<IEnumerable<TestSessionSkillModel>> GetTestSessionSkills(int testSessionId, int credentialTypeId);

        GenericResponse<IEnumerable<LookupTypeModel>> GetSelectedTestSessionSkills(int testSessionId);

        GenericResponse<IEnumerable<object>> ValidateTestSession(TestSessionRequestModel testSessionModel);
        GenericResponse<IEnumerable<object>> ValidateTestSession(TestSessionDetails testSessionModel);

        GenericResponse<IEnumerable<object>> ValidateTestSessionRehearsal(TestSessionDetails testSessionModel);
        GenericResponse<TestSessionSkillValidationModel> ValidateTestSessionSkills(TestSessionSkillValidationRequest testSessionModel);
        GenericResponse<IEnumerable<object>> ValidateNewTestSessionApplicants(CredentialRequestBulkActionRequest testSessionModel);
        GenericResponse<GetTestSessionSkillsResponse> GetSkillAttendeesCount(int testSessionId);

        GenericResponse<IEnumerable<CredentialRequestApplicantModel>> GetCredentialRequestApplicants(TestSessionRequest request);
        GenericResponse<bool> CheckCapacityTestSession(int credentialTypeId,
            int skillId,
            int testSessionId,
            int credentialStatusTypeId,
            int preferredTestLocationId);
        GenericResponse<IEnumerable<ValidationMessage>> ValidatestTestSitting(int testSessionId);
        BusinessServiceResponse CreateOrUpdateTestSession(TestSessionBulkActionWizardModel wizardModel);
        GenericResponse<IEnumerable<EmailMessageModel>> GetCreateOrUpdateTestSessionEmailPreview(TestSessionBulkActionWizardModel wizardModel);

        GenericResponse<IEnumerable<EmailMessageModel>> GetAllocateRolePlayerEmailPreview(RolePlayerBulkAssignmentWizard request);
        TestSessionCheckOptionData GetCheckOptionMessage(TestSessionDetails testSessionDetails);
        TestSessionCheckOptionData GetAllocateRolePlayersCheckOptionMessage(TestSessionDetails testSessionDetails);
        
        GenericResponse<IEnumerable<TestSessionRolePlayerAvailabilityModel>> GetAvailableAndExistingRolePlayers(RolePlayerAssignmentRequest request);

        GenericResponse<RolePlayerAllocationDetails> GetRolePlayerAllocationDetails(RolePlayerAssignmentRequest request);
        GenericResponse<RolePlayerAllocationDetails> GetRolePlayerAllocationTasks(int testSessionId, int skillId);

        GenericResponse<IList<TestSessionRolePlayerModel>> GetPersonRolePlays(int naatiNumber);

        GetTestSessionSpecificationResponse GetTestSpecificationDetails(RolePlayerAssignmentRequest request);
        GenericResponse<IEnumerable<object>> ValidateTestSpecification(RolePlayerAssignmentRequest request);

        GetTestSessionLanguageResponse GetLanguageDetails(RolePlayerAssignmentRequest request);
        GenericResponse<IEnumerable<object>> ValidateLanguage(RolePlayerAssignmentRequest request);

        GenericResponse<IEnumerable<object>> ValidateRolePlayersAllocation(RolePlayerAssignmentRequest request);

        BusinessServiceResponse ExecuteRolePlayersAction(RolePlayerBulkAssignmentWizard wizardModel);

        GenericResponse<UpsertRolePlayerResultModel> UpsertTestSessionRolePlayer(UpsertSessionRolePlayerRequest request);

        GenericResponse<RolePlayerAllocationDetails> GetRolePlayerAllocationDetails(RolePlayerAllocationDetailsRequest request);

        GenericResponse<IEnumerable<TestSessionRolePlayerAvailabilityModel>> GetTestSessionRolePlayers(TestSessionRolePlayersRequest request);

        BusinessServiceResponse UpdateRolePlayers(UpdateRolePLayersRequest request);

        BusinessServiceResponse PerformRolePlayerAction(int testSessionRolePlayerId,RolePlayerUpdateWizardModel wizardModel);
        IList<SystemActionNameModel> GetValidRolePlayerActions(SystemActionTypeName lastSystemActionTypeId);
        GenericResponse<IEnumerable<TestSpecificationLookupTypeModel>> GetTestSpecificationsByCredentialTypeId(int credentialTypeId);
        BusinessServiceResponse AllocatePastTestSession(AllocateTestSessionRequest allocateTestSessionRequest);
        GenericResponse<IEnumerable<TestSessionSkillModel>> GetSelectedTestSessionSkillDetails(int testSessionId);

    }

    public class TestSessionRolePlayersRequest
    {
        public int TestSessionId { get; set; }
        public int SkillId { get; set; }
    }

    public class UpdateRolePLayersRequest
    {
        public IEnumerable<RolePlayerAction> RolePlayerActions { get; set; }
    }

    public class RolePlayerAction
    {
        public int TestSessionRolePlayerId { get; set; }
        public int SystemActionTypeId { get; set; }
    }
    public class TestSessionCheckOptionData
    {
        public bool NotifyApplicantsChecked { get; set; }
        public bool NotifyRolePlayersChecked { get; set; }
        public string NotifyApplicantsMessage { get; set; }
        public string NotifyRolePlayersMessage { get; set; }

        public string OnDisableApplicantsMessage { get; set; }
        public string OnDisableRolePlayersMessage { get; set; }
        public bool ReadOnly { get; set; }
        public bool IsRolePlayerAvailable { get; set; }
    }

    public class TestSessionRolePlayerModel
    {
        public int TestSessionRolePlayerId { get; set; }
        public int TestSessionId { get; set; }
        public int RolePlayerId { get; set; }
        public bool Attended { get; set; }
        public bool Rehearsed { get; set; }
        public bool Rejected { get; set; }
        public int RolePlayerStatusId { get; set; }
        public DateTime StatusChangeDate { get; set; }
        public int StatusChangeUserId { get; set; }
        public DateTime? RehearsalDate { get; set; }//from test session
        public DateTime TestSessionDate { get; set; }//from test session
        public string TestSessionName { get; set; }//from test session
        public string TestSessionLocation { get; set; }//from roleplayertestlocation ?
        public string RolePlayerStatus { get; set; }//similar to id above except the name
        public string RolePlayerStatusDisplayName { get; set; }//similar to id above except the name

        public ObjectStatusTypeName ObjectStatus { get; set; }

        public IList<TestSessionRolePlayerDetailModel> Details { get; set; }
    }

    public class TestSessionRolePlayerDetailModel
    {
        public int TestSessionRolePlayerDetailId { get; set; }
        public int SkillId { get; set; }
        public int TestComponentId { get; set; }
        public int LanguageId { get; set; }
        public int RolePlayerRoleTypeId { get; set; }

        public ObjectStatusTypeName ObjectStatus { get; set; }
        public string SkillName { get; set; }
        public string TestComponentName { get; set; }//Task
        public string RolePlayerRoleTypeName { get; set; }//Role Play
    }

    public class RolePlayerAllocationDetailsRequest
    {
        public int TestSpecificationId { get; set; }
        public int SkillId { get; set; }
    }
    public class RolePlayerAllocationDetails
    {
       public SkillDetailsModel Skill { get; set; }
       public IEnumerable<TestTaskModel> Tasks { get; set; }
    }

    public class SkillDetailsModel
    {
        public int SkillId { get; set; }
        public int Language1Id { get; set; }
        public int Language2Id { get; set; }

        public string SkillDisplayName { get; set; }
        public string Language1DisplayName { get; set; }
        public string Language2DisplayName { get; set; }
    }
    public class GetTestSessionSpecificationResponse
    {
        public IEnumerable<TestSessionSpecificationDetailsModel> Results { get; set; }
    }

    public class GetTestSessionLanguageResponse
    {
        public IEnumerable<TestSessionLanguageDetailsModel> Results { get; set; }
    }

    public class TestSessionSpecificationDetailsModel
    {
        public int Id { get; set; }
        public string Description { get; set; }
        public int NumberOfTasksWithRequiredRolePlayers { get; set; }
        public int TasksWithoutRolePlayers { get; set; }
    }

    public class TestSessionLanguageDetailsModel
    {
        public int SkillId { get; set; }
        public string Description { get; set; }
        public int NumberOfTasksWithRequiredRolePlayers { get; set; }
        public int TasksWithoutRolePlayers { get; set; }
    }
    public class ValidationMessage
    {
        public int IssueNumber { get; set; }
        public string Message { get; set; }
        public int TotalFound { get; set; }
        public IEnumerable<string> Details { get; set; }
    }

    public enum TestSessionWizardSteps
    {
        Details = 1,
        Skills = 2,
        MatchingApplicants = 3,
        Notes = 4,
        PreviewEmail = 5,
        CheckOption = 6,
        RehearsalDetails = 7
    }

    public enum AllocateRolePlayersWizardSteps
    {
        TestSpecification = 1,
        Skill = 2,
        AllocateRolePlayers = 3,
        EmailPreview = 4,
        Notes = 5,
		SendEmail = 6,
	}

    public class TestSessionRolePlayerAvailabilityModel : RolePlayerAvailabilityModel
    {
        public int TestSessionRolePlayerId { get; set; }
        public bool Attended { get; set; }
        public bool Rehearsed { get; set; }
        public bool Rejected { get; set; }

        [LookupDisplay(LookupType.RolePlayerStatusType, "RolePlayerStatus")]
        public int RolePlayerStatusId { get; set; }
        public int LastSystemActionTypeId { get; set; }

        

        public IList<TestSessionRolePlayerTaskModel> Details { get; set; }

    }

    public class TestSessionRolePlayerTaskModel
    {
        public int LanguageId { get; set; }
        public int TestComponentId { get; set; }
        public int RolePlayerRoleTypeId { get; set; }
    }

    public class RolePlayerAvailabilityModel
    {
        public int RolePlayerId { get; set; }
        public int CustomerNo { get; set; }
        public int? LastAttendedTestSessionId { get; set; }
        public DateTime? LastAttendedTestSessionDateTime { get; set; }

        public string Name { get; set; }
        public int SessionLimit { get; set; }
        public decimal? Rating { get; set; }
        public bool Senior { get; set; }

        public bool Available { get; set; }
        public bool IsInTestLocation { get; set; }

        public bool HasCapacity { get; set; }

        public string Gender { get; set; }

        public IList<int> LanguageIds { get; set; }
        public bool ReadOnly { get; set; }
        public int Age { get; set; }
        public IEnumerable<LookupTypeModel> AvailableTestLocations { get; set; }

        public int DisplayOrder { get; set; }
    }
}
