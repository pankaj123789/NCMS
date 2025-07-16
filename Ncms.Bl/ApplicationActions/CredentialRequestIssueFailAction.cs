using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using F1Solutions.Global.Common.Logging;
using F1Solutions.Global.Common.Mapping;
using F1Solutions.Naati.Common.Contracts.Bl.DTO;
using F1Solutions.Naati.Common.Contracts.Dal;
using F1Solutions.Naati.Common.Contracts.Dal.DTO;
using F1Solutions.Naati.Common.Contracts.Dal.Enum;
using F1Solutions.Naati.Common.Contracts.Security;
using Ncms.Contracts;
using Ncms.Contracts.Models;
using Ncms.Contracts.Models.Application;
using Ncms.Contracts.Models.Person;

namespace Ncms.Bl.ApplicationActions
{
    public class CredentialRequestIssueFailAction : CredentialRequestStateAction
    {
        protected override CredentialRequestStatusTypeName[] CredentialRequestEntryStates => new[] { CredentialRequestStatusTypeName.TestSat };

        protected override CredentialRequestStatusTypeName CredentialRequestExitState => CredentialRequestStatusTypeName.TestFailed;

        protected override SecurityNounName? RequiredSecurityNoun => SecurityNounName.TestResult;
        protected override SecurityVerbName? RequiredSecurityVerb => SecurityVerbName.Issue;

        protected override TestResultStatusTypeName RequiredTestResultStatus => TestResultStatusTypeName.Failed;
        protected bool RollbackRequired { get; set; }

        protected CredentialRequestModel ConcededCredentialRequest { get; set; }

        private DowngradedCredentialRequestModel _downgradedCredential;
        protected DowngradedCredentialRequestModel DowngradedCredential => _downgradedCredential ?? (_downgradedCredential = GetDowngradeCredential());

        private DowngradedCredentialRequestModel GetDowngradeCredential()
        {
            return ApplicationService.GetDowngradedCredentialRequest(CredentialRequestModel.Id);
        }


        protected GenericResponse<UpsertApplicationResultModel> SavedApplication { get; set; }
        protected override IList<Action> Preconditions => new List<Action>
        {
            ValidateEntryState,
            ValidateUserPermissions,
            ValidateExaminers,
            ValidateAllowIssue,
            ValidateTestResultStatus,
            ValidateStandardMarks
        };
        

        protected override IList<Action> SystemActions => new List<Action>
        {
            ClearOwner,
            CreateNote,
            ValidateStandardSupplementaryComponentResults,
            ValidateRubricSupplementaryComponentResults,
            SetStandardSupplementaryComponentResults,
            SetRubricSupplementaryComponentResults,
            CreateConcededCredentialRequest,
            SetApplicationStatus,
            SetExitState,
            CreateCredential,
            CreatePersonNotes,
            AssignPractitionNumber
        };

        protected void AssignPractitionNumber()
        {

            if (AllowConcededPass && DowngradedCredential?.Certification == true && String.IsNullOrEmpty(ApplicationModel.ApplicantDetails.PractitionerNumber))
            {
                var stepData = WizardModel.IssueCredentialStep.Data;
                PersonService.AssignPractitionerNumber(ApplicationModel.ApplicantDetails.PersonId, stepData.PractitionerNumber?.Value);
            }
        }

        public  override void CreatePersonNotes()
        {
            if (Output.PendingCredential == null)
            {
                return;
            }

            base.CreatePersonNotes();
        }

        protected override IEnumerable<string> GetPersonNotes()
        {
            
            var notes = new List<string>();
            var dateFormat = "dd MM yyyy";

            if (Output.PendingCredential?.CertificationPeriod != null)
            {
                notes.Add(string.Format(Naati.Resources.Application.CredentialCertificationIssuedNote,
                    DowngradedCredential.CredentialTypeExternalName,
                    DowngradedCredential.Skill,
                    Output.PendingCredential?.CertificationPeriod.StartDate.ToString(dateFormat),
                    Output.PendingCredential?.CertificationPeriod.EndDate.ToString(dateFormat),
                    Output.PendingCredential.StartDate.ToString(dateFormat)));

                if (Output.PendingCredential.CertificationPeriod.Id == 0)
                {
                    notes.Add(string.Format(Naati.Resources.Application.NewCertificationPeriodNote,
                        Output.PendingCredential?.CertificationPeriod.StartDate.ToString(dateFormat),
                        Output.PendingCredential?.CertificationPeriod.EndDate.ToString(dateFormat)
                    ));
                }
            }
            else if (Output.PendingCredential != null)
            {
                var endDate = Output.PendingCredential.ExpiryDate.HasValue
                    ? string.Format(Naati.Resources.Application.CredentialExpiryDate,
                        Output.PendingCredential.ExpiryDate.Value.ToString(dateFormat))
                    : Naati.Resources.Application.CredentialNotExpiryDate;

                notes.Add(string.Format(Naati.Resources.Application.CredentialIssuedNote,
                    DowngradedCredential.CredentialTypeExternalName,
                    DowngradedCredential.Skill,
                    Output.PendingCredential.StartDate.ToString(dateFormat),
                    endDate));
            }

            return notes;
        }

