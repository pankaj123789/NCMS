using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using AutoMapper;
using F1Solutions.Global.Common;
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
using Ncms.Contracts.Models.Accounting;
using Ncms.Contracts.Models.Application;
using Ncms.Contracts.Models.Common;
using Ncms.Contracts.Models.File;
using F1Solutions.Naati.Common.Contracts.Dal.QueryServices;

namespace Ncms.Bl.ApplicationActions
{
    public class ApplicationStateAction : SystemAction<CredentialApplicationDetailedModel, ApplicationActionWizardModel, ApplicationActionOutput, UpsertApplicationResultModel, CredentialApplicationEmailMessageModel>
    {
        private readonly IPayrollQueryService mPayrollService;
        protected virtual CredentialApplicationStatusTypeName CurrentEntryState { get; private set; }
        public ApplicationStateAction(IServiceLocator serviceLocator = null) : base(serviceLocator)
        {
            mPayrollService = ServiceLocatorInstance.Resolve<IPayrollQueryService>();
        }

        internal CredentialApplicationDetailedModel ApplicationModel => ActionModel;

        protected bool Sponsored => ApplicationModel.ApplicationInfo.SponsorInstitutionId.HasValue;

        protected virtual SystemActionTypeName? OnInvoicePaymentActionType { get; }
        protected virtual SystemActionTypeName? OnInvoiceCreationActionType { get; }
        protected virtual SystemActionTypeName? OnCreditNoteCreationActionType { get; }
        protected virtual SystemActionTypeName? OnCreditNotePaymentActionType { get; }



        protected CredentialApplicationTypeModel ApplicationType => ApplicationModel.ApplicationType;
        protected virtual CredentialApplicationStatusTypeName[] ApplicationEntryStates { get; }
        protected virtual CredentialApplicationStatusTypeName ApplicationExitState { get; }

        protected virtual int MinimumRequiredCredentialRequests => 1;
        protected virtual bool ProcessInBatch => (_processInBatch ?? (_processInBatch = CanProcessInBatch())).GetValueOrDefault();
        protected virtual bool IsBackgroundAction => WizardModel.IsBackgroundAction;

        protected virtual ProductSpecificationModel ActionFee => null;
        protected virtual bool InvoiceRequired => ActionFee != null;
        private bool? _processInBatch;

        protected override SecurityNounName? RequiredSecurityNoun => SecurityNounName.Application;
        protected override SecurityVerbName? RequiredSecurityVerb => SecurityVerbName.Update;


        private CredentialWorkflowFeeModel _workflowFee;
        protected CredentialWorkflowFeeModel WorkflowFee => _workflowFee ?? (_workflowFee = GetWorkflowFee());

        private IList<LookupTypeModel> _credentialTypes;
        protected IList<LookupTypeModel> CredentialTypes => _credentialTypes ?? (_credentialTypes = ApplicationService.GetLookupType("CredentialType").Data
                                                                .Select(AutoMapperHelper.Mapper.Map<LookupTypeModel>)
                                                                .ToList());

        private IList<LookupTypeModel> _credentialRequestStatusTypes;
        protected IList<LookupTypeModel> CredentialRequestStatusTypes => _credentialRequestStatusTypes ?? (_credentialRequestStatusTypes = ApplicationService.GetLookupType("CredentialRequestStatusType").Data
                                                                .Select(AutoMapperHelper.Mapper.Map<LookupTypeModel>)
                                                                .ToList());

        private IList<LookupTypeModel> _credentialApplicationStatusTypes;
        protected IList<LookupTypeModel> CredentialApplicationStatusTypes => _credentialApplicationStatusTypes ?? (_credentialApplicationStatusTypes = ApplicationService.GetLookupType("CredentialApplicationStatusType").Data
                                                                .Select(AutoMapperHelper.Mapper.Map<LookupTypeModel>)
                                                                .ToList());

        private IList<LookupTypeModel> _credentialRequestTypes;
        protected IList<LookupTypeModel> CredentialRequestTypes => _credentialRequestTypes ?? (_credentialRequestTypes = ApplicationService.GetLookupType("CredentialRequestType").Data
                                                                .Select(AutoMapperHelper.Mapper.Map<LookupTypeModel>)
                                                                .ToList());

        protected override GenericResponse<CredentialApplicationEmailMessageModel> SaveEmailMessage(CredentialApplicationEmailMessageModel message)
        {
            return EmailMessageService.CreateEmailMessage(message);
        }

        protected override GenericResponse<UpsertApplicationResultModel> SaveActionData()
        {
            var request = AutoMapperHelper.Mapper.Map<UpsertApplicationRequestModel>(ApplicationModel);
            request.StandardTestComponents = ApplicationModel?.StandardTestComponentModelsToUpdate;
            request.RubricTestComponents = ApplicationModel?.RubricTestComponentModelsToUpdate;
            request.Recertification = ApplicationModel?.Recertification;
            request.PdActivities = ApplicationModel?.PdActivities;
            request.ProcessFee = Output.PendingProcessFee;
            return ApplicationService.UpsertApplication(request);
        }

