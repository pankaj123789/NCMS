using System;
using System.Collections.Generic;
using F1Solutions.Naati.Common.Contracts.Bl.DTO;
using F1Solutions.Naati.Common.Contracts.Dal.DTO;
using F1Solutions.Naati.Common.Contracts.Dal.Enum;
using F1Solutions.Naati.Common.Contracts.Dal.Request;
using F1Solutions.Naati.Common.Contracts.Dal.Response;

namespace F1Solutions.Naati.Common.Contracts.Dal.QueryServices
{

    public interface IApplicationQueryService : IQueryService
    {
   
        ApplicationSearchResultResponse SearchApplication(GetApplicationSearchRequest request);

        CredentialRequestSummarySearchResultResponse SearchCredentialRequestSummary(
            GetCredentialRequestSummarySearchRequest request);

  
        ApplicationsWithCredentialRequestsResponse GetApplicationsWithCredentialRequests(
            GetApplicationSearchRequest request);

      
        CredentialLookupTypeResponse GetAvailableCredentials(int applicationId, int naatiNumber);

        
        GetSkillsForCredentialTypeResponse GetAdditionalSkills(GetCredentialTypeSkillsRequest request);

        
        GetTestTaskResponse GetTestTask(GetTestTaskRequest request);

        
        ApplicationSearchResultResponse GetApplicationsForCredential(int credentialId);

        
        GetApplicationDetailsResponse GetApplicationDetails(GetApplicationDetailsRequest request);

        
        CredentialRequestsResponse GetCredentialRequests(int credentialApplicationId, IEnumerable<CredentialRequestStatusTypeName> excludedStatuses);

        
        AvailableTestSessionsResponse GetAllAvailableTestSessionsAndRejectableTestSession(int credentialRequestId);

        
        CredentialTypeUpgradePathResponse GetValidCredentialTypeUpgradePaths(GetUpgradeCredentialPathRequest request);
        
        CredentialRequestBasicResponse GetBasicCredentialRequestsByApplicationId(int credentialApplicationId,IEnumerable<CredentialRequestStatusTypeName> excludedStatuses);

        
        CredentialRequestTestResponse GetAllCredentialTests(GetAllCredentialTestsRequest request);

        
        CredentialRequestsResponse GetOtherCredentialRequests(int credentialApplicationId);

        
        CredentialRequestResponse GetCredentialRequest(int credentialRequestId);

        RefundDtoResponse GetCredentialRequestRefunds(int credentialRequestId);



        CredentialRequestResponse GetCredentialRequestForUser(int naatiNumber, int credentialRequestId);

        
        LookupTypeResponse GetLookupType(string lookupType);

        
        GetCredentialApplicationTypeResponse GetCredentialApplicationType(int credentialApplicationTypeId);

        
        UpsertApplicationResponse UpsertApplication(UpsertCredentialApplicationRequest request);

        
        GetApplicationResponse GetApplication(int applicationId);

        
        GetApplicationResponse GetApplicationByCredentialRequestId(int credentialRequestId);

        
        GetApplicationFieldsDataResponse GetApplicationFieldsData(int applicationId);

        
        CredentialLookupTypeResponse GetCredentialTypesForApplication(int applicationId, int? categoryId = null);

        
        LookupTypeResponse GetCredentialTypesForApplicationType(int applicationTypeId);

        
        GetSkillsForCredentialTypeResponse GetSkillsForCredentialType(GetSkillsForCredentialTypeRequest request);

        
        LookupTypeResponse GetCredentialCategoriesForApplicationType(int applicationId);

        
        LookupTypeResponse GetVenue(IEnumerable<int> testLocationId);

        /// <summary>
        /// Returns a list of Venues for a location.
        /// Active venues are at the top and inactive ones are labelled and at the bottom
        /// </summary>
        /// <param name="testLocationId"></param>
        /// <returns></returns>
        LookupTypeResponse GetVenuesShowingInactive(IEnumerable<int> testLocationId);

        LookupTypeResponse GetCredentialTypeSkills(GetCredentialTypeSkillsRequest request);

        List<SkillEmailTokenObject> GetCredentialSkills(GetCredentialSkillsRequest request);