        protected override IList<ActionEventLevel> GetActionEvents()
        {
            var level = 0;
            var events = new List<ActionEventLevel>
            {
                new ActionEventLevel {Level = level, Event = SystemActionEventTypeName.None},
            };

            if (AllowSupplementary)
            {
                events.Add(new ActionEventLevel { Level = level, Event = SystemActionEventTypeName.SupplementaryTestOffered, });
            }

            if (AllowConcededPass && DowngradedCredential?.HasCredential == false)
            {
                events.Add(new ActionEventLevel { Level = level, Event = SystemActionEventTypeName.ConcededPassOffered, });
            }

            return events;
        }

        protected override void ValidateStandardSupplementaryComponentResults()
        {
            if (AllowSupplementary)
            {
                base.ValidateStandardSupplementaryComponentResults();
            }
        }

        private void CreateConcededCredentialRequest()
        {
            if (!AllowConcededPass)
            {
                return;
            }


            var downgradeCredential = DowngradedCredential;
            if (downgradeCredential.HasCredential)
            {
                return;
            }

            ConcededCredentialRequest = new CredentialRequestModel
            {
                Id = 0,
                CredentialTypeId = downgradeCredential.CredentailTypeId,
                CategoryId = downgradeCredential.CategorId,
                SkillId = downgradeCredential.SkillId,
                StatusChangeUserId = CurrentUser.Id,
                StatusChangeDate = DateTime.Now,
                CredentialRequestPathTypeId = (int)CredentialRequestPathTypeName.Conceded,
                CredentialId = WizardModel.CredentialId,
                WorkPractices = new List<WorkPracticeDataModel>(),
                ConcededFromCredentialRequestId = CredentialRequestModel.Id,
                StatusTypeId = (int)CredentialRequestStatusTypeName.CertificationIssued
            };

            ApplicationModel.CredentialRequests.Add(ConcededCredentialRequest);

            var noteModel = new ApplicationNoteModel
            {
                ApplicationId = ApplicationModel.ApplicationInfo.ApplicationId,
                CreatedDate = DateTime.Now,
                Note = string.Format(Naati.Resources.Application.ConcededPassCredentialCreated, downgradeCredential.CredentialTypeInternalName, downgradeCredential.Skill),
                UserId = CurrentUser.Id,
                ReadOnly = true,

            };
            ApplicationModel.Notes.Add(noteModel);

        }

        protected override void ValidateRubricSupplementaryComponentResults()
        {
            if (AllowSupplementary)
            {
                base.ValidateRubricSupplementaryComponentResults();
            }
        }

        protected override void GetEmailTokens(Dictionary<string, string> tokenDictionary)
        {
            GetStandardTestResultEmailTokens(tokenDictionary);
            GetRubricTestResultEmailTokens(tokenDictionary);
            if (AllowConcededPass)
            {
                tokenDictionary.Add(TokenReplacementService.GetTokenNameFor(TokenReplacementField.ConcededCredentialType), DowngradedCredential?.CredentialTypeExternalName ?? string.Empty);
            }
          
        }

        protected override bool IsTaskSelected(ITestComponentModel component)
        {
            return WizardModel.SupplementaryTestComponents.Any(x => x == component.Id);
        }


        private void SetStandardSupplementaryComponentResults()
        {
            if (!AllowSupplementary)
            {
                return;
            }

            if (TestSpecification.MarkingSchemaTypeId != (int)MarkingSchemaTypeName.Standard)
            {
                return;
            }

            foreach (var component in StandardMarks.Components)
            {
                if (!IsTaskSelected(component))
                {
                    component.MarkingResultTypeId = (int)MarkingResultTypeName.EligableForSupplementary;
                    ApplicationModel.StandardTestComponentModelsToUpdate.Add(component);
                }
            }
        }

