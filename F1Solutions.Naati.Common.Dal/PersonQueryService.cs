using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Policy;
using AutoMapper;
using F1Solutions.Global.Common.Logging;
using F1Solutions.Global.Common.Mapping;
using F1Solutions.Naati.Common.Contracts.Bl;
using F1Solutions.Naati.Common.Contracts.Bl.DTO;
using F1Solutions.Naati.Common.Contracts.Bl.ExtensionHelpers;
using F1Solutions.Naati.Common.Contracts.Dal;
using F1Solutions.Naati.Common.Contracts.Dal.DTO;
using F1Solutions.Naati.Common.Contracts.Dal.Enum;
using F1Solutions.Naati.Common.Contracts.Dal.Portal.CacheQuery;
using F1Solutions.Naati.Common.Contracts.Dal.QueryServices;
using F1Solutions.Naati.Common.Contracts.Dal.Request;
using F1Solutions.Naati.Common.Contracts.Dal.Response;
using F1Solutions.Naati.Common.Contracts.Security;
using F1Solutions.Naati.Common.Dal.Domain;
using F1Solutions.Naati.Common.Dal.Domain.Extensions;
using F1Solutions.Naati.Common.Dal.NHibernate.Configuration;
using F1Solutions.Naati.Common.Dal.NHibernate.DataAccess;
using F1Solutions.Naati.Common.Dal.Portal.Repositories;
using F1Solutions.Naati.Common.Dal.QueryHelper;
using NHibernate.Linq;
using NHibernate.Util;

namespace F1Solutions.Naati.Common.Dal
{

    public class PersonQueryService : IPersonQueryService
    {
        private IFileCompressionHelper _fileCompressionHelper;
        private ISharedAccessSignature _sharedAccessSignature;
        private readonly IAutoMapperHelper _autoMapperHelper;
        private IStateRepository _stateRepository;
        private IAddressRepository _addressRepository;
        private IPostcodesCacheQueryService _postcodesCacheQueryService;

        public PersonQueryService(IFileCompressionHelper fileCompressionHelper, ISharedAccessSignature sharedAccessSignature, IAutoMapperHelper autoMapperHelper, IStateRepository stateRepository, IAddressRepository addressRepository, IPostcodesCacheQueryService postcodesCacheQueryService)
        {
            _fileCompressionHelper = fileCompressionHelper;
            _sharedAccessSignature = sharedAccessSignature;
            _autoMapperHelper = autoMapperHelper;
            _stateRepository = stateRepository;
            _addressRepository = addressRepository;
            _postcodesCacheQueryService = postcodesCacheQueryService;
        }

        public SearchResponse Search(SearchRequest request)
        {
            var term = (request.Term ?? String.Empty).Replace(" ", "%");

            var termNumber = 0;
            var people = Enumerable.Empty<Person>();
            var institutions = Enumerable.Empty<Institution>();

            var searchPerson = request.Type == EntitySearchType.Person || request.Type == EntitySearchType.None;
            var searchInstitution = request.Type == EntitySearchType.Institution || request.Type == EntitySearchType.None;

            if (int.TryParse(term, out termNumber))
            {
                people = searchPerson ? SearchPeople(null, termNumber) : Enumerable.Empty<Person>();
                institutions = searchInstitution ? SearchInstitutions(null, termNumber) : Enumerable.Empty<Institution>();
            }
            else if (!String.IsNullOrWhiteSpace(term))
            {
                people = searchPerson ? SearchPeople(term, null) : Enumerable.Empty<Person>();
                institutions = searchInstitution ? SearchInstitutions(term, null) : Enumerable.Empty<Institution>();
            }

            return new SearchResponse
            {
                Results = people
                    .Select(x => new NaatiEntityDto
                    {
                        EntityId = x.Entity.Id,
                        NaatiNumber = x.Entity.NaatiNumber,
                        Name = x.FullName,
                        PersonId = x.Id,
                        PrimaryEmail = x.PrimaryEmailAddress
                    })
                    .Union(
                        institutions
                            .Select(x => new NaatiEntityDto
                            {
                                EntityId = x.Entity.Id,
                                NaatiNumber = x.Entity.NaatiNumber,
                                Name = x.CurrentName.Name,
                                InstitutionId = x.Id,
                                PrimaryEmail = x.Entity.PrimaryEmail?.EmailAddress
                            })
                    )
            };
        }

        public PersonSearchResponse SearchPerson(GetPersonSearchRequest request)
        {
            var personQueryHelper = new PersonQueryHelper();
            return new PersonSearchResponse { Results = personQueryHelper.SearchPeople(request) };
        }
        
        public IList<string> GetPersonPhotoFiles(GetPersonPhotoFileRequest request)
        {
            var folderName = Guid.NewGuid().ToString();
            var folderPath = Path.Combine(request.FolderPath, folderName);
            Directory.CreateDirectory(folderPath);
            var count = 0;
            var hasResults = false;

            var filePaths = new List<string>();

            var personNaatiNumbers = request.PersonNaatiNumbers.Concat(new[] { 0 }).ToList();
            do
            {
                var naatiNumbers = personNaatiNumbers.Skip(count * 10).Take(10).ToList();
                var personPhotos = NHibernateSession.Current.Query<PersonImage>().Where(x => x.Id > 0 && naatiNumbers.Contains(x.Person.Entity.NaatiNumber)).Select(y => new
                {
                    y.Person.Entity.NaatiNumber,
                    y.Photo
                }).ToList();

                hasResults = naatiNumbers.Any();
                foreach (var personPhoto in personPhotos)
                {
                    var photoFilePath = Path.Combine(folderPath, personPhoto.NaatiNumber + ".png");
                    if (filePaths.Contains(photoFilePath))
                    {
                        continue;
                    }

                    using (var personPhotoFile = new FileStream(photoFilePath, FileMode.CreateNew))
                    {
                        personPhotoFile.Write(personPhoto.Photo, 0, personPhoto.Photo.Length);
                    }

                    filePaths.Add(photoFilePath);
                }
                count++;
            } while (hasResults);

            return filePaths;

        }


        public PersonCredentialRequestsResponse GetPersonCredentialRequests(int personId)
        {
            var personQueryHelper = new PersonQueryHelper();
            return new PersonCredentialRequestsResponse { Results = personQueryHelper.GetPersonCredentialRequests(personId) };
        }

        public PersonCredentialsResponse GetCredentialsByPersonId(int personId)
        {
            var p = NHibernateSession.Current.Query<Person>()
                .FirstOrDefault(x => x.Id == personId);
            return GetPersonCredentials(p.Entity.NaatiNumber);
        }


        /// <summary>
        /// Gets basic details of credentials for a given person.
        /// </summary>
        public PersonCredentialsResponse GetPersonCredentials(int naatiNumber)
        {
            var p = NHibernateSession.Current.Query<Person>()
                .FirstOrDefault(x => x.Entity.NaatiNumber == naatiNumber);

            if (p == null)
            {
                throw new ArgumentException($"Person with NAATI Number {naatiNumber} not found");
            }
            var queryHelper = new ApplicationQueryHelper();

            var credentials = p.CredentialApplications
                .SelectMany(x => x.CredentialRequests
                    .Where(y => y.CredentialRequestPathType.Id !=
                                (int)CredentialRequestPathTypeName.Recertify && y.CredentialRequestStatusType.Id == (int)CredentialRequestStatusTypeName.CertificationIssued)) // I know, yuck, bizlogic here!
                .SelectMany(x => x.CredentialCredentialRequests)
                .Select(x =>
                {
                    var status = queryHelper.GetCredentialStatuses(new List<int> { x.CredentialRequest.Id }).Values.First();
                    var c = new CredentialDto
                    {
                        Id = x.Credential.Id,
                        CategoryId = x.CredentialRequest.CredentialType.CredentialCategory.Id,
                        CertificationPeriod = _autoMapperHelper.Mapper.Map<CertificationPeriodDto>(x.Credential.CertificationPeriod),
                        CredentialTypeId = x.CredentialRequest.CredentialType.Id,
                        Certification = x.CredentialRequest.CredentialType.Certification,
                        CredentialTypeInternalName = x.CredentialRequest.CredentialType.InternalName,
                        CredentialTypeExternalName = x.CredentialRequest.CredentialType.ExternalName,
                        CredentialCategoryName = x.CredentialRequest.CredentialType.CredentialCategory.Name,
                        ExpiryDate = x.Credential.ExpiryDate,
                        ShowInOnlineDirectory = x.Credential.ShowInOnlineDirectory,
                        SkillDisplayName = x.CredentialRequest.Skill.DisplayName,
                        SkillId = x.CredentialRequest.Skill.Id,
                        StartDate = x.Credential.StartDate,
                        TerminationDate = x.Credential.TerminationDate,
                        StatusId = (int)status,
                        Status = status.ToString(),
                        StoredFileIds = x.Credential.CredentialAttachments.Select(y => y.StoredFile.Id).ToList()
                    };
                    return c;
                })
                .ToList();

            return new PersonCredentialsResponse { Data = credentials };
        }

