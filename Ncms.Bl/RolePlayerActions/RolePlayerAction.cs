using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using F1Solutions.Global.Common.Logging;
using F1Solutions.Global.Common.Mapping;
using F1Solutions.Naati.Common.Contracts.Bl;
using F1Solutions.Naati.Common.Contracts.Bl.DTO;
using F1Solutions.Naati.Common.Contracts.Dal;
using F1Solutions.Naati.Common.Contracts.Dal.Enum;
using F1Solutions.Naati.Common.Contracts.Dal.Request;
using F1Solutions.Naati.Common.Contracts.Dal.Response;
using F1Solutions.Naati.Common.Contracts.Security;
using Ncms.Bl.SystemActions;
using Ncms.Contracts;
using Ncms.Contracts.Models;
using Ncms.Contracts.Models.Application;
using Ncms.Contracts.Models.RolePlayer;
using Ncms.Contracts.Models.System;

namespace Ncms.Bl.RolePlayerActions
{
    public class RolePlayerAction<TWizardModelType> : SystemAction<RolePlayerActionModel, TWizardModelType, RolePlayerActionOutput, UpsertRolePlayerResultModel, EmailMessageModel> where TWizardModelType : SystemActionWizardModel
    {
        protected override SecurityNounName? RequiredSecurityNoun => SecurityNounName.RolePlayer;
        protected override SecurityVerbName? RequiredSecurityVerb => SecurityVerbName.Update;

        public RolePlayerAction(IServiceLocator serviceLocator = null) : base(serviceLocator)
        {
        }

        protected virtual RolePlayerStatusTypeName[] RolePlayerEntryStates { get; }
        protected virtual RolePlayerStatusTypeName RolePlayerExitState { get; }


        private IList<LookupTypeModel> _rolePlayerStatusTypes;
        protected IList<LookupTypeModel> RolePlayerStatusType => _rolePlayerStatusTypes ?? (_rolePlayerStatusTypes = ApplicationService.GetLookupType(LookupType.RolePlayerStatusType.ToString()).Data
                                                                                 .Select(AutoMapperHelper.Mapper.Map<LookupTypeModel>)
                                                                                 .ToList());


        public static RolePlayerAction<TWizardModelType> CreateAction(SystemActionTypeName actionType, RolePlayerActionModel actionModel, TWizardModelType wizardModel)
        {
            var action = CreateSystemAction<RolePlayerAction<TWizardModelType>>(ActionDict, actionType, actionModel, wizardModel, new RolePlayerActionOutput());

            return action;
        }

        protected override void CreateEmailAttachmentsIfApplicable()
        {

        }

        protected override GenericResponse<EmailMessageModel> SaveEmailMessage(EmailMessageModel message)
        {
            return EmailMessageService.CreateGenericEmailMessage(message);
        }

        protected override GenericResponse<UpsertRolePlayerResultModel> SaveActionData()
        {
            var request = AutoMapperHelper.Mapper.Map<UpsertSessionRolePlayerRequest>(ActionModel);

            request.PersonNotes = ActionModel.PersonNotes.Select(x => new PersonNoteData()
            {
                NoteId = x.NoteId ?? 0,
                CreatedDate = x.CreatedDate ?? DateTime.Now,
                Description = x.Note,
                Highlight = x.Highlight,
                ReadOnly = x.ReadOnly,
                UserId = x.UserId,
                ModifiedDate = x.ModifiedDate ?? DateTime.Now
            }).ToList();

            var response = TestSessionService.UpsertTestSessionRolePlayer(request);

            return response;
        }

        public override IList<EmailTemplateModel> GetEmailTemplates()
        {
            if (!CanSendEmail())
            {
                return new List<EmailTemplateModel>();
            }

            GetEmailTemplateResponse serviceReponse = null;
            serviceReponse = EmailMessageService.GetSystemEmailTemplate(new GetSystemEmailTemplateRequest
            {
                Actions = new []{ (SystemActionTypeName)WizardModel.ActionType },
                ActionEvents = ActionEvents.GroupBy(x => x.Level).OrderByDescending(y => y.Key).First().Select(x => x.Event)
            });

            var emailTemplates = serviceReponse.Data.Select(AutoMapperHelper.Mapper.Map<EmailTemplateModel>).ToList();

            return emailTemplates;
        }

