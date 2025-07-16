using F1Solutions.Global.Common;
using F1Solutions.Global.Common.Logging;
using F1Solutions.Naati.Common.Contracts.Bl.DTO;
using F1Solutions.Naati.Common.Contracts.Dal;
using F1Solutions.Naati.Common.Contracts.Dal.DTO;
using F1Solutions.Naati.Common.Contracts.Dal.Enum;
using F1Solutions.Naati.Common.Contracts.Dal.Request;
using F1Solutions.Naati.Common.Contracts.Security;
using Ncms.Contracts;
using Ncms.Contracts.Models;
using Ncms.Contracts.Models.Application;
using Ncms.Contracts.Models.Person;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.IO;
using System.Linq;
using System.Text;

namespace Ncms.Bl.ApplicationActions
{
    public class CredentialRequestIssueCredentialAction : CredentialRequestStateAction
    {

        protected override SecurityNounName? RequiredSecurityNoun => SecurityNounName.Credential;
        protected override SecurityVerbName? RequiredSecurityVerb => SecurityVerbName.Issue;
        protected override CredentialRequestStatusTypeName[] CredentialRequestEntryStates => new[] { CredentialRequestStatusTypeName.AssessmentComplete, CredentialRequestStatusTypeName.IssuedPassResult, CredentialRequestStatusTypeName.ToBeIssued, CredentialRequestStatusTypeName.CredentialOnHold, CredentialRequestStatusTypeName.OnHoldToBeIssued};

        protected override CredentialRequestStatusTypeName CredentialRequestExitState => CredentialRequestStatusTypeName.CertificationIssued;

        protected bool RollbackRequired { get; set; }

        protected override IList<Action> Preconditions => new List<Action>
                                                          {
                                                              ValidateEntryState,
                                                              ValidateUserPermissions,
                                                              ValidateApplicationState,
                                                              ValidateMandatoryFields,
                                                              ValidateMandatoryDocuments,
                                                              ValidateMandatoryPersonFields,
                                                              ValidateCredentialRequestInvoices
                                                          };

        protected override IList<Action> SystemActions => new List<Action>
                                                          {
                                                              DeleteCredentialPreviewFiles,
                                                              ClearOwner,
                                                              CreateNote,
                                                              SetApplicationStatus,
                                                              SetExitState,
                                                              MoveOnHoldCredentialsToOnHoldToBeIssued,
                                                              CreateCredential,
                                                              CreatePersonNotes,
                                                              AssignPractitionNumber
                                                          };


        protected void MoveOnHoldCredentialsToOnHoldToBeIssued()
        {
            if (WizardModel.Data == null)
            {
                return;
            }

            var onHoldCredentialToIssueDataIndex = -1;
            var count = 0;

            foreach (var data in WizardModel.Data)
            {
                // Step Id for Issue On Hold Credentials is 24
                if (data.Id == 24)
                {
                    onHoldCredentialToIssueDataIndex = count;
                    break;
                }

                count++;
            }

            if (onHoldCredentialToIssueDataIndex == -1)
            {
                return;
            }

            var onHoldCredentialToIssueData = JsonConvert.DeserializeObject<IssueOnHoldCredentialRequest>(WizardModel.Data[count].Data.ToString());

            if (!onHoldCredentialToIssueData.Checked)
            {
                return;
            }

            foreach (var onHoldCredentialToIssue in onHoldCredentialToIssueData.OnHoldCredentials)
            {
                onHoldCredentialToIssue.CurrentUserId = CurrentUser.Id;

                var response = ApplicationService.UpdateOnHoldCredentialToOnHoldToBeIssued(onHoldCredentialToIssue);

                if (!response.Success)
                {
                    var errors = new StringBuilder();

                    foreach (var error in response.Errors)
                    {
                        errors.AppendLine(error);
                    }

                    throw new UserFriendlySamException(errors.ToString());
                }

                var noteModel = new ApplicationNoteModel
                {
                    ApplicationId = onHoldCredentialToIssue.CredentialApplicationId,
                    CreatedDate = DateTime.Now,
                    Note = string.Empty,
                    UserId = CurrentUser.Id,
                    ReadOnly = true
                };

                noteModel.Note = GetOnHoldToOnHoldToBeIssuedNote(onHoldCredentialToIssue);

                if (string.IsNullOrWhiteSpace(noteModel.Note))
                {
                    continue;
                }

                var createNotesForOnHoldToBeIssuedResponse = ApplicationService.CreateApplicationNotesForOnHoldToBeIssued(noteModel);

                if (!createNotesForOnHoldToBeIssuedResponse.Success)
                {
                    throw new UserFriendlySamException(createNotesForOnHoldToBeIssuedResponse.Errors.ToString());
                }

                continue;
            }
        }

