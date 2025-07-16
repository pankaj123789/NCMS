using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.ServiceModel;
using F1Solutions.Naati.Common.Contracts.Bl;
using F1Solutions.Naati.Common.Contracts.Bl.DTO;
using F1Solutions.Naati.Common.Contracts.Dal.Response;

namespace MyNaati.Contracts.BackOffice
{
    
    public interface ICredentialApplicationService : IInterceptableservice
    {
        
        bool IsDisplayBills(int naatiNumber);
        
        FormLookupResponse GetPublicApplicationForms(bool isPractitioner, bool isRecertification, bool isAutenticated);

        
        LookupResponse GetPrivateApplicationForms();

        
        LookupResponse GetPractionerApplicationForms();
        
        LookupResponse GetRecertificationApplicationForms();

        
        LookupResponse GetActiveApplicationForms();

        
        GetApplicationFormResponse GetCredentialApplicationForm(int applicationFormId);

        
        GetApplicationFormSectionsResponse GetCredentialApplicationFormSections(GetFormSectionRequestContract request);

        
        GetApplicationDetailsResponse SaveApplicationForm(SaveApplicationFormRequestContract request);

        GetApplicationDetailsResponse GetApplicationDetails(int applicationId);


        VerifyPersonResponse VerifyPerson(VerifyPersonRequestContract request);

        
        VerifyPersonResponse UpdateOrVerifyPersonDetails(VerifyPersonRequestContract request, bool isLoggedIn);

        
        GetCredentialApplicationResponse GetExistingDraftApplication(ExitingApplicationRequestContract request);

        
        CreateCredentialApplicationResponse CreateCredentialApplication(CreateApplicationRequest request);

        
        PersonDetailsResponse GetPersonDetails(int naatiNumber);

        
        LookupResponse GetLanguagesForApplicationForm(int applicationFormId, int applicationId, int naatiNumber);
        
        GetEndorsedQualificationForApplicationFormResponse GetEndorsedQualificationForApplicationForm(int applicationFormId, int applicationId, int nAATINumber);

        
        SkillLookupResponse GetLanguagesForCredentialTypes(List<int> credentialTypes);

        
        GetCredentialRequestsResponse GetCredentialRequests(int applicationId);

        
        LookupResponse GetCredentialCategoriesForApplication(int applicationId, int applicationFormId);

        
        CredentialLookupTypeResponse GetAvailableCredentials(int applicationId, int naatiNumber, int applicationFormId);

        
        SkillLookupResponse GetAdditionalSkills(GetSkillsForApplicationForCredentialTypeRequest request);

        
        LookupResponse GetCredentialTypesForApplicationForm(int applicationFormId, int applicationId, int? categoryId);

        
        SkillLookupResponse GetSkillsForApplicationForCredentialType(GetSkillsForApplicationForCredentialTypeRequest request);

        
        SaveCredentialRequestResponse SaveCredentialRequest(CredentialRequestRequestContract request);

        
        void DeleteCredentialRequest(DeleteCredentialRequestContract request);

        
        CreateOrReplaceAttachmentResponse CreateOrReplaceAttachment(CreateOrReplaceAttachmentContract request);

        
        void DeleteAttachment(int attachmentId);


        GetSittableCredentialRequestResponse GetSittableRequestsByNaatiNumberAndApplicationId(int naatiNumber, int applicationId);

        GetSittableCredentialRequestResponse GetSittableRequestsByNaatiNumber(int? naatiNumber);


        GetSittableCredentialRequestResponse GetPayableRequestsByNaatiNumber(int? naatiNumber);

        GetAvailableTestSessionsResponse GetAllAvailableTestSessionsAndRejectableTestSessions(int credentialRequestId);

		
		GetTestResultsResponse GetTestResults(int naatiNumber);

		
        bool CheckCredentialRequestBelongsToCurrentUser(int? naatiNumber, int credentialRequestId);

        
        bool HasAvailableTests(int? naatiNumber);

        
        bool HasCredentialsByNaatiNumber(int? naatiNumber);

        GenericResponse<bool> GetIsPractitionerFromNaatiNumber(int naatiNumber);

        GetManageTestSessionSamResponse GetManageTestSession(int naatiNumber, int credentialRequestId);

        
        AttachmentsResponse GetAttachments(int applicationId);

