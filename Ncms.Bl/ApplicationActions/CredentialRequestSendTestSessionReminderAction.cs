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
    public class CredentialRequestSendTestSessionReminderAction : CredentialRequestStateAction
    {
        protected override SecurityNounName? RequiredSecurityNoun => SecurityNounName.Email;
        protected override SecurityVerbName? RequiredSecurityVerb => SecurityVerbName.Send;

        protected override CredentialRequestStatusTypeName[] CredentialRequestEntryStates => new[] { CredentialRequestStatusTypeName.TestSessionAccepted };

        protected override CredentialRequestStatusTypeName CredentialRequestExitState => CredentialRequestStatusTypeName.TestSessionAccepted;

        protected override IList<Action> Preconditions => new List<Action>
        {
            ValidateEntryState,
            ValidateUserPermissions

        };

        protected override IList<Action> SystemActions => new List<Action>
        {
            CreateNote
        };

        protected override void GetEmailTokens(Dictionary<string, string> tokenDictionary)
        {
            DateTime testDate;
            DateTime testTime;
            string venueName;
            string venueAddress;
            string publicNotes;
            int arrivalTime;
            int duration;
            int sessionId = 0;

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
        }

        protected override string GetNote()
        {
            return Naati.Resources.TestSession.TestSessionReminderSent;
        }
    }
}
