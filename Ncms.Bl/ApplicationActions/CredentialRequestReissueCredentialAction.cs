using System;
using System.Collections.Generic;
using System.Linq;
using F1Solutions.Naati.Common.Contracts.Dal;
using F1Solutions.Naati.Common.Contracts.Dal.Enum;
using F1Solutions.Naati.Common.Contracts.Security;
using Ncms.Contracts.Models.Application;

namespace Ncms.Bl.ApplicationActions
{
    public class CredentialRequestReissueCredentialAction : CredentialRequestIssueCredentialAction
    {

        protected override SecurityNounName? RequiredSecurityNoun => SecurityNounName.Credential;
        protected override SecurityVerbName? RequiredSecurityVerb => SecurityVerbName.Issue;

        protected override CredentialRequestStatusTypeName[] CredentialRequestEntryStates => new[] { CredentialRequestStatusTypeName.CertificationIssued };

        protected override CredentialRequestStatusTypeName CredentialRequestExitState => CredentialRequestStatusTypeName.CertificationIssued;

        protected override IList<Action> Preconditions => new List<Action>
                                                        {
                                                            ValidateEntryState,
                                                            ValidateUserPermissions,
                                                            ValidateMandatoryFields,
                                                            ValidateMandatoryDocuments,
                                                            ValidateMandatoryPersonFields,
                                                            ValidateCredentialStatus
                                                        };


         protected override IList<Action> SystemActions => new List<Action>
                                                          {
                                                              DeleteCredentialPreviewFiles,
                                                              ClearOwner,
                                                              CreatePendingEmailIfApplicable,
                                                              CreateCredential,
                                                              CreateNote,
                                                              CreatePersonNotes,
                                                              AssignPractitionNumber
                                                          };

        protected virtual void ValidateCredentialStatus()
        {
            var credentialRequest =
                ApplicationModel.CredentialRequests.FirstOrDefault(x => x.Id == WizardModel.CredentialRequestId);
            if (credentialRequest?.Credentials?.FirstOrDefault() == null)
            {
                throw new UserFriendlySamException(string.Format(Naati.Resources.Application.InvalidCredentialRequestMessage,WizardModel.CredentialRequestId));
            }
            var credentialStatusId = credentialRequest.Credentials.First().StatusId;

            if (credentialStatusId == (int)CredentialStatusTypeName.Terminated || credentialStatusId == (int)CredentialStatusTypeName.Expired)
            {
                throw new UserFriendlySamException(string.Format(Naati.Resources.Application.InvalidCredentialStatus, (CredentialStatusTypeName)credentialStatusId));
            }
        }
        protected override void ValidateExistingCredential(CredentialModel existingCredential)
        {
        }

        protected override string GetNote()
        {
            var dateFormat = "dd MM yyyy";
            if (Output.PendingCredential?.CertificationPeriod!= null)
            {
                return string.Format(Naati.Resources.Application.CredentialCertificationReissuedNote,
                    CredentialRequestModel.CredentialType.ExternalName,
                    CredentialRequestModel.Direction,
                    Output.PendingCredential?.CertificationPeriod.StartDate.ToString(dateFormat),
                    Output.PendingCredential?.CertificationPeriod.EndDate.ToString(dateFormat),
                    Output.PendingCredential.StartDate.ToString(dateFormat));
            }

            if (Output.PendingCredential != null)
            {
                var endDate = Output.PendingCredential.ExpiryDate.HasValue
                    ? string.Format(Naati.Resources.Application.CredentialExpiryDate,
                        Output.PendingCredential.ExpiryDate.Value.ToString(dateFormat))
                    : Naati.Resources.Application.CredentialNotExpiryDate;
                
                return string.Format(Naati.Resources.Application.CredentialReissuedNote,
                    CredentialRequestModel.CredentialType.ExternalName,
                    CredentialRequestModel.Direction,
                    Output.PendingCredential.StartDate.ToString(dateFormat),
                    endDate);
            }

            return string.Format(Naati.Resources.Application.CredentialRequestStateChangeNote,
                CredentialRequestModel.CredentialType.ExternalName,
                CredentialRequestModel.Direction,
                CredentialRequestModel.Status,
                CredentialRequestStatusTypes.Single(x => x.Id == (int)CredentialRequestExitState).DisplayName);
        }

        protected override IEnumerable<string> GetPersonNotes()
        {
            var notes = new List<string>();
            var dateFormat = "dd MM yyyy";
            notes.Add(GetNote());

            if (Output.PendingCredential?.CertificationPeriod != null 
                && Output.PendingCredential.CertificationPeriod.Id == 0)
            {
                notes.Add(string.Format(Naati.Resources.Application.NewCertificationPeriodNote,
                    Output.PendingCredential?.CertificationPeriod.StartDate.ToString(dateFormat),
                    Output.PendingCredential?.CertificationPeriod.EndDate.ToString(dateFormat)
                ));
            }
         
            return notes;
        }

        protected override int GetExistingCredentialId()
        {
            // not required when reissuing
            return 0;
        }

        protected override void ValidateUserPermissions()
        {
            // todo - mm - make better
            if (!UserService.HasPermission(SecurityNounName.Credential, SecurityVerbName.Issue))
            {
                throw new UserFriendlySamException(Naati.Resources.Server.UnauthorisedAccess);
            }
        }
       
    }
}
