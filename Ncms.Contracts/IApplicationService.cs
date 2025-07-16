using System;
using System.Collections.Generic;
using F1Solutions.Naati.Common.Contracts.Bl.DTO;
using F1Solutions.Naati.Common.Contracts.Dal;
using F1Solutions.Naati.Common.Contracts.Dal.DTO;
using F1Solutions.Naati.Common.Contracts.Dal.Request;
using F1Solutions.Naati.Common.Contracts.Dal.Response;
using Ncms.Contracts.Models;
using Ncms.Contracts.Models.Accounting;
using Ncms.Contracts.Models.Application;
using Ncms.Contracts.Models.Common;
using Ncms.Contracts.Models.CredentialPrerequisite;

namespace Ncms.Contracts
{
    public interface IApplicationService
    {
        GenericResponse<IEnumerable<ApplicationSearchResultModel>> Search(SearchRequest request);
        GenericResponse<IEnumerable<LookupTypeModel>> GetLookupType(string lookupType);
        GenericResponse<CredentialApplicationInfoModel> GetApplication(int applicationId);
        GenericResponse<IEnumerable<CredentialRequestModel>> GetCredentialRequests(int credentialApplicationId);
        GenericResponse<IEnumerable<CredentialRequestModel>> GetOtherCredentialRequests(int credentialApplicationId);
        GenericResponse<CredentialRequestModel> GetCredentialRequest(int credentialRequestId);
        IEnumerable<int> GetAvailableCredentialsToRecertify(int applicationId);
        GenericResponse<BasicCertificationPeriodModel> GetCertificationPeriod(int applicationId);
        void UpdateCredential(CredentialModel credential);
        GenericResponse<IEnumerable<CredentialRequestModel>> GetAllCredentialRequests(GetApplicationSearchRequest request);
        GenericResponse<IEnumerable<CredentialRequestTestRequestModel>> GetAllCredentialTests(CredentialTestSearchRequestModel request);
        GenericResponse<IEnumerable<CredentialApplicationInfoModel>> ApplicationsForCredential(int credentialId);
        bool HasValidCredentialRequest(int credentialApplicationId, int categoryId, int credentialTypeId, int skillId);
        bool CanWithdrawApplicationUnderPaidReview(int credentialRequestId);

        GenericResponse<IEnumerable<CredentialApplicationSectionModel>> GetApplicationFieldsData(int applicationId);
        GenericResponse<UpsertApplicationResultModel> CreateApplication(UpsertApplicationRequestModel model);
        GenericResponse<IEnumerable<LookupTypeModel>> GetVenue(IEnumerable<int> testLocation);

        /// <summary>
        /// Returns a list of Venues for a location.
        /// Active venues are at the top and inactive ones are labelled and at the bottom
        /// </summary>
        /// <param name="testLocationId"></param>
        /// <returns></returns>
        GenericResponse<IEnumerable<LookupTypeModel>> GetVenuesShowingInactive(IEnumerable<int> testLocation);
        GenericResponse<IEnumerable<LookupTypeModel>> GetCredentialTypeSkills(IEnumerable<int> credentialTypeIds);
        GenericResponse<List<SkillEmailTokenObject>> GetCredentialSkills(IEnumerable<int> credentialIds);
        GenericResponse<UpsertApplicationResultModel> UpsertApplication(UpsertApplicationRequestModel applicationRequestModel);
        GenericResponse<IEnumerable<ValidationResultModel>> ValidateActionPreconditions(ApplicationActionWizardModel wizardModel);
        BusinessServiceResponse PerformAction(ApplicationActionWizardModel wizardModel);

        BusinessServiceResponse PerformCredentialRequestsBulkAction(CredentialRequestsBulkActionWizardModel wizardModel);

        BusinessServiceResponse PerformTestMaterialAssignementBulkAction(TestMaterialAssignmentBulkModel wizardModel);

        GenericResponse<IEnumerable<CredentialLookupTypeModel>> GetCredentialTypesForApplication(int applicationId, int categoryId);

        GenericResponse<IEnumerable<CredentialTypeUpgradePathModel>> GetValidCredentialTypeUpgradePaths(CredentialPathRequestModel request);