        protected override IEnumerable<EmailMessageModel> GetEmails(EmailTemplateModel template, EmailMessageModel baseEmail)
        {
            var rolePlayerEmails = new List<EmailMessageModel>();

            if (template.EmailTemplateDetails.Contains(EmailTemplateDetailTypeName.SendToRolePlayer))
            {
                var applicantEmail = AutoMapperHelper.Mapper.Map<EmailMessageModel>(baseEmail);
                applicantEmail.RecipientEmail = ActionModel.PersonDetails.PrimaryEmail;
                rolePlayerEmails.Add(applicantEmail);
            }

            return rolePlayerEmails;
        }

        protected override void GetEmailTokens(Dictionary<string, string> tokenDictionary)
        {
            tokenDictionary.Add(TokenReplacementService.GetTokenNameFor(TokenReplacementField.NaatiNo), ActionModel.PersonDetails.NaatiNumber.ToString());
            tokenDictionary.Add(TokenReplacementService.GetTokenNameFor(TokenReplacementField.NaatiNumber), ActionModel.PersonDetails.NaatiNumber.ToString());
            tokenDictionary.Add(TokenReplacementService.GetTokenNameFor(TokenReplacementField.GivenName), ActionModel.PersonDetails.GivenName);
            tokenDictionary.Add(TokenReplacementService.GetTokenNameFor(TokenReplacementField.OtherNames), ActionModel.PersonDetails.OtherNames);
            tokenDictionary.Add(TokenReplacementService.GetTokenNameFor(TokenReplacementField.FamilyName), ActionModel.PersonDetails.FamilyName);
            tokenDictionary.Add(TokenReplacementService.GetTokenNameFor(TokenReplacementField.PrimaryEmail), ActionModel.PersonDetails.PrimaryEmail);
            tokenDictionary.Add(TokenReplacementService.GetTokenNameFor(TokenReplacementField.PrimaryPhone), ActionModel.PersonDetails.PrimaryContactNumber);
            tokenDictionary.Add(TokenReplacementService.GetTokenNameFor(TokenReplacementField.ActionPublicNote), WizardModel.PublicNotes);
        }

        protected override EmailMessageModel CreateEmail(EmailTemplateModel template, IDictionary<string, string> tokenDictionary)
        {
            IEnumerable<string> replacementErrros;
            var stringValue = string.Empty;
            void ReplaceString(string token, string value) => stringValue = stringValue.Replace(token, value ?? string.Empty);

            stringValue = template.Content;
            TokenReplacementService.ReplaceTemplateFieldValues(stringValue, ReplaceString, tokenDictionary, true, out replacementErrros);
            var content = stringValue;
            stringValue = template.Subject;
            TokenReplacementService.ReplaceTemplateFieldValues(stringValue, ReplaceString, tokenDictionary, true, out replacementErrros);
            var subject = stringValue;
            var fromAddress = string.IsNullOrWhiteSpace(template.FromAddress)
                ? SystemService.GetSystemValue("DefaultEmailSenderAddress")
                : template.FromAddress;

            var email = new EmailMessageModel
            {
                Subject = subject,
                Body = content,
                From = fromAddress,
                RecipientEntityId = ActionModel.PersonDetails.EntityId,
                CreatedDate = DateTime.Now,
                CreatedUserId = CurrentUser.Id,
                Attachments = new List<EmailMessageAttachmentModel>(),
                EmailSendStatusTypeId = (int)EmailSendStatusTypeName.Requested,
                EmailTemplateId = template.Id
            };

            return email;
        }

        protected virtual void ValidateEntryState()
        {
            if (!RolePlayerEntryStates.Contains((RolePlayerStatusTypeName)ActionModel.TestSessionRolePlayer?.RolePlayerStatusId))
            {
                var entryStateNames = RolePlayerEntryStates.Select(x => RolePlayerStatusType.SingleOrDefault(y => y.Id == (int)x)?.DisplayName);
                throw new UserFriendlySamException(String.Format(Naati.Resources.Person.WrongRolePlayerStatus,
                    string.Join(", ", entryStateNames)));
            }
        }

