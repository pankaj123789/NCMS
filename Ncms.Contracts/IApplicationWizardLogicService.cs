using System.Collections.Generic;
using F1Solutions.Naati.Common.Contracts.Bl.DTO;
using F1Solutions.Naati.Common.Contracts.Dal.Enum;
using Ncms.Contracts.Models;
using Ncms.Contracts.Models.Application;
using Ncms.Contracts.Models.System;
using Ncms.Contracts.Models.Test;

namespace Ncms.Contracts
{
    public interface IApplicationWizardLogicService
    {
        GenericResponse<NoteFieldRules> GetNoteFieldRules(int actionId, int applicationId);
        GenericResponse<NoteFieldRules> GetNoteFieldRulesByApplicationTypeId(int actionId, int applicationTypeId);
        GenericResponse<NoteFieldRules> GetNoteFieldRulesForAllocateTestSessionAction(int applicationTypeId);
        GenericResponse<IEnumerable<SystemActionNameModel>> GetValidActions(int applicationId);
        GenericResponse<IEnumerable<ApplicationWizardSteps>> GetWizardSteps(int applicationStatusId, int actionId, int applicationId, int applicationTypeId, int credentialTypeId, int credentialRequestId);
        GenericResponse<IEnumerable<CredentialRequestWizardSteps>> GetCredentialRequestWizardSteps(int actionId, int applicationTypeId, int credentialRequestStatusTypeId);
        GenericResponse<IEnumerable<TestSessionWizardSteps>> GetTestSessionWizardSteps(int? testSessionId);
        GenericResponse<IEnumerable<AllocateRolePlayersWizardSteps>> GetAllocateRolePlayersSteps(int? testSessionId);
        IList<SystemActionNameModel> GetValidCredentialRequestActions(CredentialRequestStatusTypeName credentialRequestStatus, int credentialRequestId);
        IList<SystemActionNameModel> GetValidCredentialRequestActions(int credentialRequestStatus, int credentialRequestId);
        IList<SystemActionNameModel> GetValidTestSittingActions(int testSittingId);
        IList<SystemActionNameModel> GetValidCredentialRequestSummaryActions(int credentialRequestStatusId);
        IList<SystemActionNameModel> GetValidRolePlayerActions(RolePlayerStatusTypeName roleplayerStatus);

        GenericResponse<IEnumerable<TestMaterialWizardSteps>> GetTestMaterialWizardSteps(TestMaterialBulkAssignmentRequest request);

        GenericResponse<string> GetConfirmationMessage(ApplicationWizardSteps step, int applicationId, int credentialRequestId);
        GenericResponse<bool> CanShowSessionStep(SessionStepValidationModel request);
        GenericResponse<string> GetStepMessage(SystemActionTypeName action);
    }
}