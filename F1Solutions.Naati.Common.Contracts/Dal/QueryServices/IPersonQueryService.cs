using System;
using System.Collections.Generic;
using F1Solutions.Naati.Common.Contracts.Bl.DTO;
using F1Solutions.Naati.Common.Contracts.Dal.DTO;
using F1Solutions.Naati.Common.Contracts.Dal.Request;
using F1Solutions.Naati.Common.Contracts.Dal.Response;
using F1Solutions.Naati.Common.Contracts.Dal.Services;

namespace F1Solutions.Naati.Common.Contracts.Dal.QueryServices
{
    
    public interface IPersonQueryService : IQueryService
    {
        
        SearchResponse Search(SearchRequest request);

        
        PersonSearchResponse SearchPerson(GetPersonSearchRequest request);

        IList<string> GetPersonPhotoFiles(GetPersonPhotoFileRequest request);


        PersonCredentialRequestsResponse GetPersonCredentialRequests(int personId);

        
        PersonCredentialsResponse GetPersonCredentials(int naatiNumber);

        /// <summary>
        /// Alternate call for GetPersonCredentials
        /// Gets credentials by person Id
        /// </summary>
        /// <param name="personId"></param>
        /// <returns></returns>
        PersonCredentialsResponse GetCredentialsByPersonId(int personId);



        GetPersonImageResponse GetPersonImage(GetPersonDetailsRequest request);

        
        GetPersonPhotoResponse GetPersonPhoto(GetPersonPhotoByNaatiNumber request);

        
        GetPersonDetailsResponse GetPersonDetails(GetPersonDetailsRequest request);

        
        GetPersonDetailsBasicResponse GetPersonDetailsBasic(GetPersonDetailsRequest request);

        
        void UpdatePersonDetails(UpdatePersonDetailsRequest request);

        
        void UpdatePersonSettings(UpdatePersonSettingsRequest request);

        
        GetContactDetailsResponse GetPersonContactDetails(GetContactDetailsRequest request);

        
        GetPersonAddressResponse GetPersonAddress(GetAddressRequest request);

        
        GetAddressesResponse GetPersonAddresses(GetAddressRequest request);

        
        GetPersonPhoneResponse GetPersonPhone(GetPhoneRequest request);

        
        GetPersonPhonesResponse GetPersonPhones(GetPhonesRequest request);

        
        GetPersonEmailResponse GetPersonEmail(GetEmailRequest request);

        
        GetEmailsResponse GetPersonEmails(GetEmailRequest request);

        
        GetPersonWebsiteResponse GetPersonWebsite(GetWebsiteRequest request);

        
        GetSuburbsResponse GetSuburbs();

        
        UpdateObjectResponse UpdatePersonAddress(UpdatePersonAddressRequest request);

        
        UpdateObjectResponse UpdatePersonPhone(UpdatePersonPhoneRequest request);

        
        UpdateObjectResponse UpdatePersonEmail(UpdatePersonEmailRequest request);

        
        void UpdatePersonWebsite(UpdatePersonWebsiteRequest request);

        
        void DeletePersonAddress(DeleteObjectRequest request);

        
        void UpdatePhoto(UpdatePhotoDto request);

        
        DeleteObjectResponse DeletePersonPhone(DeleteObjectRequest request);

        
        DeleteObjectResponse DeletePersonEmail(DeleteObjectRequest request);

        
        GetPersonNameResponse GetPersonName(GetPersonNameRequest request);

        
        AddNameResponse AddName(AddNameRequest request);

        
     

        
        GetCountryResponse GetCountry(int countryId);

        
        CreatePersonResponse CreatePerson(CreatePersonRequest request);

        
        CheckPersonResponse CheckPerson(CreatePersonRequest request);

        
        bool TestPractitionerNumberUniqueness(string practitionerNumber);

        
        ServiceResponse AssignPractitionerNumber(AssignPractitionerNumberRequest request);

        
        GetPersonMembershipRolesResponse GetPersonMembershipRolesByEntityId(int entityId);

        
        GetExminerRoleFlagResponse HasExminerRoleByEntityId(GetExminerRoleFlagRequest request);

        
        GetCertificationPeriodsResponse GetCertificationPeriods(GetCertificationPeriodsRequest request);

        
        DateTime? GetCertificationPeriodEarliestDate(int periodId);

        
        void SetCertificationEndDate(SetCertificationEndDateRequest request);

        
        void SetCredentialTerminateDate(SetCredentialTerminateDateRequest request);

        
        GetTokenResponse GetPersonToken(GetTokenRequest request);

        
        void UpdateMyNaatiDetails(MyNaatiAccountDetails request);
        
		void SaveCertificationPeriod(CertificationPeriodDto serviceRequest);

        
        void DeleteMyNaatiDetails(int naatiNumber);

        
        CreateOrReplacePersonAttachmentResponse CreateOrReplaceAttachment(CreateOrReplacePersonAttachmentRequest request);

        
        GetPersonAttachmentsResponse GetAttachments(GetPersonAttachmentsRequest request);

        
        DeletePersonAttachmentResponse DeleteAttachment(DeletePersonAttachmentRequest request);
        
        CertificationPeriodDto CreateCertificationPeriod(CreateCertificationPeriodRequest request);

      
        ApiPersonImageResponse GetApiPersonImage(GetPublicPersonPhotoRequest request);

        GenericResponse<PersonMfaResponse> GetPersonMfaDetails(int personId);

        GenericResponse<bool> GetAccessDisabledByNcms(int personId);


        BusinessServiceResponse SetPersonMfaDetails(PersonMfaRequest personMfaRequest);

        GenericResponse<List<QrCodeSummaryModelDto>> GetQrCodeSummary(int personId);

        GenericResponse<List<QrCodeSummaryModelDto>> GetQrCodes(int naatiNumber);

        /// <summary>
        /// Sets or removes the Inactive Flag
        /// </summary>
        /// <param name="qrCode"></param>
        /// <returns></returns>
        GenericResponse<bool> ToggleQrCode(Guid qrCode);
    }
}