        protected virtual void CreatePersonNote()
        {
            var noteModel = new PersonNoteModel()
            {
                CreatedDate = DateTime.Now,
                Note = string.Empty,
                UserId = CurrentUser.Id,
                ReadOnly = true
            };

            noteModel.Note = GetPersonNote();
            if (!CanSendEmail())
            {
                noteModel.Note = $"{noteModel.Note ?? string.Empty} - {Naati.Resources.Shared.EmailSkipped}";
            }
            if (!String.IsNullOrWhiteSpace(WizardModel.PublicNotes))
            {
                noteModel.Note = $"{noteModel.Note}\n\nPublic Comments: {WizardModel.PublicNotes}";
                noteModel.Highlight = true;
            }
            if (!String.IsNullOrWhiteSpace(WizardModel.PrivateNotes))
            {
                noteModel.Note = $"{noteModel.Note}\n\nPrivate Comments: {WizardModel.PrivateNotes}";
            }
            if (!String.IsNullOrWhiteSpace(noteModel.Note))
            {
                ActionModel.PersonNotes.Add(noteModel);
            }
        }

        protected virtual string GetPersonNote()
        {
            var entryStateName = RolePlayerStatusType.Single(x => x.Id == ActionModel.TestSessionRolePlayer.RolePlayerStatusId).DisplayName;
            var exitStateName = RolePlayerStatusType.Single(x => x.Id == (int)RolePlayerExitState).DisplayName;
            if (entryStateName == exitStateName)
            {
                return string.Empty;
            }
            return String.Format(Naati.Resources.Shared.RolePlayerStatusChange, entryStateName, exitStateName);
        }

        protected virtual void SetExitState()
        {
            ActionModel.TestSessionRolePlayer.RolePlayerStatusId = (int)RolePlayerExitState;
            ActionModel.TestSessionRolePlayer.StatusChangeUserId = CurrentUser.Id;
            ActionModel.TestSessionRolePlayer.StatusChangeDate = DateTime.Now;
        }

        protected override void LogAction()
        {
            LoggingHelper.LogInfo("Performing workflow action {Action} for RP{RolePlayerId} NAATI #{NaatiNumber}",
                (SystemActionTypeName)WizardModel.ActionType, ActionModel?.TestSessionRolePlayer?.RolePlayerId, ActionModel?.PersonDetails?.NaatiNumber);
        }

        protected virtual void RemoveRolePlayer()
        {
            ActionModel.TestSessionRolePlayer.ObjectStatus = ObjectStatusTypeName.Deleted;
        }

        protected virtual void RemoveRolePlayerTasks()
        {
            var taksToRemove = ActionModel.TestSessionRolePlayer.Details;

            foreach (var removedTask in taksToRemove)
            {
                removedTask.ObjectStatus = ObjectStatusTypeName.Deleted;
            }
        }

        private static readonly Dictionary<SystemActionTypeName, Type> ActionDict =
            new Dictionary<SystemActionTypeName, Type>
            {
                {SystemActionTypeName.AllocateRolePlayer,typeof(RolePlayerAllocateAction)},
                {SystemActionTypeName.RolePlayerRemoveFromTestSession,typeof(RolePlayerRemoveFromTestSessionAction)},
                {SystemActionTypeName.NotifyTestSessionRehearsalDetails,typeof(RolePlayerNotifyTestSessionRehearsalDetailsAction)},
                {SystemActionTypeName.RolePlayerNotifyAllocationUpdate,typeof(RolePlayerNotifyAllocationUpdateAction)},
                {SystemActionTypeName.RolePlayerMarkAsRejected,typeof(RolePlayerMarkAsRejectedAction)},
                {SystemActionTypeName.RolePlayerMarkAsAttendedRehearsal,typeof(RolePlayerMarkAsRehearsedAction)},
                {SystemActionTypeName.RolePlayerMarkAsPending,typeof(RolePlayerMarkAsPendingAction)},
                {SystemActionTypeName.RolePlayerMarkAsAttendedTest,typeof(RolePlayerMarkAsAttendedAction)},
                {SystemActionTypeName.RolePlayerMarkAsAccepted,typeof(RolePlayerMarkAsAcceptedAction)},
                {SystemActionTypeName.RolePlayerMarkAsNoShow,typeof(RolePlayerMarkAsNoShowAction)},
                {SystemActionTypeName.RolePlayerMarkAsRemoved,typeof(RolePlayerMarkAsRemovedAction)},
                {SystemActionTypeName.RolePlayerAcceptTestSessionFromMyNaati,typeof(RolePlayerAcceptTestSessionFromMyNaatiAction)},
                {SystemActionTypeName.RolePlayerRejectTestSessionFromMyNaati,typeof(RolePlayerRejectTestSessionFromMyNaatiAction)},
            };
    }
}
