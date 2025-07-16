using System;
using System.Collections.Generic;
using System.Linq;
using F1Solutions.Naati.Common.Contracts.Dal;
using F1Solutions.Naati.Common.Contracts.Dal.Enum;
using Ncms.Contracts.Models.Application;
using Ncms.Contracts.Models.System;

namespace Ncms.Bl.ApplicationActions
{
    public class CredentialRequestAllocateAndAcceptTestSesssionAction : CredentialRequestAllocateTestSessionAction
    {
        protected override CredentialRequestStatusTypeName[] CredentialRequestEntryStates => new[] { CredentialRequestStatusTypeName.EligibleForTesting, CredentialRequestStatusTypeName.TestAccepted, CredentialRequestStatusTypeName.TestSessionAccepted };



        protected override IList<Action> Preconditions => new List<Action>
        {
            ValidateEntryState,
            ValidateTestSessionRejectionDate,
            ValidateTestSessionAvailabilityDate,
            ValidateTestSessionClosedDate,
            ValidatTestSessionCapacity
        };

        private void ValidatTestSessionCapacity()
        {
            var testSessionModel = TestSessionModel;
            if (string.IsNullOrWhiteSpace(WizardModel.PaymentReference)
                && !TestSessionService.CheckCapacityTestSession(CredentialRequestModel.CredentialTypeId,
                    CredentialRequestModel.SkillId, testSessionModel.TestSessionId, CredentialRequestModel.StatusTypeId,
                    ActionModel.ApplicationInfo.PreferredTestLocationId).Data)
            {
                throw new UserFriendlySamException($"Test Session TS{testSessionModel.TestSessionId} is full.");
            }
        }

        protected override IList<Action> SystemActions => new List<Action>
        {
            ClearOwner,
            CreateNote,
            CreateInvoiceIfApplicable,
            RejectTestSession,
            AllocateTestSession,
            SetExitState
        };

        protected override void GetEmailTokens(Dictionary<string, string> tokenDictionary)
        {
            var sessionModel = GetTestSessionModel();
            var sessionResponse = TestSessionService.GetTestSessionById(sessionModel.TestSessionId);
            var testSitting = TestService.GetTestSittingByCredentailRequestId(CredentialRequestModel.Id,
                sessionModel.Supplementary);

            tokenDictionary.Add(TokenReplacementService.GetTokenNameFor(TokenReplacementField.TestSessionDate), sessionResponse.Data.TestDate.ToString("dd MMMM yyyy"));
            tokenDictionary.Add(TokenReplacementService.GetTokenNameFor(TokenReplacementField.TestSessionStartTime), sessionResponse.Data.TestDate.ToString("h:mm tt"));
            tokenDictionary.Add(TokenReplacementService.GetTokenNameFor(TokenReplacementField.VenueName), sessionResponse.Data.VenueName);
            tokenDictionary.Add(TokenReplacementService.GetTokenNameFor(TokenReplacementField.VenueAddress), sessionResponse.Data.VenueAddress);
            tokenDictionary.Add(TokenReplacementService.GetTokenNameFor(TokenReplacementField.TestSessionId), $"TS{sessionModel.TestSessionId}");
            tokenDictionary.Add(TokenReplacementService.GetTokenNameFor(TokenReplacementField.TestSessionArrivalTime), sessionResponse.Data.TestDate.AddMinutes(-sessionResponse.Data.ArrivalTime ?? 0).ToString("h:mm tt"));
            tokenDictionary.Add(TokenReplacementService.GetTokenNameFor(TokenReplacementField.TestSessionCompletionTime), sessionResponse.Data.TestDate.AddMinutes(sessionResponse.Data.Duration ?? 0).ToString("h:mm tt"));
            tokenDictionary.Add(TokenReplacementService.GetTokenNameFor(TokenReplacementField.TestSessionPublicNotes), sessionResponse.Data.PublicNotes);
            tokenDictionary.Add(TokenReplacementService.GetTokenNameFor(TokenReplacementField.TestAttendanceId), $"{testSitting.Data.TestAttendanceId}");

            if (HasCreditCardPayment || HasPayPalPayment)
            {
                tokenDictionary.Add(TokenReplacementService.GetTokenNameFor(TokenReplacementField.PaymentReference), WizardModel.PaymentReference);
                tokenDictionary.Add(TokenReplacementService.GetTokenNameFor(TokenReplacementField.PaymentType), HasCreditCardPayment?"Credit Card":"PayPal");
            }
        }

        protected override void AllocateTestSession()
        {
            var testSessionModel = TestSessionModel;
            if (testSessionModel.TestSpecificationId == 0)
            {
                throw new Exception($"No Test Specification configured for Credentialtype Id: {CredentialRequestModel.CredentialType.Id}");
            }
            var credentialTestSession = new CredentialRequestTestSessionModel
            {
                Name = testSessionModel.Name,
                TestDate = testSessionModel.TestDate,
                TestSessionId = testSessionModel.TestSessionId,
                Supplementary = CredentialRequestModel.Supplementary,
                TestSpecificationId = testSessionModel.TestSpecificationId,
                MarkingSchemaTypeId = testSessionModel.MarkingSchemaTypeId,
                AllocatedDate = DateTime.Now
            };
            CredentialRequestModel.TestSessions.Add(credentialTestSession);
        }

        public override void RejectTestSession()
        {
            var previousTestSessionModel = CredentialRequestModel.TestSessions.OrderByDescending(x => x.CredentialTestSessionId).FirstOrDefault(x => !x.Rejected && x.Supplementary == CredentialRequestModel.Supplementary);
            if (previousTestSessionModel != null)
            {
                previousTestSessionModel.Rejected = true;
                previousTestSessionModel.RejectedDate = DateTime.Now; 
                CreateTestSessionRejectedNote(previousTestSessionModel);
            }
        }

        protected override void ValidateTestSessionRejectionDate()
        {
            if (WizardModel.Source == SystemActionSource.MyNaati)
            {
                var previousTestSessionModel = CredentialRequestModel.TestSessions.OrderByDescending(x => x.CredentialTestSessionId).FirstOrDefault(x => !x.Rejected && x.Supplementary == CredentialRequestModel.Supplementary);
                if (previousTestSessionModel != null)
                {
                    var days = CredentialRequestModel.CredentialType.TestSessionBookingRejectHours / 24;

                    if (previousTestSessionModel.TestDate.AddDays(-days).Date < DateTime.Now.Date)
                    {
                        throw new Exception("Person not allowed to rejected this session.");
                    }
                }
            }
        }
    }
}