        /// <summary>
        /// Need to confirm that the StoredFileId belongs to the ucrrent user and is a Credential Attachment
        /// </summary>
        /// <param name="storedFileId"></param>
        /// <param name="currentUserNaatiNumber"></param>
        /// <returns></returns>
        GenericResponse<bool> DoesCredentialBelongToUser(int storedFileId, int currentUserNaatiNumber);

        DocumentTypesResponse GetDocumentTypes(GetDocumentTypesRequestContract request);

        
        LookupResponse GetLocations(int applicationFormId);

        
        void UpdatePersonPhoto(UpdatePhotoRequestContract request);

        
        PersonImageResponse GetPersonImage(GetImageRequestContract request);

        
        bool PersonHasPhoto(int naatiNumber);

        
        GetAllCredentialsResponse GetAllCredentialsByNaatiNumber(int naatiNumber);

        
        bool ValidateApplicationToken(int applicationId, int token);

        
        GetPersonDetailsBasicResponse GetPersonDetailsBasic(int naatiNumber);

        
        bool ValidatePersonToken(int naatiNumber, int token);

        
        void UpdateCredential(CredentialContract credential);

        
        GetCredentialAttachmentsByIdResponse GetCredentialAttachmentsById(int credentialId, int naatiNumber);

        
        GetCredentialAttachmentFileResponse GetCredentialAttachmentFileByCredentialAttachmentId(GetCredentialAttachmentFileRequest request);
        bool IsDisplayInvoices(int naatiNumber);

        GetTestSessionSkillAvailabilityResponse GetAvailableTestSessions(GetAvailableTestSessionRequest request);

        
        GetApplicationTypeFeesResponse GetApplicationTypeFees(int applicationFormId);

        
        string GetInvoiceNumberByApplicationId(int applicationId);

        
        int? GetFeeQuestionId();

        
        int GetNaatiNumberByApplicationId(int applicationId);
        
        ApplicationFormQuestionContract ReplaceQuestionFormTokens(ReplaceFormTokenRequest request);

        
        WorkPracticeStatusResponse GetRecertficationtWorkPracticeStatus(WorkPracticeStatusRequest request);

        

        RecertificationOptionsResponse GetRecertificationOptions(int naatiNumber);

        
        PdPointsStatusResponse GetRecertficationPdPointsStatus(PdPointsStatusRequest request);

        
        GetApplicationFormSectionsResponse GetPaidReviewSections(int naatiNumber, int testSittingId);
        
        GetApplicationFormSectionsResponse GetSupplementaryTestSections(int naatiNumber, int testSittingId);
        
        GetApplicationFormSectionsResponse GetSelectTestSessionSections(int naatiNumber, int testSessionId, int credentialRequestId, int credentialApplicationId);


        
        TestFeeContract GetPaidReviewTestDetails(int naatiNumber, int testSittingId);
        
        TestFeeContract GetSupplementaryTestDetails(int naatiNumber, int testSittingId);
        
        TestFeeContract GetSelectTestSessionTestDetails(int testSessionId, int applicationId, int credentialRequestId);

        
        SupplementaryTestTaskResponse GetSupplementaryTestTasks(int naatiNumber, int testSittingId);
        
        GetCredentialsResponse GetActiveAndFutureCredentials(int naatiNumber);

        
        GetCredentialRequestsResponse GetInProgressCredentialRequests(int naatiNumber);

        
        CredentialTypeResponse GetCredentialTypeByTestSittingId(int testSittingId);

        
        bool IsValidCredentialByNaatiNumber(int credentialId, int naatiNumber);

        
        IEnumerable<int> GetPersonNaatiNumber(string email);

        
        bool HasSubmittedApplications(int naatiNumber);

        CredentialApplicationRefundPolicyResponse GetCredentialApplicationRefundPolicy(int credentailTypeId, int applicationTypeId, int productSpecifcationId);
    }

    public class GetCredentialsResponse
    {
        public IEnumerable<CredentialContract> Credentials { get; set; }
    }

  




    public class RecertificationOptionsResponse
    {
        
        public IEnumerable<AnswerCredentialOptionContract> Options { get; set; }
    }

    
    public class WorkPracticeStatusRequest
    {
        