        GenericResponse<IEnumerable<LookupTypeModel>> GetCredentialTypesForApplicationType(int applicationTypeId);
        GenericResponse<IEnumerable<SkillLookupTypeModel>> GetSkillsForCredentialType(IEnumerable<int> credentialTypes, IEnumerable<int> credentialApplicationTypes);
        IEnumerable<CredentialApplicationAttachmentModel> ListAttachments(int id);
        int CreateOrReplaceAttachment(CredentialApplicationAttachmentModel request);
        void DeleteAttachment(int id);
        FileModel ExportApplicationsExcel(ApplicationExport request);
        GenericResponse<IEnumerable<LookupTypeModel>> GetCredentialCategoriesForApplicationType(int applicationId);
        GenericResponse<IEnumerable<string>> GetDocumentTypesForApplicationType(int applicationId);
        GenericResponse<IEnumerable<LookupTypeModel>> GetCredentialTypeDomains(IEnumerable<int> credentialTypeIds);
        GenericResponse<InvoicePreviewModel> GetInvoicePreview(ApplicationActionWizardModel wizardModel);
        GenericResponse<IEnumerable<CredentialApplicationEmailMessageModel>> GetEmailPreview(ApplicationActionWizardModel model);
        GenericResponse<IEnumerable<EmailTemplateModel>> GetEmailTemplates(ApplicationActionWizardModel model);

        GenericResponse<IEnumerable<CredentialApplicationEmailMessageModel>> GetCredentialRequestBulkActionEmailPreview(CredentialRequestsBulkActionWizardModel wizardModel);
        GenericResponse<bool> CheckDuplicatedApplication(int naatiNumber, int credentialApplicationTypeId);
        // TODO: this should not be returning a type from the service API. Use a BusinessService type. 
        GenericResponse<IEnumerable<DocumentData>> GetIssueCredentialPreview(DocumentsPreviewRequestModel model);
        GenericResponse<WizardIssueCredentialStepModel> GetWizardIssueCredentialData(int applicationId, int credentialRequestId, int actionId);

        GenericResponse<IEnumerable<InvoiceModel>> GetWorkflowCredentialRequestActionInvoices(int actionId, int credentialRequestId);
        GenericResponse<IEnumerable<InvoiceModel>> GetWorkflowApplicationActionInvoices(int actionId, int applicationId);
        GenericResponse<IEnumerable<int>> UpdateOutstandingInvoices(UpdateOutstandingInvoicesRequestModel request);
        BusinessServiceResponse UpdateOutstandingRefunds(OutstandingRefundsRequestModel request);

        GenericResponse<IEnumerable<CredentialRequestSummarySearchResultModel>> SearchCredentialRequestSummary(CredentialRequestSummarySearchRequest request);
        GenericResponse<CredentialRequestSummaryItemModel> GetCredentialRequestSummaryItem(CredentialRequestBulkActionRequest request);
        
        ProductSpecificationModel GetApplicationAssessmentFee(int applicationId);
        ProductSpecificationModel GetApplicationFee(int applicationId);
        ProductSpecificationModel GetTestFee(int applicationTypeId, int credentialTypeId);
        ProductSpecificationModel GetSupplementaryTestFee(int applicationTypeId, int credentialTypeId);
        ProductSpecificationModel GetPaidReviewFee(int applicationTypeId, int credentialTypeId);
        void ProcessInvoiceFee(int feeId);
        void ProcessPaymentFee(int feeId);
        void RemoveFee(int feeId);
        IList<ValidationResultModel> ValidateNewCertificationPeriod(string practitionerNumber, DateTime periodStart, DateTime periodEnd);
        bool HasTestFee(int credentialApplicationTypeId, int credentialTypeId);
        bool HasTest(int credentialApplicationTypeId, int credentialTypeId);
        GenericResponse<IEnumerable<TestTaskLookupTypeModel>> GetTestTask(IEnumerable<int> credentialTypes);

        ApplicationInvoiceCreateRequestModel GetMyNaatiInvoicePreview(ApplicationActionWizardModel wizardModel);

        bool HasAnyFee(int applicationTypeId);

        string GetInvoiceNumber(int applicationId);

