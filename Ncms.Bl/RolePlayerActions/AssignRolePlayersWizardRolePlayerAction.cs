using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using F1Solutions.Naati.Common.Contracts.Dal;
using F1Solutions.Naati.Common.Contracts.Dal.Enum;
using Ncms.Contracts;
using Ncms.Contracts.Models.RolePlayer;

namespace Ncms.Bl.RolePlayerActions
{
    public class AssignRolePlayersWizardRolePlayerAction:RolePlayerAction<AssignRolePlayersWizardModel>
    {

        protected override void GetEmailTokens(Dictionary<string, string> tokenDictionary)
        {
            var sessionModel = TestSessionService.GetTestSessionById(WizardModel.TestSessionId).Data;
            var testTime = DateTime.Parse(sessionModel.TestTime);
            var rehearsalTime = DateTime.Parse(sessionModel.RehearsalTime);
            var rolePLayTasks = GetRolePlayerTasksEmailToken();

            tokenDictionary.Add(TokenReplacementService.GetTokenNameFor(TokenReplacementField.VenueName), sessionModel.VenueName);
            tokenDictionary.Add(TokenReplacementService.GetTokenNameFor(TokenReplacementField.VenueAddress), sessionModel.VenueAddress);
            tokenDictionary.Add(TokenReplacementService.GetTokenNameFor(TokenReplacementField.TestSessionId), $"TS{sessionModel.TestSessionId}");
            tokenDictionary.Add(TokenReplacementService.GetTokenNameFor(TokenReplacementField.TestSessionName), sessionModel.Name);
            tokenDictionary.Add(TokenReplacementService.GetTokenNameFor(TokenReplacementField.TestSessionDate), sessionModel.TestDate.ToString("dd MMMM yyyy"));
            tokenDictionary.Add(TokenReplacementService.GetTokenNameFor(TokenReplacementField.TestSessionStartTime), testTime.ToString("h:mm tt"));
            tokenDictionary.Add(TokenReplacementService.GetTokenNameFor(TokenReplacementField.TestSessionArrivalTime),sessionModel.TestDate.AddMinutes(-sessionModel.ArrivalTime ?? 0).ToString("h:mm tt"));
            tokenDictionary.Add(TokenReplacementService.GetTokenNameFor(TokenReplacementField.TestSessionCompletionTime), sessionModel.TestDate.AddMinutes(sessionModel.Duration ?? 0).ToString("h:mm tt"));
            tokenDictionary.Add(TokenReplacementService.GetTokenNameFor(TokenReplacementField.TestSessionPublicNotes), sessionModel.PublicNotes);
            tokenDictionary.Add(TokenReplacementService.GetTokenNameFor(TokenReplacementField.RehearsalDate), sessionModel.RehearsalDate.GetValueOrDefault().ToString("dd MMMM yyyy"));
            tokenDictionary.Add(TokenReplacementService.GetTokenNameFor(TokenReplacementField.RehearsalTime), rehearsalTime.ToString("h:mm tt"));
            tokenDictionary.Add(TokenReplacementService.GetTokenNameFor(TokenReplacementField.RehearsalNotes), sessionModel.RehearsalNotes);
            tokenDictionary.Add(TokenReplacementService.GetTokenNameFor(TokenReplacementField.RolePlayTasks), rolePLayTasks);
            base.GetEmailTokens(tokenDictionary);
        }


        private string GetRolePlayerTasksEmailToken()
        {
            var allocationDetails = TestSessionService.GetRolePlayerAllocationDetails(
                new RolePlayerAllocationDetailsRequest
                {
                    TestSpecificationId = WizardModel.TestSpecificationId,
                    SkillId = WizardModel.SkillId
                }).Data;

            var availablRoles = ApplicationService.GetLookupType(LookupType.RolePlayerRoleType.ToString())
                .Data.ToDictionary(x => x.Id, y => y);

            var availableTasks = allocationDetails.Tasks.ToDictionary(x => x.Id, y => y);

            var tokenBuilder = new StringBuilder();
            tokenBuilder.AppendLine("<ul>");
            foreach (var detail in WizardModel.RolePlayer.Details)
            {
                var task = availableTasks[detail.TestComponentId];
                var taskName = task.TaskName;
                
                var language = detail.LanguageId == allocationDetails.Skill.Language1Id
                    ? allocationDetails.Skill.Language1DisplayName
                    : allocationDetails.Skill.Language2DisplayName;

                var role = availablRoles[detail.RolePlayerRoleTypeId].DisplayName;

                tokenBuilder.AppendLine("<li>");
                tokenBuilder.AppendLine($"{task.TypeLabel}{task.TaskLabel} - {taskName} ({role}, {language})");
                tokenBuilder.AppendLine("</li>");
            }

            tokenBuilder.AppendLine("</ul>");

            return tokenBuilder.ToString();
        }
    
    }
}