        public int ApplicationId { get; set; }
        
        public int NaatiNumber { get; set; }
    }
    
    public class WorkPracticeStatusResponse
    {
        
        public bool Met { get; set; }
    }

    
    public class PdPointsStatusResponse
    {
        
        public bool Met { get; set; }
    }

    
    public class PdPointsStatusRequest
    {
        
        public int ApplicationId { get; set; }
        
        public int NaatiNumber { get; set; }
    }

    
    public class ReplaceFormTokenRequest
    {
        
        public  int QuestionId { get; set; }
        
        public  int ApplicationFormId { get; set; }
        
        public  int NaatiNumber { get; set; }

        
        public IEnumerable<KeyValuePair<string, string>> ExternalUrls { get; set; }
    }


    
    public class GetAvailableTestSessionRequest
    {
        
        public int ApplicationId { get; set; }
      
    }

    
    public class GetTestSessionSkillAvailabilityResponse
    {
        
        public IEnumerable<TestSessionContract> Results { get; set; }
    }

    
    public class TestSessionContract
    {
        
        public DateTime TestDateTime { get; set; }
        
        public int?  Duration { get; set; }
        
        public string VenueName  { get; set; }
        
        public string CredentialTypeName  { get; set; }
        
        public string SkillName { get; set; }
        
        public int AvailableSeats { get; set; }

        public string TestLocation{ get; set; }
        public bool IsPreferedLocation { get; set; }

    }


    
    public class GetCredentialAttachmentFileRequest
    {
        
        public int StoredFileId { get; set; }
        
        
        public string TempFileStorePath { get; set; }

        
        public int NaatiNumber { get; set; }
    }

    
    public class GetCredentialAttachmentFileResponse
    {
        [MessageHeader]
        public string FileName { get; set; }

        
        public string[] FilePaths { get; set; }
    }

    
    public class GetAllCredentialsResponse
    {
        
        public IEnumerable<CredentialContract> Results { get; set; }

    }

    
    
    public class GetSittableCredentialRequestResponse
    {
        
        public IEnumerable<SittableCredentialRequestContract> Results { get; set; }
	}

	
	public class GetAvailableTestSessionsResponse
	{
		public IEnumerable<AvailableTestSessionContract> AvailableTestSessions { get; set; }
        public AvailableTestSessionContract AllocatedTestSession { get; set; }
	}

	
	public class GetTestResultsResponse
	{
		
		public IEnumerable<TestResultContract> Results { get; set; }
	}

	
    public class GetManageTestSessionSamResponse
    {
        
        public ManageTestSessionContract Result { get; set; }
    }

    
    public class GetCredentialAttachmentsByIdResponse
    {
        
        public IEnumerable<CredentialAttachmentContract> Results { get; set; }
    }


    
    public class CreateOrReplaceAttachmentResponse
    {
        
        public int StoredFileId { get; set; }
    }

    
    public class DocTypeContract
    {
        
        public int Id { get; set; }
        
        public string DisplayName { get; set; }
        
        public bool Required { get; set; }
    }

    
    public class GetDocumentTypesRequestContract
    {
        
        public int ApplicationId { get; set; }

        
        public int ApplicationFormId { get; set; }

        
        public IEnumerable<KeyValuePair<string, string>> ExternalUrls { get; set; }
        
        public int NaatiNumber { get; set; }
    }

    
    public class DocumentTypesResponse
    {
        
        public IEnumerable<DocTypeContract> Results { get; set; }
    }


    
    public class AttachmentsResponse
    {
        
        public IEnumerable<AttachmentContract> Results { get; set; }
    }

    
    public class AttachmentContract
    {
        
        public int Id { get; set; }
        
        public string Name { get; set; }
        
        public int TypeId { get; set; }
        
        public string Type { get; set; }
        
        public long Size { get; set; }
    }

    
    public class GetSkillsForApplicationForCredentialTypeRequest
    {
        
        public int NAATINumber { get; set; }

        
        public IEnumerable<int> CredentialTypeIds { get; set; }

        
        public int CredentialRequestPathTypeId { get; set; }
        public int ApplicationId { get; set; }
    }

    
    public class CreateOrReplaceAttachmentContract
    {
        
