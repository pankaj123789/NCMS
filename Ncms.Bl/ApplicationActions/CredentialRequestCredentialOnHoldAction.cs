using System;
using System.Collections.Generic;
using F1Solutions.Global.Common;
using F1Solutions.Naati.Common.Contracts.Dal;
using F1Solutions.Naati.Common.Contracts.Dal.Enum;
using F1Solutions.Naati.Common.Contracts.Security;
using Ncms.Contracts.Models.Application;

namespace Ncms.Bl.ApplicationActions
{
    public class CredentialRequestCredentialOnHoldAction : CredentialRequestStateAction
    {

        protected override SecurityNounName? RequiredSecurityNoun => SecurityNounName.Credential;
        protected override SecurityVerbName? RequiredSecurityVerb => SecurityVerbName.Issue;
        protected override CredentialRequestStatusTypeName[] CredentialRequestEntryStates => new[] { CredentialRequestStatusTypeName.AssessmentComplete, CredentialRequestStatusTypeName.IssuedPassResult, CredentialRequestStatusTypeName.ToBeIssued, CredentialRequestStatusTypeName.CredentialOnHold };

        protected override CredentialRequestStatusTypeName CredentialRequestExitState => CredentialRequestStatusTypeName.CredentialOnHold;

        protected bool RollbackRequired { get; set; }

        protected override IList<Action> Preconditions => new List<Action>
                                                          {
                                                              ValidateEntryState,
                                                              ValidateUserPermissions,
                                                              ValidateApplicationState,
                                                          };

        protected override IList<Action> SystemActions => new List<Action>
                                                          {
                                                              CreateNote,
                                                              SetExitState,
                                                          };


        protected virtual void ValidateExistingCredential(CredentialModel existingCredential)
        {
            if (existingCredential.CertificationPeriod.Id == Output.PendingCredential.CertificationPeriod.Id)
            {
                throw new UserFriendlySamException(Naati.Resources.Application.InvalidRecertificationCertificationPeriod);
            }
        }

        protected virtual int GetExistingCredentialId()
        {
            // if this is a recertification application, we won't create a new credential, we'll just update the existing one
            // this needs to be done before updating the application, so the recertification status is still BeingAssessed
            if (ApplicationModel.ApplicationType.Category == CredentialApplicationTypeCategoryName.Recertification)
            {
                var existingCred = ApplicationService.GetCredentialBeingRecertified(ApplicationModel.ApplicationInfo.NaatiNumber, Output.PendingCredential.SkillId,
                    Output.PendingCredential.CredentialTypeId);
                existingCred.NotNull("Can't find existing credential being placed on hold");
                ValidateExistingCredential(existingCred);
                return existingCred.Id;

            }

            // not required for other types of application as we will create a new credential
            return 0;
        }

        public override void SaveChanges()
        {
            base.SaveChanges();
        }


    }
}