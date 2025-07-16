
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using AutoMapper;
using F1Solutions.Global.Common.Mapping;
using F1Solutions.Naati.Common.Contracts.Bl.AddressParser;
using F1Solutions.Naati.Common.Contracts.Bl.DTO;
using F1Solutions.Naati.Common.Contracts.Dal.Enum;
using F1Solutions.Naati.Common.Contracts.Dal.Portal;
using F1Solutions.Naati.Common.Contracts.Dal.Portal.CacheQuery;
using F1Solutions.Naati.Common.Dal.Domain;
using F1Solutions.Naati.Common.Dal.Portal.Repositories;
using MyNaati.Contracts.BackOffice.Common;
using MyNaati.Contracts.BackOffice.PersonalDetails;
using NHibernate.Util;
using Postcode = F1Solutions.Naati.Common.Dal.Domain.Postcode;

namespace MyNaati.Bl.BackOffice
{

    public class PersonalDetailsService : IPersonalDetailsService
    {
        private IAddressRepository mAddressRepository;
        private ICountryRepository mCountryRepository;
        private IEmailRepository mEmailRepository;
        private INaatiEntityRepository mNaatiEntityRepository;
        private IPersonRepository mPersonRepository;
        private IPhoneRepository mPhoneRepository;
        private IPostcodeRepository mPostcodeRepository;
        private IStateRepository mStateRepository;
        private readonly IAddressParserHelper mAddressParserHelper;
        private readonly IAutoMapperHelper _autoMapperHelper;
        private readonly IPostcodesCacheQueryService _postcodesCacheQueryService;

        private const string RESOURCE_NO_LONGER_AVAILABLE = "This {0} is no longer available.";
        private const string ADDRESS_COUNTRY_REQUIRED = "A country is required.";
        private const string DUPLICATE_ADDRESS = "This address has already been added.";
        private const string ADDRESS_PREFERRED_REQUIRED = "You must have one preferred address. Tip: First mark another address as preferred.";
        private const string EMAIL_ALREADY_EXISTS = " The email address {0} cannot be used.  Please get in touch with NAATI.";

        public PersonalDetailsService(
            IAddressRepository addressRepository, 
            ICountryRepository countryRepository,
            IEmailRepository emailRepository, 
            INaatiEntityRepository naatiEntityRepository, 
            IPersonRepository personRepository, 
            IPhoneRepository phoneRepository, 
            IPostcodeRepository postcodeRepository,
            IStateRepository stateRepository, 
            IAddressParserHelper addressParserHelper,
            IAutoMapperHelper autoMapperHelper,
            IPostcodesCacheQueryService postcodesCacheQueryService)
        {
            mAddressRepository = addressRepository;
            mCountryRepository = countryRepository;
            mEmailRepository = emailRepository;
            mNaatiEntityRepository = naatiEntityRepository;
            mPersonRepository = personRepository;
            mPhoneRepository = phoneRepository;
            mPostcodeRepository = postcodeRepository;
            mStateRepository = stateRepository;
            mAddressParserHelper = addressParserHelper;
            _autoMapperHelper = autoMapperHelper;
            _postcodesCacheQueryService = postcodesCacheQueryService;
        }

        public PersonalDetailsGetLastUpdatedResponse GetLastUpdated(PersonNaatiNumberRequest request)
        {
            var response = new PersonalDetailsGetLastUpdatedResponse
            {
                LastUpdated =
                    mPersonRepository.FindByNaatiNumber(request.NaatiNumber).PersonalDetailsLastUpdatedOnEportal
            };


            return response;
        }

        public PersonalDetailsGetAddressesResponse GetAddresses(NaatiNumberRequest request)
        {
            IList<Address> addresses = mAddressRepository.FindAddressesEligibleForPDListing(request.NaatiNumber);

            var response = new PersonalDetailsGetAddressesResponse();
            response.Addresses = _autoMapperHelper.Mapper.Map<IEnumerable<Address>, PersonalAddress[]>(addresses);

            return response;
        }

