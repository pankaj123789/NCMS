using System;
using System.Collections.Generic;
using System.Linq;
using F1Solutions.Naati.Common.Contracts.Dal;
using F1Solutions.Naati.Common.Contracts.Dal.Enum;
using F1Solutions.Naati.Common.Contracts.Security;
using Ncms.Contracts.Models.Application;

namespace Ncms.Bl.ApplicationActions
{
    public class CredentialRequestTestInvoicePaidAction : CredentialRequestStateAction
    {
        protected override CredentialRequestStatusTypeName[] CredentialRequestEntryStates => new[] { CredentialRequestStatusTypeName.AwaitingTestPayment};

        protected override CredentialRequestStatusTypeName CredentialRequestExitState => GetExitState();

        protected bool HasTestSessionAllocated => TestSessionModel != null &&
                                               TestSessionModel.Supplementary == CredentialRequestModel.Supplementary;

        protected override SecurityNounName? RequiredSecurityNoun => GetRequiredSecurityNoun();
        protected override SecurityVerbName? RequiredSecurityVerb => GetRequiredSecurityVerb();

        protected override IList<Action> Preconditions => new List<Action>
                                                            {
                                                                ValidateEntryState,
                                                                ValidateUserPermissions,
                                                                ValidateCredentialRequestInvoices
                                                            };

        protected override IList<Action> SystemActions => new List<Action>
                                                            {
                                                                ClearOwner,
                                                                CreateNote,
                                                                SetExitState,
                                                                ProcessFee,
                                                            };


        protected virtual SecurityNounName GetRequiredSecurityNoun()
        {
            return SecurityNounName.PersonFinanceDetails;
        }

        protected virtual SecurityVerbName GetRequiredSecurityVerb()
        {
            return SecurityVerbName.Manage;
        }

        private CredentialRequestStatusTypeName GetExitState()
        {
            if (HasTestSessionAllocated)
            {
                return CredentialRequestStatusTypeName.TestSessionAccepted;
            }

            return CredentialRequestStatusTypeName.TestAccepted;
        }

        protected override IList<ActionEventLevel> GetActionEvents()
        {
            var events = base.GetActionEvents();

            if (HasTestSessionAllocated)
            {
                var maxLevel = events.Max(x => x.Level);
                var creditCardEvent = new ActionEventLevel { Event = SystemActionEventTypeName.TestSessionConfirmed, Level = maxLevel + 1 };
                events.Add(creditCardEvent);
            }

            return events;
        }

        protected override void GetEmailTokens(Dictionary<string, string> tokenDictionary)
        {
            if (HasTestSessionAllocated)
            {
                var sessionModel = GetTestSessionModel();
                var sessionResponse = TestSessionService.GetTestSessionById(sessionModel.TestSessionId);
                var testDate = sessionResponse.Data.TestDate;
                var testTime = testDate;
                var venueName = sessionResponse.Data.VenueName;
                var venueAddress = sessionResponse.Data.VenueAddress;

                tokenDictionary.Add(TokenReplacementService.GetTokenNameFor(TokenReplacementField.TestSessionDate), testDate.ToString("dd MMMM yyyy"));
                tokenDictionary.Add(TokenReplacementService.GetTokenNameFor(TokenReplacementField.TestSessionStartTime), testTime.ToString("h:mm tt"));
                tokenDictionary.Add(TokenReplacementService.GetTokenNameFor(TokenReplacementField.VenueName), venueName);
                tokenDictionary.Add(TokenReplacementService.GetTokenNameFor(TokenReplacementField.VenueAddress), venueAddress); 
               
                tokenDictionary.Add(TokenReplacementService.GetTokenNameFor(TokenReplacementField.TestSessionId), $"TS{sessionModel.TestSessionId}");
                tokenDictionary.Add(TokenReplacementService.GetTokenNameFor(TokenReplacementField.TestSessionArrivalTime), sessionResponse.Data.TestDate.AddMinutes(-sessionResponse.Data.ArrivalTime ?? 0).ToString("h:mm tt"));
                tokenDictionary.Add(TokenReplacementService.GetTokenNameFor(TokenReplacementField.TestSessionCompletionTime), sessionResponse.Data.TestDate.AddMinutes(sessionResponse.Data.Duration ?? 0).ToString("h:mm tt"));
                tokenDictionary.Add(TokenReplacementService.GetTokenNameFor(TokenReplacementField.TestSessionPublicNotes), sessionResponse.Data.PublicNotes);
                tokenDictionary.Add(TokenReplacementService.GetTokenNameFor(TokenReplacementField.TestAttendanceId), $"{sessionModel.CredentialTestSessionId}");
            }
        }



        protected override CredentialWorkflowFeeModel GetWorkflowFee()
        {
            var fees = CredentialRequestModel.CredentialWorkflowFees
                .Where(x => x.OnPaymentActionType == SystemActionTypeName.TestInvoicePaid
                            && x.PaymentActionProcessedDate == null)
                .ToList();

            if (fees.Count > 1)
            {
                throw new UserFriendlySamException(
                    $"{fees.Count} fees found for {CredentialRequestModel.CredentialName} - {CredentialRequestModel.Skill.DisplayName} on APP{ApplicationModel.ApplicationInfo.ApplicationId}. Expecting only 1.");
            }

            return fees.SingleOrDefault();
        }
    
    }
}