        public override IList<CredentialApplicationEmailMessageModel> GetEmailPreviews()
        {
            var templates = GetEmailTemplates();

            if (!templates.Any())
            {
                return new List<CredentialApplicationEmailMessageModel>();
            }

            OverrideTemplates(templates);

            var tokenDictionary = new Dictionary<string, string>();

            if (WizardModel.PublicNotes != null)
            {
                tokenDictionary.Add(TokenReplacementService.GetTokenNameFor(TokenReplacementField.ActionPublicNote),
                    (WizardModel.PublicNotes ?? String.Empty).Replace("\n", "<br />"));
            }

            if (WizardModel.CredentialRequestId != 0 || ApplicationModel.CredentialRequests.Count > 0)
            {
                var credentialRequest = ApplicationModel.CredentialRequests.FirstOrDefault(x => x.Id == WizardModel.CredentialRequestId)
                                        ?? ApplicationModel.CredentialRequests.First();

                var productTypeTokenValue = credentialRequest.Category != null
                    ? string.Equals(credentialRequest.Category, "Translator") ? "a Translator Stamp" : "an ID Card"
                    : string.Empty;

                tokenDictionary.Add(TokenReplacementService.GetTokenNameFor(TokenReplacementField.CredentialType), credentialRequest.CredentialType.ExternalName);
                tokenDictionary.Add(TokenReplacementService.GetTokenNameFor(TokenReplacementField.CredentialRequestType), credentialRequest.CredentialType.ExternalName);
                tokenDictionary.Add(TokenReplacementService.GetTokenNameFor(TokenReplacementField.Skill), credentialRequest.Direction);
                tokenDictionary.Add(TokenReplacementService.GetTokenNameFor(TokenReplacementField.ProductType), productTypeTokenValue);
            }

            tokenDictionary.Add(TokenReplacementService.GetTokenNameFor(TokenReplacementField.CredentialRequests),
                string.Join(string.Empty, ApplicationModel.CredentialRequests.Select(r => $"<li>{r.ExternalCredentialName} {r.Direction}</li>")));

            GetEmailTokens(tokenDictionary);
            return CreateEmails(templates, tokenDictionary).ToList();
        }



        protected virtual void AttachWorkPractices()
        {
            AttachWorkPractices(ApplicationModel.CredentialRequests);
        }

        protected void AttachWorkPractices(IList<CredentialRequestModel> credentialRequests)
        {
            if (ApplicationModel.ApplicationType.Category != CredentialApplicationTypeCategoryName.Recertification)
            {
                return;
            }
            var certificationPeriodId = ApplicationModel.ApplicationInfo.CertificationPeriodId;
            if (certificationPeriodId == 0)
            {
                throw new Exception("Certification Period not found");
            }


            var credentials = PersonService.GetPersonCredentials(ApplicationModel.ApplicationInfo.NaatiNumber);
            var periodCredentials = credentials.Data.Where(x => x.CertificationPeriod?.Id == certificationPeriodId);
            foreach (var credential in periodCredentials)
            {
                var selectedCredentialRequest =
                    credentialRequests.FirstOrDefault(
                        x => x.SkillId == credential.SkillId && x.CredentialTypeId == credential.CredentialTypeId);

                if (selectedCredentialRequest != null)
                {
                    var credentialDetails = CredentialPointsService.GetCertificationPeriodCredential(ApplicationModel.ApplicationInfo.NaatiNumber, certificationPeriodId, credential.Id);

                    foreach (var workPracticeId in credentialDetails.IncludedWorkPracticeIds)
                    {
                        selectedCredentialRequest.WorkPractices.Add(new WorkPracticeDataModel()
                        {
                            WorkPracticeId = workPracticeId,
                            ObjectStatusId = (int)ObjectStatusTypeName.Created
                        });

                    }
                }
            }

        }


        /*** preconditions ***/

        protected virtual void ValidateEntryState()
        {
            if (!ApplicationEntryStates.Contains((CredentialApplicationStatusTypeName)ApplicationModel.ApplicationStatus.Id))
            {
                var entryStateNames = ApplicationEntryStates.Select(x => CredentialApplicationStatusTypes.SingleOrDefault(y => y.Id == (int)x)?.DisplayName);
                throw new UserFriendlySamException(String.Format(Naati.Resources.Application.WrongApplicationStatusErrorMessage,
                    string.Join(", ", entryStateNames)));
            }
        }


        protected virtual void ValidateMinimumCredentialRequests()
        {
            if (ApplicationModel.CredentialRequests == null || ApplicationModel.CredentialRequests.Count < MinimumRequiredCredentialRequests)
            {
                ValidationErrors.Add(
                    new ValidationResultModel
                    {
                        Message = String.Format(Naati.Resources.Application.MinimumCredentialRequestsErrorMessage, MinimumRequiredCredentialRequests),
                    });
            }
        }

        protected virtual void ValidateMandatoryFields()
        {
            // todo: validate preferred test location for application types which require testing

            if (ApplicationModel.ApplicationInfo.ReceivingOfficeId < 1)
            {
                ValidationErrors.Add(
                    new ValidationResultModel
                    {
                        Message = String.Format(Naati.Resources.Application.MissingMandatoryFieldErrorMessage, "Entered Office"),
                        Field = "EnteredOffice"
                    });
            }

            var mandatoryFieldIds = ApplicationModel.ApplicationType.CredentialApplicationFields
                .Where(x => x.Mandatory)
                .Select(x => x.Id);

            var missingFields = ApplicationModel.Fields
                .Where(x => mandatoryFieldIds.Contains(x.FieldTypeId)
                            && String.IsNullOrEmpty(x.Value))
                .ToList();

            foreach (var field in missingFields)
            {
                ValidationErrors.Add(
                    new ValidationResultModel
                    {
                        Message = String.Format(Naati.Resources.Application.MissingMandatoryFieldErrorMessage, field.Name),
                    });
            }
        }

        protected virtual void ValidateMandatoryDocuments()
        {
            IEnumerable<DocumentTypeModel> mandatoryTypes =
                ApplicationModel.ApplicationType.CredentialApplicationTypeDocumentTypes
                    .Where(x => x.Mandatory)
                    .Select(x => x.DocumentType);

            IEnumerable<CredentialApplicationAttachmentModel> docs = ApplicationService.ListAttachments(ApplicationModel.ApplicationInfo.ApplicationId);

            var missingTypes = mandatoryTypes
                .Where(x => docs.All(y => y.Type.ToString() != x.Name))
                .ToList();

            if (missingTypes.Any())
            {
                ValidationErrors.Add(
                    new ValidationResultModel
                    {
                        Message = String.Format(
                            Naati.Resources.Application.MissingMandatoryDocumentErrorMessage,
                            String.Join(", ", missingTypes.Select(x => x.DisplayName)))
                    });
            }
        }