        private void SetRubricSupplementaryComponentResults()
        {
            if (!AllowSupplementary)
            {
                return;
            }

            if (TestSpecification.MarkingSchemaTypeId != (int)MarkingSchemaTypeName.Rubric)
            {
                return;
            }

            foreach (var component in RubricMarks.TestComponents)
            {
                if (!IsTaskSelected(component))
                {
                    component.MarkingResultTypeId = (int)MarkingResultTypeName.EligableForSupplementary;
                    ApplicationModel.RubricTestComponentModelsToUpdate.Add(component);
                }
            }
        }
        protected void CreateCredential()
        {
            if (ConcededCredentialRequest == null)
            {
                return;
            }

            if (WizardModel.IssueCredentialStep == null)
            {
                throw new Exception("IssueCredentialStep not found");
            }

            var stepData = WizardModel.IssueCredentialStep.Data;
            var credential = new CredentialModel
            {
                StartDate = stepData.StartDate.Value,
                ShowInOnlineDirectory = stepData.ShowInOnlineDirectory.Value,
                SkillId = ConcededCredentialRequest.SkillId,
                CredentialTypeId = ConcededCredentialRequest.CredentialTypeId,
            };

            if (DowngradedCredential.Certification)
            {
                if (stepData.SelectedCertificationPeriodId.Value.ToString() == "")
                {
                    throw new Exception("CreateCredential: Certification Period ID was not provided.");
                }

                var certificationPeriod = new CertificationPeriodModel
                {
                    Id = (int)stepData.SelectedCertificationPeriodId.Value,
                    StartDate = stepData.CertificationPeriodStart.Value,
                    EndDate = stepData.CertificationPeriodEnd.Value,
                    OriginalEndDate = stepData.CertificationPeriodEnd.Value,
                    CredentialApplicationId = ApplicationModel.ApplicationInfo.ApplicationId
                };

                credential.CertificationPeriod = certificationPeriod;
                credential.ExpiryDate = null;
            }
            else
            {
                credential.ExpiryDate = stepData.ExpiryDate.Value;
            }

            Output.PendingCredential = credential;
        }
        protected override GenericResponse<UpsertApplicationResultModel> SaveActionData()
        {
            return SavedApplication;

        }

        public override void SaveChanges()
        {
            var applicationOriginalState = ApplicationService.GetApplicationDetails(ApplicationModel.ApplicationInfo.ApplicationId);
            var credentialRequestOriginalState = applicationOriginalState.CredentialRequests.First(x => x.Id == CredentialRequestModel.Id);
            RollbackRequired = true;
            try
            {
                var request = AutoMapperHelper.Mapper.Map<UpsertApplicationRequestModel>(ApplicationModel);
                request.StandardTestComponents = ApplicationModel?.StandardTestComponentModelsToUpdate;
                request.RubricTestComponents = ApplicationModel?.RubricTestComponentModelsToUpdate;
                request.Recertification = ApplicationModel?.Recertification;
                request.PdActivities = ApplicationModel?.PdActivities;
                SavedApplication = ApplicationService.UpsertApplication(request);

                var exsitingRequests = applicationOriginalState.CredentialRequests.Select(x => x.Id);
                var concededRequestId = SavedApplication.Data.CredentialRequestIds.FirstOrDefault(x => exsitingRequests.All(y => y != x));

                if (concededRequestId > 0)
                {
                    // create or update the credential
                    CertificationPeriodDto period = null;
                    if (Output.PendingCredential.CertificationPeriod != null)
                    {
                        period = new CertificationPeriodDto
                        {
                            Id = Output.PendingCredential.CertificationPeriod.Id,
                            NaatiNumber = Output.PendingCredential.CertificationPeriod.NaatiNumber,
                            Notes = Output.PendingCredential.CertificationPeriod.Notes,
                            StartDate = Output.PendingCredential.CertificationPeriod.StartDate,
                            EndDate = Output.PendingCredential.CertificationPeriod.EndDate,
                            OriginalEndDate = Output.PendingCredential.CertificationPeriod.OriginalEndDate,
                            CertificationPeriodStatus = Output.PendingCredential.CertificationPeriod.CertificationPeriodStatus,
                            CredentialApplicationId = Output.PendingCredential.CertificationPeriod.CredentialApplicationId
                        };
                    }

                    var createUpdatedRequest = new CreateOrUpdateCredentialModel
                    {
                        CertificationPeriod = period,
                        StartDate = Output.PendingCredential.StartDate,
                        ExpiryDate = Output.PendingCredential.ExpiryDate,
                        TerminationDate = Output.PendingCredential.TerminationDate,
                        ShowInOnlineDirectory = Output.PendingCredential.ShowInOnlineDirectory
                    };
                  
                    createUpdatedRequest.CredentialRequestId = concededRequestId;
                    Output.PendingCredential = ApplicationService.CreateOrUpdateCredential(createUpdatedRequest);
                }
                
                base.SaveChanges();
            }
            catch (Exception ex1)
            {
                if (RollbackRequired)
                {
                    try
                    {

                        ApplicationService.RollbackIssueCredential(new RollbackIssueCredentialModel
                        {
                            ApplicationId = ApplicationModel.ApplicationInfo.ApplicationId,
                            CredentialRequestId = CredentialRequestModel.Id,
                            ApplicationOriginalStatusId = applicationOriginalState.ApplicationStatus.Id,
                            ApplicationOriginalStatusDate = applicationOriginalState.ApplicationInfo.StatusChangeDate,
                            ApplicationOriginalStatusUserId = applicationOriginalState.ApplicationInfo.StatusChangeUserId,
                            CredentialRequestOriginalStatusId = credentialRequestOriginalState.StatusTypeId,
                            CredentialRequestOriginalStatusDate = credentialRequestOriginalState.StatusChangeDate,
                            CredentialRequestOriginalStatusUserId = credentialRequestOriginalState.StatusChangeUserId,
                            Credential = null,
                            StoredFileIds = Attachments.Where(x => x.AttachmentType == AttachmentType.CredentialDocument).Select(y => y.StoredFileId),
                        });
                        LoggingHelper.LogError("APP{ApplicationId} CR{CredentialRequestId} Issue Credential action rolled back due to exception", ApplicationModel.ApplicationInfo.ApplicationId, CredentialRequestModel.Id);
                    }
                    catch (Exception ex2)
                    {
                        throw new Exception(
                            $"While trying to roll back the Issue Credential action for APP{ApplicationModel.ApplicationInfo.ApplicationId}/CR{CredentialRequestModel.Id} due to an error, another error occurred. The original error was: {ex1.Message}; the second error was: {ex2.Message}.",
                            ex2);
                    }
                }
                throw;
            }
        }

