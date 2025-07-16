using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using AutoMapper;
using F1Solutions.Global.Common.Mapping;
using F1Solutions.Naati.Common.Contracts.Dal;
using F1Solutions.Naati.Common.Contracts.Dal.Enum;
using Ncms.Contracts;
using Ncms.Contracts.Models;
using Ncms.Contracts.Models.Application;
using Ncms.Contracts.Models.MaterialRequest;

namespace Ncms.Bl.MaterialRequestActions
{
    public class MaterialRequestUpdateMembersAction : MaterialRequestAction
    {
        protected override MaterialRequestStatusTypeName[] MaterialRequestEntryStates => new[] { MaterialRequestStatusTypeName.InProgress };
        protected override MaterialRequestStatusTypeName MaterialRequestExitState => MaterialRequestStatusTypeName.InProgress;

        private bool _membersUpdated;

        private IList<MaterialRequestPanelMembershipModel> _newMembers = new List<MaterialRequestPanelMembershipModel>();

        private IList<MaterialRequestPanelMembershipModel> _removedMembers = new List<MaterialRequestPanelMembershipModel>();

        protected override IList<Action> Preconditions
        {
            get
            {
                var actions = new List<Action>
                {
                    ValidateUserPermissions,
                    ValidateEntryState,
                    ValidateMemberTasks
                };

                return actions;
            }
        }

        protected override IList<Action> SystemActions
        {
            get
            {
                var actions = new List<Action>
                {
                    AddMembers,
                    SetTestMaterialDomain,
                    CreateNote,
                    SetExitState
                };

                return actions;
            }
        }

        private void ReplaceMemberDetails(MaterialRequestEmailMessageModel template, Dictionary<string, string> memberTokens)
        {
            IEnumerable<string> replacementErrors;

            string stringValue;
            void ReplaceString(string token, string value) => stringValue = stringValue.Replace(token, value ?? string.Empty);

            stringValue = template.Body;
            TokenReplacementService.ReplaceTemplateFieldValues(stringValue, ReplaceString, memberTokens, true, out replacementErrors);
            template.Body = stringValue;
            stringValue = template.Subject;
            TokenReplacementService.ReplaceTemplateFieldValues(stringValue, ReplaceString, memberTokens, true, out replacementErrors);
            template.Subject= stringValue;
        }

        protected override IEnumerable<MaterialRequestEmailMessageModel> GetEmails(
            EmailTemplateModel template,
            MaterialRequestEmailMessageModel baseEmail)
        {
            var emails = base.GetEmails(template, baseEmail).ToList();

            if (template.EmailTemplateDetails.Contains(EmailTemplateDetailTypeName.SendToCollaborator))
            {
                if (template.SystemActionEventType == SystemActionEventTypeName.CollaboratorAdded)
                {
                    foreach (var member in _newMembers)
                    {
                        var memberEmail = new MaterialRequestEmailMessageModel();
                        AutoMapperHelper.Mapper.Map(baseEmail, memberEmail);

                        var memberTokens = GetMemberEmailTokens(member);
                        memberEmail.RecipientEmail = member.PrimaryEmail;
                        memberEmail.RecipientEntityId = member.EntityId;
                        ReplaceMemberDetails(memberEmail, memberTokens);
                        emails.Add(memberEmail);
                    }
                }
                else if (template.SystemActionEventType == SystemActionEventTypeName.CollaboratorRemoved)
                {
                    foreach (var member in _removedMembers)
                    {
                        var memberEmail = new MaterialRequestEmailMessageModel();
                        AutoMapperHelper.Mapper.Map(baseEmail, memberEmail);

                        var memberTokens = GetMemberEmailTokens(member);
                        memberEmail.RecipientEmail = member.PrimaryEmail;
                        memberEmail.RecipientEntityId = member.EntityId;
                        ReplaceMemberDetails(memberEmail, memberTokens);
                        emails.Add(memberEmail);
                    }
                }
            }

            return emails;
        }

        protected override void AddMembers()
        {
            if (_membersUpdated)
            {
                return;
            }

            var previousMembers =
                ActionModel.MaterialRequestInfo.Members.ToDictionary(x => x.PanelMemberShipId, y => y);

            CheckMembers();

            var updatedMembers = ActionModel.MaterialRequestInfo.Members.ToDictionary(x => x.PanelMemberShipId, y => y);
            _newMembers = ActionModel.MaterialRequestInfo.Members.Where(x => !previousMembers.ContainsKey(x.PanelMemberShipId)).ToList();
            _removedMembers = previousMembers.Values.Where(x => !updatedMembers.ContainsKey(x.PanelMemberShipId)).ToList();
            _membersUpdated = true;
        }

        public override IList<EmailTemplateModel> GetEmailTemplates()
        {
            AddMembers();
            return base.GetEmailTemplates();
        }

        protected override IList<ActionEventLevel> GetActionEvents()
        {
            var events = base.GetActionEvents();

            var maxLevel = events.Max(x => x.Level);
            if (_newMembers.Any())
            {
                var newMemberEvent = new ActionEventLevel
                    { Event = SystemActionEventTypeName.CollaboratorAdded, Level = maxLevel + 1 };
                events.Add(newMemberEvent);
            }

            if (_removedMembers.Any())
            {
                var removedMemberEvent = new ActionEventLevel
                    { Event = SystemActionEventTypeName.CollaboratorRemoved, Level = maxLevel + 1 };
                events.Add(removedMemberEvent);
            }

            return events;
        }

        protected Dictionary<string, string> GetMemberEmailTokens(MaterialRequestPanelMembershipModel model)
        {
            var MaterialRequestTaskTypes = ApplicationService.GetLookupType(nameof(LookupType.MaterialRequestTaskType)).Data.ToList();
            var MaterialRequestMembershipTypes = ApplicationService.GetLookupType(nameof(LookupType.MaterialRequestRoundMembershipType)).Data.ToList();

            var memberTokens = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            TryInsertToken(memberTokens, TokenReplacementField.PanelMembershipType, () => MaterialRequestMembershipTypes.First(x => x.Id == model.MemberTypeId).DisplayName);
            TryInsertToken(memberTokens, TokenReplacementField.PanelMembershipTask, () => MaterialRequestTaskTypes.Any()?MaterialRequestTaskTypes.First(x=>x.Id == model.Tasks.First().MaterialRequestTaskTypeId).DisplayName:"None");
            TryInsertToken(memberTokens, TokenReplacementField.CollaboratorName, () => model.GivenName);
            return memberTokens;
        }
    }
}