        protected void CreateCredential()
        {
            var issueCredentialStepData = WizardModel.IssueCredentialData;
            var credentialRequest = ApplicationModel.CredentialRequests
                .FirstOrDefault(x => x.Id == WizardModel.CredentialRequestId);

            if (credentialRequest == null)
            {
                throw new Exception($"CredentialRequest {WizardModel.CredentialRequestId} was not found");
            }

            var credential = new CredentialModel
            {
                StartDate = issueCredentialStepData.StartDate,
                ShowInOnlineDirectory = issueCredentialStepData.ShowInOnlineDirectory,
                SkillId = credentialRequest.SkillId, // required for recertification
                CredentialTypeId = credentialRequest.CredentialTypeId, // required for recertification
            };

            if (credentialRequest.CredentialType.Certification)
            {
                if (!issueCredentialStepData.SelectedCertificationPeriodId.HasValue || issueCredentialStepData.SelectedCertificationPeriodId.Value.ToString() == "")
                {
                    throw new Exception("CreateCredential: Certification Period ID was not provided.");
                }

                if (!issueCredentialStepData.CertificationPeriodStart.HasValue)
                {
                    throw new Exception("CreateCredential: Certification Period Start Date has no value.");
                }
                if (!issueCredentialStepData.CertificationPeriodEnd.HasValue)
                {
                    throw new Exception("CreateCredential: Certification Period Start Date has no value.");
                }

                if (issueCredentialStepData.CertificationPeriodStart.Value < (DateTime)SqlDateTime.MinValue ||
                    issueCredentialStepData.CertificationPeriodEnd.Value < (DateTime)SqlDateTime.MinValue)
                {
                    throw new Exception("CreateCredential: Certification Period Start or End time was below minimum value.");
                }

                if (ApplicationModel.ApplicationInfo.ApplicationId <= 0)
                {
                    throw new Exception("CreateCredential: Application ID was not above 0.");
                }

                var certificationPeriod = new CertificationPeriodModel
                {
                    Id = (int)issueCredentialStepData.SelectedCertificationPeriodId.Value,
                    StartDate = issueCredentialStepData.CertificationPeriodStart.Value,
                    EndDate = issueCredentialStepData.CertificationPeriodEnd.Value,
                    OriginalEndDate = issueCredentialStepData.CertificationPeriodEnd.Value,
                    CredentialApplicationId = ApplicationModel.ApplicationInfo.ApplicationId //>0
                };

                credential.CertificationPeriod = certificationPeriod;
                credential.ExpiryDate = null;
            }
            else
            {
                credential.ExpiryDate = issueCredentialStepData.ExpiryDate.Value;
            }

            Output.PendingCredential = credential;
        }

        protected void AssignPractitionNumber()
        {
            var issueCredentialStepData = WizardModel.IssueCredentialData;
            if (CredentialRequestModel.CredentialType.Certification && String.IsNullOrEmpty(ApplicationModel.ApplicantDetails.PractitionerNumber))
            {
                PersonService.AssignPractitionerNumber(ApplicationModel.ApplicantDetails.PersonId, issueCredentialStepData.PractitionerNumber);
            }
        }

        protected void DeleteCredentialPreviewFiles()
        {
            if (WizardModel.CredentialPreviewFiles != null)
            {
                foreach (var fileData in WizardModel.CredentialPreviewFiles)
                {
                    if (File.Exists(fileData.FilePath))
                    {
                        File.Delete(fileData.FilePath);
                    }
                }
            }
        }

        protected override IEnumerable<string> GetPersonNotes()
        {
            var notes = new List<string>();
            var dateFormat = "dd MM yyyy";

            if (Output.PendingCredential?.CertificationPeriod != null)
            {
                notes.Add(string.Format(Naati.Resources.Application.CredentialCertificationIssuedNote,
                    CredentialRequestModel.CredentialType.ExternalName,
                    CredentialRequestModel.Direction,
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
                    CredentialRequestModel.CredentialType.ExternalName,
                    CredentialRequestModel.Direction,
                    Output.PendingCredential.StartDate.ToString(dateFormat),
                    endDate));
            }

            return notes;
        }

        public virtual CreateCredentialDocumentsRequestModel GetCredentialDocuments(DocumentsPreviewRequestModel request)
        {

            var model = new CreateCredentialDocumentsRequestModel
            {
                CredentialName = CredentialRequestModel.CredentialType.ExternalName,
                CredentialTypeId = CredentialRequestModel.CredentialTypeId,
                Skill = CredentialRequestModel.Skill.DisplayName,
                NaatiNumber = ApplicationModel.ApplicantDetails.NaatiNumber,
                UserId = CurrentUser.Id,
                ApplicationId = ApplicationModel.ApplicationInfo.ApplicationId,
                CredentialExpiryDate = request.CredentialExpiryDate,
                CredentialStartDate = request.CredentialStartDate,
                CredentialId = 0,
                PractitionerNumber = request.PractitionerNumber,
            };
            return model;
        }