        public int CredentialApplicationAttachmentId { get; set; }
        
        public int CredentialApplicationId { get; set; }
        
        public int StoredFileId { get; set; }
        
        public string FileName { get; set; }
        
        public string Title { get; set; }
        
        public string FilePath { get; set; }
        
        public string StoragePath { get; set; }
        
        public string DocumentType { get; set; }
        
        public string UploadedByName { get; set; }
        
        public string UserName { get; set; }
        
        public int UploadedByUserId { get; set; }
        
        public DateTime UploadedDateTime { get; set; }
        
        public long FileSize { get; set; }
        
        public string FileType { get; set; }
        
        public int Type { get; set; }
        
        public string TokenToRemoveFromFilename { get; set; }
        public bool? PrerequisiteApplicationDocument { get; set; }

    }


    public class CredentialRequestRequestContract
    {
        
        public int ApplicationId { get; set; }
        
        public int CategoryId { get; set; }
        
        public int LevelId { get; set; }
        
        public int SkillId { get; set; }
        
        public int QuestionId { get; set; }
        
        public string UserName { get; set; }
    }

    
    public class SaveCredentialRequestResponse
    {
        
        public int CredentialRequestId { get; set; }
        
        public int CredentialRequestPathTypeId { get; set; }
    }


    
    public class GetCredentialRequestsResponse
    {
        
        public IEnumerable<CredentialRequestContract> Results { get; set; }
    }

    
    public class PersonDetailsResponse
    {
        
        public int NaatiNumber { get; set; }
        
        public string PhoneNumber { get; set; }
        
        public AddressContract Address { get; set; }
        
        public int EntityId { get; set; }

        public bool Deceased { get; set; }
    }

    
    public class AddressContract
    {
        
        public int Id { get; set; }
        
        public string StreetDetails { get; set; }

        
        public bool IsFromAustralia { get; set; }

        
        public string CountryName { get; set; }

        
        public int? PostCodeId { get; set; }

        
        public string CountryId { get; set; }

        
        public string SuburbName { get; set; }

        
        public string Postcode { get; set; }

        
        public string State { get; set; }

        
        public int? Latitude { get; set; }
        
        public int? Longitude { get; set; }
        
        public bool ValidateInExternalTool { get; set; }
    }

    
    public class GetFormSectionRequestContract
    {
        
        public int ApplicationId { get; set; }

        
        public int ApplicationFormId { get; set; }

        
        public IEnumerable<KeyValuePair<string, string>> ExternalUrls { get; set; }
        
        public bool IsPractitioner { get; set; }
        
        public bool IsRecertificationUser { get; set; }
        
        public int NaatiNumber { get; set; }
    }

    
    public class UpdatePhotoRequestContract
    {
        
        public int NaatiNumber { get; set; }

        public string PractitionerNumber { get; set; }
        
        public string FilePath { get; set; }
        
        public string TokenToRemoveFromFilename { get; set; }
    }

    
    public class GetImageRequestContract
    {
        
        public int? NaatiNumber { get; set; }
        public string PractitionerId { get; set; }
        
        public int? Width { get; set; }
        
        public int? Height { get; set; }
        
        public string TempFolderPath { get; set; }
    }

    
    public class PersonImageResponse
    {
        
        public string FilePath { get; set; }
    }

    
    public class GetCredentialApplicationResponse
    {
        
        public CredentialApplicationContract CredentialApplication { get; set; }

        
        public string Message { get; set; }
    }

    
    public class CredentialApplicationContract
    {
        
        public int Id { get; set; }
        
        public string ApplicationStatus { get; set; }
        
        public DateTime EnteredDate { get; set; }
        
        public DateTime StatusChangeDate { get; set; }
        
        public IEnumerable<ApplicationFormSectionContract> Sections { get; set; }
    }

    
    public class VerifyPersonRequestContract
    {
        
        public string FirstName { get; set; }
        
        public string FamilyName { get; set; }
        
        public DateTime DateOfBirth { get; set; }
        
        public string Email { get; set; }
        
        public int? NaatiNumber { get; set; }
        
        public string MiddleNames { get; set; }

        
        public int Title { get; set; }

        public string Gender { get; set; }

        public int CountryOfBirth { get; set; }
    }

    
    public class SaveApplicationFormRequestContract
    {
        