        public PersonalDetailsGetEmailsResponse GetEmails(PersonNaatiNumberRequest request)
        {
            IList<Email> emails = mEmailRepository.FindEmailsEligibleForPDListing(request.NaatiNumber);
            emails = emails.Where(em => !em.Invalid).ToList();

            var response = new PersonalDetailsGetEmailsResponse();
            response.Emails = emails.Select(MapEmail).ToArray();

            if (OnlyOneContactInPD(request.NaatiNumber))
            {
                foreach (PersonalEmail email in response.Emails)
                {
                    if (email.IsCurrentlyListed)
                    {
                        email.IsLastContactInPD = true;
                        break;
                    }
                }
            }

            return response;
        }

        public PersonalEmail MapEmail(Email email)
        {
            return new PersonalEmail
            {
                EmailId = email.Id,
                Email = email.EmailAddress,
                IsPreferred = email.IsPreferredEmail,
                IsCurrentlyListed = email.IncludeInPD,
                ExaminerCorrespondence = email.ExaminerCorrespondence
            };
        }

        public PersonalDetailsGetPhonesResponse GetPhones(PersonNaatiNumberRequest request)
        {
            IList<Phone> phones = mPhoneRepository.FindPhonesEligibleForPDListing(request.NaatiNumber);
            phones = phones.Where(ph => !ph.Invalid).ToList();

            var response = new PersonalDetailsGetPhonesResponse();
            response.Phones = _autoMapperHelper.Mapper.Map<IEnumerable<Phone>, PersonalPhone[]>(phones);

            if (OnlyOneContactInPD(request.NaatiNumber))
            {
                foreach (PersonalPhone phone in response.Phones)
                {
                    if (phone.IsCurrentlyListed)
                    {
                        phone.IsLastContactInPD = true;
                        break;
                    }
                }
            }

            return response;
        }

        public bool IsValidAddress(int addressId, int naatiNumber)
        {
            var person = mPersonRepository.FindByNaatiNumber(naatiNumber);
            var addresses = person.Addresses.ToList().Select(x => x.Id);
            
            if (addresses.Contains(addressId))
            {
                return true;
            }
            return false;
        }
        public PersonalDetailsGetAddressResponse GetAddress(PersonalDetailsGetAddressRequest request)
        {
            var response = new PersonalDetailsGetAddressResponse();
            Address address = mAddressRepository.Get(request.AddressId);

            if (address == null || address.Invalid)
            {
                response.ErrorMessage = string.Format(RESOURCE_NO_LONGER_AVAILABLE, "address");
                return response;
            }

            response.Address = _autoMapperHelper.Mapper.Map<Address, PersonalViewAddress>(address);

            return response;
        }

        public PersonalDetailsGetEmailResponse GetEmail(PersonalDetailsGetEmailRequest request)
        {
            var response = new PersonalDetailsGetEmailResponse();
            Email email = mEmailRepository.Get(request.EmailId);

            if (email == null)
            {
                response.ErrorMessage = string.Format(RESOURCE_NO_LONGER_AVAILABLE, "email address");
                return response;
            }

            response.Email = _autoMapperHelper.Mapper.Map<Email, PersonalViewEmail>(email);

            return response;
        }

        public bool IsValidPhoneNumber(int phoneNumberId, int naatiNumber)
        {
            var phoneNumbers = mPhoneRepository.GetPhoneNumbers(naatiNumber).Select(x=>x.Id).ToList();

            if (phoneNumbers.Contains(phoneNumberId))
            {
                return true;
            }
            return false;
        }

        public PersonalDetailsGetPhoneResponse GetPhone(PersonalDetailsGetPhoneRequest request)
        {
            var response = new PersonalDetailsGetPhoneResponse();
            Phone phone = mPhoneRepository.Get(request.PhoneId);

            if (phone == null)
            {
                response.ErrorMessage = string.Format(RESOURCE_NO_LONGER_AVAILABLE, "phone number");
                return response;
            }
            
            response.Phone = _autoMapperHelper.Mapper.Map<Phone, PersonalViewPhone>(phone);

            return response;
        }

        public PersonalDetailsGetVerifyOnline GetPersonByPractitionerNo(string practitionerNo)
        {
            var person = mPersonRepository.FindByPractitionerNo(practitionerNo);
            return new PersonalDetailsGetVerifyOnline { NaatiNumber = person?.Entity?.NaatiNumber, AllowVerifyOnline = person?.AllowVerifyOnline ?? false };
        }