        protected override IEnumerable<BusinessServiceResponse> SendEmails()
        {
            RollbackRequired = false;
            var responses = new List<BusinessServiceResponse>();
            foreach (var emailModel in Output.PendingEmails)
            {
                var emailSendResponse = EmailMessageService.SendEmailMessageById(emailModel.EmailMessageId);

                responses.Add(emailSendResponse);
            }
            return responses;
        }
        public override IEnumerable<DocumentData> GetDocumentsPreview(DocumentsPreviewRequestModel request)
        {
            var credentialDocumentModel = GetCredentialDocuments(request);
            var respose = ApplicationService.CreateCredentialDocuments(credentialDocumentModel);
            return respose.Data;
        }
        protected override void CreateEmailAttachmentsIfApplicable()
        {
            // create the credential documents
            var stepData = WizardModel.IssueCredentialStep?.Data;
            if (stepData == null)
            {
                return;
            }
            var documentsRequest = GetCredentialDocuments(new DocumentsPreviewRequestModel
            {
                ActionId = WizardModel.ActionType,
                ApplicationId = ApplicationModel.ApplicationInfo.ApplicationId,
                CredentialExpiryDate = stepData.ExpiryDate.Value,
                CredentialStartDate = stepData.StartDate.Value,
            });

            documentsRequest.CredentialId = Output.PendingCredential.Id;
            var documents = ApplicationService.SaveCredentialDocuments(documentsRequest);
            Attachments.AddRange(documents.StoredFileIds.Select(x => new EmailMessageAttachmentModel() { StoredFileId = x, AttachmentType = AttachmentType.CredentialDocument }));

        }

        protected virtual CreateCredentialDocumentsRequestModel GetCredentialDocuments(DocumentsPreviewRequestModel request)
        {

            var concededCredential = DowngradedCredential;
            var model = new CreateCredentialDocumentsRequestModel
            {
                CredentialName = concededCredential.CredentialTypeExternalName,
                CredentialTypeId = CredentialRequestModel.CredentialTypeId,
                Skill = concededCredential.Skill,
                NaatiNumber = concededCredential.NaatiNumber,
                UserId = CurrentUser.Id,
                ApplicationId = ApplicationModel.ApplicationInfo.ApplicationId,
                CredentialExpiryDate = request.CredentialExpiryDate,
                CredentialStartDate = request.CredentialStartDate,
                CredentialId = 0,
                PractitionerNumber = request.PractitionerNumber,
            };
            return model;
        }

    }
}