        public int ApplicationId { get; set; }
        
        public string UserName { get; set; }

        
        public int ApplicationFormId { get; set; }

        
        public int NaatiNumber { get; set; }

        
        public IList<ApplicationFormSectionContract> Sections { get; set; }
    }


    
    public class SaveApplicationFormResponse
    {
        
        public int ApplicationId { get; set; }
        
        public string ApplicationReference { get; set; }

        public string ErrorMessage { get; set; }
    }

    
    public class CreateCredentialApplicationResponse
    {
        
        public int ApplicationId { get; set; }
        
        public string ApplicationReference { get; set; }

    }

   
    public class CreateApplicationResultModel
    {
        public int CredentialApplicationId { get; set; }
    }

    
    public class VerifyPersonResponse
    {
        
        public int NaatiNumber { get; set; }

        
        public string Message { get; set; }
        
        public bool IsNewPerson { get; set; }

        
        public int Token { get; set; }
    }


    
    public class GetApplicationFormResponse
    {
        
        public ApplicationFormContract Result { get; set; }
    }

    
    public class ApplicationFormSectionContract
    {
        
        public int Id { get; set; }
        
        public string Name { get; set; }
        
        public int DisplayOrder { get; set; }
        
        public string Description { get; set; }
        
        public IEnumerable<ApplicationFormQuestionContract> Questions { get; set; }
        
        public bool HasTokens { get; set; }
    }

    
    public class ApplicationFormContract
    {
        
        public int Id { get; set; }
        
        public int CredentialApplicationTypeId { get; set; }
        
        public string CredentialApplicationTypeName { get; set; }
        
        public string Name { get; set; }
        
        public string Description { get; set; }
        
        public int FormUserTypeId { get; set; }
        
        public bool Inactive { get; set; }

    }

    
    public class ApplicationContract
    {
        
        public int Id { get; set; }
    }

    
    public class ExitingApplicationRequestContract
    {
        
        public int NaatiNumber { get; set; }
        
        public int ApplicationFormId { get; set; }
        
        public  IEnumerable<KeyValuePair<string, string>> ExternalUrls { get; set; }
    }

    
    public class CreateApplicationRequest
    {
        
        public int NaatiNumber { get; set; }
        
        public int ApplicationFormId { get; set; }
        
        public string UserName { get; set; }
    }

    
    public class GetApplicationFormSectionsResponse
    {

        
        public IEnumerable<ApplicationFormSectionContract> Results { get; set; }
    }


    
    public class ApplicationFormQuestionContract
    {
        
        public int Id { get; set; }
        
        public string Text { get; set; }
        
        public string Description { get; set; }
        
        public int DisplayOrder { get; set; }
        
        public int AnswerTypeId { get; set; }
        
        public string AnswerTypeName { get; set; }
        
        public int? CredentialApplicationFieldId { get; set; }
        
        public IEnumerable<AnswerOptionContract> AnswerOptions { get; set; }
        
        public IEnumerable<QuestionLogicContract> QuestionLogics { get; set; }
        
        public IList<object> Responses { get; set; }
        
        public object Response { get; set; }
        
        public bool HasTokens { get; set; }
    }

    
    public class ManageTestSessionContract
    {
        
        public int? TestSessionId { get; set; }
        
        public int? TestSessionCredentialRequestId { get; set; }
        
        public string CustomerNo { get; set; }
        
        public string TestStart { get; set; }
        
        public string Application { get; set; }
        
        public DateTime? TestDate { get; set; }
        
        public int Duration { get; set; }
        
        public int ArrivalTime { get; set; }
        
        public string CredentialType { get; set; }
        
        public string VenueName { get; set; }
        
        public string VenueAddress { get; set; }
        
        public string VenueCoordinates { get; set; }
        
        public string Skill { get; set; }
        
        public string Status { get; set; }
        
        public string Notes { get; set; }
        
        public int CredentialRequestId { get; set; }
        
        public int CredentialApplicationId { get; set; }
       
        
        public bool CanRejectTest { get; set; }
        
        public bool CanChangeRejectTestDate { get; set; }
    }