        public SearchResponse SearchPerson(SearchRequest request)
        {
            var term = (request.Term ?? String.Empty).Replace(" ", "%");

            var termNumber = 0;
            var people = Enumerable.Empty<Person>();
            var institutions = Enumerable.Empty<Institution>();

            if (int.TryParse(term, out termNumber))
            {
                people = SearchPeople(null, termNumber);
                institutions = SearchInstitutions(null, termNumber);
            }
            else if (!String.IsNullOrWhiteSpace(term))
            {
                people = SearchPeople(term, null);
                institutions = SearchInstitutions(term, null);
            }

            return new SearchResponse
            {
                Results = people
                    .Select(x => new NaatiEntityDto
                    {
                        EntityId = x.Entity.Id,
                        NaatiNumber = x.Entity.NaatiNumber,
                        Name = x.FullName,
                        PersonId = x.Id
                    })
                    .Union(
                        institutions
                            .Select(x => new NaatiEntityDto
                            {
                                EntityId = x.Entity.Id,
                                NaatiNumber = x.Entity.NaatiNumber,
                                Name = x.CurrentName.Name,
                                InstitutionId = x.Id
                            })
                    )
            };
        }

        private IEnumerable<Institution> SearchInstitutions(string term, int? termNumber)
        {
            var hasTerm = !String.IsNullOrWhiteSpace(term);
            var hasTermNumber = termNumber.GetValueOrDefault() != default(int);

            if (!hasTerm && !hasTermNumber)
            {
                return Enumerable.Empty<Institution>();
            }

            var query = NHibernateSession.Current.Query<Institution>();

            if (hasTerm)
            {
                query = query.Where(x => x.InstitutionName.StartsWith(term));
            }

            if (hasTermNumber)
            {
                query = query.Where(x => x.Entity.NaatiNumber == termNumber.Value);
            }

            return query.ToArray();
        }

        private IEnumerable<Person> SearchPeople(string term, int? termNumber)
        {
            var hasTerm = !String.IsNullOrWhiteSpace(term);
            var hasTermNumber = termNumber.GetValueOrDefault() != default(int);

            if (!hasTerm && !hasTermNumber)
            {
                return Enumerable.Empty<Person>();
            }

            var query = NHibernateSession.Current.Query<Person>();

            if (hasTerm)
            {
                query = query.Where(x =>
                    (x.Surname + x.GivenName + x.OtherNames).StartsWith(term) ||
                    (x.GivenName + x.Surname).StartsWith(term) ||
                    (x.GivenName + x.OtherNames + x.Surname).StartsWith(term)
                );
            }

            if (hasTermNumber)
            {
                query = query.Where(x => x.Entity.NaatiNumber == termNumber.Value);
            }

            return query.ToArray();
        }

        public GetPersonImageResponse GetPersonImage(GetPersonDetailsRequest request)
        {
            var query = NHibernateSession.Current.Query<Person>();
            var person = query.SingleOrDefault(x => x.Entity.NaatiNumber == request.NaatiNumber);

            if (person == null)
            {
                throw new WebServiceException("Referenced person does not exist");
            }

            return new GetPersonImageResponse
            {
                PersonImageData = person.HasPhoto
                    ? person.Photo
                    : null
            };
        }

    
        public ApiPersonImageResponse GetApiPersonImage(GetPublicPersonPhotoRequest request)
        {
            var response = new ApiPersonImageResponse();

            var query = NHibernateSession.Current.Query<Person>();
            var personData = new Person();

            switch (request.PropertyType)
            {
                case ApiPublicPhotoRequestPropertyType.PersonId:
                    personData = query.SingleOrDefault(x => x.Id == int.Parse(request.Value));
                    break;
                case ApiPublicPhotoRequestPropertyType.NaatiNumber:
                    personData = query.SingleOrDefault(x => x.Entity.NaatiNumber == int.Parse(request.Value));
                    break;
                case ApiPublicPhotoRequestPropertyType.PractitionerId:
                    personData = query.SingleOrDefault(x => string.Equals(x.PractitionerNumber, request.Value));
                    break;
            }

            if (personData == null)
            {
                response.IsPersonExist = false;
                return response;
            }

            response.IsDeceased = personData.Deceased;
            response.IsPersonExist = true;
            response.PersonImageData = personData.HasPhoto ? personData.Photo : new byte[0];
            response.ShowPhotoOnline = personData.ShowPhotoOnline;

            return response;
        }

        public GetPersonPhotoResponse GetPersonPhoto(GetPersonPhotoByNaatiNumber request)
        {
            var query = NHibernateSession.Current.Query<Person>();
            Person person = null;
            if (request.NaatiNumber.HasValue)
            {
                person = query.SingleOrDefault(x => x.Entity.NaatiNumber == request.NaatiNumber);
            }
            if (request.PractitionerNumber.IsNotNullOrEmpty())
            {
                person = query.SingleOrDefault(x => x.PractitionerNumber == request.PractitionerNumber);
            }

            if (person == null)
            {
                throw new WebServiceException("Referenced person does not exist");
            }
            var photo = person.Photo ?? new byte[0];
            var filePath = request.TempFolderPath + '\\' + Guid.NewGuid();
            File.WriteAllBytes(filePath, photo);

            return new GetPersonPhotoResponse
            {
                PersonPhotoFilePath = filePath
            };
        }

        public GetPersonDetailsResponse GetPersonDetails(GetPersonDetailsRequest request)
        {
            var entity = NHibernateSession.Current.Query<NaatiEntity>()
                .SingleOrDefault(x => x.NaatiNumber == request.NaatiNumber);

            if (entity == null)
            {
                throw new WebServiceException(string.Format("Entity not found (NAATI Number {0})", request.NaatiNumber));
            }

            return new GetPersonDetailsResponse
            {
                Results = NHibernateSession.Current.TransformSqlQueryDataRowResult<PersonEntityDto>($"exec PersonSelect {entity.Id}")
            };
        }

        public GetPersonDetailsBasicResponse GetPersonDetailsBasic(GetPersonDetailsRequest request)
        {
            var response = new GetPersonDetailsBasicResponse();
            Person person;

            if (request.EntityId.HasValue)
            {
                person = NHibernateSession.Current.Query<Person>()
                    .SingleOrDefault(x => x.Entity.Id == request.EntityId);

                if (person == null)
                {
                    response.Error = true;
                    response.ErrorMessage = $"Person not found (Entity ID {request.EntityId})";
                }
            }
            else if (request.NaatiNumber.HasValue)
            {
                person = NHibernateSession.Current.Query<Person>()
                    .SingleOrDefault(x => x.Entity.NaatiNumber == request.NaatiNumber);

                if (person == null)
                {
                    response.Error = true;
                    response.ErrorMessage = $"Person not found (NAATI Number {request.NaatiNumber})";
                }
            }
            else if (request.PersonId.HasValue)
            {
                person = NHibernateSession.Current.Query<Person>()
                    .SingleOrDefault(x => x.Id == request.PersonId);

                if (person == null)
                {
                    response.Error = true;
                    response.ErrorMessage = $"Person not found (Person Id {request.PersonId})";
                }
            }
            else
            {
                throw new ArgumentException("Either NAATI Number or Entity ID or Person Id must be provided");
            }

            if (!response.Error)
            {
                response.PersonDetails = PersonToBasicDetails(person);
            }

            return response;
        }