        public PersonalDetailsGetPersonResponse GetPersonByNaatiNo(NaatiNumberRequest request)
        {
            var person = mPersonRepository.FindByNaatiNumber(request.NaatiNumber);
            
            var response = new PersonalDetailsGetPersonResponse();

            if (person != null)
            {
                response.Person = MapPerson(person);
            }

            return response;
        }

        public PersonalDetailsGetWebsiteResponse GetWebsite(PersonNaatiNumberRequest request)
        {
            Person person = mPersonRepository.FindByNaatiNumber(request.NaatiNumber);

            var response = new PersonalDetailsGetWebsiteResponse();
            response.Website = person.Entity.WebsiteUrl;
            response.LastUpdated = person.PersonalDetailsLastUpdatedOnEportal;
            response.IsCurrentlyListed = person.Entity.WebsiteInPD;

            return response;
        }

        public GenericResponse<bool> SetOdAddressVisibilityTypeId(int addressRowId, int odAddressVisibilityTypeId)
        {
            return mAddressRepository.SetOdAddressVisibilityTypeId(addressRowId, odAddressVisibilityTypeId);
        }

        public GenericResponse<bool> SetOdPhoneVisibility(int phoneRowId, bool includeInOd)
        {
            return mPhoneRepository.SetOdPhoneVisibility(phoneRowId, includeInOd);
        }

        public GenericResponse<bool> SetOdEmailVisibility(int emailRowId, bool includeInOd)
        {
            return mEmailRepository.SetOdEmailVisibility(emailRowId, includeInOd);
        }

        public virtual PersonalDetailsUpdateAddressResponse UpdateAddress(PersonalDetailsUpdateAddressRequest request)
        {
            using (var transaction = mAddressRepository.CreateSyncTransaction())
            {

                request.Address.OdAddressVisibilityTypeId = request.Address.OdAddressVisibilityTypeId != 0 ? request.Address.OdAddressVisibilityTypeId : (int)OdAddressVisibilityTypeName.DoNotShow;

                var response = new PersonalDetailsUpdateAddressResponse();

                bool doUpdate = (request.Delete || HasChangesMade(request));
                bool isExistingPrimaryContact = (request.Address.AddressId != 0 && mAddressRepository.IsPrimaryContact(request.Address.AddressId));

                if (isExistingPrimaryContact && (request.Delete || !request.Address.IsPreferred))
                {
                    response.ErrorMessage = ADDRESS_PREFERRED_REQUIRED;
                }

                else if (!request.Delete && !request.Address.IsFromAustralia && request.Address.CountryId == 0)
                {
                    response.ErrorMessage = ADDRESS_COUNTRY_REQUIRED;
                }
                else if (!request.Delete && mAddressRepository.IsDuplicateAddress(request.Address, request.NaatiNumber))
                {
                    response.ErrorMessage = DUPLICATE_ADDRESS;
                }
                else
                {
                    if (doUpdate)
                    {
                        if (request.Address.AddressId != 0)
                        {
                            mAddressRepository.MarkAsInvalid(request.Address.AddressId);
                        }

                        if (!request.Delete)
                        {
                            bool hasPostcode = request.Address.PostcodeId != 0 && (request.Address.CountryId == 0 || request.Address.IsFromAustralia);
                            bool hasCountry = request.Address.CountryId != 0 && !request.Address.IsFromAustralia;

                            if (request.Address.IsPreferred)
                            {
                                mAddressRepository.RemoveIsPreferred(request.NaatiNumber);
                            }

                            if (request.Address.OdAddressVisibilityTypeId != (int)OdAddressVisibilityTypeName.DoNotShow)
                            {
                                mAddressRepository.RemoveIncludeInPdFlag(request.NaatiNumber);
                            }


                            var address = new Address
                            {
                                Entity = mNaatiEntityRepository.FindByNaatiNumber(request.NaatiNumber),
                                StartDate = DateTime.Today,
                                Note = string.Empty,
                                StreetDetails = ConvertNewlinesToDatabase(request.Address.StreetDetails) ?? "",
                                Postcode = (hasPostcode ? mPostcodeRepository.Get(request.Address.PostcodeId) : null),
                                Country = hasCountry
                                    ? mCountryRepository.Get(request.Address.CountryId)
                                    : mCountryRepository.GetAustralia(),
                                PrimaryContact = request.Address.IsPreferred,
                                OdAddressVisibilityType = mAddressRepository.GetOdAddressVisibilityType(request.Address.OdAddressVisibilityTypeId),
                                ValidateInExternalTool = request.Address.ValidateInExternalTool
                            };

                            if (request.Address.ExaminerCorrespondence)
                            {
                                foreach(var tempAddress in address.Entity.Addresses)
                                {
                                    tempAddress.ExaminerCorrespondence = false;
                                }
                            }

                            address.ExaminerCorrespondence = request.Address.ExaminerCorrespondence;


                            mAddressRepository.SaveOrUpdate(address);
                        }
                    }
                }

                if (doUpdate && response.WasSuccessful)
                {
                    RefreshLastUpdated(request.NaatiNumber);
                }

                transaction.Commit();
                return response;
            }

        }