    public class AddressMapContract
    {
        public decimal Lat { get; set; }
        public decimal Lng { get; set; }
    }

    
    public class AnswerCredentialOptionContract : AnswerOptionContract
    {
        
        public int CredentialTypeId { get; set; }
    }

    
    public class AnswerOptionContract
    {
        
        public int Id { get; set; }
        
        public int FormAnswerOptionId { get; set; }
        
        public bool DefaultAnswer { get; set; }
        
        public int? CredentialApplicationFieldId { get; set; }
        
        public int DisplayOrder { get; set; }
        
        public string Option { get; set; }
        
        public string Description { get; set; }
        
        public int? FieldOptionId { get; set; }
        
        public OptionActionContract Function { get; set; }
        
        public IEnumerable<AnswerDocumentContract> Documents { get; set; }

        
        public string FieldData { get; set; }
        
        public bool HasTokens { get; set; }
    }

    
    public class OptionActionContract
    {
        
        public int ActionTypeId { get; set; }
        
        public string Parameter { get; set; }
        
        public int Order { get; set; }
        
        public string Name { get; set; }
    }

    
    public class AnswerDocumentContract
    {
        
        public int Id { get; set; }
        
        public int DocumentTypeId { get; set; }
        
        public string DisplayName { get; set; }
    }

    
    public class QuestionLogicContract
    {
        
        public int Id { get; set; }
        
        public int AnswerId { get; set; }
        
        public bool Not { get; set; }
        
        public bool And { get; set; }
        
        public QuestionLogicType Type { get; set; }
        
        public  int Group { get; set; }
        
        public int Order { get; set; }
        public bool? PdPointsMet { get; set; }
        public bool? WorkPracticeMet { get; set; }

        public int SkillId { get; set; }
    }

    [KnownType(typeof(LanguageLookupContract))]
    public class LookupResponse : BaseResponse
    {
        public string BaseUrl;
      
        public IEnumerable<LookupContract> Results { get; set; }

        public bool IsAuthenticated { get; set; }
    }

    
    public class FormLookupResponse
    {
        public string BaseUrl;

        
        public IEnumerable<FormLookupContract> Results { get; set; }

        public bool IsAuthenticated { get; set; }
    }

    public class CredentialLookupTypeContract : LookupContract
    {
        public int DisplayOrder { get; set; }

        public int CategoryId { get; set; }

        public IEnumerable<CredentialLookupTypeContract> Children { get; set; }

        public int CredentialRequestPathTypeId { get; set; }
    }

    
    public class CredentialLookupTypeResponse
    {
        
        public IEnumerable<CredentialLookupTypeContract> Results { get; set; }
    }

    
    public class SkillLookupResponse
    {
        
        public IEnumerable<SkillLookupContract> Results { get; set; }
    }

    public class GetEndorsedQualificationForApplicationFormResponse
    {
        public IEnumerable<EndorsedQualificationContract> Results { get; set; }
    }

    public class LanguageLookupResponse
    {

        public IEnumerable<LanguageLookupContract> Results { get; set; }
    }


    public class EndorsedQualificationContract
    {
        public int EndorsedQualificationId { get; set; }
        public int InstitutionId { get; set; }
        public int CredentialTypeId { get; set; }
        public string CredentialTypeExternalName { get; set; }
        public string Qualification { get; set; }
        public string Institution { get; set; }
        public string Location { get; set; }
        public string Notes { get; set; }
        public DateTime EndorsementPeriodFrom { get; set; }
        public DateTime EndorsementPeriodTo { get; set; }
        public bool Active { get; set; }
    }


    public class LookupContract
    {

        
        public int Id { get; set; }

        
        public string DisplayName { get; set; }
        
        public string ExtraData { get; set; }
    }


    
    public class FormLookupContract : LookupContract
    {
        
        public string Url { get; set; }
    }


    public class LanguageLookupContract : LookupContract
    {
        public int SkillId { get; set; }
    }


    public class SkillLookupContract : LookupContract
    {
        
        public int Language1Id { get; set; }

        
        public int Language2Id { get; set; }

        
        public string Language1Name { get; set; }

        
        public string Language2Name { get; set; }

        
        public string DirectionDisplayName { get; set; }

        
        public int CredentialTypeId { get; set; }
    }

    
    public class CredentialRequestContract
    {
        public int Id { get; set; }
        