        GetDocumentTypesForApplicationTypeResponse GetDocumentTypesForApplicationType(int applicationId);

        
        CreateOrReplaceApplicationAttachmentResponse CreateOrReplaceAttachment(
            CreateOrReplaceApplicationAttachmentRequest request);

        
        GetApplicationAttachmentsResponse GetAttachments(GetApplicationAttachmentsRequest request);

        
        DeleteApplicationAttachmentResponse DeleteAttachment(DeleteApplicationAttachmentRequest serviceRequest);

        
        CreateCredentialCertificateResponse CreateCredentialDocuments(CreateCredentialDocumentsRequest request);

        
        GetCredentialTypeTemplateResponse GetCredentialTypeTemplates(GetCredentialTypeTemplateRequest request);

        
        GetEmailTemplateResponse GetEmailTemplate(GetEmailTemplateRequest request);

        
        bool CheckDuplicatedApplication(int naatiNumber, int credentialApplicationTypeId);

        
        GetCredentialApplicationFormResponse GetCredentialApplicationForm(int applicationFormId);

        
        GetCredentialApplicationFormSectionsResponse GetCredentialApplicationFormSections(int applicationFormId);

        
        CredentialApplicationFormQuestionDto GetCredentialApplicationFormQuestion(int questionId);

        
        CredentialLookupTypeResponse GetCredentialTypesForApplicationForm(int applicatonFormId);

        
        GetSkillsDetailsResponse GetSkillsDetailsForCredentialType(GetSkillsDetailsRequest request);

        
        void UpdateCredential(CredentialDto credentialDto);

        
        GetUserResponse GetUser(GetUserRequest request);

        
        CreateOrUpdateCredentialResponse CreateOrUpdateCredential(CreateOrUpdateCredentialRequest request);

        void UpdateCredentialApplicationRefundRequest(UpdateCredentialApplicationRefundRequest refundDto);


        void RollbackIssueCredential(RollbackIssueCredentialRequest request);

        
        GetCredentialAttachmentsByIdResponse GetCredentialAttachmentsById(GetCredentialAttachmentsByIdRequest request);

        
        GetCredentialAttachmentFileResponse GetCredentialAttachmentFileByCredentialAttachmentId(
            GetCredentialAttachmentFileRequest request);

        
        CredentialsResponse GetCredential(GetCredentialRequest request);

        
        GetInvoicesResponse GetActionInvoices(GetActionInvoicesRequest request);

        
        GetOutstandingInvoicesResponse GetOutstandingInvoices(GetOutstandingInvoicesRequest request);

        
        void UpdateProcessedWorkflowFees(UpdateProcessedWorkflowFeeRequest request);

        
        void RemoveWorkFlowFees(RemoveWorkflowFeeRequest request);

        
        GetApplicationTypeFeesResponse GetApplicationTypeFees(int credentialApplicationTypeId, FeeTypeName feeType);

        
        CredentialRequestApplicantsResponse GetCredentialRequestApplicants(CredentialRequestApplicantsRequest request);

        
        GetCredentialRequestsCountResponse GetCredentialRequestsCount(int credentialApplicationId,
            IEnumerable<CredentialRequestStatusTypeName> excludedStatuses);

        
        bool GetCheckCredentialRequestBelongsToCurrentUser(int credentialApplicationId,
            IEnumerable<CredentialRequestStatusTypeName> excludedStatuses, int checkCredentialRequestId);

        
        bool HasTestFee(int credentialApplicationTypeId, int credentialTypeId);

        
        bool HasTest(int credentialApplicationTypeId, int credentialTypeId);

        
        bool HasAnyFee(int credentialApplicationTypeId);

        
        GetCredentialsCountResponse GetCredentialsCount(int credentialApplicationId);

        
        void DeleteWorkflowFees(DeleteVoidedInvoicesRequest request);

        
        string GetInvoiceNumberByApplicationId(int applicationId);

        
        int? GetFeesQuestionId();

        
        int GetNaatiNumberByApplicationId(int applicationId);
     

        
        WorkFlowFeesResponse GetInvoicesAndPaymentsToProcess();

        RefundDtoResponse GetCreditNotesAndPaymentsToProcess();

        RefundDtoResponse GetOutstandingRefunds(GetOutstandingRefundRequest request);

        CredentialWorkFlowFeesCredentialRequestResponse GetPaidWorkflowFeesForCredentialRequest(int credentialRequestId);

        LookupTypeResponse GetCredentialTypeSkillsTestSession(GetCredentialTypeSkillsRequest request);

        
        RecertificationRequestStatusResponse GetRecertificationRequestStatus(int credentialId);

        
        CertificationPeriodDetailsResponse GetCertificationPeriodDetails(int certificationPeriodId);

        
        MoveCredentialResponse MoveCredential(MoveCredentialRequest request);

        
        LookupTypeResponse GetLocations(int applicationFormId);

        
        DowngradedCredentialRequestDto GetDowngradedCredentialRequest(int credentialRequestId);

        
        CredentialRequestInfoDto GetCredentialRequestForCredential(int credentialId);

        
        CredentialRequestInfoDto GetSubmittedRecertificationRequestForCredential(int credentialId);

        
        FormLookupTypeResponse GetCredentialApplicationForms(GetFormRequest request);

        
        CredentialLookupTypeResponse GetCredentialApplicationFormCredentialTypes(int credentailApplicationFormId);

        
        CredentialTypeResponse GetCredentialTypeByTestSittingId(int testSittingId);

        
        bool IsValidCredentialByNaatiNumber(int credentialId, int naatiNumber);
        