        private bool HasChangesMade(PersonalDetailsUpdateAddressRequest request)
        {
            Address existingAddress = (request.Address.AddressId != 0 ? mAddressRepository.Get(request.Address.AddressId) : null);

            if (existingAddress == null)
            {
                return true;
            }

            // record from database returns \r\n for line breaks while form returns \n.
            bool streetDetailsChanged = !existingAddress.StreetDetails.Replace("\r\n", Environment.NewLine)
                                             .Equals(request.Address.StreetDetails.Replace("\n", Environment.NewLine));

            bool postcodeChanged = request.Address.IsFromAustralia &&
                                   (existingAddress.Postcode == null || existingAddress.Postcode.Id != request.Address.PostcodeId);

            bool countryChanged = !request.Address.IsFromAustralia &&
                                  (existingAddress.Country == null || existingAddress.Country.Id != request.Address.CountryId);

            bool preferredChanged = existingAddress.PrimaryContact != request.Address.IsPreferred;

            bool odAddressVisibilityTypeChanged = existingAddress.OdAddressVisibilityType.Id != request.Address.OdAddressVisibilityTypeId;

            var examinerCorrespondenceChanged = existingAddress.ExaminerCorrespondence != request.Address.ExaminerCorrespondence;

            return streetDetailsChanged || postcodeChanged || countryChanged || preferredChanged || odAddressVisibilityTypeChanged || examinerCorrespondenceChanged;
        }

        private string ConvertNewlinesToDatabase(string webString)
        {
            return webString.Replace("\n", Environment.NewLine);
        }

        // [Transaction]
        public virtual PersonalDetailsUpdateEmailResponse UpdateEmail(PersonalDetailsUpdateEmailRequest request)
        {
            using (var transaction = mEmailRepository.CreateSyncTransaction())
            {
                try
                {
                    if (request.Delete)
                    {
                        return DeleteEmail(request);
                    }

                    var response = new PersonalDetailsUpdateEmailResponse();
                    if (request.Email.EmailId == 0)
                    {
                        return CreateNewEmail(request);
                    }

                    var email = mEmailRepository.Get(request.Email?.EmailId ?? 0);
                    if (email == null)
                    {
                        response.ErrorMessage = string.Format(RESOURCE_NO_LONGER_AVAILABLE, "Email address");
                        return response;
                    }

                    var existingEmails = mEmailRepository.ExistingEmails(request.Email.Email);
                    if (existingEmails.Any(x => x.Id != email.Id))
                    {
                        response.ErrorMessage = string.Format(EMAIL_ALREADY_EXISTS, request.Email?.Email);
                        return response;
                    }

                    if (email.IsPreferredEmail && !request.Email.IsPreferred)
                    {
                        response.ErrorMessage =
                            "You must have one preferred email address. Tip: First mark another email address as preferred.";
                        return response;
                    }

                    if (CreateEmailChangeRequest(email, request))
                    {
                        response.ChangePrimaryEmail = true;
                        return response;
                    }

                    if (request.Email.IsPreferred && !email.IsPreferredEmail)
                    {
                        mEmailRepository.RemoveIsPreferred(email.Entity.NaatiNumber);
                    }

                    if (request.Email.ExaminerCorrespondence)
                    {
                        foreach(var tempEmail in email.Entity.Emails)
                        {
                            tempEmail.ExaminerCorrespondence = false;
                        }
                    }
                    email.ExaminerCorrespondence = request.Email.ExaminerCorrespondence;
                    email.IsPreferredEmail = request.Email.IsPreferred;
                    email.IncludeInPD = request.Email.IsCurrentlyListed;
                    email.EmailAddress = request.Email.Email;
                    mEmailRepository.SaveOrUpdate(email);
                    RefreshLastUpdated(email.Entity.NaatiNumber);
                    transaction.Commit();
                    return response;
                }
                catch (Exception)
                {
                    transaction.Rollback();
                    throw;
                }
                finally
                {
                    if (transaction.IsActive)
                    {
                        transaction.Commit();
                    }
                }
               
            }
        }