        public string Category { get; set; }
        
        public string Level { get; set; }
        
        public string Skill { get; set; }
        
        public int CategoryId { get; set; }
        
        public int LevelId { get; set; }
        
        public int SkillId { get; set; }
        
        public int PathTypeId { get; set; }

        public string ApplicationReference { get; set; }
        public string ApplicationTypeDisplayName { get; set; }
    }

    public class DeleteCredentialRequestContract
    {
        public int CredentialRequestId { get; set; }
        public int ApplicationId { get; set; }
        public string UserName { get; set; }
    }


    public class CredentialDetailRequestContract
    {
        public int Id { get; set; }
        public string Category { get; set; }
        public int CategoryId { get; set; }
        public string CredentialName { get; set; }
        public string Direction { get; set; }
        public string Status { get; set; }
        public int StatusTypeId { get; set; }
        public int SkillId { get; set; }
        public DateTime StatusChangeDate { get; set; }
        public string ModifiedBy { get; set; }
        public int StatusChangeUserId { get; set; }
        public int CredentialTypeId { get; set; }
        public CredentialTypeContract CredentialType { get; set; }
        public SkillContract Skill { get; set; }
        public IEnumerable<CredentialRequestFieldContract> Fields { get; set; }
        public IList<CredentialContract> Credentials { get; set; }
        public IEnumerable<dynamic> Actions { get; set; }
        public bool Certification { get; set; }
	}

	public class SittableCredentialRequestContract
	{
		public int? TestSessionId { get; set; }
		public DateTime? TestDate { get; set; }
		public string ApplicationTypeDisplayName { get; set; }
		public string CredentialTypeDisplayName { get; set; }
		public string SkillDisplayName { get; set; }
		public string VenueName { get; set; }
		public string Status { get; set; }
		public int CredentialRequestStatusId { get; set; }
		public int CredentialRequestId { get; set; }
		public bool CanOpenDetails { get; set; }
		public bool CanSelectTestSession { get; set; }
		public int CredentialApplicationId { get; set; }
	
	    public string Direction { get; set; }
	    public bool Certification { get; set; }
        public bool CanRequestRefund { get; set; }
    }

	public class TestResultContract
	{
		public int TestSittingId { get; set; }
		public DateTime TestDate { get; set; }
		public string CredentialTypeDisplayName { get; set; }
		public string SkillDisplayName { get; set; }
		public string VenueName { get; set; }
		public string State { get; set; }
		public string TestLocationName { get; set; }
		public bool EligibleForAPaidTestReview { get; set; }
		public bool EligibleForASupplementaryTest { get; set; }
		public string OverallResult { get; set; }
	    public bool Supplementary { get; set; }
    }

	public class AvailableTestSessionContract
    {
        public int TestSessionId { get; set; }
        public string Name { get; set; }
        public DateTime TestDate { get; set; }

        public bool Selected { get; set; }
        public int TestSessionDuration { get; set; }
        public string TestLocation { get; set; }
        public string VenueName { get; set; }
        public string VenueAddress { get; set; }
        public int AvailableSeats { get; set; }
        public bool IsPreferedLocation { get; set; }
        public bool TestFeePaid { get; set; }
   
    }

    public class CredentialContract
    {
        public int Id { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? ExpiryDate { get; set; }

        public bool ShowInOnlineDirectory { get; set; }
        public string CredentialTypeInternalName { get; set; }
        public string CredentialTypeExternalName { get; set; }
        public string CredentialCategoryName { get; set; }
        public string SkillDisplayName { get; set; }

        public int SkillId { get; set; }
        public bool Certification { get; set; }
        public string  Status { get; set; }
    }

    public class CredentialAttachmentContract
    {
        public int CredentialId { get; set; }
        public int CredentialAttachmentId { get; set; }
        public int StoredFileId { get; set; }
        public string FileName { get; set; }
        public string FilePath { get; set; }
        public string DocumentType { get; set; }
        public string UploadedByName { get; set; }
        public DateTime UploadedDateTime { get; set; }
        public long FileSize { get; set; }
        public string ApplicationTypeDisplayName { get; set; }
    }

    public class CredentialRequestFieldContract : CredentialApplicationFieldContract
    {
    }