        private void ValidatePersonField(object value, string fieldName)
        {
            if (String.IsNullOrWhiteSpace(value?.ToString()))
            {
                ValidationErrors.Add(
                    new ValidationResultModel
                    {
                        Message = String.Format(Naati.Resources.Application.MissingMandatoryPersonFieldsErrorMessage, fieldName)
                    });
            }
        }

        protected virtual void ValidateMandatoryPersonFields()
        {
            ValidatePersonField(ApplicationModel.ApplicantDetails.GivenName, "Given Name");
            ValidatePersonField(ApplicationModel.ApplicantDetails.FamilyName, "Surname");
            ValidatePersonField(ApplicationModel.ApplicantDetails.PrimaryEmail, "Primary Email");
            ValidatePersonField(ApplicationModel.ApplicantDetails.PrimaryContactNumber, "Primary Contact Number");
            ValidatePersonField(ApplicationModel.ApplicantDetails.AddressLine1, "Primary Address");
            ValidatePersonField(ApplicationModel.ApplicantDetails.DateOfBirth, "Date of Birth");
            ValidatePersonField(ApplicationModel.ApplicantDetails.CountryOfBirth, "Country of Birth");
            ValidatePersonField(ApplicationModel.ApplicantDetails.Gender, "Gender");
        }

        protected virtual void ValidateAllApplicationInvoices()
        {
            var response = ApplicationService.GetWorkflowApplicationActionInvoices(WizardModel.ActionType, WizardModel.ApplicationId);
            Validate(response);
        }

        protected virtual void Validate(GenericResponse<IEnumerable<InvoiceModel>> invoicesResponse)
        {
            foreach (var warning in invoicesResponse.Warnings)
            {
                ValidationErrors.Add(new ValidationResultModel
                {
                    Message = warning
                });
            }

            foreach (var error in invoicesResponse.Errors)
            {
                ValidationErrors.Add(new ValidationResultModel
                {
                    Message = error
                });
            }

            if (ValidationErrors.Any())
            {
                return;
            }

            var notPaidInovices = invoicesResponse.Data.Where(x => x.Balance > 0 && x.Status != InvoiceStatusModel.Canceled).ToList();

            foreach (var invoice in notPaidInovices)
            {
                ValidationErrors.Add(new ValidationResultModel
                {
                    Message = string.Format(Naati.Resources.Application.UnpaidApplicationInvoiceErrorMessage, invoice.InvoiceNumber)
                });
            }
        }

        protected virtual void ValidateNotTrustedApplicationInvoices()
        {
            if (!ApplicationModel.ApplicationInfo.OwnedByApplicant && ApplicationModel.ApplicationInfo.TrustedInstitutionPayer.GetValueOrDefault())
            {
                return;
            }

            ValidateAllApplicationInvoices();
        }

        /*** system actions ***/

        protected virtual string GetNote()
        {
            var entryStateName = CredentialApplicationStatusTypes.Single(x => x.Id == ApplicationModel.ApplicationStatus.Id).DisplayName;
            var exitStateName = CredentialApplicationStatusTypes.Single(x => x.Id == (int)ApplicationExitState).DisplayName;
            if (entryStateName == exitStateName)
            {
                return string.Empty;
            }
            return String.Format(Naati.Resources.Application.ApplicationStatusChangeNote, entryStateName, exitStateName);
        }

        protected virtual IEnumerable<string> GetPersonNotes()
        {
            return Enumerable.Empty<string>();
        }

        public virtual void CreatePersonNotes()
        {
            foreach (var note in GetPersonNotes())
            {
                var nodeRequest = new PersonNoteModel
                {
                    CreatedDate = DateTime.Now,
                    Note = note,
                    UserId = CurrentUser.Id,
                    ReadOnly = true,
                    Highlight = true
                };

                ApplicationModel.PersonNotes.Add(nodeRequest);
            }
        }

        protected virtual void CreateNote()
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

        protected virtual void SetOwner()
        {
            ApplicationModel.ApplicationInfo.OwnedByUserId = CurrentUser.Id;
        }

        protected virtual void ClearOwner()
        {
            ApplicationModel.ApplicationInfo.OwnedByUserId = null;
        }

        protected virtual ProductSpecificationModel GetFee(FeeTypeName feeTypeName)
        {
            if (WizardModel.DoNotInvoice)
            {
                return null;
            }
            if (feeTypeName == FeeTypeName.Application)
                return ApplicationService.GetApplicationFee(ApplicationModel.ApplicationInfo.ApplicationId);

            if (feeTypeName == FeeTypeName.ApplicationAssessment)
                return ApplicationService.GetApplicationAssessmentFee(ApplicationModel.ApplicationInfo.ApplicationId);

            //in reality it should never get here
            throw new UserFriendlySamException("No fee operation found with given fee type in the system.");
        }

        protected bool IsSponsored => ApplicationModel.ApplicationInfo.SponsorInstitutionId.HasValue;

        protected virtual void CreateInvoiceIfApplicable()
        {
            var request = GetInvoicePreview();

            if (request != null)
            {
                var invoiceResponse = AccountingService.CreateApplicationInvoice(request);
                if (invoiceResponse.Success)
                {
                    Output.InvoiceId = invoiceResponse.Data.InvoiceId;
                    Output.InvoiceNumber = invoiceResponse.Data.InvoiceNumber;
                    Output.OperationId = invoiceResponse.Data.OperationId ?? 0;
                    return;
                }
                else
                {
                    if (invoiceResponse.Data?.RateLimitExceeded ?? false)
                    {
                        throw new WiiseRateExceededException(string.Join("; ", invoiceResponse.Errors));
                    }
                    throw new UserFriendlySamException(string.Join("; ", invoiceResponse.Errors));
                }
            }
        }

