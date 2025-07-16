using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using F1Solutions.Global.Common;
using F1Solutions.Global.Common.Logging;
using F1Solutions.Global.Common.Mapping;
using F1Solutions.Naati.Common.Contracts.Dal.DTO;
using F1Solutions.Naati.Common.Contracts.Dal.Enum;
using F1Solutions.Naati.Common.Contracts.Security;
using Ncms.Contracts.Models;
using Ncms.Contracts.Models.Application;
using Ncms.Contracts.Models.Person;

namespace Ncms.Bl.ApplicationActions
{
    public class ApplicationIssueRecertificationCredentialsAction : ApplicationAssessCredentialsAction
    {
        protected override CredentialApplicationStatusTypeName[] ApplicationEntryStates => new[] { CredentialApplicationStatusTypeName.InProgress };
        protected override CredentialApplicationStatusTypeName ApplicationExitState => GetExitState();


        protected override CredentialRequestStatusTypeName RequiredCredentialRequestStatus => CredentialRequestStatusTypeName.ToBeIssued;

        private readonly IList<(ApplicationStateAction Action, WorkPracticeCredentialDto Credential)> _credentialActions = new List<(ApplicationStateAction Action, WorkPracticeCredentialDto Credential)>();
        private IssueCredentialDataModel _issueCredentialDataModel;

        protected override SecurityNounName? RequiredSecurityNoun => SecurityNounName.Credential;
        protected override SecurityVerbName? RequiredSecurityVerb => SecurityVerbName.Issue;

        protected override IList<Action> Preconditions => new List<Action>
        {
            ValidateEntryState,
            ValidateUserPermissions,
        };

        protected override IList<Action> SystemActions => new List<Action>
        {
            ProcesssCredentials,
            ClearOwner,
            CreateNote
            //SetExitState
        };


        protected override bool FulFillPrerequisites()
        {
            return base.FulFillPrerequisites() && ValidateCertificationPeriod();
        }

        private CredentialApplicationStatusTypeName GetExitState()
        {
            throw new NotSupportedException("This method should not be called because exit state must be set by the issue credential action");
        }

        protected override bool ValidateCredentialRequests()
        {
            _credentialActions.Clear();
            return base.ValidateCredentialRequests();
        }

        private bool ValidateCertificationPeriod()
        {
            var currentCertificationPeriodId = ApplicationModel.ApplicationInfo.CertificationPeriodId;
            if (!_credentialActions.Any())
            {
                return false;
            }
            var credentialRequestId = _credentialActions.First().Action.WizardModel.CredentialRequestId;
            var issueCredentialStepModel = ApplicationService.GetWizardIssueCredentialData(ApplicationModel.ApplicationInfo.ApplicationId, credentialRequestId, (int)SystemActionTypeName.IssueCredential).Data;
            if (!issueCredentialStepModel.CertificationPeriods.Any())
            {
                LoggingHelper.LogError($"Error ExecutingAction{{Action}}. No certification periods found. {{ApplicationId}}", nameof(ApplicationIssueRecertificationCredentialsAction), ApplicationModel.ApplicationInfo.ApplicationId);
                Messages.Add($"No certification periods found");
                return false;
            }

            var firstPeriod = issueCredentialStepModel.CertificationPeriods.FirstOrDefault(x => x.Id == 0);
            if (firstPeriod == null)
            {
                firstPeriod = issueCredentialStepModel.CertificationPeriods.FirstOrDefault(x => x.Id > currentCertificationPeriodId);
            }

            if (firstPeriod == null)
            {
                LoggingHelper.LogError($"Error ExecutingAction{{Action}}. No certification period found. {{ApplicationId}}", nameof(ApplicationIssueRecertificationCredentialsAction), ApplicationModel.ApplicationInfo.ApplicationId);
                Messages.Add($"No certification periods found");
                return false;
            }

            issueCredentialStepModel.SelectedCertificationPeriodId = firstPeriod.Id;

            _issueCredentialDataModel = AutoMapperHelper.Mapper.Map<IssueCredentialDataModel>(issueCredentialStepModel);
            _issueCredentialDataModel.CertificationPeriodStart = firstPeriod.StartDate;
            _issueCredentialDataModel.CertificationPeriodEnd = firstPeriod.EndDate;

            return true;
        }