        LookupTypeResponse GetCredentialApplicaionTypes(GetCredentialApplicaionTypesRequest request);

        
        bool HasAvailableTestSessions(int credentialRequestId);

        
        CredentialRequestBasicResponse GetBasicCredentialRequestsByNaatiNumber(int naatiNumber,IEnumerable<CredentialRequestStatusTypeName> excludedStatuses);

        
        GetEndorsedQualificationForApplicationFormResponse GetEndorsedQualificationForApplicationForm(int applicationFormId, int applicationId);

        
        LookupTypeResponse GetEndorsementQualificationLookup(GetEndorsementQualificationLookupRequest request);

        
        LookupTypeResponse GetEndorsementLocationLookup(GetEndorsementLocationLookupRequest request);

        IEnumerable<int> GetCertificationPeriodCredentialFromApplicationId(int applicationId);
        IEnumerable<int> GetExpiringCertificationPeriodApplications(GetExpiringCertificationPeriodRequest request);
        bool HasSubmittedApplications(int naatiNumber);
        bool CanCreateNewApplication(int naatiNumber, int applicationTypeId);
        IList<BasicApplicationSearchDto> GetDraftApplications(int naatiNumber, int applicationTypeId);

        IEnumerable<int> GetApplicationIdsWithRecertificationReminders(GetApplicationIdsWithRecertificationRemindersRequest request);
        IEnumerable<TestSessionReminderObject> GetTestSessionReminders(GetTestSessionRemindersRequest request);

        CertificationPeriodDetailsResponse GetCertificationPeriodDetailsByApplicationId(int applicationId);
        IEnumerable<CredentialRequestWithPendingRefundRequest> GetCredentialRequestsWithPendingRefundRequests();

        IEnumerable<ApprovalPendingRefundRequestDto> GetApprovalPendingRefundRequests();

        CredentialWorkflowFeeDto GetCredentialWrokflowFeeById(int credentialWorkflowFeeId);

        CredentialApplicationRefundPolicyData GetCredentialApplicationRefundPolicy(int credentialTypeId, int applicationTypeId, int productSpecificationId);
        RefundDtoResponse GetNotFlushedProcessedRefunds(NotFlushedRefundRequest request);
        void FlushRefundBankDetails(IList<int> refundIds);
        IEnumerable<PaymentMethodTypeModel> GetPaymentMethodTypes();
        CredentialTypeResponse GetCredentialTypeById(int credentialTypeId);
        int GetSkillFromLanguagesAndCredentialApplicationType(int? direction, string language1, string language2, int applicationTypeId, int? credentialTypeId);

        /// <summary>
        /// Get new Credentials where the Applicant has been previously deemed eligible (TFS 200196)
        /// Only get credentials that have Eligible For Testing in their path
        /// </summary>
        /// <returns></returns>
        GenericResponse<List<NewCredentialsThatCanProgressToEligibleForTesting>> GetNewCredentialsThatCanprogressToEligibleForTesting();
        GenericResponse<DateTime?> GetPaypalPaymentProcessedDate(int credentialRequestId);
        GenericResponse<bool> UpdateOnHoldCredentialToOnHoldToBeIssued(OnHoldCredential onHoldCredential);
        GenericResponse<List<CredentialRequestDto>> GetCredentialRequestsOnHoldToBeIssued();
        /// <summary>
        /// Gets true or false depending on whether the person is a practitioner or not
        /// </summary>
        /// <param name="naatiNumber"></param>
        /// <returns>True if the person is a practitioner. False if they are not.</returns>
        GenericResponse<bool> GetIsPractitionerFromNaatiNumber(int naatiNumber);
        /// <summary>
        /// Creates notes for on hold to be issued credentials
        /// </summary>
        /// <param name="applicationNoteDto"></param>
        /// <returns>Generic Response</returns>
        GenericResponse<bool> CreateNotesForOnHoldToBeIssued(ApplicationNoteDto applicationNoteDto);

        /// <summary>
        /// Vrifies the Stored File is a Credential Attachment and that it belongs to the user.
        /// </summary>
        /// <param name="storedFileId"></param>
        /// <param name="currentUserNaatiNumber"></param>
        /// <returns></returns>
        GenericResponse<bool> DoesCredentialBelongToUser(int storedFileId, int currentUserNaatiNumber);
    }

    public enum DataTypeName
    {
        Bool = 1,
        Text = 2,
        Date = 3,
        CountryLookup = 4,
        Options = 5,
        Email = 6,
        EndorsedQualificationLookup = 7,
        EndorsedQualificationStartDate = 8,
        EndorsedQualificationEndDate = 9,

        EndorseInstitutionLookup  = 10,
        EndorsedLocationLookup = 11,
        EndorsedQualificationIdLookup = 12,
        RadioOptions = 13
    }
}