        protected virtual void CreateRefundIfApplicable()
        {
            var request = GetRefundPreview();

            if (request != null)
            {
                var refundResponse = AccountingService.CreateApplicationRefund(request);
                if (refundResponse.Success)
                {
                    Output.CreditNoteId = refundResponse.Data.CreditNoteId;
                    Output.CreditNoteNumber = refundResponse.Data.CreditNoteNumber;

                }
                else
                {
                    if (refundResponse.Data?.RateLimitExceeded ?? false)
                    {
                        throw new WiiseRateExceededException(string.Join("; ", refundResponse.Errors));
                    }

                    throw new UserFriendlySamException(string.Join("; ", refundResponse.Errors));
                }
            }
        }

        protected internal virtual ApplicationInvoiceCreateRequestModel GetInvoicePreview()
        {
            return null;
        }

        protected internal virtual ApplicationRefundCreateRequestModel GetRefundPreview()
        {
            return null;
        }

        protected virtual ApplicationInvoiceCreateRequestModel GetApplicationInvoiceCreateRequest(ProductSpecificationModel feeProductSpec)
        {
            var isSponsored = ApplicationModel.ApplicationInfo.SponsorInstitutionId != null;
            var isNaatiFunded = ApplicationModel.ApplicationInfo.SponsorInstitutionNaatiNumber == 950079;
            var isInAustralia = ApplicationModel.ApplicantDetails.HasAddressInAustralia;

            var model = new ApplicationInvoiceCreateRequestModel
            {
                CredentialApplicationId = ApplicationModel.ApplicationInfo.ApplicationId,
                CredentialFeeProductId = feeProductSpec.CredentialFeeProductId,
                EntityId = isSponsored ? 0 : ApplicationModel.ApplicantDetails.EntityId,
                NaatiNumber = isSponsored ? ApplicationModel.ApplicationInfo.SponsorInstitutionNaatiNumber : 0,
                IsInstitution = isSponsored,
                InvoiceType = InvoiceType.Invoice,
                PaymentCompletionAction = OnInvoicePaymentActionType,
                InvoiceCompletionAction = OnInvoiceCreationActionType,
                ProductSpecificationId = feeProductSpec.Id,
                LineItems = new[]
                {
                    new InvoiceLineItemModel
                    {
                        EntityId = ApplicationModel.ApplicantDetails.EntityId,
                        ProductSpecificationId = feeProductSpec.Id,
                        Quantity = 1,
                        Description = GetInvoiceItemDescription(feeProductSpec, isSponsored),
                        IncGstCostPerUnit = feeProductSpec.CostPerUnit,
                        GstApplies = isInAustralia && feeProductSpec.GstApplies,
                        ProductCode = feeProductSpec.Code,
                        GlCode = feeProductSpec.GlCode,
                    }
                },
                CancelOperationIfError = true,
                Payments = new InvoicePaymentCreateModel[0],
                BatchProcess = ProcessInBatch
            };

            model.DueDate = WizardModel.DueDate;
            model.Reference = WizardModel.InvoiceReference;
            //logic for checking NAATI Funded Scholarship below (For the creation of an additional line item in the invoice in case NAATI is the sponsor) 
            if (isNaatiFunded && !model.LineItems.Any(li => li.Description.Contains("Scholarship – NAATI funded")))
            {
                var items = model.LineItems.ToList();
                var response = mPayrollService.GetProductSpecificationByCode("SCHOLARSHIPNAATIFUND");
                var offsetSpec = response.ProductSpecificationDetails.Single();
                items.Add(new InvoiceLineItemModel
                {
                    EntityId = ApplicationModel.ApplicantDetails.EntityId,
                    ProductSpecificationId = offsetSpec.Id,                                     
                    Quantity = 1,
                    Description = offsetSpec.Description,
                    IncGstCostPerUnit = -feeProductSpec.CostPerUnit,
                    GstApplies = false,
                    ProductCode = offsetSpec.Code,
                    GlCode = offsetSpec.GlCode
                });

                // write back
                model.LineItems = items.ToArray();
            }
            return model;
        }

        protected virtual ApplicationRefundCreateRequestModel GetApplicationRefundCreateRequest(RefundModel refundModel)
        {
            var isSponsored = ApplicationModel.ApplicationInfo.SponsorInstitutionId != null;

            var responseModel = new ApplicationRefundCreateRequestModel()
            {
                CredentialApplicationRefundId = refundModel.Id,
                EntityId = isSponsored ? 0 : ApplicationModel.ApplicantDetails.EntityId,
                NaatiNumber = isSponsored ? ApplicationModel.ApplicationInfo.SponsorInstitutionNaatiNumber : 0,
                IsInstitution = isSponsored,
                InvoiceType = InvoiceType.CreditNote,
                CreditNotePaymentCompletionAction = OnCreditNotePaymentActionType,
                CreditNoteCreateCompletionAction = OnCreditNoteCreationActionType,
                CancelOperationIfError = true,
                Payments = new InvoicePaymentCreateModel[0],
                LineItems = new List<InvoiceLineItemModel>(),
                BatchProcess = ProcessInBatch,
                DueDate = DateTime.Now.AddDays(7)
            };

            return responseModel;
        }


