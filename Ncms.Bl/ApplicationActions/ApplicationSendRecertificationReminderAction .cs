using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using F1Solutions.Naati.Common.Contracts.Dal;
using F1Solutions.Naati.Common.Contracts.Dal.Enum;
using F1Solutions.Naati.Common.Contracts.Security;
using Ncms.Contracts;
using Ncms.Contracts.Models.Application;

namespace Ncms.Bl.ApplicationActions
{
    public class ApplicationSendRecertificationReminderAction : ApplicationStateAction
    {
        protected override SecurityNounName? RequiredSecurityNoun => SecurityNounName.Email;
        protected override SecurityVerbName? RequiredSecurityVerb => SecurityVerbName.Send;

        protected override CredentialApplicationStatusTypeName[] ApplicationEntryStates => new[] {  CredentialApplicationStatusTypeName.Completed, CredentialApplicationStatusTypeName.InProgress };
        protected override CredentialApplicationStatusTypeName ApplicationExitState => GetApplicationExitState();

        protected override IList<Action> Preconditions => new List<Action>
        {
            ValidateEntryState,
            ValidateUserPermissions,
            //TFS165774 - Removed this validation step as per details in bug
            //ValidateCredentialRequests
        };

        protected override IList<Action> SystemActions => new List<Action>
        {
            CreateNote
        };

        private CredentialApplicationStatusTypeName GetApplicationExitState()
        {
            return (CredentialApplicationStatusTypeName) ActionModel.ApplicationInfo.ApplicationStatusTypeId;
        }

        protected override void GetEmailTokens(Dictionary<string, string> tokenDictionary)
        {
            var availableRecertifyCredentials = ApplicationService.GetAvailableCredentialsToRecertify(ApplicationModel.ApplicationInfo.ApplicationId);

            var skillTypes = ApplicationService.GetCredentialSkills(availableRecertifyCredentials).Data.ToList();
            var eligibleRecertificationCredentials = string.Join(", ", skillTypes.Select(s => s.Externalname + "-" + s.DisplayName));

            var certificationPeriod = ApplicationService.GetCertificationPeriod(ApplicationModel.ApplicationInfo.ApplicationId).Data;
            var certificationPeriodHtml = certificationPeriod.EndDate.ToShortDateString();


            tokenDictionary.Add(TokenReplacementService.GetTokenNameFor(TokenReplacementField.EligibleRecertificationCredentials), eligibleRecertificationCredentials);
            tokenDictionary.Add(TokenReplacementService.GetTokenNameFor(TokenReplacementField.CertificationPeriodEndDate), certificationPeriodHtml);

            base.GetEmailTokens(tokenDictionary);
        }

        protected override string GetNote()
        {
            return Naati.Resources.Application.RecertificationReminderSent; 
        }

    }
}