        private bool CreateEmailChangeRequest(Email currentEmail, PersonalDetailsUpdateEmailRequest request)
        {
            if ((currentEmail.IsPreferredEmail || request.Email.IsPreferred)
                && !request.AllowChangePrimary
                && (request.Email.IsPreferred != currentEmail.IsPreferredEmail
                || string.Compare(request.Email.Email, currentEmail.EmailAddress, StringComparison.InvariantCultureIgnoreCase) != 0))
            {
                return true;
            }

            return false;

        }


        private PersonalDetailsUpdateEmailResponse CreateNewEmail(PersonalDetailsUpdateEmailRequest request)
        {
            var response = new PersonalDetailsUpdateEmailResponse();
            IList<Email> existingEmails = mEmailRepository.ExistingEmails(request.Email.Email);

            if (existingEmails.Any())
            {
                response.ErrorMessage = string.Format(EMAIL_ALREADY_EXISTS, request.Email?.Email);
                return response;
            }

            var email = new Email
            {
                Note = "",
                IsPreferredEmail = false,
                Entity = mNaatiEntityRepository.FindByNaatiNumber(request.NaatiNumber),
                EmailAddress = request.Email.Email,
                IncludeInPD = request.Email.IsCurrentlyListed
            };

            mEmailRepository.SaveOrUpdate(email);

            if (request.Email.IsPreferred)
            {
                response.ChangePrimaryEmail = true;
            }
            RefreshLastUpdated(email.Entity.NaatiNumber);
            return response;
        }


        private PersonalDetailsUpdateEmailResponse DeleteEmail(PersonalDetailsUpdateEmailRequest request)
        {
            var response = new PersonalDetailsUpdateEmailResponse();

            if (mEmailRepository.FindEmailsEligibleForPDListing(request.NaatiNumber).Count == 1)
            {
                response.ErrorMessage = "You must have at least one email address.";
                return response;
            }

            var email = mEmailRepository.Get(request.Email?.EmailId ?? 0);
            if (email == null)
            {
                response.ErrorMessage = string.Format(RESOURCE_NO_LONGER_AVAILABLE, "Email address");
                return response;
            }

            email.Invalid = true;
            mEmailRepository.SaveOrUpdate(email);

            return response;
        }


        public PersonalDetailsUpdatePhoneResponse UpdatePhone(PersonalDetailsUpdatePhoneRequest request)
        {
            var response = new PersonalDetailsUpdatePhoneResponse();
            Phone phone;

            using (var transaction = mPhoneRepository.CreateSyncTransaction())
            {

                if (request.Phone.PhoneId != 0)
                {
                    phone = mPhoneRepository.Get(request.Phone.PhoneId);

                    if (phone == null)
                    {
                        response.ErrorMessage = string.Format(RESOURCE_NO_LONGER_AVAILABLE, "Phone number");
                        return response;
                    }
                }
                else
                {
                    phone = new Phone
                    {
                        Entity = mNaatiEntityRepository.FindByNaatiNumber(request.NaatiNumber),
                        Note = ""
                    };
                }

                if (phone.PrimaryContact && (request.Delete || !request.Phone.IsPreferred))
                {
                    response.ErrorMessage =
                        "You must have one preferred phone. Tip: First mark another phone preferred.";
                }

                else if (request.Delete)
                {
                    try
                    {
                        mPhoneRepository.Delete(phone);
                    }
                    catch (SqlException)
                    {
                        response.ErrorMessage = "Could not delete the phone number.";
                    }
                }
                else
                {
                    phone.AreaCode = string.Empty;
                    phone.CountryCode = string.Empty;
                    phone.LocalNumber = request.Phone.PhoneNumber ?? string.Empty;

                    if (request.Phone.IsPreferred && phone.PrimaryContact != request.Phone.IsPreferred)
                    {
                        mPhoneRepository.RemoveIsPreferred(phone.Entity.NaatiNumber);
                    }

                    phone.PrimaryContact = request.Phone.IsPreferred;
                    phone.IncludeInPD = request.Phone.IsCurrentlyListed;
                    if (request.Phone.ExaminerCorrespondence)
                    {
                        foreach(var tempPhone in phone.Entity.Phones)
                        {
                            tempPhone.ExaminerCorrespondence = false;
                        }
                    }

                    phone.ExaminerCorrespondence = request.Phone.ExaminerCorrespondence;

                    mPhoneRepository.SaveOrUpdate(phone);


                    if (response.WasSuccessful)
                    {
                        RefreshLastUpdated(phone.Entity.NaatiNumber);
                    }

                }
                transaction.Commit();
            }

            return response;
        }
        
