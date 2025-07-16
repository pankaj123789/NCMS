using System;
using System.Collections.Generic;
using System.Linq;
using F1Solutions.Naati.Common.Contracts.Dal;
using F1Solutions.Naati.Common.Contracts.Dal.Enum;
using F1Solutions.Naati.Common.Contracts.Security;
using F1Solutions.Naati.Common.Dal.Domain.Extensions;
using Ncms.Contracts.Models;
using Ncms.Contracts.Models.Application;
using Ncms.Contracts.Models.System;

namespace Ncms.Bl.ApplicationActions
{
    public class CredentialRequestAllocateTestSessionAction : CredentialRequestInvoiceBatchedAction
    {
        protected override SecurityNounName? RequiredSecurityNoun => SecurityNounName.TestSession;
        protected override SecurityVerbName? RequiredSecurityVerb => SecurityVerbName.Assign;
        protected override CredentialRequestStatusTypeName[] CredentialRequestEntryStates => new[] { CredentialRequestStatusTypeName.TestAccepted, CredentialRequestStatusTypeName.EligibleForTesting };
        
        protected override ProductSpecificationModel ActionFee => GetActionFee();//null 

        protected override SystemActionTypeName InvoiceProcessedAction => SystemActionTypeName.TestInvoiceProcessed;
        protected override SystemActionTypeName InovicePaidAction => SystemActionTypeName.TestInvoicePaid;
        protected override CredentialRequestStatusTypeName ProcessingInvoiceStatus => CredentialRequestStatusTypeName.ProcessingTestInvoice;
        protected override CredentialRequestStatusTypeName PendingPaymentStatus => CredentialRequestStatusTypeName.AwaitingTestPayment;
        protected override CredentialRequestStatusTypeName InovicePaidStatus => CredentialRequestStatusTypeName.TestSessionAccepted;

        // True if test was allocated via Allocate Test Session Wizard in NCMS or if the credential application type is a CLA, otherwise false
        private bool TestAllocatedWithoutPayment => (!IsBackgroundAction && WizardModel.Source == SystemActionSource.Ncms) 
            || ApplicationModel.ApplicationInfo.ApplicationTypeId == 9 //CLA
            || HasPreviousRejectedTestSession()
            || CredentialRequestModel.Supplementary;

        protected override IList<Action> Preconditions => new List<Action>
                                                            {
                                                                ValidateEntryState,
                                                                ValidateUserPermissions ,
                                                                ValidateMandatoryFields,
                                                                ValidateMandatoryPersonFields,
                                                                ValidateMandatoryDocuments
                                                            };

        protected override IList<Action> SystemActions => new List<Action>
                                                            {
                                                                SetOwner,
                                                                CreateNote,
                                                                CreateInvoiceIfApplicable,
                                                                AllocateTestSession,
                                                                SetExitState
                                                            };


        private ProductSpecificationModel GetActionFee()
        {
            return CurrentRequestEntryState == CredentialRequestStatusTypeName.TestAccepted || CurrentRequestEntryState == CredentialRequestStatusTypeName.TestSessionAccepted ? null : GetFee(FeeTypeName.Test);
        }

        protected override void CreateInvoiceIfApplicable()
        {
            if (IsSponsored || WizardModel.Source == SystemActionSource.MyNaati)
            {
                base.CreateInvoiceIfApplicable();
            }
        }

        protected override CredentialRequestStatusTypeName GetCredentialrequestExitState()
        {
            if (HasCreditCardPayment || HasPayPalPayment || IsSponsored)
            {
                if (IsSponsored || WizardModel.Source == SystemActionSource.MyNaati)
                {
                    return base.GetCredentialrequestExitState();
                };

                //if from NCMS
                return CredentialRequestStatusTypeName.TestSessionAccepted;
            }

            if (TestAllocatedWithoutPayment)//This means (!IsBackgroundAction && WizardModel.Source == SystemActionSource.Ncms) || ApplicationModel.ApplicationInfo.ApplicationTypeId == 9 //CLA|| HasPreviousRejectedTestSession()|| CredentialRequestModel.Supplementary;
            {
                return CredentialRequestStatusTypeName.TestSessionAccepted;
            }

            //do not move forward the application if a payment has not been processed successfully and
            //the action was not called from Allocate Test Session Wizard in NCMS (TFS206320)
            // or the application is not sponsored
            // or the application is not of type CLA (CLA does not require payment to allocate a test session as the payment happens during Application Assessment)
            return CredentialRequestStatusTypeName.EligibleForTesting;
        }

        protected override void CreateNote()
        {
            base.CreateNote();
            var noteModel = new ApplicationNoteModel
            {
                ApplicationId = ApplicationModel.ApplicationInfo.ApplicationId,
                CreatedDate = DateTime.Now,
                Note = string.Empty,
                UserId = CurrentUser.Id,
                ReadOnly = true
            };

            var credentialTypeName = CredentialRequestModel.CredentialType.InternalName;
            var skill = CredentialRequestModel.Skill.DisplayName;

            var testSessionId = TestSessionModel.TestSessionId;
            var testSessionDate = TestSessionModel.TestDate;
            noteModel.Note = string.Format(Naati.Resources.Application.TestSessionAllocatedNote, credentialTypeName, skill, testSessionId, testSessionDate);

            ApplicationModel.Notes.Add(noteModel);

        }
       