        public static PersonDetailsBasicDto PersonToBasicDetails(Person person)
        {
            try
            {
                var address = person.PrimaryAddress ?? person.Addresses?.FirstOrDefault();
                var isAustraliaAddress = address?.Country?.Id == 13; // Australia -? Move this to common config;
                string postCode = null;
                string suburb = null;
                bool? isOverseas = null;
                if(address != null && address.Postcode != null)
                {
                    postCode = address.Postcode.PostCode;
                    suburb = address.Postcode.Suburb.Name;
                }
                if(address!= null && address.Country!= null)
                {
                    isOverseas = address.Country.Id != 13;
                }
                return new PersonDetailsBasicDto
                {
                    EntityId = person.Entity.Id,
                    Title = person.Title,
                    TitleId = person.TitleId,
                    PersonId = person.Id,
                    CountryOfBirth = person.BirthCountry?.Name,
                    CountryOfBirthId = person.BirthCountry?.Id,
                    DateOfBirth = person.BirthDate,
                    FamilyName = person.Surname,
                    OtherNames = person.OtherNames,
                    Gender = person.Gender,
                    GivenName = person.GivenName,
                    NaatiNumber = person.Entity.NaatiNumber,
                    PractitionerNumber = person.PractitionerNumber,
                    AddressLine1 = address?.StreetDetails,
                    AddressLine2 = address?.SuburbOrCountry,
                    PostCode = address?.Postcode?.PostCode,
                    Suburb = address?.Postcode?.Suburb.Name,
                    IsOverseas = isOverseas,
                    PrimaryContactNumber = person.PrimaryContactNumber,
                    PrimaryEmail = person.PrimaryEmailAddress,
                    HasPhoto = person.HasPhoto,
                    EthicalCompetency = person.EthicalCompetency,
                    InterculturalCompetency = person.InterculturalCompetency,
                    KnowledgeTest = person.KnowledgeTest,
                    IsMyNaatiRegistered = person.IsEportalActive == true,
                    DoNotSendCorrespondence = person.DoNotSendCorrespondence,
                    Deceased = person.Deceased,
                    EntityTypeId = person.Entity.EntityTypeId,
                    AllowAutoRecertification = person.AllowAutoRecertification,
                    HasAddressInAustralia = isAustraliaAddress
                };
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }

        public void UpdatePersonDetails(UpdatePersonDetailsRequest request)
        {
            var person = NHibernateSession.Current.Get<Person>(request.PersonId);

            if (person == null)
            {
                throw new WebServiceException(string.Format("Person not found (Person ID {0})", request.PersonId));
            }

            _autoMapperHelper.Mapper.Map(request, person);

            if (request.BirthCountryId.HasValue &&
                (person.BirthCountry == null || request.BirthCountryId != person.BirthCountry.Id))
            {
                person.BirthCountry = NHibernateSession.Current.Query<Country>()
                    .SingleOrDefault(x => x.Id == request.BirthCountryId);
            }

            if (request.EntityTypeId != person.Entity.EntityTypeId)
            {
                person.Entity.EntityTypeId = request.EntityTypeId;
            }

            NHibernateSession.Current.Evict(person.Entity.PrimaryAddresses); // prevent "collection was not processed by flush()" exception

            if (string.IsNullOrWhiteSpace(person.Gender))
            {
                person.Gender = null;
            }
            NHibernateSession.Current.Save(person);
            NHibernateSession.Current.Flush();
        }

        public void UpdatePersonSettings(UpdatePersonSettingsRequest request)
        {
            var entity = GetNaatiEntity(request.EntityId);

            request.Gender = string.IsNullOrWhiteSpace(request.Gender) ? null : request.Gender;
            var person = NHibernateSession.Current.Get<Person>(request.PersonId);

            if (person == null)
            {
                throw new WebServiceException(string.Format("Person not found (Person ID {0})", request.PersonId));
            }

            if (request.BirthCountryId.HasValue)
            {
                person.BirthCountry = NHibernateSession.Current.Get<Country>(request.BirthCountryId);
            }
            else
            {
                person.BirthCountry = null;
            }

            _autoMapperHelper.Mapper.Map(request, entity);
            _autoMapperHelper.Mapper.Map(request, person);

            using (var transaction = NHibernateSession.Current.BeginTransaction())
            {
                NHibernateSession.Current.Save(entity);
                NHibernateSession.Current.Save(person);

                if (person.Deceased == true)           
                {
                    DateTime now = DateTime.Now;

                    var creds = NHibernateSession.Current
                                  .Query<Credential>()
                                  .Where(c =>
                                         c.CertificationPeriod != null &&
                                         c.CertificationPeriod.Person.Id == person.Id &&
                                         c.TerminationDate == null)        
                                  .ToList();

                    foreach (var cred in creds)
                    {
                        cred.TerminationDate = now;   
                        cred.ShowInOnlineDirectory = false; 
                        NHibernateSession.Current.SaveOrUpdate(cred);
                    }
                }

                if (request.RolePlayerSettingsRequest != null)
                {
                    var examinerToolService = new ExaminerToolsService(_autoMapperHelper);
                    examinerToolService.SaveRolePlayerSettings(request.RolePlayerSettingsRequest, transaction);
                }
                NHibernateSession.Current.Flush();
                transaction.Commit();
            }

        }

        private NaatiEntity GetNaatiEntity(int entityId)
        {
            var entity = NHibernateSession.Current.Get<NaatiEntity>(entityId);

            if (entity == null)
            {
                throw new WebServiceException(string.Format("Entity not found (Entity ID {0})", entityId));
            }

            return entity;
        }

        public GetAddressesResponse GetPersonAddresses(GetAddressRequest request)
        {
            return new GetAddressesResponse
            {
                Addresses = GetPersonAddresses(request.EntityId)
            };
        }

        private IList<AddressDetailsDto> GetPersonAddresses(int entityId)
        {
            var addresses = NHibernateSession.Current.TransformSqlQueryDataRowResult<AddressDetailsDto>("exec AddressSelect " + entityId);
            return addresses;
        }

        private IList<PhoneDetailsDto> GetPersonPhones(int entityId)
        {
            return NHibernateSession.Current.TransformSqlQueryDataRowResult<PhoneDetailsDto>("exec PhoneSelect " + entityId);
        }

        public GetEmailsResponse GetPersonEmails(GetEmailRequest request)
        {
            return new GetEmailsResponse
            {
                Emails = GetPersonEmails(request.EntityId)
            };
        }

        private IList<EmailDetailsDto> GetPersonEmails(int entityId)
        {
            return NHibernateSession.Current.TransformSqlQueryDataRowResult<EmailDetailsDto>("exec EmailSelect " + entityId);
        }

        public CreatePersonResponse CreatePerson(CreatePersonRequest request)
        {

            if (!string.IsNullOrWhiteSpace(request.PrimaryEmail))
            {
                var emailValidationResult = ValidateEmail(0, request.PrimaryEmail, false, true, 0);
                if (emailValidationResult.Result != EmailValidationResult.Ok)
                {
                    return new CreatePersonResponse { ErrorMessage = emailValidationResult.Message };
                }
            }

            var naatiEntity = new NaatiEntity
            {
                WebsiteUrl = string.Empty,
                WebsiteInPD = false,
                Abn = string.Empty,
                Note = string.Empty,
                UseEmail = false,
                GstApplies = false,
                NaatiNumber = GetNextPersonNaatiNumber(),
                EntityTypeId = 1
            };
            request.DateOfBirth = request.DateOfBirth == new DateTime() ? null : request.DateOfBirth;

            var person = new Person
            {
                Entity = naatiEntity,
                Gender = string.IsNullOrWhiteSpace(request.Gender) ? null : request.Gender,
                BirthDate = request.DateOfBirth,
                Deceased = false,
                ReleaseDetails = false,
                DoNotInviteToDirectory = false,
                ExpertiseFreeText = string.Empty,
                NameOnAccreditationProduct = string.Empty,
                DoNotSendCorrespondence = false,
                ScanRequired = false,
                IsEportalActive = false,
                AllowVerifyOnline = true,
                ShowPhotoOnline = true,
                EnteredDate = DateTime.Now,
                BirthCountry = request.BirthCountryId.HasValue ? NHibernateSession.Current.Get<Country>(request.BirthCountryId) : null,
                AllowAutoRecertification = request.AllowAutoRecertification
            };

            var personName = new PersonName
            {
                GivenName = request.GivenName?.Trim(),
                Surname = request.SurName?.Trim(),
                EffectiveDate = DateTime.Now,
                AlternativeGivenName = string.Empty,
                AlternativeSurname = string.Empty,
                OtherNames = (request.OtherNames ?? string.Empty).Trim(),
                Title = NHibernateSession.Current.Get<Title>(request.Title ?? 0)
            };

            personName.ChangePerson(person);

            var email = new Email
            {
                Entity = naatiEntity,
                EmailAddress = request.PrimaryEmail,
                IsPreferredEmail = true,
                Note = string.Empty,
                IncludeInPD = false,
                Invalid = false
            };

            using (var transaction = NHibernateSession.Current.BeginTransaction())
            {
                NHibernateSession.Current.Save(naatiEntity);
                NHibernateSession.Current.Save(person);
                NHibernateSession.Current.Save(email);
                NHibernateSession.Current.Save(personName);
                transaction.Commit();
            }

            LoggingHelper.LogInfo("Person created: NAATI No. {NaatiNumber}, ID {PersonId}", naatiEntity.NaatiNumber, person.Id);

            return new CreatePersonResponse { NaatiNumber = naatiEntity.NaatiNumber, PersonId = person.Id };
        }

        public CheckPersonResponse CheckPerson(CreatePersonRequest request)
        {
            if (!string.IsNullOrWhiteSpace(request.PrimaryEmail))
            {
                var emailValidationResult = ValidateEmail(0, request.PrimaryEmail, false, true, 0);
                if (emailValidationResult.Result != EmailValidationResult.Ok)
                {
                    return new CheckPersonResponse { ErrorMessage = emailValidationResult.Message };
                }
            }
            request.OtherNames = request.OtherNames ?? string.Empty;
            request.SurName = request.SurName ?? string.Empty;

            var people = NHibernateSession.Current.Query<Person>().Where(x => x.GivenName == request.GivenName);
            people = people.Where(x => x.OtherNames == (request.OtherNames ?? string.Empty));
            people = people.Where(x => x.Surname == request.SurName);
            people = people.Where(x => x.BirthCountry.Id == request.BirthCountryId);
            people = people.Where(x => x.BirthDate == request.DateOfBirth);
            people = people.Where(x => x.Gender == request.Gender);

            var result = people.ToList();
            var dtos = new List<CheckPersonDto>();

            foreach (var person in result)
            {
                var primaryEmail =
                    person.Entity.Emails.FirstOrDefault(e => !e.Invalid && e.IsPreferredEmail)?.EmailAddress ??
                    string.Empty;
                var dto = new CheckPersonDto
                {
                    GivenName = person.GivenName,
                    OtherNames = person.OtherNames,
                    SurName = person.Surname,
                    PrimaryEmail = primaryEmail,
                    BirthCountry = person.BirthCountry != null ? person.BirthCountry.Name : "",
                    DateOfBirth = person.BirthDate
                };

                dtos.Add(dto);
            }
            return new CheckPersonResponse
            {
                Results = dtos
            };
        }



        private int GetNextPersonNaatiNumber()
        {
            return KeyAllocation.GetSingleKey("PersonNaatiNumber");
        }

        private IList<WebsiteDetailsDto> GetPersonWebsites(int entityId)
        {
            var entity = GetNaatiEntity(entityId);
            var list = new List<WebsiteDetailsDto>();

            if (!string.IsNullOrWhiteSpace(entity.WebsiteUrl))
            {
                list.Add(new WebsiteDetailsDto
                {
                    EntityId = entityId,
                    IncludeInPd = entity.WebsiteInPD,
                    Url = entity.WebsiteUrl
                });
            }

            return list;
        }

        public GetContactDetailsResponse GetPersonContactDetails(GetContactDetailsRequest request)
        {
            var person =
                NHibernateSession.Current.TransformSqlQueryDataRowResult<PersonEntityDto>(
                    $"exec PersonSelect {request.EntityId}").FirstOrDefault();

            var addresses = GetPersonAddresses(request.EntityId);
            var phones = GetPersonPhones(request.EntityId);
            var emails = GetPersonEmails(request.EntityId);
            var websites = GetPersonWebsites(request.EntityId);
            var isExaminer = person?.IsExaminer ?? false;

            if (isExaminer)
            {
                foreach(var address in addresses)
                {
                    address.IsExaminer = true;
                }

                foreach (var phone in phones)
                {
                    phone.IsExaminer = true;
                }

                foreach (var email in emails)
                {
                    email.IsExaminer = true;
                }
            }

            var personContactDetails = new GetContactDetailsResponse
            {
                Addresses = addresses,
                Phones = phones,
                Emails = emails,
                Websites = websites,
                ShowWebsite =
                    person != null &&
                    (person.IsFuturePractitioner || person.IsPractitioner || person.IsFormerPractitioner),
                IsMyNaatiRegistered = person?.IsEportalActive == true
            };

            return personContactDetails;
        }

        public GetPersonAddressResponse GetPersonAddress(GetAddressRequest request)
        {
            var addresses = GetPersonAddresses(request.EntityId);
            return new GetPersonAddressResponse { Address = addresses.SingleOrDefault(x => x.AddressId == request.AddressId) };
        }

        public GetPersonPhoneResponse GetPersonPhone(GetPhoneRequest request)
        {
            var phones = GetPersonPhones(request.EntityId);
            return new GetPersonPhoneResponse { Phone = phones.SingleOrDefault(x => x.PhoneId == request.PhoneId) };
        }

        public GetPersonPhonesResponse GetPersonPhones(GetPhonesRequest request)
        {
            var phones = GetPersonPhones(request.EntityId);
            return new GetPersonPhonesResponse { Phones = phones };
        }

        public GetPersonEmailResponse GetPersonEmail(GetEmailRequest request)
        {
            var email = GetPersonEmails(request.EntityId);

            var getPersonEmailResponse = new GetPersonEmailResponse
            {
                Email = email.SingleOrDefault(x => x.EmailId == request.EmailId)
            };
            return getPersonEmailResponse;
        }

        public GetPersonWebsiteResponse GetPersonWebsite(GetWebsiteRequest request)
        {
            var entity = GetNaatiEntity(request.EntityId);
            return new GetPersonWebsiteResponse
            {
                Website = new WebsiteDetailsDto
                {
                    EntityId = entity.Id,
                    IncludeInPd = entity.WebsiteInPD,
                    Url = entity.WebsiteUrl
                }
            };
        }

        public GetSuburbsResponse GetSuburbs()
        {
            var sql = @"
                SELECT Suburb, 
                       tblSuburb.SuburbId,
                       State, 
                       tluState.StateId,
                       Postcode, 
                       MAX(PostcodeId) as PostCodeId
                FROM   tblSuburb JOIN 
	                   tblPostcode ON tblPostcode.SuburbId = tblSuburb.SuburbId JOIN 
	                   tluState ON tluState.StateId = tblSuburb.StateId
                GROUP BY Suburb,tblSuburb.SuburbId,State,tluState.StateId,Postcode";

            return new GetSuburbsResponse
            {
                Suburbs = NHibernateSession.Current.TransformSqlQueryDataRowResult<SuburbStatePostCodeDto>(sql)
            };
        }

        private void RemoveIncludeInPdFlag(int entityId)
        {
            IList<Address> addresses = NHibernateSession.Current.Query<Address>().Where(x => x.Entity.Id == entityId).Fetch(x => x.Postcode).ThenFetch(x => x.Suburb).ToList();
            var odAddressDoNotShowVisibilityType = NHibernateSession.Current.Load<OdAddressVisibilityType>((int)OdAddressVisibilityTypeName.DoNotShow);

            foreach (var address in addresses.Where(a => a.OdAddressVisibilityType.Id != (int)OdAddressVisibilityTypeName.DoNotShow))
            {
                address.OdAddressVisibilityType = odAddressDoNotShowVisibilityType;
                NHibernateSession.Current.Save(address);
                NHibernateSession.Current.Flush();
            }
        }

        public UpdateObjectResponse UpdatePersonAddress(UpdatePersonAddressRequest request)
        {
            if (request.Address?.OdAddressVisibilityTypeId == 0)
            {
                request.Address.OdAddressVisibilityTypeId = (int)OdAddressVisibilityTypeName.DoNotShow;
            }
            if (!request.Address.AddressId.HasValue && NHibernateSession.Current.Query<Address>().Count(x => x.Entity.Id == request.Address.EntityId && !x.Invalid) > 1)
            {
                throw new WebServiceException("Cannot have more than two addresses");
            }

            var address = request.Address.AddressId.HasValue
                ? NHibernateSession.Current.Get<Address>(request.Address.AddressId)
                : new Address
                {
                    Entity = NHibernateSession.Current.Load<NaatiEntity>(request.Address.EntityId),
                    StartDate = DateTime.Today
                };

            _autoMapperHelper.Mapper.Map(request.Address, address);

            if (request.Address.OdAddressVisibilityTypeId != (int)OdAddressVisibilityTypeName.DoNotShow)
            {
                RemoveIncludeInPdFlag(request.Address.EntityId);
            }

            address.OdAddressVisibilityType = NHibernateSession.Current.Load<OdAddressVisibilityType>(request.Address.OdAddressVisibilityTypeId);

            if (request.Address.CountryId.HasValue)
            {
                address.Country = NHibernateSession.Current.Load<Country>(request.Address.CountryId);
            }
            else
            {
                if (!string.IsNullOrEmpty(request.Address.CountryName))
                {
                    address.Country = NHibernateSession.Current.Query<Country>().FirstOrDefault(
                        x => x.Name.ToLower() == request.Address.CountryName.ToLower());
                    if (address.Country == null)
                    {
                        throw new WebServiceException("Country name is unrecognised: " + request.Address.CountryName);
                    }
                }
            }

            if (address.Country == null)
            {
                throw new WebServiceException("Address must have a country");
            }

            if (request.Address.PostcodeId.HasValue)
            {
                address.Postcode = NHibernateSession.Current.Load<Postcode>(request.Address.PostcodeId);
            }
            else
            {
                address.Postcode = null;

                if (!string.IsNullOrEmpty(request.Address.Suburb))
                {
                    var replacedSuburb =
                        request.Address.Suburb.Replace("saint ", "st ", StringComparison.CurrentCultureIgnoreCase); // Fix bug 87094
                                                                                                                    // find the suburb with matching postcode
                    var suburb = NHibernateSession.Current.Query<Suburb>().FirstOrDefault(x =>
                        (x.Name.ToLower() == request.Address.Suburb.ToLower() || x.Name.ToLower() == replacedSuburb)
                        && x.Postcodes.Any(y => y.PostCode == request.Address.Postcode));

                    if (suburb != null)
                    {
                        address.Postcode = suburb.Postcodes.FirstOrDefault(x => x.PostCode == request.Address.Postcode);
                    }
                    else
                    {
                        //db doesnt have the entry. Create it
                        // since this suburb was not recognised we must add it to the naati database
                        address.Postcode = AddNewPostCode(new AddNewSuburbRequest
                        {
                            Postcode = request.Address.Postcode,
                            State = request.Address.StateAbbreviation,
                            SuburbName = request.Address.Suburb?.ToUpper()
                        });

                        if (address.Postcode == null)
                        {
                            LoggingHelper.LogError(string.Empty, "The suburb provided was not found. Please enter your suburb or full address and try again. If you continue to experience a problem, please use the link below to contact NAATI.");
                        }
                    }
                }
            }

            if (request.Address.ExaminerCorrespondence)
            {
                foreach(var tempAddress in address.Entity.Addresses)
                {
                    tempAddress.ExaminerCorrespondence = false;
                }
                address.ExaminerCorrespondence = request.Address.ExaminerCorrespondence;
            }

            NHibernateSession.Current.Evict(address.Entity.PrimaryAddresses); // prevent "collection was not processed by flush()" exception

            NHibernateSession.Current.Save(address);
            NHibernateSession.Current.Flush();

            return new UpdateObjectResponse { ObjectId = address.Id };
        }

        private Postcode AddNewPostCode(AddNewSuburbRequest request)
        {
            try
            {
                // Need to check whether Suburb exists in the State
                // If so, link to new Post Code
                // Otherwise create and then link to Post Code

                var state = _stateRepository.GetState(request.State);

                if (state == null)
                {
                    return null;
                }

                var databaseSuburb = _stateRepository.GetSuburb(state, request.SuburbName);

                if (databaseSuburb == null)
                {
                    var newSuburb = new Suburb();
                    var newPostcode = new Postcode();

                    //_addressRepository.Session.Transaction.Begin();

                    newSuburb.Name = request.SuburbName;
                    newSuburb.State = state;

                    newPostcode.PostCode = request.Postcode;
                    newPostcode.Suburb = newSuburb;

                    _addressRepository.Session.Save(newSuburb);
                    _addressRepository.Session.Save(newPostcode);

                    //_addressRepository.Session.Transaction.Commit();
                    //_addressRepository.Session.Flush();
                    _postcodesCacheQueryService.AddOrRefreshItem(newPostcode.Id);

                    return newPostcode;
                }
                else
                {
                    var newPostcode = new Postcode();

                    //_addressRepository.Session.Transaction.Begin();

                    databaseSuburb.Name = request.SuburbName;
                    databaseSuburb.State = state;

                    newPostcode.PostCode = request.Postcode;
                    newPostcode.Suburb = databaseSuburb;

                    _addressRepository.Session.Save(newPostcode);

                    //_addressRepository.Session.Transaction.Commit();
                    //_addressRepository.Session.Flush();
                    //_postcodesCacheQueryService.AddOrRefreshItem(newPostcode.Id);

                    return newPostcode;
                }
            }
            catch
            {
                return null;
            }
        }

        public UpdateObjectResponse UpdatePersonPhone(UpdatePersonPhoneRequest request)
        {
            Phone phone;

            if (!request.Phone.PhoneId.HasValue && NHibernateSession.Current.Query<Phone>().Count(x => x.Entity.Id == request.Phone.EntityId && !x.Invalid) > 1)
            {
                throw new WebServiceException("Cannot have more than two phone numbers");
            }

            if (request.Phone.PhoneId.HasValue)
            {
                phone = NHibernateSession.Current.Get<Phone>(request.Phone.PhoneId);
            }
            else
            {
                phone = new Phone
                {
                    Entity = NHibernateSession.Current.Load<NaatiEntity>(request.Phone.EntityId)
                };
            }

            _autoMapperHelper.Mapper.Map(request.Phone, phone);

            phone.CountryCode = string.Empty;
            phone.AreaCode = string.Empty;

            if (request.Phone.ExaminerCorrespondence)
            {
                phone.Entity.Phones.ForEach(x => x.ExaminerCorrespondence = false);
                phone.ExaminerCorrespondence = request.Phone.ExaminerCorrespondence;
            }

            NHibernateSession.Current.Evict(phone.Entity.PrimaryAddresses); // prevent "collection was not processed by flush()" exception

            NHibernateSession.Current.Save(phone);
            NHibernateSession.Current.Flush();

            return new UpdateObjectResponse { ObjectId = phone.Id };
        }

        enum EmailValidationResult
        {
            Ok,
            MyNaatiRegistered,
            MyNaatiRegisteredOtherPerson
        }

        private (EmailValidationResult Result, string Message) ValidateEmail(int emailId, string emailAddress, bool invalid, bool isPreferred, int entityId)
        {
            var query = from email in NHibernateSession.Current.Query<Email>()
                        join person in NHibernateSession.Current.Query<Person>() on email.Entity.Id equals person.Entity.Id
                        where email.IsPreferredEmail
                              && !email.Invalid
                              && (bool)person.IsEportalActive
                              && email.EmailAddress == emailAddress
                        select new { emailId = email.Id, entityId = email.Entity.Id };

            var existingEmails = query.ToList();

            if (!invalid && existingEmails.Any(x => x.entityId != entityId))
            {
                return (EmailValidationResult.MyNaatiRegisteredOtherPerson, $"Another person is already registered in myNAATI with email {emailAddress}");
            }

            var primaryEmailQuery = from email in NHibernateSession.Current.Query<Email>()
                                    join person in NHibernateSession.Current.Query<Person>() on email.Entity.Id equals person.Entity.Id
                                    where email.IsPreferredEmail
                                            && !email.Invalid
                                            && email.Entity.Id == entityId
                                    select new { email.Id, email.EmailAddress, person.IsEportalActive };

            var primaryEmail = primaryEmailQuery.FirstOrDefault();

            if (primaryEmail == null || !primaryEmail.IsEportalActive.GetValueOrDefault())
            {
                return (EmailValidationResult.Ok, null);
            }

            if (emailId != 0)
            {
                var email = NHibernateSession.Current.Get<Email>(emailId);

                if (primaryEmail.Id == email.Id &&
                    (invalid || emailAddress.ToLower() != email.EmailAddress.ToLower() || !isPreferred))
                {
                    return (EmailValidationResult.MyNaatiRegistered, $"This person is already registered in myNAATI with email {primaryEmail.EmailAddress}");
                }
            }

            if (primaryEmail.Id != emailId && isPreferred && !invalid)
            {
                return (EmailValidationResult.MyNaatiRegistered, $"This person is already registered in myNAATI with email {primaryEmail.EmailAddress}");
            }

            return (EmailValidationResult.Ok, null);
        }

        public UpdateObjectResponse UpdatePersonEmail(UpdatePersonEmailRequest request)
        {
            Email email;

            var validationResult = ValidateEmail(request.Email.EmailId.GetValueOrDefault(),
                request.Email.Email,
                request.Email.Invalid,
                request.Email.IsPreferredEmail,
                request.Email.EntityId);

            if (validationResult.Result != EmailValidationResult.Ok)
            {
                if (validationResult.Result != EmailValidationResult.MyNaatiRegistered || !request.AllowUpdateWhenMyNaatiRegistered)
                {
                    return new UpdateObjectResponse { ErrorMessage = validationResult.Message };
                }
            }

            if (!request.Email.EmailId.HasValue && NHibernateSession.Current.Query<Email>().Count(x => x.Entity.Id == request.Email.EntityId && !x.Invalid) > 1)
            {
                throw new WebServiceException("Cannot have more than two email addresses");
            }

            if (request.Email.EmailId.HasValue)
            {
                email = NHibernateSession.Current.Get<Email>(request.Email.EmailId);
            }
            else
            {
                email = new Email
                {
                    Entity = NHibernateSession.Current.Load<NaatiEntity>(request.Email.EntityId)
                };
            }

            _autoMapperHelper.Mapper.Map(request.Email, email);

            if (request.Email.ExaminerCorrespondence)
            {
                foreach(var tempEmail in email.Entity.Emails)
                {
                    tempEmail.ExaminerCorrespondence = false;
                }
                email.ExaminerCorrespondence = request.Email.ExaminerCorrespondence;
            }

            email.EmailAddress = request.Email.Email;

            NHibernateSession.Current.Evict(email.Entity.PrimaryAddresses); // prevent "collection was not processed by flush()" exception

            NHibernateSession.Current.Save(email);
            NHibernateSession.Current.Flush();

            return new UpdateObjectResponse { ObjectId = email.Id };
        }


        public void UpdatePersonWebsite(UpdatePersonWebsiteRequest request)
        {
            var entity = GetNaatiEntity(request.Website.EntityId);
            entity.WebsiteUrl = request.Website.Url;
            entity.WebsiteInPD = request.Website.IncludeInPd;

            NHibernateSession.Current.Save(entity);
            NHibernateSession.Current.Flush();
        }

        public void DeletePersonAddress(DeleteObjectRequest request)
        {
            var address = NHibernateSession.Current.Get<Address>(request.ObjectId);
            var odAddressVisibilityType = NHibernateSession.Current.Get<OdAddressVisibilityType>((int)OdAddressVisibilityTypeName.DoNotShow);

            if (address == null)
            {
                throw new WebServiceException(string.Format("Address not found (ID {0})", request.ObjectId));
            }

            address.Invalid = true;
            address.PrimaryContact = false;
            address.OdAddressVisibilityType = odAddressVisibilityType;

            NHibernateSession.Current.Evict(address.Entity.PrimaryAddresses); // prevent "collection was not processed by flush()" exception

            NHibernateSession.Current.Save(address);
            NHibernateSession.Current.Flush();
        }

        public DeleteObjectResponse DeletePersonPhone(DeleteObjectRequest request)
        {
            var phone = NHibernateSession.Current.Get<Phone>(request.ObjectId);
            if (phone == null)
            {
                throw new WebServiceException(string.Format("Phone number not found (ID {0})", request.ObjectId));
            }

            phone.Invalid = true;
            phone.IncludeInPD = false;

            NHibernateSession.Current.Evict(phone.Entity.PrimaryAddresses); // prevent "collection was not processed by flush()" exception

            NHibernateSession.Current.Save(phone);
            NHibernateSession.Current.Flush();

            return new DeleteObjectResponse { Success = true };
        }

        public DeleteObjectResponse DeletePersonEmail(DeleteObjectRequest request)
        {
            var email = NHibernateSession.Current.Get<Email>(request.ObjectId);
            if (email == null)
            {
                throw new WebServiceException(string.Format("Email address not found (ID {0})", request.ObjectId));
            }

            email.Invalid = true;
            email.IsPreferredEmail = false;
            email.IncludeInPD = false;

            NHibernateSession.Current.Evict(email.Entity.PrimaryAddresses); // prevent "collection was not processed by flush()" exception

            NHibernateSession.Current.Save(email);
            NHibernateSession.Current.Flush();

            return new DeleteObjectResponse { Success = true };
        }

        public GetPersonNameResponse GetPersonName(GetPersonNameRequest request)
        {
            var person = NHibernateSession.Current.Get<Person>(request.PersonId);
            if (person == null)
            {
                throw new WebServiceException(string.Format("Person not found (Person ID {0})", request.PersonId));
            }

            return new GetPersonNameResponse
            {
                Names = person.PersonNames.Select(a =>
                {
                    var name = _autoMapperHelper.Mapper.Map<PersonNameDto>(a);
                    name.TitleId = a.Title == null ? null : (int?)a.Title.Id;
                    name.PersonNameId = a.Id;
                    return name;
                })
            };
        }

        public AddNameResponse AddName(AddNameRequest request)
        {
            if (request.Name == null)
            {
                throw new WebServiceException("Name is required");
            }

            var person = NHibernateSession.Current.Query<Person>().FirstOrDefault(p => p.Entity.NaatiNumber == request.NaatiNumber);
            if (person == null)
            {
                throw new WebServiceException(string.Format("Person not found (Naati Number {0})", request.NaatiNumber));
            }

            var name = new PersonName();
           _autoMapperHelper.Mapper.Map(request.Name, name);

            Title title;
            if (request.Name.TitleId.HasValue)
            {
                title = NHibernateSession.Current.Load<Title>(request.Name.TitleId.Value);
                name.Title = title;
            }

            name.EffectiveDate = DateTime.Now;

            person.AddPersonName(name);

            NHibernateSession.Current.Evict(person.Entity.PrimaryAddresses);

            NHibernateSession.Current.Save(name);
            NHibernateSession.Current.Flush();

            return new AddNameResponse();
        }
        public GetCountryResponse GetCountry(int countryId)
        {
            var country = NHibernateSession.Current.Get<Country>(countryId);
            var dto = new CountryDto();

            if (country != null)
            {
                dto.Code = country.Code;
                dto.CountryId = country.Id;
                dto.Name = country.Name;
            }

            return new GetCountryResponse
            {
                Country = dto
            };
        }

        public void UpdatePhoto(UpdatePhotoDto request)
        {
            var photo = NHibernateSession.Current.Query<PersonImage>().FirstOrDefault(p => p.Person.Entity.NaatiNumber == request.NaatiNumber);
            if (photo == null)
            {
                var person = NHibernateSession.Current.Query<Person>().First(p => p.Entity.NaatiNumber == request.NaatiNumber);

                photo = new PersonImage { Person = person };
            }

            photo.PhotoDate = DateTime.Now;
            var filePath = request.FilePath;

            try
            {
                photo.Photo = File.ReadAllBytes(filePath);
            }
            finally
            {
                File.Delete(filePath);
            }

            NHibernateSession.Current.Evict(photo.Person.Entity.PrimaryAddresses); // prevent "collection was not processed by flush()" exception
            NHibernateSession.Current.Evict(photo.Person.PersonImages); // prevent "collection was not processed by flush()" exception
            NHibernateSession.Current.Evict(photo.Person.PersonNames); // prevent "collection was not processed by flush()" exception

            NHibernateSession.Current.Save(photo);
            NHibernateSession.Current.Flush();
        }

        public bool TestPractitionerNumberUniqueness(string practitionerNumber)
        {
            var query = NHibernateSession.Current.CreateQuery(
                    "SELECT COUNT(*) FROM Person WHERE PractitionerNumber = :pn")
                .SetParameter("pn", practitionerNumber);

            var result = query.List<long>();
            return result.First() == 0;
        }

        public ServiceResponse AssignPractitionerNumber(AssignPractitionerNumberRequest request)
        {
            var response = new ServiceResponse();
            var person = NHibernateSession.Current.Get<Person>(request.PersonId);
            if (!String.IsNullOrEmpty(person.PractitionerNumber))
            {
                response.Error = true;
                response.ErrorMessage = "Person already has a Practitioner Number.";
            }
            else
            {
                person.PractitionerNumber = request.PractitionerNumber;
                NHibernateSession.Current.Evict(person.Entity.PrimaryAddresses); // prevent "collection was not processed by flush()" exception
                NHibernateSession.Current.Evict(person.PersonImages); // prevent "collection was not processed by flush()" exception
                NHibernateSession.Current.Evict(person.PersonNames); // prevent "collection was not processed by flush()" exception
                NHibernateSession.Current.Save(person);
                NHibernateSession.Current.Flush();
            }
            return response;
        }

        public GetPersonMembershipRolesResponse GetPersonMembershipRolesByEntityId(int entityId)
        {
            var person = NHibernateSession.Current.Query<Person>()
                .SingleOrDefault(x => x.Entity.Id == entityId);

            if (person == null)
            {
                throw new WebServiceException(string.Format("Person not found (Entity Id {0})", entityId));
            }

            var personId = person.Id;

            var personMembershipRoleList = NHibernateSession.Current.Query<PanelMembership>()
                .Where(x => x.Person.Id == personId).ToList();

            var personMembershipRoleDtoList = new List<PersonMembershipRoleDto>();

            if (personMembershipRoleList.Any())
            {
                foreach (var personMembershipRole in personMembershipRoleList)
                {
                    personMembershipRoleDtoList.Add(new PersonMembershipRoleDto
                    {
                        PersonId = personMembershipRole.Id,
                        FamilyName = personMembershipRole.Person.Surname,
                        GivenName = personMembershipRole.Person.GivenName,
                        PrimaryEmail = personMembershipRole.Person.PrimaryEmailAddress,
                        PanelId = personMembershipRole.Panel.Id,
                        PanelTypeId = personMembershipRole.Panel.PanelType.Id,
                        PanelName = personMembershipRole.Panel.Name,
                        PanelRoleId = personMembershipRole.PanelRole.Id,
                        PanelRoleName = personMembershipRole.PanelRole.Name,
                        StateDate = personMembershipRole.StartDate,
                        EndDate = personMembershipRole.EndDate,
                        PanelRoleCategory = (PanelRoleCategoryName)personMembershipRole.PanelRole.PanelRoleCategory.Id
                    });
                }
            }

            return new GetPersonMembershipRolesResponse
            {
                PersonMembershipRoles = personMembershipRoleDtoList
            };

        }


        public GetExminerRoleFlagResponse HasExminerRoleByEntityId(GetExminerRoleFlagRequest request)
        {
            var exminerRoleFlagResponse = new GetExminerRoleFlagResponse();
            var isInstitution =
                NHibernateSession.Current.Query<Institution>().Any(x => x.Entity.Id == request.EntityId);

            if (isInstitution)
            {
                return exminerRoleFlagResponse;
            }

            var personMembershipRoles = GetPersonMembershipRolesByEntityId(request.EntityId).PersonMembershipRoles;


            if (personMembershipRoles.Any())
            {
                exminerRoleFlagResponse.HasExaminerRole =
                    personMembershipRoles.Any(x => x.PanelRoleCategory == PanelRoleCategoryName.Examiner
                                                   && x.EndDate.HasValue && x.EndDate >= DateTime.Now.Date &&
                                                   x.StateDate.HasValue && x.StateDate <= DateTime.Now.Date);
            }

            return exminerRoleFlagResponse;
        }

        public DateTime? GetCertificationPeriodEarliestDate(int periodId)
        {
            var query = NHibernateSession.Current.Query<Credential>()
                .Where(c => c.CertificationPeriod.Id == periodId)
                .OrderBy(c => c.StartDate);

            DateTime? result = query.FirstOrDefault()?.StartDate;

            return result;
        }

        public GetCertificationPeriodsResponse GetCertificationPeriods(GetCertificationPeriodsRequest request)
        {
            var query = NHibernateSession.Current.Query<CertificationPeriod>();

            if (request.PersonId.HasValue)
            {
                query = query.Where(c => c.Person.Id == request.PersonId.Value);
            }
            else if (request.NaatiNumber.HasValue)
            {
                query = query.Where(c => c.Person.Entity.NaatiNumber == request.NaatiNumber.Value);
            }
            else if (!String.IsNullOrEmpty(request.PractitionerNumber))
            {
                query = query.Where(c => c.Person.PractitionerNumber.ToUpper() == request.PractitionerNumber);
            }

            var hasExpired = request.CertificationPeriodStatus.Any(c => c == CertificationPeriodStatus.Expired);
            var hasCurrent = request.CertificationPeriodStatus.Any(c => c == CertificationPeriodStatus.Current);
            var hasFuture = request.CertificationPeriodStatus.Any(c => c == CertificationPeriodStatus.Future);

            query = query.Where(c =>
                (hasExpired && c.EndDate.Date < DateTime.Now.Date) ||
                (hasCurrent && c.StartDate.Date <= DateTime.Now.Date && DateTime.Now.Date <= c.EndDate.Date) ||
                (hasFuture && c.StartDate.Date > DateTime.Now.Date)
            );

            if (request.PersonId.HasValue)
            {
                query = query.Where(c => c.Person.Id == request.PersonId.Value);
            }

            var periods = query.OrderBy(c => c.StartDate).ToList();

            return new GetCertificationPeriodsResponse
            {
                Results = periods.Select(MapCertificationPeriodDto)
            };
        }


        private CertificationPeriodDto MapCertificationPeriodDto(CertificationPeriod period)
        {

            return new CertificationPeriodDto
            {
                Id = period.Id,
                EndDate = period.EndDate,
                OriginalEndDate = period.OriginalEndDate,
                StartDate = period.StartDate,
                CertificationPeriodStatus = BusinessLogicHelper.GetCertificationPeriodStatus(period.StartDate, period.EndDate),
                CredentialApplicationId = period.CredentialApplication.Id
            };
        }

        public void SetCertificationEndDate(SetCertificationEndDateRequest request)
        {
            var period = NHibernateSession.Current.Load<CertificationPeriod>(request.CertificationPeriodId);

            if (period == null)
            {
                throw new WebServiceException("Referenced certification period does not exist");
            }

            using (var tran = NHibernateSession.Current.BeginTransaction())
            {
                try
                {
                    var noteService = new NoteQueryService(_autoMapperHelper);
                    noteService.CreateOrUpdateNote(new CreateNoteRequest
                    {
                        EntityId = period.Person.Entity.Id,
                        NoteType = NoteType.NaatiEntity,
                        Note = $"Modified the Certification Period End Date from {period.EndDate:dd/MM/yyyy} to {request.NewEndDate:dd/MM/yyyy} (Certification Period ID: {period.Id}).\n{request.Notes}",
                        UserId = request.UserId,
                        Highlight = true,
                        ReadOnly = true
                    });

                    period.EndDate = request.NewEndDate;
                    NHibernateSession.Current.Save(period);

                    tran.Commit();
                }
                catch
                {
                    tran.Rollback();
                    throw;
                }
            }
        }

        public void SetCredentialTerminateDate(SetCredentialTerminateDateRequest request)
        {
            var credential = NHibernateSession.Current.Load<Credential>(request.CredentialId);

            if (credential == null)
            {
                throw new WebServiceException("Referenced credential does not exist");
            }

            using (var tran = NHibernateSession.Current.BeginTransaction())
            {
                try
                {
                    var credentialRequest = credential.CredentialCredentialRequests.First().CredentialRequest;
                    var entityId = credentialRequest.CredentialApplication.Person.Entity.Id;

                    var noteService = new NoteQueryService(_autoMapperHelper);
                    if (request.NewTerminationDate.HasValue)
                    {
                        noteService.CreateOrUpdateNote(new CreateNoteRequest
                        {
                            EntityId = entityId,
                            NoteType = NoteType.NaatiEntity,
                            Note = $"Set a new termination date of {request.NewTerminationDate:dd/MM/yyyy} for {credentialRequest.CredentialType.InternalName}, {credentialRequest.Skill.DisplayName}.\n{request.Notes}",
                            UserId = request.UserId,
                            Highlight = true,
                            ReadOnly = true
                        });
                    }
                    else
                    {
                        noteService.CreateOrUpdateNote(new CreateNoteRequest
                        {
                            EntityId = entityId,
                            NoteType = NoteType.NaatiEntity,
                            Note = $"Removed the termination date of {credential.TerminationDate:dd/MM/yyyy} for {credentialRequest.CredentialType.InternalName}, {credentialRequest.Skill.DisplayName}.\n{request.Notes}",
                            UserId = request.UserId,
                            Highlight = true,
                            ReadOnly = true
                        });
                    }

                    if (request.CertificationPeriodEndDate.HasValue)
                    {
                        if (credential.CertificationPeriod == null)
                        {
                            throw new WebServiceException("Referenced certification period does not exist");
                        }

                        noteService.CreateOrUpdateNote(new CreateNoteRequest
                        {
                            EntityId = entityId,
                            NoteType = NoteType.NaatiEntity,
                            Note = $"Modified the Certification Period (ID: {credential.CertificationPeriod.Id}) End Date from {credential.CertificationPeriod.EndDate:dd/MM/yyyy} to {request.CertificationPeriodEndDate.Value:dd/MM/yyyy} as a result of terminating {credentialRequest.CredentialType.InternalName}, {credentialRequest.Skill.DisplayName}.",
                            UserId = request.UserId,
                            Highlight = true,
                            ReadOnly = true
                        });

                        credential.CertificationPeriod.EndDate = request.CertificationPeriodEndDate.Value;
                        NHibernateSession.Current.Save(credential.CertificationPeriod);
                    }

                    credential.TerminationDate = request.NewTerminationDate;
                    NHibernateSession.Current.Save(credential);

                    tran.Commit();
                }
                catch
                {
                    tran.Rollback();
                    throw;
                }
            }
        }

        public void DeleteMyNaatiDetails(int naatiNumber)
        {
            var person = NHibernateSession.Current
                .Query<Person>()
                .First(x => x.Entity.NaatiNumber == naatiNumber);
            person.IsEportalActive = false;
            person.PersonalDetailsLastUpdatedOnEportal = null;
            person.WebAccountCreateDate = null;
            NHibernateSession.Current.SaveOrUpdate(person);
            NHibernateSession.Current.Flush();
        }

        public void UpdateMyNaatiDetails(MyNaatiAccountDetails request)
        {
            var person = NHibernateSession.Current
                .Query<Person>()
                .First(x => x.Entity.NaatiNumber == request.NaatiNumber);
            person.IsEportalActive = request.Active ?? person.IsEportalActive;
            person.PersonalDetailsLastUpdatedOnEportal = request.LastUpdate ?? person.PersonalDetailsLastUpdatedOnEportal;
            person.WebAccountCreateDate = request.DateCreated ?? person.WebAccountCreateDate;
            NHibernateSession.Current.SaveOrUpdate(person);
            NHibernateSession.Current.Flush();
        }
        public GetTokenResponse GetPersonToken(GetTokenRequest request)
        {
            var queryHelper = new PersonQueryHelper();
            int? hash = null;
            if (request.Type == TokenRequestType.NaatiNumber)
            {
                hash = queryHelper.GetPersonHashByNaatiNumber(request.Value);
            }

            if (request.Type == TokenRequestType.ApplicationId)
            {
                hash = queryHelper.GetPersonHashByApplicationId(request.Value);
            }

            return new GetTokenResponse
            {
                Token = hash
            };
        }

        public void SaveCertificationPeriod(CertificationPeriodDto request)
        {
            var period = NHibernateSession.Current.Get<CertificationPeriod>(request.Id);
            var note = $@"Certification Period {request.Id} has been modified.  \n
Original Dates: Start Date {period.StartDate:dd/MM/yyyy}, Original End Date {period.OriginalEndDate:dd/MM/yyyy}, End Date {period.EndDate:dd/MM/yyyy}.\n
New Dates: Start Date {request.StartDate:dd/MM/yyyy}, Original End Date {request.OriginalEndDate:dd/MM/yyyy}, End Date {request.EndDate:dd/MM/yyyy}.\n
Reason: {request.Notes}";

            period.StartDate = request.StartDate;
            period.EndDate = request.EndDate;
            period.OriginalEndDate = request.OriginalEndDate;
            NHibernateSession.Current.Save(period);

            var entity = NHibernateSession.Current.Query<NaatiEntity>().FirstOrDefault(e => e.NaatiNumber == request.NaatiNumber);

            var noteService = new NoteQueryService(_autoMapperHelper);
            noteService.CreateOrUpdateNote(new CreateNoteRequest
            {
                EntityId = entity.Id,
                NoteType = NoteType.NaatiEntity,
                Note = note,
                UserId = request.UserId,
                Highlight = true,
                ReadOnly = true
            });

            NHibernateSession.Current.Flush();

        }

        public GetPersonAttachmentsResponse GetAttachments(GetPersonAttachmentsRequest request)
        {

            var query = NHibernateSession.Current.Query<PersonAttachment>();

            if (request.PersonId.HasValue)
            {
                query = query.Where(n => n.Person.Id == request.PersonId);
            }
            if (request.StoredFileId.HasValue)
            {
                query = query.Where(n => n.StoredFile.Id == request.StoredFileId);
            }

            if (request.UserRestriction != null)
            {
                var userRoles = NHibernateSession.Current.Query<UserRole>()
                    .Where(x => x.User.Id == request.UserRestriction.UserId)
                    .Select(y => y.SecurityRole.Id)
                    .ToList();
                var documentTypeQuery = NHibernateSession.Current.Query<DocumentTypeRole>().Where(x => userRoles.Contains(x.Role.Id));
                if (request.UserRestriction.Download)
                {
                    documentTypeQuery = documentTypeQuery.Where(x => x.Download);
                }
                if (request.UserRestriction.Upload)
                {
                    documentTypeQuery = documentTypeQuery.Where(x => x.Upload);
                }

                var documentTypeIds = documentTypeQuery.Select(y => y.DocumentType.Id).ToList();

                query = query.Where(x => documentTypeIds.Contains(x.StoredFile.DocumentType.Id));
            }


            var attachments = query.Select(n => new PersonAttachmentDto
            {
                PersonAttachmentId = n.Id,
                PersonId = n.Person.Id,
                StoredFileId = n.StoredFile.Id,
                FileName = n.StoredFile.FileName,
                Description = n.Description,
                DocumentType = n.StoredFile.DocumentType.DisplayName,
                UploadedByName = n.StoredFile.UploadedByUser.FullName,
                UploadedDateTime = n.StoredFile.UploadedDateTime,
                FileSize = n.StoredFile.FileSize,
                Type = (StoredFileType)n.StoredFile.DocumentType.Id,
                SoftDeleteDate = n.StoredFile.StoredFileStatusType.Name != "Current"? n.StoredFile.StoredFileStatusChangeDate:null
            }).ToList();

            return new GetPersonAttachmentsResponse
            {
                Attachments = attachments.ToArray()
            };
        }

        public CreateOrReplacePersonAttachmentResponse CreateOrReplaceAttachment(CreateOrReplacePersonAttachmentRequest request)
        {
            var fileService = new FileSystemFileStorageService(_autoMapperHelper);

            var response = fileService.CreateOrUpdateFile(new CreateOrUpdateFileRequest
            {
                UpdateStoredFileId = request.StoredFileId != 0 ? (int?)request.StoredFileId : null,
                UpdateFileName = request.StoredFileId != 0 ? request.FileName : null,
                Type = request.Type,
                StoragePath = request.StoragePath,
                UploadedByUserId = request.UploadedByUserId,
                UploadedDateTime = DateTime.Now,
                FilePath = request.FilePath,
                TokenToRemoveFromFilename = request.TokenToRemoveFromFilename
            });

            var storeFile = NHibernateSession.Current.Load<StoredFile>(response.StoredFileId);
            var application = NHibernateSession.Current.Load<Person>(request.PersonId);
            var personAttachment = NHibernateSession.Current.Query<PersonAttachment>().SingleOrDefault(n => n.StoredFile.Id == response.StoredFileId);
            if (personAttachment == null)
            {
                personAttachment = new PersonAttachment
                {
                    StoredFile = storeFile,
                    Person = application,
                };
            }

            personAttachment.Description = request.Title;

            NHibernateSession.Current.Save(personAttachment);
            NHibernateSession.Current.Flush();

            return new CreateOrReplacePersonAttachmentResponse
            {
                StoredFileId = response.StoredFileId
            };
        }

        public DeletePersonAttachmentResponse DeleteAttachment(DeletePersonAttachmentRequest request)
        {
            var attachment = NHibernateSession.Current.Query<PersonAttachment>().SingleOrDefault(n => n.StoredFile.Id == request.StoredFileId);
            NHibernateSession.Current.Delete(attachment);
            NHibernateSession.Current.Flush();

            var fileService = new FileSystemFileStorageService(_autoMapperHelper);
            fileService.DeleteFile(new DeleteFileRequest
            {
                StoredFileId = request.StoredFileId
            });

            return new DeletePersonAttachmentResponse();
        }

        public CertificationPeriodDto CreateCertificationPeriod(CreateCertificationPeriodRequest request)
        {
            if (request.StartDate.Year < 1900 || request.EndDate.Year < 1900)
            {
                throw new Exception("Certification period is out of range.");
            }

            var certificationPeriod = new CertificationPeriod
            {
                Person = NHibernateSession.Current.Load<Person>(request.PersonId),
                CredentialApplication =
                    NHibernateSession.Current.Load<CredentialApplication>(request.CredentialApplicationId),
                StartDate = request.StartDate,
                EndDate = request.EndDate,
                OriginalEndDate = request.OriginalEndDate
            };

            NHibernateSession.Current.Save(certificationPeriod);
            NHibernateSession.Current.Flush();

            return MapCertificationPeriodDto(certificationPeriod);
        }

        public GenericResponse<PersonMfaResponse> GetPersonMfaDetails(int naatiNumber)
        {
            var person = NHibernateSession.Current.Query<Person>().FirstOrDefault(x => x.Entity.NaatiNumber == naatiNumber);
            return new PersonMfaResponse()
            {
                Email = person.PrimaryEmailAddress,
                MfaCode = person.MfaCode,
                MfaExpireStartDate = person.MfaExpireStartDate,
                EmailCode = person.LastEmailCode,
                EmailCodeExpireStartDate = person.EmailCodeExpireStartDate,
            };
        }

        public GenericResponse<bool> GetAccessDisabledByNcms(int naatiNumber)
        {
            var person = NHibernateSession.Current.Query<Person>().FirstOrDefault(x => x.Entity.NaatiNumber == naatiNumber);
            return person.AccessDisabledByNcms;
        }

        public BusinessServiceResponse SetPersonMfaDetails(PersonMfaRequest request)
        {
            var person = NHibernateSession.Current.Query<Person>().FirstOrDefault(x => x.Entity.NaatiNumber == request.NaatiNumber);

            if (request.Disable)
            {
                person.MfaCode = null;
            }
            else if (!String.IsNullOrEmpty(request.MfaCode))
            {
                person.MfaCode = request.MfaCode;
            }

            person.MfaExpireStartDate = request.MfaExpireStartDate;
            //NHibernateSession.Current.SaveOrUpdate(person);
            NHibernateSession.Current.Flush();

            return (new BusinessServiceResponse() { Success = true });
        }

        public GenericResponse<List<QrCodeSummaryModelDto>> GetQrCodeSummary(int personId)
        {
            var dataResponse = new List<QrCodeSummaryModelDto>();
            var response = new GenericResponse<List<QrCodeSummaryModelDto>>()
            {
                Data = dataResponse
            };

            var credentials = GetCredentialsByPersonId(personId);
            foreach(var credential in credentials.Data)
            {
                var credentialQrCodes = NHibernateSession.Current.Query<CredentialQrCode>().Where(x => x.Credential.Id == credential.Id).Select(x=>x);
                foreach(var credentialQrCode in credentialQrCodes)
                {
                    dataResponse.Add(new QrCodeSummaryModelDto()
                    {
                        Credential = credential.CredentialTypeExternalName,
                        Skill = credential.SkillDisplayName,
                        GeneratedDate = credentialQrCode.IssueDate,
                        InactiveDate = credentialQrCode.InactiveDate,
                        QrCode = credentialQrCode.QrCodeGuid.ToString(),
                        ModifiedDate = credentialQrCode.ModifiedDate
                    });
                }
            }
            return response;
        }

        public GenericResponse<List<QrCodeSummaryModelDto>> GetQrCodes(int naatiNumber)
        {
            var person = NHibernateSession.Current.Query<Person>().FirstOrDefault(x => x.Entity.NaatiNumber == naatiNumber);

            return GetQrCodeSummary(person.Id);
        }

        public GenericResponse<bool> ToggleQrCode(Guid qrCode)
        {
            var qrCodeRow = NHibernateSession.Current.Query<CredentialQrCode>().FirstOrDefault(x => x.QrCodeGuid == qrCode);
            if(qrCodeRow == null)
            {
                return false;
            }

            if (qrCodeRow.InactiveDate == null)
            {
                qrCodeRow.InactiveDate = DateTime.Now;
                qrCodeRow.ModifiedDate = DateTime.Now;

                NHibernateSession.Current.Flush();
                return true;
            }

            qrCodeRow.InactiveDate = null;
            NHibernateSession.Current.Flush();
            return true;
        }

        public class AddNewSuburbRequest
        {
            public string Postcode { get; set; }
            public string State { get; set; }
            public string SuburbName { get; set; }
        }

        public class PersonalDetailsAddNewSuburbResponse : Postcode
        {
            public bool Success { get; set; }
            public int PostcodeId { get; set; }
        }
    }
}