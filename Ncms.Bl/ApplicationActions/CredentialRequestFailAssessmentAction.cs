using System;
using System.Collections.Generic;
using F1Solutions.Naati.Common.Contracts.Dal;
using F1Solutions.Naati.Common.Contracts.Dal.Enum;
using F1Solutions.Naati.Common.Contracts.Security;
using Ncms.Contracts;

namespace Ncms.Bl.ApplicationActions
{
    public class CredentialRequestFailAssessmentAction : CredentialRequestStateAction
    {
        private ICredentialService CredentialService => ServiceLocatorInstance.Resolve<ICredentialService>();


        protected override SecurityNounName? RequiredSecurityNoun => SecurityNounName.CredentialRequest;
        protected override SecurityVerbName? RequiredSecurityVerb => SecurityVerbName.Assess;
        protected override CredentialRequestStatusTypeName[] CredentialRequestEntryStates
            => new[] { CredentialRequestStatusTypeName.BeingAssessed, CredentialRequestStatusTypeName.Pending };

        protected override CredentialRequestStatusTypeName CredentialRequestExitState => CredentialRequestStatusTypeName.AssessmentFailed;

        protected override IList<Action> Preconditions => new List<Action>
                                                          {
                                                              ValidateEntryState,
                                                              ValidateUserPermissions,
                                                              ValidateApplicationState,
                                                              ValidateMandatoryFields,
                                                          };

        protected override IList<Action> SystemActions => new List<Action>
                                                          {
                                                              ClearOwner,
                                                              CreateNote,
                                                              SetApplicationStatus,
                                                              SetExitState,
                                                          };

        protected override void GetEmailTokens(Dictionary<string, string> tokenDictionary)
        {
            // this token used for recertification applications
            if (ApplicationModel.ApplicationInfo.CertificationPeriodId > 0)
            {
                var response = CredentialService.GetCertificationPeriod(ApplicationModel.ApplicationInfo.CertificationPeriodId);
                tokenDictionary[TokenReplacementService.GetTokenNameFor(TokenReplacementField.CredentialExpiryDate)] = response.Data?.EndDate.ToShortDateString();
            }
        }
    }
}