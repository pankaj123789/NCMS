using System;
using System.Collections.Generic;
using System.Linq;
using F1Solutions.Naati.Common.Contracts.Dal.Enum;
using F1Solutions.Naati.Common.Contracts.Security;
using Ncms.Contracts.Models;

namespace Ncms.Bl.ApplicationActions
{
    public class CredentialRequestPassAssessmentAction : CredentialRequestStateAction
    {
        private bool HasTest => CredentialRequestModel.CredentialType.CredentialApplicationTypeCredentialTypes.FirstOrDefault(c => c.CredentialApplicationTypeId == ApplicationModel.ApplicationType.Id)?.HasTest == true;

        private bool HasTestFee => CredentialRequestModel.CredentialType.CredentialApplicationTypeCredentialTypes.FirstOrDefault(c => c.CredentialApplicationTypeId == ApplicationModel.ApplicationType.Id)?.HasTestFee == true;

        protected override CredentialRequestStatusTypeName[] CredentialRequestEntryStates => new[] { CredentialRequestStatusTypeName.BeingAssessed, CredentialRequestStatusTypeName.Pending };


        protected override SecurityNounName? RequiredSecurityNoun => SecurityNounName.CredentialRequest;
        protected override SecurityVerbName? RequiredSecurityVerb => SecurityVerbName.Assess;

        protected override CredentialRequestStatusTypeName CredentialRequestExitState
        {
            get
            {
                if (HasTest && HasTestFee)
                {
                    return CredentialRequestStatusTypeName.EligibleForTesting;
                }
                if (HasTest && !HasTestFee)
                {
                    return CredentialRequestStatusTypeName.TestAccepted;
                }
                return CredentialRequestStatusTypeName.AssessmentComplete;
            }
        }

        protected override IList<Action> Preconditions => new List<Action>
                                                          {
                                                              ValidateEntryState,
                                                              ValidateUserPermissions,
                                                              ValidateApplicationState,
                                                              ValidateMandatoryFields,
                                                              ValidateMandatoryDocuments,
                                                              ValidateMandatoryPersonFields,
                                                              ValidateCredentialRequestInvoices
                                                          };

        protected override IList<Action> SystemActions => new List<Action>
                                                          {
                                                              ClearOwner,
                                                              CreateNote,
                                                              SetExitState,
                                                              CreateTest,
                                                          };

        private void CreateTest()
        {
            if (HasTest)
            {
                // todo for Jan/Febish - UC5000 BR14
                // Create a new Test with status Eligible for Testing
            }
        }

        public override IList<EmailTemplateModel> GetEmailTemplates()
        {
            // this is a horrible hack that will probably bite us later.
            if (!HasTest)
            {
                return new List<EmailTemplateModel>();
            }

            return base.GetEmailTemplates();
        }
    }
}