        GenericResponse<IEnumerable<LookupTypeModel>> GetCredentialTypeSkillsTestSession(
            IEnumerable<int> credentialTypeIds);

        GetEmailTemplateResponse GetCredentialApplicationEmailTemplate(GetEmailTemplateRequest request);
       
        GetApplicationDetailsResponse GetApplicationDetailsByApplicationId(GetApplicationDetailsRequest request);
		GenericResponse<IEnumerable<CredentialModel>> CredentialsForRecertification(int applicationId, int naatiNumber);

        GenericResponse<UpsertApplicationResultModel> CreateMyNaatiApplication(UpsertApplicationRequestModel model);

        void MoveCredential(MoveCredentialModel model);

        DowngradedCredentialRequestModel GetDowngradedCredentialRequest(int credentialRequestId);

        CreateCredentialCertificateResponse SaveCredentialDocuments(CreateCredentialDocumentsRequestModel request);

        CredentialModel CreateOrUpdateCredential(CreateOrUpdateCredentialModel request);
        CredentialModel GetCredentialBeingRecertified(int naatinumber, int skillId, int credentialTypeId);

        /// <summary>
        /// Same as GetCredentialBeingRecertified except doesnt exlude if status is not set to being assessed.
        /// </summary>
        /// <param name="naatinumber"></param>
        /// <param name="skillId"></param>
        /// <param name="credentialTypeId"></param>
        /// <returns></returns>
        CredentialModel GetCredentialRecertifications(int naatinumber, int skillId, int credentialTypeId);
        CredentialApplicationDetailedModel GetApplicationDetails(int applicationId);
        void RollbackIssueCredential(RollbackIssueCredentialModel model);
        CheckOptionData GetCheckOptionMessage();
        CheckOptionData GetIncompletePrerequisiteCheckOptionData();
        CheckOptionData GetIssueOnHoldCredentialsCheckOptionData();

        GenericResponse<IEnumerable<DocumentData>> CreateCredentialDocuments(CreateCredentialDocumentsRequestModel request);
        GenericResponse<IEnumerable<LookupTypeModel>> GetCredentialApplicaionTypes(IEnumerable<int> skillTypeIds);

        GenericResponse<IEnumerable<LookupTypeModel>> GetEndorsementQualificationLookup(GetEndorsementQualificationLookupRequestModel request);
        GenericResponse<IEnumerable<LookupTypeModel>> GetEndorsementLocationLookup(GetEndorsementLocationLookupRequestModel request);
        void UpdateCredentialApplicationRefundRequest(RefundModel refundModel);
        GenericResponse<IEnumerable<RefundRequestsGroupingModel>> GetApprovalPendingRefundRequests();
        GenericResponse<bool> ApproveRefundRequests(IEnumerable<RefundRequestsGroupingModel> model);

        DateTime GetLeapYearAdjustedEndDate(DateTime startDate, int policyYears);
        GenericResponse<IEnumerable<PaymentMethodTypeModel>> GetPaymentMethodTypes();
        GenericResponse<PrerequisiteApplicationsResult> GetPrerequisiteApplications(int applicationId);
        GenericResponse<List<PrerequisiteApplicationsNullableApplicationsModel>> GetPrerequisiteApplicationsNullableApplications(int credentialRequestId);
        /// <summary>
        /// Update on hold status credentials to on hold to be issued status
        /// </summary>
        /// <param name="onHoldCredential"></param>
        /// <returns>Generic Response</returns>
        GenericResponse<bool> UpdateOnHoldCredentialToOnHoldToBeIssued(OnHoldCredential onHoldCredential);
        /// <summary>
        /// Creates notes for on hold to be issued credentials
        /// </summary>
        /// <param name="noteModel"></param>
        /// <returns></returns>
        GenericResponse<bool> CreateApplicationNotesForOnHoldToBeIssued(ApplicationNoteModel noteModel);
    }

    public class CreateOrUpdateCredentialModel
    {
        public int CredentialId { get; set; }
        public int CredentialRequestId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? ExpiryDate { get; set; }
        public DateTime? TerminationDate { get; set; }
        public bool ShowInOnlineDirectory { get; set; }
        public CertificationPeriodDto CertificationPeriod { get; set; }
    }

