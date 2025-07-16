using System;
using System.Collections.Generic;
using F1Solutions.Naati.Common.Contracts.Dal;
using F1Solutions.Naati.Common.Contracts.Dal.Enum;
using F1Solutions.Naati.Common.Contracts.Security;
using Ncms.Contracts.Models.Application;

namespace Ncms.Bl.RolePlayerActions
{
    public class RolePlayerNotifyTestSessionRehearsalDetailsAction : RolePlayerAction<TestSessionBulkActionWizardModel>
    {

        protected override RolePlayerStatusTypeName[] RolePlayerEntryStates =>
            new RolePlayerStatusTypeName[] { };

        protected override RolePlayerStatusTypeName RolePlayerExitState =>
            (RolePlayerStatusTypeName)ActionModel.TestSessionRolePlayer.RolePlayerStatusId;

        protected override SecurityNounName? RequiredSecurityNoun => SecurityNounName.RolePlayer;
        protected override SecurityVerbName? RequiredSecurityVerb => SecurityVerbName.Notify;

        protected override IList<Action> Preconditions => new List<Action>
        {
            ValidateUserPermissions,

        };

        protected override IList<Action> SystemActions => new List<Action>
        {
            CreatePersonNote
        };
        

        protected override string GetPersonNote()
        {
            return String.Format(Naati.Resources.Application.NotifyTestSessionDetailsMessage, ActionModel.TestSessionRolePlayer.TestSessionId);
        }

        protected override void GetEmailTokens(Dictionary<string, string> tokenDictionary)
        {
            var sessionModel = WizardModel.TestSessionRequestModel;
            var venueModel = TestService.GetVenueById(sessionModel.VenueId.GetValueOrDefault()).Data;
            var testDate = sessionModel.TestDate.GetValueOrDefault();
            var testTime = DateTime.Parse(sessionModel.TestTime);
            var venueName = venueModel.Name;
            var venueAddress = venueModel.Address;
            var rehearsalDate = sessionModel.RehearsalDate.HasValue
                ? sessionModel.RehearsalDate.GetValueOrDefault().ToString("dd MMMM yyyy")
                : string.Empty;

            tokenDictionary.Add(TokenReplacementService.GetTokenNameFor(TokenReplacementField.TestSessionDate), testDate.ToString("dd MMMM yyyy"));
            tokenDictionary.Add(TokenReplacementService.GetTokenNameFor(TokenReplacementField.TestSessionStartTime), testTime.ToString("h:mm tt"));
            tokenDictionary.Add(TokenReplacementService.GetTokenNameFor(TokenReplacementField.VenueName), venueName);
            tokenDictionary.Add(TokenReplacementService.GetTokenNameFor(TokenReplacementField.VenueAddress), venueAddress);
            tokenDictionary.Add(TokenReplacementService.GetTokenNameFor(TokenReplacementField.TestSessionId), $"TS{sessionModel.Id}");
            tokenDictionary.Add(TokenReplacementService.GetTokenNameFor(TokenReplacementField.TestSessionArrivalTime), testTime.AddMinutes(-sessionModel.PreparationTime ?? 0).ToString("h:mm tt"));
            tokenDictionary.Add(TokenReplacementService.GetTokenNameFor(TokenReplacementField.TestSessionCompletionTime), testTime.AddMinutes(sessionModel.SessionDuration ?? 0).ToString("h:mm tt"));
            tokenDictionary.Add(TokenReplacementService.GetTokenNameFor(TokenReplacementField.TestSessionPublicNotes), sessionModel.Notes);

            tokenDictionary.Add(TokenReplacementService.GetTokenNameFor(TokenReplacementField.RehearsalDate), rehearsalDate);
            tokenDictionary.Add(TokenReplacementService.GetTokenNameFor(TokenReplacementField.RehearsalTime), sessionModel.RehearsalTime);
            tokenDictionary.Add(TokenReplacementService.GetTokenNameFor(TokenReplacementField.RehearsalNotes), sessionModel.RehearsalNotes);

            base.GetEmailTokens(tokenDictionary);
        }
    }
}
