using System;
using System.Collections.Generic;
using System.Linq;
using F1Solutions.Naati.Common.Contracts.Dal;
using Ncms.Contracts.Models.RolePlayer;

namespace Ncms.Bl.RolePlayerActions
{
    public class RolePlayerUpdateAction :RolePlayerAction<RolePlayerUpdateWizardModel>
    {
        protected override IList<Action> Preconditions => new List<Action>
        {
            ValidateEntryState,
            ValidateUserPermissions,
        };

        protected override IList<Action> SystemActions => new List<Action>
        {
            CreatePersonNote,
            SetActionFlag,
            SetExitState
        };

        protected override void GetEmailTokens(Dictionary<string, string> tokenDictionary)
        {
            var sessionModel = TestSessionService.GetTestSessionById(ActionModel.TestSessionRolePlayer.TestSessionId).Data;
            var testTime = DateTime.Parse(sessionModel.TestTime);
            var rehearsalTime = DateTime.Parse(sessionModel.RehearsalTime);
           

            tokenDictionary.Add(TokenReplacementService.GetTokenNameFor(TokenReplacementField.VenueName), sessionModel.VenueName);
            tokenDictionary.Add(TokenReplacementService.GetTokenNameFor(TokenReplacementField.VenueAddress), sessionModel.VenueAddress);
            tokenDictionary.Add(TokenReplacementService.GetTokenNameFor(TokenReplacementField.TestSessionId), $"TS{sessionModel.TestSessionId}");
            tokenDictionary.Add(TokenReplacementService.GetTokenNameFor(TokenReplacementField.TestSessionName), sessionModel.Name);
            tokenDictionary.Add(TokenReplacementService.GetTokenNameFor(TokenReplacementField.TestSessionDate), sessionModel.TestDate.ToString("dd MMMM yyyy"));
            tokenDictionary.Add(TokenReplacementService.GetTokenNameFor(TokenReplacementField.TestSessionStartTime), testTime.ToString("h:mm tt"));
            tokenDictionary.Add(TokenReplacementService.GetTokenNameFor(TokenReplacementField.TestSessionArrivalTime), sessionModel.TestDate.AddMinutes(-sessionModel.ArrivalTime ?? 0).ToString("h:mm tt"));
            tokenDictionary.Add(TokenReplacementService.GetTokenNameFor(TokenReplacementField.TestSessionCompletionTime), sessionModel.TestDate.AddMinutes(sessionModel.Duration ?? 0).ToString("h:mm tt"));
            tokenDictionary.Add(TokenReplacementService.GetTokenNameFor(TokenReplacementField.TestSessionPublicNotes), sessionModel.PublicNotes);
            tokenDictionary.Add(TokenReplacementService.GetTokenNameFor(TokenReplacementField.RehearsalDate), sessionModel.RehearsalDate.GetValueOrDefault().ToString("dd MMMM yyyy"));
            tokenDictionary.Add(TokenReplacementService.GetTokenNameFor(TokenReplacementField.RehearsalTime), rehearsalTime.ToString("h:mm tt"));
            tokenDictionary.Add(TokenReplacementService.GetTokenNameFor(TokenReplacementField.RehearsalNotes), sessionModel.RehearsalNotes);
            base.GetEmailTokens(tokenDictionary);
        }

        protected override string GetPersonNote()
        {
            return String.Format(Naati.Resources.RolePlayer.RolePlayerStatusUpdated, ActionModel.TestSessionRolePlayer.TestSessionId, ActionModel.TestSessionRolePlayer.RolePlayerStatus, RolePlayerStatusType.Single(x => x.Id == (int)RolePlayerExitState).DisplayName);
        }

        protected virtual void SetActionFlag()
        {
          
        }

    }
}