    public class CredentialApplicationFieldContract
    {
        public int Id { get; set; }
        public int FieldDataId { get; set; }
        public int FieldTypeId { get; set; }
        public string Name { get; set; }
        public int DataTypeId { get; set; }
        public string DefaultValue { get; set; }
        public string Value { get; set; }
        public bool PerCredentialRequest { get; set; }
        public string Description { get; set; }
        public bool Mandatory { get; set; }

        public int FieldOptionId { get; set; }

        public bool FieldEnable { get; set; }

        public IEnumerable<CredentialApplicationFieldOptionContract> Options { get; set; }
    }


    public class CredentialApplicationFieldOptionContract
    {
        public int FieldOptionId { get; set; }
        public string DisplayName { get; set; }
    }

    public class CredentialTypeContract
    {
        public int Id { get; set; }
        public string ExternalName { get; set; }
        public string InternalName { get; set; }
        public int DisplayOrder { get; set; }
        public bool Simultaneous { get; set; }
        public bool Certification { get; set; }
        public IEnumerable<CredentialApplicationTypeCredentialTypeContract> CredentialApplicationTypeCredentialTypes { get; set; }
    }

    public class CredentialApplicationTypeCredentialTypeContract
    {
        public int Id { get; set; }
        public int CredentialApplicationTypeId { get; set; }
        public int CredentialTypeId { get; set; }
        public bool HasTest { get; set; }
    }

    public class SkillContract
    {
        public int Id { get; set; }
        public int Language1Id { get; set; }
        public string DisplayName { get; set; }
        public string Language1Name { get; set; }
    }

    public enum QuestionLogicType
    {
        AnswerOption = 1,
        CredentialType = 2,
        CredentialRequestPathType = 3,
        PdPonints=4,
        WorkPractice =5,
        Skill = 6
    }

    public class FeePaymentDetails
    {
        public int PaymentMethodType { get; set; }
        public int CreditCardType { get; set; }
        public string CreditCardNumber { get; set; }
        public string CreditCardName { get; set; }
        public int CreditCardExpiryMonth { get; set; }
        public int CreditCardExpiryYear { get; set; }
        public string CreditCardCCV { get; set; }
        public string CreditCardExpiry { get; set; }
        public decimal Total { get; set; }
        public string CreditCardToken { get; set; }
        public PayPalModel PayPalModel { get;set;}
    }

    public class PayPalModel
    {
        public string NaatiReference { get; set; }
        public string NaatiUnit { get; set; }

        public string OrderId { get; set; }
        public string PayerId { get; set; }
        public string PaymentId { get; set; }
    }


    public class SupplementaryTestTaskResponse
    {
        public IEnumerable<SupplementaryTestTaskContract> Tasks { get; set; }
    }

    public class SupplementaryTestTaskContract
    {
        public string Name { get; set; }
    }
    public class TestFeeContract
    {
        public  int TestAttendancId { get; set; }
        public  int CredentialRequestId { get; set; }
        public int TestSessionId { get; set; }
        public DateTime TestDate  { get; set; }
        public string TestDateString => TestDate.ToString("dd/MM/yyyy h:mm tt");
        public string TestLocationName { get; set; }
        public string CredentialTypeDisplayName { get; set; }
        public string SkillDisplayName { get; set; }
        public string State { get; set; }
        public SponsorContract Sponsor { get; set; }
        public FeeContract Fee { get; set; }

        public string ApplicationReference { get; set; }
        public int ApplicationId { get; set; }
        public int NaatiNumber { get; set; }
        public string ApplicantPrimaryEmail { get; set; }
        public string ApplicantGivenName { get; set; }
        public string ApplicantFamilyName { get; set; }

    }

    public class SponsorContract
    {
        public string OrganisationName { get; set; }
        public string Contact { get; set; }
        public bool Trusted { get; set; }
    }

    public class FeeContract
    {
        public string Code  {get ; set;}
        public string Description { get ; set;}
        public double ExGST { get; set; }
        public double GST { get; set; }
        public double Total { get; set; }
        public double PayPalSurcharge { get; set; }
    }

    public enum PaymentMethodType
    {
        CreditCard = 1,
        DirectDeposit = 2,
        ChequeCash = 3,
        PayPal = 4
    }
}
