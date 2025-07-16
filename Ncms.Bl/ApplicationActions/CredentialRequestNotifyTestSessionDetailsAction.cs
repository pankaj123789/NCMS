using System;
using System.Collections.Generic;
using F1Solutions.Naati.Common.Contracts.Dal;
using F1Solutions.Naati.Common.Contracts.Dal.Enum;
using F1Solutions.Naati.Common.Contracts.Security;
using Ncms.Contracts.Models;

namespace Ncms.Bl.ApplicationActions
{
    public class CredentialRequestNotifyTestSessionDetailsAction : CredentialRequestStateAction
    {
        protected override SecurityNounName? RequiredSecurityNoun => SecurityNounName.TestSession;
        protected override SecurityVerbName? RequiredSecurityVerb => SecurityVerbName.Notify;

        protected override CredentialRequestStatusTypeName[] CredentialRequestEntryStates =>
            new CredentialRequestStatusTypeName[] { };

        protected override CredentialRequestStatusTypeName CredentialRequestExitState =>
            (CredentialRequestStatusTypeName) CredentialRequestModel.StatusTypeId;

        protected override IList<Action> Preconditions => new List<Action>
        {
            ValidateUserPermissions,
            ValidateTestSessionDate

        };

        protected override IList<Action> SystemActions => new List<Action>
        {
            UpdateTestSpecification,
            CreateNote
        };

        public override void ValidateTestSessionDate()
        {
            if (TestSessionModel == null)
            {
                throw new UserFriendlySamException(Naati.Resources.Application.TestSessionNotFound);
            }
        }

        protected override string GetNote()
        {
          return String.Format(Naati.Resources.Application.NotifyTestSessionDetailsMessage, TestSessionModel.TestSessionId);
        }

        protected override void GetEmailTokens(Dictionary<string, string> tokenDictionary)
        {
            var sessionModel = WizardModel.TestSessionRequestModel;
            var venueModel = TestService.GetVenueById(sessionModel.VenueId.GetValueOrDefault()).Data;
            var testDate = sessionModel.TestDate.GetValueOrDefault();
            var testTime = DateTime.Parse(sessionModel.TestTime);
            var venueName = venueModel.Name;
            var venueAddress = venueModel.Address;

            tokenDictionary.Add(TokenReplacementService.GetTokenNameFor(TokenReplacementField.TestSessionDate),testDate.ToString("dd MMMM yyyy"));
            tokenDictionary.Add(TokenReplacementService.GetTokenNameFor(TokenReplacementField.TestSessionStartTime),testTime.ToString("h:mm tt"));
            tokenDictionary.Add(TokenReplacementService.GetTokenNameFor(TokenReplacementField.VenueName), venueName);
            tokenDictionary.Add(TokenReplacementService.GetTokenNameFor(TokenReplacementField.VenueAddress),venueAddress);
            tokenDictionary.Add(TokenReplacementService.GetTokenNameFor(TokenReplacementField.TestSessionId), $"TS{sessionModel.Id}");
            tokenDictionary.Add(TokenReplacementService.GetTokenNameFor(TokenReplacementField.TestSessionArrivalTime), testTime.AddMinutes(-sessionModel.PreparationTime ?? 0).ToString("h:mm tt"));
            tokenDictionary.Add(TokenReplacementService.GetTokenNameFor(TokenReplacementField.TestSessionCompletionTime), testTime.AddMinutes(sessionModel.SessionDuration ?? 0).ToString("h:mm tt"));
            tokenDictionary.Add(TokenReplacementService.GetTokenNameFor(TokenReplacementField.TestSessionPublicNotes), sessionModel.Notes);
            tokenDictionary.Add(TokenReplacementService.GetTokenNameFor(TokenReplacementField.TestAttendanceId), $"{TestSessionModel.CredentialTestSessionId}");
          
        }

        private void UpdateTestSpecification()
        {
            var testSession = TestSessionService.GetTestSessionById(WizardModel.TestSessionId.GetValueOrDefault()).Data;

            if (TestSessionModel.TestSpecificationId != testSession.DefaultTestSpecificationId && !CredentialRequestModel.Supplementary)
            {
                var previousTestSpecificationId = TestSessionModel.TestSpecificationId;
                TestSessionModel.TestSpecificationId = testSession.DefaultTestSpecificationId;
                ApplicationModel.Notes.Add(new ApplicationNoteModel
                {
                    ApplicationId = ApplicationModel.ApplicationInfo.ApplicationId,
                    CreatedDate = DateTime.Now,
                    Note = string.Format(Naati.Resources.Application.TestSpecificationChanged, TestSessionModel.CredentialTestSessionId, previousTestSpecificationId, TestSessionModel.TestSpecificationId),
                    UserId = CurrentUser.Id,
                    ReadOnly = true
                });
            }
        }
    }

}