        protected override void ProcesssCredentials()
        {
            if (!ApplicationModel.ApplicantDetails.AllowAutoRecertification || !PrerequisitesMet || ApplicationModel.CredentialRequests.Any(x => x.StatusTypeId != (int)RequiredCredentialRequestStatus))
            {
                _credentialActions.Clear();
                UpdateCredentialRequestStatus(CredentialRequestStatusTypeName.ReadyForAssessment);
            }
            else
            {
                IssueCredentials();
            }
        }


        protected override bool ValidateCredentialAction(int credentialRequestId, WorkPracticeCredentialDto credential)
        {
            var actionType = SystemActionTypeName.IssueCredential;
            var copiedModel = CloneWizard(actionType);
            copiedModel.CredentialRequestId = credentialRequestId;

            var copiedApplicationModel = CloneApplicationModel();
            var action = CreateAction(actionType, copiedApplicationModel, copiedModel);
            if (!action.ArePreconditionsMet())
            {
                action.ValidationErrors.ForEach(x => Messages.Add(x.Message));
                Messages.Add(Naati.Resources.Application.CredentialPreconditionsNotMet);

                return false;
            }
            _credentialActions.Add((action, credential));
            return true;
        }
        private void IssueCredentials()
        {

            if (_issueCredentialDataModel.SelectedCertificationPeriodId == 0)
            {
                var result = PersonService.CreateCertificationPeriod(new CreateCertificationPeriodModel
                {
                    PersonId = ApplicationModel.ApplicantDetails.PersonId,
                    StartDate = _issueCredentialDataModel.CertificationPeriodStart.GetValueOrDefault(),
                    EndDate = _issueCredentialDataModel.CertificationPeriodEnd.GetValueOrDefault(),
                    OriginalEndDate = _issueCredentialDataModel.CertificationPeriodEnd.GetValueOrDefault(),
                    CredentialApplicationId = ApplicationModel.ApplicationInfo.ApplicationId,
                });

                _issueCredentialDataModel.SelectedCertificationPeriodId = result.Id;
                _issueCredentialDataModel.CertificationPeriods.First(x => x.Id == 0).Id = result.Id;

            }

            foreach (var actionInfo in _credentialActions)
            {
                var action = actionInfo.Action;

                var issueModel = CloneIssueCredentialDataModel(_issueCredentialDataModel);
                issueModel.StartDate = actionInfo.Credential.StartDate;
                issueModel.ExpiryDate = _issueCredentialDataModel.CertificationPeriodEnd;
                action.WizardModel.SetIssueCredentialData(issueModel);

                action.Perform();
            }
        }

        public override void SaveChanges()
        {
            foreach (var applicationStateInfo in _credentialActions)
            {
                applicationStateInfo.Action.SaveChanges();
            }
            base.SaveChanges();

            Output.UpsertResults.Messages.AddRange(Messages);

        }

        protected override string GetNote()
        {
            return string.Join(". ", Messages.Select(x =>
            {
                var value = x ?? string.Empty;
                if (value.EndsWith("."))
                {
                    value = value.Substring(0, value.Length - 1);
                }
                return value;
            }).ToList());
        }

        protected override void CreateNote()
        {
            var noteModel = new ApplicationNoteModel
            {
                ApplicationId = ApplicationModel.ApplicationInfo.ApplicationId,
                CreatedDate = DateTime.Now,
                Note = string.Empty,
                UserId = CurrentUser.Id,
                ReadOnly = true
            };

            noteModel.Note = GetNote();
            if (!CanSendEmail())
            {
                noteModel.Note = $"{noteModel.Note ?? string.Empty} - {Naati.Resources.Application.EmailSkipped}";
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
                ApplicationModel.Notes.Add(noteModel);
            }
        }
    }
}