        protected virtual string GetInvoiceItemDescription(ProductSpecificationModel feeProductSpec, bool isSponsored)
        {
            var description = feeProductSpec.Description ?? string.Empty;
            if (isSponsored)
            {
                description =
                    $"{string.Format(Naati.Resources.Application.CandidateInvoiceCustomerDetails, ApplicationModel.ApplicationInfo.NaatiNumber)} \n" +
                    $"{ApplicationModel.ApplicationInfo.ApplicantGivenName} " + $"{ApplicationModel.ApplicationInfo.ApplicantFamilyName} \n" +
                    $"{description}";

            }
            return description;
        }

        protected virtual string GetRefundItemDescription(ProductSpecificationModel feeProductSpec, bool isSponsored)
        {
            var description = feeProductSpec.Description ?? string.Empty;
            if (isSponsored)
            {
                description =
                    $"{string.Format(Naati.Resources.Application.CandidateInvoiceCustomerDetails, ApplicationModel.ApplicationInfo.NaatiNumber)} \n" +
                    $"{ApplicationModel.ApplicationInfo.ApplicantGivenName} " + $"{ApplicationModel.ApplicationInfo.ApplicantFamilyName} \n" +
                    $"{description}";

            }
            return description;
        }


        protected override IEnumerable<CredentialApplicationEmailMessageModel> GetEmails(EmailTemplateModel template, CredentialApplicationEmailMessageModel baseEmail)
        {
            var applicantEmails = new List<CredentialApplicationEmailMessageModel>();
            var sponsorEmails = new List<CredentialApplicationEmailMessageModel>();
            if (template.EmailTemplateDetails.Contains(EmailTemplateDetailTypeName.SendToApplicant))
            {
                var applicantEmail = AutoMapperHelper.Mapper.Map<CredentialApplicationEmailMessageModel>(baseEmail);
                applicantEmail.RecipientEmail = ApplicationModel.ApplicationInfo.ApplicantPrimaryEmail;
                applicantEmails.Add(applicantEmail);
            }

            if (template.EmailTemplateDetails.Contains(EmailTemplateDetailTypeName.SendToSponsor))
            {
                if (string.IsNullOrEmpty(ApplicationModel.ApplicationInfo.SponsorEmail))
                {
                    throw new UserFriendlySamException(
                        $"Unable to email Sponsor Organisation: no email address defined for NAATI #{ApplicationModel.ApplicationInfo.SponsorInstitutionNaatiNumber}.");
                }
                var sponsorEmail = AutoMapperHelper.Mapper.Map<CredentialApplicationEmailMessageModel>(baseEmail);
                sponsorEmail.RecipientEmail = ApplicationModel.ApplicationInfo.SponsorEmail;
                sponsorEmail.RecipientEntityId = ApplicationModel.ApplicationInfo.SponsorEntityId.GetValueOrDefault();
                sponsorEmails.Add(sponsorEmail);
            }

            if (template.EmailTemplateDetails.Contains(EmailTemplateDetailTypeName.AttachInoviceToApplicant))
            {
                SetAttachments(applicantEmails, AttachmentType.Invoice);
            }

            if (template.EmailTemplateDetails.Contains(EmailTemplateDetailTypeName.AttachCredentialDocsToApplicant))
            {
                SetAttachments(applicantEmails, AttachmentType.CredentialDocument);
            }

            if (template.EmailTemplateDetails.Contains(EmailTemplateDetailTypeName.AttachCandidateBriefToApplicant))
            {
                SetAttachments(applicantEmails, AttachmentType.CandidateBrief);
            }

            if (template.EmailTemplateDetails.Contains(EmailTemplateDetailTypeName.AttachInoviceToSponsor))
            {
                SetAttachments(sponsorEmails, AttachmentType.Invoice);
            }

            if (template.EmailTemplateDetails.Contains(EmailTemplateDetailTypeName.AttachCreditNoteToApplicant))
            {
                SetAttachments(applicantEmails, AttachmentType.CreditNote);
            }
            if (template.EmailTemplateDetails.Contains(EmailTemplateDetailTypeName.AttachCreditNoteToSponsor))
            {
                SetAttachments(sponsorEmails, AttachmentType.CreditNote);
            }

            return applicantEmails.Concat(sponsorEmails);
        }

        protected override void CreateEmailAttachmentsIfApplicable()
        {
            if (!string.IsNullOrEmpty(Output.InvoiceNumber))
            {
                //var invoiceTempFilePath = GetInvoiceToTempFile(Output.InvoiceId, Output.InvoiceNumber);

                //var type = "Invoice";
                //var fileSaveRequest = new CreateOrUpdateFileRequestModel
                //{
                //    Title = $"Invoice {Output.InvoiceNumber}",
                //    Type = type,
                //    FilePath = invoiceTempFilePath,
                //    UploadedByUserId = CurrentUser.Id,
                //    StoragePath = $@"{type}\{Path.GetFileName(invoiceTempFilePath)}",
                //};
                //var storedFile = FileService.Create(fileSaveRequest);

                //Attachments.Add(new EmailMessageAttachmentModel
                //{
                //    StoredFileId = storedFile.StoredFileId,
                //    AttachmentType = AttachmentType.Invoice
                //});
            }
        }

        protected string GetInvoiceToTempFile(Guid invoiceId, string invoicenumber)
        {
            var invoicePdf = invoiceId != default(Guid) ? AccountingService.GetInvoicePdfById(invoiceId, invoicenumber, InvoiceTypeModel.Invoice)
                : AccountingService.GetInvoicePdf(invoicenumber, InvoiceTypeModel.Invoice);
            return CreateInvoiceFile(invoicePdf);
        }

        protected string GetCreditNoteToTempFile(Guid creditNoteId, string creditNoteNumber)
        {
            var invoicePdf = creditNoteId != default(Guid) ? AccountingService.GetInvoicePdfById(creditNoteId, creditNoteNumber, InvoiceTypeModel.CreditNote)
                : AccountingService.GetInvoicePdf(creditNoteNumber, InvoiceTypeModel.CreditNote);
            return CreateInvoiceFile(invoicePdf);
        }