        protected virtual void ValidateExistingCredential(CredentialModel existingCredential)
        {
            if (existingCredential.CertificationPeriod.Id == Output.PendingCredential.CertificationPeriod.Id)
            {
                throw new UserFriendlySamException(Naati.Resources.Application.InvalidRecertificationCertificationPeriod);
            }
        }

        protected virtual int GetExistingCredentialId()
        {
            // if this is a recertification application, we won't create a new credential, we'll just update the existing one
            // this needs to be done before updating the application, so the recertification status is still BeingAssessed
            if (ApplicationModel.ApplicationType.Category == CredentialApplicationTypeCategoryName.Recertification)
            {
                var existingCred = ApplicationService.GetCredentialBeingRecertified(ApplicationModel.ApplicationInfo.NaatiNumber, Output.PendingCredential.SkillId,
                    Output.PendingCredential.CredentialTypeId);
                existingCred.NotNull("Can't find existing credential being recertified");
                ValidateExistingCredential(existingCred);
                return existingCred.Id;

            }

            // not required for other types of application as we will create a new credential
            return 0;
        }

        public override void SaveChanges()
        {
            var credential = CredentialRequestModel.Credentials.FirstOrDefault();
            var applicationOriginalState = ApplicationService.GetApplicationDetails(ApplicationModel.ApplicationInfo.ApplicationId);
            var credentialRequestOriginalState = applicationOriginalState.CredentialRequests.First(x => x.Id == CredentialRequestModel.Id);

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

            var credRequest = new CreateOrUpdateCredentialModel
            {
                CertificationPeriod = period,
                StartDate = Output.PendingCredential.StartDate,
                ExpiryDate = Output.PendingCredential.ExpiryDate,
                TerminationDate = Output.PendingCredential.TerminationDate,
                ShowInOnlineDirectory = Output.PendingCredential.ShowInOnlineDirectory
            };
            credRequest.CredentialRequestId = CredentialRequestModel.Id;
            credRequest.CredentialId = GetExistingCredentialId();

            RollbackRequired = true;
            try
            {
                Output.PendingCredential = ApplicationService.CreateOrUpdateCredential(credRequest);
                base.SaveChanges();
                RollbackRequired = false;
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
                            Credential = credential,
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

            var documentsRequest = GetCredentialDocuments(new DocumentsPreviewRequestModel
            {
                ActionId = WizardModel.ActionType,
                ApplicationId = ApplicationModel.ApplicationInfo.ApplicationId,
                // set expiry date using certification period if the credential has no Expiry Date
                CredentialExpiryDate = WizardModel.IssueCredentialData.ExpiryDate != null ? WizardModel.IssueCredentialData.ExpiryDate : WizardModel.IssueCredentialData.CertificationPeriodEnd,
                CredentialStartDate = WizardModel.IssueCredentialData.StartDate,
            });

            documentsRequest.CredentialId = Output.PendingCredential.Id;
            var documents = ApplicationService.SaveCredentialDocuments(documentsRequest);
            Attachments.AddRange(documents.StoredFileIds.Select(x => new EmailMessageAttachmentModel() { StoredFileId = x, AttachmentType = AttachmentType.CredentialDocument }));

        }

        protected override void ConfigureInstance(CredentialApplicationDetailedModel application, ApplicationActionWizardModel wizardModel, ApplicationActionOutput output)
        {
            base.ConfigureInstance(application, wizardModel, output);
            if (wizardModel.IsBackgroundAction && wizardModel.IssueCredentialData == null)
            {
                var issueCredentialData = ApplicationService.GetWizardIssueCredentialData(application.ApplicationInfo.ApplicationId,
                        wizardModel.CredentialRequestId, wizardModel.ActionType).Data;

                var issueCredentialDataModel = AutoMapperHelper.Mapper.Map<IssueCredentialDataModel>(issueCredentialData);

                issueCredentialDataModel.CertificationPeriodStart = issueCredentialData.CertificationPeriods.FirstOrDefault() == null ? null : (DateTime?)issueCredentialData.CertificationPeriods.FirstOrDefault().StartDate;
                issueCredentialDataModel.CertificationPeriodEnd = issueCredentialData.CertificationPeriods.FirstOrDefault() == null ? null : (DateTime?)issueCredentialData.CertificationPeriods.FirstOrDefault().EndDate;

                wizardModel.SetIssueCredentialData(issueCredentialDataModel);
            }
        }

        private string GetOnHoldToOnHoldToBeIssuedNote(OnHoldCredential onHoldCredentialToBeIssued)
        {
            return string.Format(Naati.Resources.Application.OnHoldToOnHoldToBeIssuedNote,
                onHoldCredentialToBeIssued.CredentialTypeAndSkillDisplayText);
        }
    }
}