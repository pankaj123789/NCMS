using System;
using System.Collections.Generic;
using System.Linq;
using F1Solutions.Naati.Common.Contracts.Dal.Enum;
using Ncms.Contracts;

namespace Ncms.Bl.RolePlayerActions
{
    public class RolePlayerAllocateAction : AssignRolePlayersWizardRolePlayerAction
    {
        protected override RolePlayerStatusTypeName[] RolePlayerEntryStates => new[] { RolePlayerStatusTypeName.None, RolePlayerStatusTypeName.Pending, RolePlayerStatusTypeName.Accepted };
        protected override RolePlayerStatusTypeName RolePlayerExitState => GetExitState();

        protected override IList<Action> Preconditions => new List<Action>
        {
            ValidateEntryState,
            ValidateUserPermissions,
        };

        protected override IList<Action> SystemActions => new List<Action>
        {
            AllocateRolePlayerTasks,
            CreatePersonNote,
            SetExitState
        };

        private RolePlayerStatusTypeName GetExitState()
        {
            if (ActionModel.TestSessionRolePlayer.RolePlayerStatusId ==  (int)RolePlayerStatusTypeName.None )
            {
                return RolePlayerStatusTypeName.Pending;
            }

            return (RolePlayerStatusTypeName)ActionModel.TestSessionRolePlayer.RolePlayerStatusId;
        }
        private void AllocateRolePlayerTasks()
        {
            var deletedTasks = new List<TestSessionRolePlayerDetailModel>();

            var existingDetails = ActionModel.TestSessionRolePlayer.Details;
            var wizardDetails = WizardModel.RolePlayer.Details;


            foreach (var detail in existingDetails)
            {
                if (wizardDetails.All(x => x.TestComponentId != detail.TestComponentId))
                {
                    deletedTasks.Add(detail);
                }
            }

            deletedTasks.ForEach(v=> existingDetails.Remove(v));
           

            foreach (var wizardDetail in wizardDetails)
            {
                var existingTask =
                    existingDetails.FirstOrDefault(x => x.TestComponentId == wizardDetail.TestComponentId );
                if (existingTask == null)
                {
                    existingTask = new TestSessionRolePlayerDetailModel
                    {
                        SkillId = WizardModel.SkillId,
                        TestComponentId = wizardDetail.TestComponentId,
                        LanguageId = wizardDetail.LanguageId
                    };
                    existingDetails.Add(existingTask);
                }

                existingTask.RolePlayerRoleTypeId = wizardDetail.RolePlayerRoleTypeId;

            }

        }

        protected override string GetPersonNote()
        {
            return String.Format(Naati.Resources.Shared.RolePlayedAssignedToTestSession, WizardModel.TestSessionId);
        }
    }
}