        protected virtual string CreateInvoiceFile(FileModel invoicePdf)
        {
            var tempFilePath = Path.Combine(ConfigurationManager.AppSettings["tempFilePath"], invoicePdf.FileName);
            var fileStream = File.Create(tempFilePath);
            invoicePdf.FileData.CopyTo(fileStream);
            fileStream.Close();

            return tempFilePath;
        }

        protected override void ConfigureInstance(CredentialApplicationDetailedModel application, ApplicationActionWizardModel wizardModel, ApplicationActionOutput output)
        {
            base.ConfigureInstance(application, wizardModel, output);
            if (wizardModel.ApplicationId > 0)
            {
                CurrentEntryState = (CredentialApplicationStatusTypeName)ApplicationModel.ApplicationInfo.ApplicationStatusTypeId;
            }
        }


        protected virtual void UpdateCredentialRequestStatus() { }

        protected virtual void SetEnteredUser()
        {
            ApplicationModel.ApplicationInfo.EnteredUserId = CurrentUser.Id;
        }

        protected virtual void SetReceivingOffice()
        {
            ApplicationModel.ApplicationInfo.ReceivingOfficeId = CurrentUser.OfficeId;
        }

        protected virtual void SetExitState()
        {
            ApplicationModel.ApplicationInfo.ApplicationStatusTypeId = (int)ApplicationExitState;
            ApplicationModel.ApplicationInfo.StatusChangeUserId = CurrentUser.Id;
            ApplicationModel.ApplicationInfo.StatusChangeDate = DateTime.Now;
        }

        protected override IList<ActionEventLevel> GetActionEvents()
        {
            var events = base.GetActionEvents();

            if (InvoiceRequired)
            {
                var invoiceCreatedEvent = new ActionEventLevel { Level = 1 };
                if (Sponsored)
                {

                    if (!ApplicationModel.ApplicationInfo.TrustedInstitutionPayer.GetValueOrDefault())
                    {
                        invoiceCreatedEvent.Event = SystemActionEventTypeName.InvoiceCreatedToUntrustedSponsor;
                        events.Add(invoiceCreatedEvent);
                    }

                }
                else
                {
                    invoiceCreatedEvent.Event = SystemActionEventTypeName.InvoiceCreatedToApplicant;
                    events.Add(invoiceCreatedEvent);
                }
            }

            return events;

        }

        protected virtual bool CanProcessInBatch()
        {

            return InvoiceRequired && WizardModel.ProcessInBatch;
        }


        protected virtual CredentialWorkflowFeeModel GetWorkflowFee()
        {
            throw new NotSupportedException(nameof(GetWorkflowFee));
        }

        protected virtual void ValidateProcessedFee()
        {
            var fee = WorkflowFee;

            if (fee == null)
            {
                throw new UserFriendlySamException(string.Format(Naati.Resources.Test.FeeNotFound, ApplicationModel.ApplicationInfo.ApplicationId));
            }

            WizardModel.SetInvoiceDetails(fee.InvoiceId, fee.InvoiceNumber);
        }

        protected virtual void ProcessFee()
        {
            var fee = WorkflowFee;
            if (fee != null)
            {
                Output.PendingProcessFee = new ProcessFeeModel
                {
                    CredentialWorkflowFeeId = fee.Id,
                    Type = ProcessTypeName.Payment,
                    PaymentReference = WizardModel.PaymentReference,
                    TransactionId = WizardModel.TransactionId,
                    OrderNumber = WizardModel.OrderNumber
                };
            }
        }

        public virtual IEnumerable<DocumentData> GetDocumentsPreview(DocumentsPreviewRequestModel request)
        {
            throw new Exception("No Documents preview has been implemented for action");
        }

        public static ApplicationStateAction CreateAction(SystemActionTypeName actionType, CredentialApplicationDetailedModel applicationModel, ApplicationActionWizardModel wizardModel)
        {
            var action = CreateSystemAction<ApplicationStateAction>(ActionDict, actionType, applicationModel, wizardModel, new ApplicationActionOutput());

            return action;
        }

        public override IList<EmailTemplateModel> GetEmailTemplates()
        {
            if (!CanSendEmail())
            {
                return new List<EmailTemplateModel>();
            }
            ApplicationModel?.ApplicationType.FailIfNull();

            GetEmailTemplateResponse serviceReponse = null;
            serviceReponse = ApplicationService.GetCredentialApplicationEmailTemplate(new GetEmailTemplateRequest
            {
                ApplicationType = ApplicationModel.ApplicationType.CredentialApplicationType,
                Action = (SystemActionTypeName)WizardModel.ActionType,
                ActionEvents = ActionEvents.GroupBy(x => x.Level).OrderByDescending(y => y.Key).First().Select(x => x.Event)
            });

            var emailTemplates = serviceReponse.Data.Select(AutoMapperHelper.Mapper.Map<EmailTemplateModel>).ToList();

            return emailTemplates;

        }