        protected override CredentialRequestTestSessionModel GetTestSessionModel()
        {
            if (WizardModel.TestSessionId.GetValueOrDefault() == 0)
            {
                var testSession = WizardModel.TestSessionRequestModel;
                testSession.CredentialTypeId = CredentialRequestModel.CredentialTypeId;
                testSession.CredentialApplicationTypeId = ApplicationModel.ApplicationType.Id;
                var testSessionResponse = TestSessionService.CreateOrUpdateTestSession(testSession);
                WizardModel.SetNewTestSessionId(testSessionResponse.Data);
            }
            var result = TestSessionService.GetTestSessionById(WizardModel.TestSessionId.GetValueOrDefault()).Data;

            return new CredentialRequestTestSessionModel
            {
                CredentialTestSessionId = 0,
                Completed = result.Completed,
                Name = result.Name,
                Rejected = false,
                TestSessionId = result.TestSessionId,
                TestDate = result.TestDate,
                Supplementary = CredentialRequestModel.Supplementary,
                TestSpecificationId = CredentialRequestModel.Supplementary ? TestSpecification.TestSpecificationId : result.DefaultTestSpecificationId,
                MarkingSchemaTypeId = CredentialRequestModel.Supplementary ? TestSpecification.MarkingSchemaTypeId : result.MarkingSchemaTypeId,
                AllocatedDate = DateTime.Now
            };
        }
      

        protected override void GetEmailTokens(Dictionary<string, string> tokenDictionary)
        {
            DateTime testDate;
            DateTime testTime;
            string venueName;
            string venueAddress;
            string publicNotes;
            int arrivalTime;
            int duration;
            int sessionId =0;
           
            if (WizardModel.TestSessionId.GetValueOrDefault() != 0)
            {
                var sessionModel = GetTestSessionModel();
                var sessionResponse = TestSessionService.GetTestSessionById(sessionModel.TestSessionId);
                testDate = sessionResponse.Data.TestDate;
                testTime = testDate;
                venueName = sessionResponse.Data.VenueName;
                venueAddress = sessionResponse.Data.VenueAddress;
                publicNotes = sessionResponse.Data.PublicNotes;
                arrivalTime = sessionResponse.Data.ArrivalTime.GetValueOrDefault();
                duration = sessionResponse.Data.Duration.GetValueOrDefault();
                sessionId = sessionResponse.Data.TestSessionId;
            }
            else
            {
                var sessionModel = WizardModel.TestSessionRequestModel;
                var venueModel = TestService.GetVenueById(sessionModel.VenueId.GetValueOrDefault()).Data;
                testDate = sessionModel.TestDate.GetValueOrDefault();
                testTime = DateTime.Parse(sessionModel.TestTime);
                venueName = venueModel.Name;
                venueAddress = venueModel.Address;
                publicNotes = sessionModel.Notes;
                arrivalTime = sessionModel.PreparationTime.GetValueOrDefault();
                duration = sessionModel.SessionDuration.GetValueOrDefault();
            }
           
            var testSitting = TestService.GetTestSittingByCredentailRequestId(CredentialRequestModel.Id,
                CredentialRequestModel.Supplementary);
         

            tokenDictionary.Add(TokenReplacementService.GetTokenNameFor(TokenReplacementField.TestSessionDate), testDate.ToString("dd MMMM yyyy"));
            tokenDictionary.Add(TokenReplacementService.GetTokenNameFor(TokenReplacementField.TestSessionStartTime), testTime.ToString("h:mm tt"));
            tokenDictionary.Add(TokenReplacementService.GetTokenNameFor(TokenReplacementField.VenueName), venueName);
            tokenDictionary.Add(TokenReplacementService.GetTokenNameFor(TokenReplacementField.VenueAddress), venueAddress);
            tokenDictionary.Add(TokenReplacementService.GetTokenNameFor(TokenReplacementField.TestSessionId), $"TS{sessionId}");
            tokenDictionary.Add(TokenReplacementService.GetTokenNameFor(TokenReplacementField.TestSessionArrivalTime), testTime.AddMinutes(-arrivalTime).ToString("h:mm tt"));
            tokenDictionary.Add(TokenReplacementService.GetTokenNameFor(TokenReplacementField.TestSessionCompletionTime), testTime.AddMinutes(duration).ToString("h:mm tt"));
            tokenDictionary.Add(TokenReplacementService.GetTokenNameFor(TokenReplacementField.TestSessionPublicNotes), publicNotes);
            tokenDictionary.Add(TokenReplacementService.GetTokenNameFor(TokenReplacementField.TestAttendanceId), $"{testSitting.Data?.TestAttendanceId?.ToString() ?? Naati.Resources.Shared.ToBeGenerated}");

            if (HasCreditCardPayment)
            {
                tokenDictionary.Add(TokenReplacementService.GetTokenNameFor(TokenReplacementField.PaymentReference), WizardModel.PaymentReference);
            }
        }

        private bool HasPreviousRejectedTestSession()
        {
            var previouslyRejectedTestSession = CredentialRequestModel.TestSessions.OrderByDescending(x => x.CredentialTestSessionId).FirstOrDefault(x => x.Rejected);

            if (previouslyRejectedTestSession.IsNull())
            {
                return false;
            }

            var newlySelectedTestSession = CredentialRequestModel.TestSessions.OrderByDescending(x => x.CredentialTestSessionId).FirstOrDefault(x => !x.Rejected && x.Supplementary == CredentialRequestModel.Supplementary);

            if (newlySelectedTestSession.IsNull())
            {
                return false;
            }

            // if the credential request has had a previously rejected test session and now has a newly selected test session, then return true, otherwise return false;
            return previouslyRejectedTestSession.IsNotNull() && newlySelectedTestSession.IsNotNull(); 
        }

    }
}
