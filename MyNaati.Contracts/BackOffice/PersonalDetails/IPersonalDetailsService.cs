using System;
using F1Solutions.Naati.Common.Contracts.Bl;
using F1Solutions.Naati.Common.Contracts.Bl.DTO;
using F1Solutions.Naati.Common.Contracts.Dal.Portal;
using MyNaati.Contracts.BackOffice.Common;

namespace MyNaati.Contracts.BackOffice.PersonalDetails
{
    
    public interface IPersonalDetailsService : IInterceptableservice
    {
        
        PersonalDetailsAddNewSuburbResponse AddNewPostcode(PersonalDetailsAddNewSuburbRequest requestSuburb);

        
        PersonalDetailsGetLastUpdatedResponse GetLastUpdated(PersonNaatiNumberRequest request);

        
        PersonalDetailsGetAddressesResponse GetAddresses(NaatiNumberRequest request);

        
        PersonalDetailsGetEmailsResponse GetEmails(PersonNaatiNumberRequest request);

        
        PersonalDetailsGetPhonesResponse GetPhones(PersonNaatiNumberRequest request);

        
        PersonalDetailsGetAddressResponse GetAddress(PersonalDetailsGetAddressRequest request);

        
        PersonalDetailsGetEmailResponse GetEmail(PersonalDetailsGetEmailRequest request);

        
        PersonalDetailsGetPhoneResponse GetPhone(PersonalDetailsGetPhoneRequest request);

        
        PersonalDetailsGetWebsiteResponse GetWebsite(PersonNaatiNumberRequest request);

        
        PersonalDetailsUpdateAddressResponse UpdateAddress(PersonalDetailsUpdateAddressRequest request);

        
        PersonalDetailsUpdateEmailResponse UpdateEmail(PersonalDetailsUpdateEmailRequest request);

        
        PersonalDetailsUpdatePhoneResponse UpdatePhone(PersonalDetailsUpdatePhoneRequest request);

        
        PersonalDetailsUpdateWebsiteResponse UpdateWebsite(PersonalDetailsUpdateWebsiteRequest request);

        
        PersonalDetailsGetPersonResponse GetPerson(PersonNaatiNumberRequest request);

        
        ParseAddressResponse ParseAddress(GeoResultModel request);

        
        PersonalDetailsGetPeopleResponse FindByNameAndBirthDate(string givenName, string familyName, DateTime birthDate);

        
        ValidatePrimaryEmailResponse ValidatePrimaryEmail(string email);

        
        PersonalDetailsGetVerifyOnline GetPersonByPractitionerNo(string practitionerNo);

        
        PersonalDetailsGetPersonResponse GetPersonByNaatiNo(NaatiNumberRequest naatiNo);

        
        bool IsValidAddress(int addressId, int naatiNumber);

        
        bool IsValidPhoneNumber(int phoneNumberId, int naatiNumber);

        /// <summary>
        /// Set address Online Directory Preference from MyPerson Details screen
        /// </summary>
        /// <param name="addressRowId"></param>
        /// <param name="odAddressVisibilityTypeId"></param>
        /// <returns></returns>
        GenericResponse<bool> SetOdAddressVisibilityTypeId(int addressRowId, int odAddressVisibilityTypeId);

        /// <summary>
        /// Set phone Online Directory Preference from MyPerson Details screen
        /// </summary>
        /// <param name="phoneRowId"></param>
        /// <param name="odAddressVisibilityTypeId"></param>
        /// <returns></returns>
        GenericResponse<bool> SetOdPhoneVisibility(int phoneRowId, bool odAddressVisibilityTypeId);

        /// <summary>
        /// Set email Online Directory Preference from MyPerson Details screen
        /// </summary>
        /// <param name="emailRowId"></param>
        /// <param name="odAddressVisibilityTypeId"></param>
        /// <returns></returns>
        GenericResponse<bool> SetOdEmailVisibility(int emailRowId, bool odAddressVisibilityTypeId);


    }
}