        public virtual PersonalDetailsUpdateWebsiteResponse UpdateWebsite(PersonalDetailsUpdateWebsiteRequest request)
        {
            using (var transaction = mNaatiEntityRepository.CreateSyncTransaction())
            {
                NaatiEntity entity = mNaatiEntityRepository.FindByNaatiNumber(request.NaatiNumber);

                entity.WebsiteUrl = request.WebsiteUrl ?? "";

                if (entity.WebsiteUrl == "")
                    entity.WebsiteInPD = false;

                mNaatiEntityRepository.SaveOrUpdate(entity);

                var response = new PersonalDetailsUpdateWebsiteResponse();

                RefreshLastUpdated(entity.NaatiNumber);
                transaction.Commit();
                return response;
            }

        }

        private void RefreshLastUpdated(int naatiNumber)
        {
            if (naatiNumber <= 0)
            {
                return;
            }

            Person person = mPersonRepository.FindByNaatiNumber(naatiNumber);
            person.PersonalDetailsLastUpdatedOnEportal = DateTime.Now;
        }

        public PersonalDetailsGetPersonResponse GetPerson(PersonNaatiNumberRequest request)
        {
            var person = mPersonRepository.FindByNaatiNumber(request.NaatiNumber);

            var response = new PersonalDetailsGetPersonResponse();

            if (person != null)
            {
                response.Person = MapPerson(person);
            }

            return response;
        }

        public ParseAddressResponse ParseAddress(GeoResultModel request)
        {
            var address = _autoMapperHelper.Mapper.Map<GeoResultDto>(request);
            var result = mAddressParserHelper.ParseAddress(address);
            var response = _autoMapperHelper.Mapper.Map<ParseAddressResponse>(result);
            return response;
        }


        private PersonalEditPerson MapPerson(Person person)
        {
            return new PersonalEditPerson
            {
                EntityTypeId = person.Entity.EntityTypeId,
                Photo = person.Photo,
                PractitionerNumber = person.PractitionerNumber,
                NaatiNumber = person.Entity.NaatiNumber,
                FullName = person.FullName,
                GivenName = person.GivenName,
                Surname = person.Surname.Equals(PersonName.SurnameNotStated, StringComparison.CurrentCultureIgnoreCase) ? String.Empty : person.Surname,
                IsEportalActive = person.IsEportalActive ?? false,
                Deceased = person.Deceased,
                Title = person.Title,
                AllowVerifyOnline = person.AllowVerifyOnline,
                ShowPhotoOnline = person.ShowPhotoOnline,
                IsPractitioner = person.IsPractitioner,
                IsFormerPractitioner = person.IsFormerPractitioner,
                IsFuturePractitioner = person.IsFuturePractitioner,
                IsApplicant = person.IsApplicant,
                IsExaminer = person.IsExaminer,
                Country = person.PrimaryAddress?.Country.Name,
                Email = person.PrimaryEmailAddress,                
            };
        }

        private bool OnlyOneContactInPD(int naatiNumber)
        {
            IList<Email> emails = mEmailRepository.FindEmailsEligibleForPDListing(naatiNumber);
            IList<Phone> phones = mPhoneRepository.FindPhonesEligibleForPDListing(naatiNumber);
            int numberInPD = 0;

            foreach (Phone phone in phones)
            {
                if (phone.IncludeInPD)
                    numberInPD++;
            }

            foreach (Email email in emails)
            {
                if (email.IncludeInPD)
                    numberInPD++;
            }

            return numberInPD == 1;
        }