        protected override CredentialApplicationEmailMessageModel CreateEmail(EmailTemplateModel template, IDictionary<string, string> tokenDictionary)
        {

            var applicationDetailsResponse = ApplicationService.GetApplicationDetailsByApplicationId(new GetApplicationDetailsRequest { ApplicationId = WizardModel.ApplicationId });
            var personInfoResponse = PersonService.GetPersonInfoResponse(applicationDetailsResponse.ApplicantDetails.NaatiNumber);
            var content = TokenReplacementService.ReplaceTemplateFieldValues(template.Content, applicationDetailsResponse, personInfoResponse.PersonDetails, personInfoResponse.ContactDetails, tokenDictionary, true,
                out _);
            var subject = TokenReplacementService.ReplaceTemplateFieldValues(template.Subject, applicationDetailsResponse, personInfoResponse.PersonDetails, personInfoResponse.ContactDetails, tokenDictionary, true,
                out _);

            var fromAddress = string.IsNullOrWhiteSpace(template.FromAddress)
                ? SystemService.GetSystemValue("DefaultEmailSenderAddress")
                : template.FromAddress;

            var email = new CredentialApplicationEmailMessageModel
            {
                Subject = subject,
                Body = content,
                From = fromAddress,
                RecipientEntityId = ApplicationModel.ApplicantDetails.EntityId,
                CreatedDate = DateTime.Now,
                CreatedUserId = CurrentUser.Id,
                CredentialApplicationId = WizardModel.ApplicationId,
                Attachments = new List<EmailMessageAttachmentModel>(),
                EmailSendStatusTypeId = (int)EmailSendStatusTypeName.Requested,
                EmailTemplateId = template.Id
            };

            return email;
        }

        protected override void LogAction()
        {
            LoggingHelper.LogInfo("Performing workflow action {Action} for APP{ApplicationId}",
                (SystemActionTypeName)WizardModel.ActionType, WizardModel.ApplicationId);
        }

        protected virtual ApplicationActionWizardModel CloneWizard(SystemActionTypeName action)
        {
            var copiedModel = new ApplicationActionWizardModel();
            AutoMapperHelper.Mapper.Map(WizardModel, copiedModel);
            copiedModel.ActionType = (int)action;
            copiedModel.SetBackGroundAction(WizardModel.IsBackgroundAction);
            copiedModel.SetProcessInBatch(WizardModel.ProcessInBatch);
            return copiedModel;
        }

        protected virtual CredentialApplicationDetailedModel CloneApplicationModel()
        {
            var copiedApplicationModel = new CredentialApplicationDetailedModel();
            AutoMapperHelper.Mapper.Map(ApplicationModel, copiedApplicationModel);
            return copiedApplicationModel;
        }

        protected virtual IssueCredentialDataModel CloneIssueCredentialDataModel(IssueCredentialDataModel model)
        {
            var copiedApplicationModel = new IssueCredentialDataModel();
            AutoMapperHelper.Mapper.Map(model, copiedApplicationModel);
            return copiedApplicationModel;
        }


