using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using F1Solutions.Naati.Common.Contracts.Bl.DTO;
using F1Solutions.Naati.Common.Contracts.Dal.DTO;
using F1Solutions.Naati.Common.Contracts.Dal.Response;
using Ncms.Contracts.Models;
using Ncms.Contracts.Models.Application;
using Ncms.Contracts.Models.Logbook;
using Ncms.Contracts.Models.Person;

namespace Ncms.Contracts
{
    public interface IPersonService
    {
        GenericResponse<IEnumerable<EntitySearchResultModel>> PersonSearch(QueryRequest request);
        GenericResponse<IEnumerable<PersonResultModel>> Search(PersonSearchRequest request);
        GenericResponse<PersonSummaryModel> GetPersonSummary(int naatiNumber);
        GenericResponse<MyNaatiUserDetailsModel> GetMyNaatiDetails(string username);
        GenericResponse<bool> UnlockMyNaatiUser(string username);
        GenericResponse<bool> DeleteMyNaatiUser(string username);
        GenericResponse<bool> DeleteMfaAccount(int naatiNumber);
        GenericResponse<bool> SoftDeletePerson(Ncms.Contracts.Models.Person.DeletePersonRequestModel request);


        GenericResponse<IEnumerable<PersonCredentialRequestModel>> GetPersonCredentialRequests(int personId);
        GenericResponse<IEnumerable<EntitySearchResultModel>> NaatiEntitySearch(EntitySearchRequest request);
        GenericResponse<IEnumerable<EntitySearchResultModel>> InstitutionSearch(QueryRequest request);
        GenericResponse<IEnumerable<EntitySearchResultModel>> PersonAndAndInstitutionSearch(QueryRequest query);
        GenericResponse<PersonModel> GetPerson(int personId);
        GenericResponse<PersonBasicModel> GetPersonBasic(int naatiNumber);
        GenericResponse<EntitySearchResultModel> GetEntity(int naatiNumber);
        void UpdateDetails(PersonModel model);
        void UpdateSettings(PersonModel model);
        void UpdateAddress(AddressModel model);
        void UpdatePhone(PhoneModel model);
        BusinessServiceResponse UpdateEmail(EmailModel model);
        void UpdateWebsite(WebsiteModel model);
        void DeleteAddress(int addressId);
        DeleteResponseModel DeletePhone(DeleteRequestModel request);
        DeleteResponseModel DeleteEmail(DeleteRequestModel request);
        void DeleteWebsite(int entityId);
        ContactDetailsModel GetContactDetails(int entityId);
        AddressModel GetAddress(int entityId, int addressId);
        PhoneModel GetPhone(int entityId, int phoneId);
        EmailModel GetEmail(int entityId, int emailId);
        WebsiteModel GetWebsite(int entityId);
        IEnumerable<SuburbModel> GetSuburbs();
        void AddName(int naatiNumber, PersonNameModel name);
        GetPersonDetailsBasicResponse GetPersonDetailsByEntityId(int entityId);
        GenericResponse<int> CreatePerson(CreatePersonModel request);
        GenericResponse<IEnumerable<PersonCheckModel>> CheckPerson(PersonCheckModel request);
        void AssignPractitionerNumber(int personId, string practitionerNumber);
        PhoneModel GetPersonPrimaryPhone(int entityId);
        GenericResponse<IEnumerable<CertificationPeriodModel>> GetCertificationPeriods(GetCertificationPeriodsRequestModel request);
        void SetCertificationEndDate(SetCertificationEndDateRequestModel request);
        void SetCredentialTerminateDate(SetCredentialTerminateDateModel request);
        GenericResponse<bool> CheckExaminerRole(int entityId);
        GetPersonAndContactDetailsResponse GetPersonInfoResponse(int naatiNumber);
        GenericResponse<PersonModel> GetLogBook(int naatiNumber, bool showAllCredentials = true);
		GenericResponse<IEnumerable<ActivityModel>> GetProfessionalDevelopmentActivities(int naatiNumber, int certificationPeriodId);
		GenericResponse<PdActivityPoints> GetProfessionalDevelopmentActivityPoints(int naatiNumber, int certificationPeriodId);
		GenericResponse<IEnumerable<CertificationPeriodCredentialRequests>> GetCertificationPeriodsRequests(int naatiNumber);
		GenericResponse<IEnumerable<CertificationPeriodDetailsDto>> GetCertificationPeriodDetails(int naatiNumber);
		GenericResponse<IEnumerable<ProfessionalDevelopmentRequirementResponse>> GetProfessionalDevelopmentRequirements(int categoryId);
		GenericResponse<IEnumerable<ProfessionalDevelopmentCategoryResponse>> GetProfessionalDevelopmentCategories();
        GenericResponse<IList<CredentialModel>> GetPersonCredentials(int naatiNumber, bool checkSkillAvailability = true);
		GenericResponse<CheckCertificationPeriodResult> CheckCertificationPeriod(CertificationPeriodModel request);
        GenericResponse<DateTime?> RecalculateCertificationPeriodStartDate(int periodId);

        void SaveCertificationPeriod(CertificationPeriodModel request);

        int CreateOrReplaceAttachment(PersonAttachmentModel request);

        IEnumerable<PersonAttachmentModel> ListAttachments(int personId);

        GenericResponse<IEnumerable<string>> GetDocumentTypesToUpload();

        void DeleteAttachment(int id);

        GenericResponse<List<QrCodeSummaryModel>> GetQrCodeSummary(int personId);

        /// <summary>
        /// Used for Person Screen QRCodes tabs
        /// </summary>
        /// <param name="naatiNumber"></param>
        /// <returns></returns>
        GenericResponse<List<QrCodeSummaryModel>> GetQrCodes(int naatiNumber);

        /// <summary>
        /// Sets the QrCodes inactive date or clears it
        /// </summary>
        /// <param name="qrCode"></param>
        /// <returns></returns>
        GenericResponse<bool> ToggleQrCode(Guid qrCode);

        CertificationPeriodModel CreateCertificationPeriod(CreateCertificationPeriodModel request);
    }


    public class MyNaatiUserModel
    {
        public string UserName { get; set; }
        public int NaatiNumber { get; set; }
    }
    public class CertificationPeriodCredentialRequests
    {
        [DataMember]
        public int CertificationPeriodId { get; set; }
        [DataMember]
        public IEnumerable<CertificationPeriodCredentialRequest> Requests { get; set; }
    }

    public class CertificationPeriodCredentialRequest
    {
        [DataMember]
        public string Skill { get; set; }

        [DataMember]
        public string ExternalName { get; set; }
    }

    public class PersonSummaryModel
    {
        public int PersonId { get; set; }
        public int NaatiNumber { get; set; }
        public string PersonPractitionerNumber { get; set; }
        public string GivenName { get; set; }
        public string SurName { get; set; }

        public DateTime? BirthDate { get; set; }
        public bool HasPhoto { get; set; }
        public DateTime? EportalRegistrationDate { get; set; } 
        public bool IsEportalActive { get; set; }
    }
}