        public PersonalDetailsGetPeopleResponse FindByNameAndBirthDate(string givenName, string familyName, DateTime birthDate)
        {
            var people = mPersonRepository.FindByNameAndBirthDate(givenName, familyName, birthDate);
            var peopleResponse = new PersonalEditPerson[people.Count()];
            int i = 0;
            foreach (Person p in people)
            {
                string NaatiNumberDisplay;
                NaatiNumberDisplay = p.NaatiNumberDisplay.Contains("NC") ? p.NaatiNumberDisplay.Replace("NC", "") : p.NaatiNumberDisplay;

                peopleResponse[i] = new PersonalEditPerson()
                {
                    NaatiNumber = int.Parse(NaatiNumberDisplay),
                    GivenName = p.GivenName,
                    Surname = p.Surname,
                    IsEportalActive = p.IsEportalActive == true,
                    BirthDate = (DateTime)p.BirthDate,
                    Email = p.PrimaryEmailAddress
                };
                i++;
            }
            return new PersonalDetailsGetPeopleResponse { People = peopleResponse };
        }

        public ValidatePrimaryEmailResponse ValidatePrimaryEmail(string email)
        {
            var response = new ValidatePrimaryEmailResponse();
            var existingEmails = mEmailRepository.ExistingEmails(email);

            var repeatedPrimaryEmails = existingEmails.Count(e => e.IsPreferredEmail);
            if (repeatedPrimaryEmails == 0)
            {
                response.ErrorMessage = "Email Not found";
                return response;
            }

            if (repeatedPrimaryEmails > 1)
            {
                response.ErrorMessage = $"More than one person found with the email {email}";
                return response;
            }

            response.NaatiNumber = existingEmails.First(e => e.IsPreferredEmail).Entity.NaatiNumber;
            return response;

        }

        public PersonalDetailsAddNewSuburbResponse AddNewPostcode(PersonalDetailsAddNewSuburbRequest requestSuburb)
        {
            try
            {
                // Need to check whether Suburb exists in the State
                // If so, link to new Post Code
                // Otherwise create and then link to Post Code

                var state = mStateRepository.GetState(requestSuburb.State);

                if (state == null)
                {
                    return new PersonalDetailsAddNewSuburbResponse
                    {
                        Success = false,
                        PostcodeId = 0
                    };
                }

                var databaseSuburb = mStateRepository.GetSuburb(state, requestSuburb.SuburbName);

                if (databaseSuburb == null)
                {
                    var newSuburb = new Suburb();
                    var newPostcode = new Postcode();

                    mAddressRepository.Session.Transaction.Begin();

                    newSuburb.Name = requestSuburb.SuburbName;
                    newSuburb.State = state;

                    newPostcode.PostCode = requestSuburb.Postcode;
                    newPostcode.Suburb = newSuburb;

                    mAddressRepository.Session.Save(newSuburb);
                    mAddressRepository.Session.Save(newPostcode);

                    mAddressRepository.Session.Transaction.Commit();
                    mAddressRepository.Session.Flush();
                    _postcodesCacheQueryService.AddOrRefreshItem(newPostcode.Id);

                    return new PersonalDetailsAddNewSuburbResponse
                    {
                        Success = true,
                        PostcodeId = newPostcode.Id
                    };
                }
                else
                {
                    var newPostcode = new Postcode();

                    mAddressRepository.Session.Transaction.Begin();

                    databaseSuburb.Name = requestSuburb.SuburbName;
                    databaseSuburb.State = state;

                    newPostcode.PostCode = requestSuburb.Postcode;
                    newPostcode.Suburb = databaseSuburb;

                    mAddressRepository.Session.Save(newPostcode);

                    mAddressRepository.Session.Transaction.Commit();
                    mAddressRepository.Session.Flush();
                    _postcodesCacheQueryService.AddOrRefreshItem(newPostcode.Id);

                    return new PersonalDetailsAddNewSuburbResponse
                    {
                        Success = true,
                        PostcodeId = newPostcode.Id
                    };
                }
            }
            catch
            {
                return new PersonalDetailsAddNewSuburbResponse
                {
                    Success = false,
                    PostcodeId = 0
                };
            }
        }
    }
}