    public class RollbackIssueCredentialModel
    {
        public int ApplicationId { get; set; }
        public int CredentialRequestId { get; set; }
        public int ApplicationOriginalStatusId { get; set; }
        public DateTime ApplicationOriginalStatusDate { get; set; }
        public int ApplicationOriginalStatusUserId { get; set; }
        public int CredentialRequestOriginalStatusId { get; set; }
        public DateTime CredentialRequestOriginalStatusDate { get; set; }
        public int CredentialRequestOriginalStatusUserId { get; set; }
        public CredentialModel Credential { get; set; }
        public IEnumerable<int> StoredFileIds { get; set; }
    }


    
    public class CheckOptionData
    {
        public bool Checked { get; set; }
        public string Message { get; set; }

        public string OnDisableMessage { get; set; }

        public bool ReadOnly { get; set; }
    }

    public class IssueOnHoldCredentialCheckOptionData : CheckOptionData
    {
        public string OnEnableMessage { get; set; }
    }
    public class DowngradedCredentialRequestModel
    {
        public int CredentailTypeId { get; set; }
        public string CredentialTypeInternalName { get; set; }
        public string CredentialTypeExternalName { get; set; }
        public int SkillId { get; set; }
        public string Skill { get; set; }
        public int CategorId { get; set; }
        public int NaatiNumber { get; set; }

        public bool HasCredential { get; set; }
        public bool Certification { get; set; }
    }

    public enum ApplicationWizardSteps
    {
        SelectCredential = 1,
        Notes = 2,
        ViewInvoice = 3,
        IssueCredential = 4,
        DocumentsPreview = 5,
        EmailPreview = 6,
        DeleteConfirmation = 7,
        SupplementaryTest = 8,
        IssueConcededPass = 9,
        ExistingConcededCredential= 10,
        NotFoundConcededCredential= 11,
        CheckOption = 12,
        ConfigureRefund = 13,
        ApproveRefund = 14,
        ViewMessage = 15,
        PrerequisiteSummary = 16,
        IncompletePrerequisiteCredentials = 17,
        PrerequisiteApplications = 18,
        PrerequisiteMandatoryFields = 19,
        PrerequisiteMandatoryDocumentTypes = 20,
        PrerequisiteConfirmApplicationCreation = 21,
        NoNeedToContinue = 22, //This is only used in the scope of JS, but will put it here to line up with enums.js
        PrerequisiteExemptions = 23,
        PrerequisiteIssueOnHoldCredentials = 24,
        ComposeEmail = 25,
    }

    public enum CredentialRequestWizardSteps
    {
        TestSession = 1,
        ExistingApplicants = 2,
        NewApplicants = 3,
        Notes = 4,
        PreviewInvoice = 5,
        ViewEmailAttachments = 6,
        CheckOption = 7,
        ViewInvoice =8,
        ViewMessage = 9
    }

 

    public class SkillLookupTypeModel : LookupTypeModel
    {
        public int CredentialTypeId { get; set; }
        public int DirectionTypeId { get; set; }
        public int Language1Id { get; set; }
        public int Language2Id { get; set; }
    }

    public class TestTaskLookupTypeModel : LookupTypeModel
    {
        public int TestComponentBaseTypeId { get; set; }
        public int TestSpecificationId { get; set; }
        public bool Active { get; set; }
        public bool CandidateBriefRequired { get; set; }
        public double DefaultMaterialRequestHours { get; set; }
    }

    public class BasicCertificationPeriodModel
    {
        public int Id { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public DateTime OriginalEndDate { get; set; }

        public int CredentialApplicationId { get; set; }
    }

    public enum DirectionTypeNames
    {
        Language1toLanguage2 = 1,
        Language2toLanguage1 = 2,
        Language1AndLanguage2 = 3,
        Language1 = 4,
    }

    public class GetEndorsementQualificationLookupRequestModel
    {
        public IEnumerable<string> Locations { get; set; }
        public int[] InstitutionNaatiNumbers { get; set; }
    }

    public class GetEndorsementLocationLookupRequestModel
    {
        public int[] InstitutionNaatiNumbers { get; set; }
    }
}