        private static readonly Dictionary<SystemActionTypeName, Type> ActionDict =
            new Dictionary<SystemActionTypeName, Type>
            {
                {SystemActionTypeName.CreateApplication, typeof(ApplicationCreateAction)},
                {SystemActionTypeName.NewCredentialRequest, typeof(CredentialRequestCreateAction)},
                {SystemActionTypeName.SubmitApplication, typeof(ApplicationSubmitAction)},
                {SystemActionTypeName.StartChecking, typeof(ApplicationStartCheckingAction)},
                {SystemActionTypeName.CancelChecking, typeof(ApplicationCancelCheckingAction)},
                {SystemActionTypeName.FinishChecking, typeof(ApplicationFinishCheckingAction)},
                {SystemActionTypeName.RejectApplication, typeof(ApplicationRejectAction)},
                {SystemActionTypeName.RejectApplicationAfterInvoice, typeof(ApplicationRejectAction)},
                {SystemActionTypeName.FinaliseApplication, typeof(ApplicationFinaliseAction)},
                {SystemActionTypeName.StartAssessing, typeof(CredentialRequestStartAssessingAction)},
                {SystemActionTypeName.CancelAssessment, typeof(CredentialRequestCancelAssessmentAction)},
                {SystemActionTypeName.FailAssessment, typeof(CredentialRequestFailAssessmentAction)},
                {SystemActionTypeName.FailPendingAssessment, typeof(CredentialRequestFailAssessmentAction)},
                {SystemActionTypeName.PendAssessment, typeof(CredentialRequestPendAssessmentAction)},
                {SystemActionTypeName.PassAssessment, typeof(CredentialRequestPassAssessmentAction)},
                {SystemActionTypeName.PassReview, typeof(CredentialRequestPassReviewAction)},
                {SystemActionTypeName.CreatePaidReview, typeof(CredentialRequestCreateReviewAction)},
                {SystemActionTypeName.FailReview, typeof(CredentialRequestFailReviewAction)},
                {SystemActionTypeName.IssueCredential, typeof(CredentialRequestIssueCredentialAction)},
                {SystemActionTypeName.CancelRequest, typeof(CredentialRequestCancelAction)},
                {SystemActionTypeName.DeleteRequest, typeof(CredentialRequestDeleteAction)},
                {SystemActionTypeName.DeleteApplication, typeof(ApplicationDeleteAction)},
                {SystemActionTypeName.ReissueCredential, typeof(CredentialRequestReissueCredentialAction)},
                {SystemActionTypeName.AssessmentInvoicePaid, typeof(ApplicationAssessmentInvoicePaidAction)},
                {SystemActionTypeName.ApplicationInvoicePaid, typeof(ApplicationInvoicePaidAction)},
                {SystemActionTypeName.WithdrawRequest, typeof(CredentialRequestWithdrawAction)},
                {SystemActionTypeName.WithdrawSupplementaryTest,typeof(CredentialRequestWithdrawSupplementaryTestAction)},
                {SystemActionTypeName.TestInvoicePaid, typeof(CredentialRequestTestInvoicePaidAction)},
                {SystemActionTypeName.CancelTestInvitation,typeof(CredentialRequestCancelTestInvitationAction)},
                {SystemActionTypeName.SupplementaryTestInvoicePaid,typeof(CredentialRequestSupplementaryTestInvoicePaidAction)},
                {SystemActionTypeName.AllocateTestSession, typeof(CredentialRequestAllocateTestSessionAction)},
                {SystemActionTypeName.RejectTestSession, typeof(CredentialRequestRejectTestSessionAction)},
                {SystemActionTypeName.RejectTestSessionFromMyNaati, typeof(CredentialRequestRejectTestSessionAction)},
                {SystemActionTypeName.CheckIn, typeof(CredentialRequestCheckInAction)},
                {SystemActionTypeName.MarkAsSat, typeof(CredentialRequestMarkAsSatAction)},
                {SystemActionTypeName.UndoCheckIn, typeof(CredentialRequestUndoCheckInAction)},
                {SystemActionTypeName.UndoMarkAsSat, typeof(CredentialRequestUndoMarkAsSatAction)},
                {SystemActionTypeName.IssueFail, typeof(CredentialRequestIssueFailAction)},
                {SystemActionTypeName.IssuePass, typeof(CredentialRequestIssuePassAction)},
                {SystemActionTypeName.CreatePaidTestReview,typeof(CredentialRequestCreatePaidTestReviewAction)},
                {SystemActionTypeName.IssueFailAfterReview,typeof(CredentialRequestIssueFailAfterReviewAction)},
                {SystemActionTypeName.IssuePassAfterReview,typeof(CredentialRequestIssuePassAfterReviewAction)},
                {SystemActionTypeName.CreateSupplementaryTest,typeof(CredentialRequestCreateSupplementaryTestAction)},
                {SystemActionTypeName.AllocateTestSessionFromMyNaati,typeof(CredentialRequestAllocateAndAcceptTestSesssionAction)},
                {SystemActionTypeName.ApplicationSubmissionProcessed,typeof(ApplicationSubmissionProcessedAction)},
                {SystemActionTypeName.AssignTestMaterial, typeof(CredentialRequestAssignTestMaterialAction)},
                {SystemActionTypeName.Reassess, typeof(CredentialRequestReassessAction)},
                {SystemActionTypeName.ReactivateApplication, typeof(ApplicationReactivateAction)},
                {SystemActionTypeName.CloseApplication, typeof(ApplicationCloseAction)},
                {SystemActionTypeName.PaidReviewInvoiceProcessed,typeof(CredentialRequestPaidReviewInvoiceProcessedAction)},
                {SystemActionTypeName.PaidReviewInvoicePaid,typeof(CredentialRequestPaidReviewInvoicePaidAction)},
                {SystemActionTypeName.WithdrawPaidReview, typeof(CredentialRequestWithdrawPaidReviewAction)},
                {SystemActionTypeName.NotifyTestSessionDetails,typeof(CredentialRequestNotifyTestSessionDetailsAction)},
                {SystemActionTypeName.SupplementaryTestInvoiceProcessed,typeof(CredentialRequestSupplementaryTestInvoiceProcessedAction)},
                {SystemActionTypeName.SendCandidateBrief,typeof(CredentialRequestSendCandidateBriefAction)},
                {SystemActionTypeName.RevertResults,typeof(CredentialRequestRequestRevertResultsAction)},
                {SystemActionTypeName.ComputeFinalMarks,typeof(CredentialRequestComputeFinalMarksAction)},
                {SystemActionTypeName.InvalidateTest,typeof(CredentialRequestInvalidateTestAction)},
                {SystemActionTypeName.IssueRecertificationCredentials,typeof(ApplicationIssueRecertificationCredentialsAction)},
                {SystemActionTypeName.MarkForAssessment,typeof(CredentialRequestMarkForAssessmentAction)},
                {SystemActionTypeName.TestInvoiceProcessed,typeof(CredentialRequestTestInvoiceProcessedAction)},
                {SystemActionTypeName.AssessCredentials,typeof(ApplicationAssessCredentialsAction)},
                {SystemActionTypeName.SendRecertificationReminder,typeof(ApplicationSendRecertificationReminderAction)},
                {SystemActionTypeName.SendTestSessionReminder,typeof(CredentialRequestSendTestSessionReminderAction)},
                {SystemActionTypeName.RequestRefund,typeof(CredentialRequestRequestRefundAction)},
                {SystemActionTypeName.SendSessionAvailabilityNotice,typeof(CredentialRequestSendTestSessionAvailabilityNoticeAction)},
                {SystemActionTypeName.ProcessRefund,typeof(CredentialRequestProcessRefundAction)},
                {SystemActionTypeName.CreditNoteProcessed,typeof(CredentialRequestRefundCreditNoteProcessedAction)},
                {SystemActionTypeName.CreditNotePaid,typeof(CredentialRequestRefundCreditNotePaidAction)},
                {SystemActionTypeName.ApproveRefund,typeof(CredentialRequestApproveRefundAction)},
                {SystemActionTypeName.RejectRefund,typeof(CredentialRequestRejectRefundAction)},
                {SystemActionTypeName.MakeApplicationPayment,typeof(MakeApplicationPaymentAction)},
                {SystemActionTypeName.ApplicationInvoiceProcessed,typeof(ApplicationInvoiceProcessedAction)},
                {SystemActionTypeName.CertificationOnHold, typeof(CredentialRequestCredentialOnHoldAction)},
                {SystemActionTypeName.CreatePreRequisiteApplications, typeof(CredentialRequestCreatePreRequisiteApplicationsAction)},
                //{SystemActionTypeName.RequestPaidReviewRefund, typeof(CredentialRequestProcessPaidReviewRefundAction)}
                {SystemActionTypeName.IssuePracticeTestResults, typeof(CredentialRequestIssuePracticeTestResultsAction)},
                {SystemActionTypeName.ProgressCredentialToEligibleForTesting, typeof(ProgressCredentialToEligibleForTestingAction)},
                {SystemActionTypeName.SendEmail, typeof(ApplicationSendEmailAction)},
            };
    }
}