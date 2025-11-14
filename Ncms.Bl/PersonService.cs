using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using AutoMapper;
using F1Solutions.Global.Common;
using F1Solutions.Global.Common.Logging;
using F1Solutions.Global.Common.Mapping;
using F1Solutions.Naati.Common.Bl;
using F1Solutions.Naati.Common.Contracts.Bl;
using F1Solutions.Naati.Common.Contracts.Bl.DTO;
using F1Solutions.Naati.Common.Contracts.Dal;
using F1Solutions.Naati.Common.Contracts.Dal.DTO;
using F1Solutions.Naati.Common.Contracts.Dal.Enum;
using F1Solutions.Naati.Common.Contracts.Dal.QueryServices;
using F1Solutions.Naati.Common.Contracts.Dal.Request;
using F1Solutions.Naati.Common.Contracts.Dal.Response;
using F1Solutions.Naati.Common.Contracts.Dal.Services;
using F1Solutions.Naati.Common.Contracts.Security;
using F1Solutions.Naati.Common.Dal;
using F1Solutions.Naati.Common.Dal.Domain;
using F1Solutions.Naati.Common.Dal.Portal.Repositories;
using Ncms.Bl.Mappers;
using Ncms.Contracts;
using Ncms.Contracts.Models;
using Ncms.Contracts.Models.Audit;
using Ncms.Contracts.Models.Application;
using Ncms.Contracts.Models.Logbook;
using Ncms.Contracts.Models.Person;
using ISystemService = Ncms.Contracts.ISystemService;
using IUserService = Ncms.Contracts.IUserService;
using SearchRequest = F1Solutions.Naati.Common.Contracts.Dal.Request.SearchRequest;

namespace Ncms.Bl
{
    public class PersonService : Contracts.IPersonService
    {
        // private readonly Search _search;
        private readonly IPersonQueryService _personQueryService;
        private readonly ISystemService _systemService;
        private readonly IUserService _userService;
        private readonly IPersonRepository _personRepository;
        //private readonly IAuditLoggingService _auditLoggingService;

        private readonly ILogbookQueryService _logbookQueryService;
        private readonly IApplicationBusinessLogicService _applicationBusinessLogicService;
        private readonly IActivityPointsCalculatorService _activityPointsCalculatorService;
        private readonly IInstitutionQueryService _institutionQueryService;
        private readonly IFinanceService _financeService;
        private readonly IFileStorageService _fileService;
        private readonly IExaminerToolsService _examinerToolsService;
        private readonly IMyNaatiIntegrationService _myNaatiIntegrationService;
        private readonly IFileCompressionHelper _fileCompressionHelper;
        private readonly ISharedAccessSignature _sharedAccessSignature;
        private readonly IAutoMapperHelper _autoMapperHelper;

        public PersonService(
            IPersonQueryService personQueryService,
            ISystemService systemService,
            IUserService userService,
            IPersonRepository personRepository,
            //IAuditLoggingService auditLoggingService,

            ILogbookQueryService logbookQueryService,
            IApplicationBusinessLogicService applicationBusinessLogicService,
            IActivityPointsCalculatorService activityPointsCalculatorService,
            IInstitutionQueryService institutionQueryService,
            IFinanceService financeService,
            IFileStorageService fileService, 
            IExaminerToolsService examinerToolsService, 
            IMyNaatiIntegrationService myNaatiIntegrationService, 
            IFileCompressionHelper fileCompressionHelper, 
            ISharedAccessSignature sharedAccessSignature, IAutoMapperHelper autoMapperHelper)
        {
            _personQueryService = personQueryService;
            _systemService = systemService;
            _userService = userService;
            _logbookQueryService = logbookQueryService;
            _applicationBusinessLogicService = applicationBusinessLogicService;
            _activityPointsCalculatorService = activityPointsCalculatorService;
            _institutionQueryService = institutionQueryService;
            _financeService = financeService;
            _personRepository = personRepository;
            //_auditLoggingService = auditLoggingService;

            _fileService = fileService;
            _examinerToolsService = examinerToolsService;
            _myNaatiIntegrationService = myNaatiIntegrationService;
            _fileCompressionHelper = fileCompressionHelper;
            _sharedAccessSignature = sharedAccessSignature;
            _autoMapperHelper = autoMapperHelper;
        }

        public GenericResponse<bool> SoftDeletePerson(DeletePersonRequestModel request)
        {
            // 1. Authorization Check
            if (request == null || request.PersonId <= 0 || string.IsNullOrWhiteSpace(request.DeletedBy))
            {
                
                return new GenericResponse<bool>
                {
                    Success = false,
                    Errors = new List<string> { "Invalid request: Person ID and deleted by user must be provided." }
                };
            }

            // 2. Load the Person Entity
            Person person = _personRepository.FindByPersonId(request.PersonId);

            if (person == null)
            {
                
                return new GenericResponse<bool>
                {
                    Success = false,
                    Errors = new List<string> { $"Person with ID {request.PersonId} not found." }
                };
            }

            try
            {
                // 3. Perform Soft Delete (Domain Layer Logic)
                _personRepository.SoftDelete(person, request.DeletedBy);

                // 4. AUDIT LOGGING (BR-03)
                // _auditLoggingService.LogAction(...); // Skipped for now

        
                return new GenericResponse<bool>
                {
                    Success = true,
                    Data = true,
                    Warnings = new List<string> { $"Profile with ID {request.PersonId} successfully soft deleted." }
                };
            }
            catch (Exception ex)
            {
                // Log exception
                //_logger.Error(ex, "Error during soft deletion of Person ID {PersonId}", request.PersonId);


                return new GenericResponse<bool>
                {
                    Success = false,
                    Errors = new List<string> { $"An error occurred during deletion: {ex.Message}" }
                };
            }
        }

        public GenericResponse<IEnumerable<EntitySearchResultModel>> PersonSearch(QueryRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Term))
            {
                throw new Exception("You must enter the term to be searched");
            }

            GetPersonSearchRequest searchRequest = new GetPersonSearchRequest
            {
                Skip = 0,
                Take = 10,
                Filters = new List<PersonSearchCriteria>() { new PersonSearchCriteria
                {
                    Filter = PersonFilterType.AnythingString, Values = new List<string>{ request.Term.Replace("%", string.Empty)}
                } }
            };

            var invoicePrefix = ConfigurationManager.AppSettings["InvoicePrefix"];
            if (!string.IsNullOrWhiteSpace(request.Term) && request.Term.StartsWith(invoicePrefix) &&
                request.Term.Length > invoicePrefix.Length)
            {
                var naatiNumbers = _financeService.GetInvoices(new GetInvoicesRequest { InvoiceNumber = new[] { request.Term } })
                    .Invoices.Where(x => x.NaatiNumber.HasValue).Select(y => y.NaatiNumber.GetValueOrDefault().ToString());
                searchRequest.Filters = new List<PersonSearchCriteria>()
                {
                    new PersonSearchCriteria
                    {
                        Filter = PersonFilterType.NaatiNumberIntList,
                        Values = naatiNumbers.Concat(new []{0.ToString()})
                    }
                };

            }
            var reponse = _personQueryService.SearchPerson(searchRequest);
            var models = reponse.Results.Select(MapPersonResultModel);
            return new GenericResponse<IEnumerable<EntitySearchResultModel>> { Data = models };
        }

        private EntitySearchResultModel MapPersonResultModel(PersonSearchDto searchDto)
        {
            return new EntitySearchResultModel
            {
                EntityId = searchDto.EntityId,
                NaatiNumber = searchDto.NaatiNumber,
                NaatiNumberDisplay = searchDto.NaatiNumber.ToString(),
                EntityTypeId = 3,
                Name = searchDto.Name,
                PersonId = searchDto.PersonId
            };

        }

        public GenericResponse<IEnumerable<PersonResultModel>> Search(PersonSearchRequest request)
        {
            var getRequest = new GetPersonSearchRequest
            {
                Skip = request.Skip,
                Take = request.Take,
                Filters = request.Filter.ToFilterList<PersonSearchCriteria, PersonFilterType>()
            };
            PersonSearchResponse serviceReponse = null;
            serviceReponse = _personQueryService.SearchPerson(getRequest);

            var models = serviceReponse.Results.Select(x => new PersonResultModel
            {
                Id = x.PersonId,
                NaatiNumber = x.NaatiNumber,
                PrimaryEmail = x.PrimaryEmail,
                Name = x.Name,
                PrimaryContactNumber = x.PrimaryContactNumber,
                PractitionerNumber = x.PractitionerNumber,
                PersonTypes = string.Join(", ", x.PersonTypes.Select(y => y.ToString()))
            }).ToList();

            var response = new GenericResponse<IEnumerable<PersonResultModel>>(models);

            if (request.Take.HasValue && models.Count == request.Take.Value)
            {
                response.Warnings.Add($"Search result were limited to {request.Take.Value} records.");
            }

            if (request.Skip.HasValue)
            {
                response.Warnings.Add($"First {request.Skip.Value} records were skipped.");
            }

            return response;
        }

        public GenericResponse<PersonSummaryModel> GetPersonSummary(int naatiNumber)
        {
            var response = _personQueryService.GetPersonDetails(new GetPersonDetailsRequest { NaatiNumber = naatiNumber });
            var person = response.Results.First();
            var model = new PersonSummaryModel
            {
                BirthDate = person.BirthDate,
                EportalRegistrationDate = person.WebAccountCreateDate,
                GivenName = person.GivenName,
                HasPhoto = person.HasPhoto,
                IsEportalActive = person.IsEportalActive,
                NaatiNumber = person.NaatiNumber.GetValueOrDefault(),
                PersonId = person.PersonId,
                PersonPractitionerNumber = person.PractitionerNumber,
                SurName = person.Surname
            };
            return model;
        }

        public GenericResponse<bool> UnlockMyNaatiUser(string username)
        {
            var response = _myNaatiIntegrationService.UnlockUser(username);
            return response;
        }

        public GenericResponse<bool> DeleteMyNaatiUser(string username)
        {
            var response = _myNaatiIntegrationService.DeleteUser(username);
            return response;
        }

        public GenericResponse<bool> DeleteMfaAccount(int naatiNumber)
        {
            var response = _personQueryService.SetPersonMfaDetails(new PersonMfaRequest()
            {
                MfaCode = null,
                MfaExpireStartDate = null,
                NaatiNumber = naatiNumber,
                Disable = true
            });
            return response.Success;
        }

        public GenericResponse<IEnumerable<PersonCredentialRequestModel>> GetPersonCredentialRequests(int personId)
        {
            PersonCredentialRequestsResponse serviceReponse = null;
            serviceReponse = _personQueryService.GetPersonCredentialRequests(personId);

            return new GenericResponse<IEnumerable<PersonCredentialRequestModel>>(serviceReponse.Results.Select(x => new PersonCredentialRequestModel
            {
                PersonId = x.PersonId,
                CredentialType = x.CredentialType,
                Direction = x.Direction,
                CredentialStatus = x.CredentialStatus
            }));
        }

        private IEnumerable<EntitySearchResultModel> NaatiEntitySearch(SearchRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Term))
            {
                throw new Exception("You must enter the term to be searched");
            }

            SearchResponse serviceResponse = _personQueryService.Search(request);

            return serviceResponse.Results.Select(r => new EntitySearchResultModel
            {
                EntityId = r.EntityId,
                NaatiNumber = r.NaatiNumber,
                Name = r.Name,
                PersonId = r.PersonId,
                InstitutionId = r.InstitutionId,
                PrimaryEmail = r.PrimaryEmail
            });
        }
        
        public GenericResponse<bool> CreatePerson()
        {
            // this method needs to be completed
            return true;
        }

        public GenericResponse<IEnumerable<EntitySearchResultModel>> NaatiEntitySearch(EntitySearchRequest request)
        {
            var serviceRequest = _autoMapperHelper.Mapper.Map<SearchRequest>(request);
            var results = NaatiEntitySearch(serviceRequest);
            return new GenericResponse<IEnumerable<EntitySearchResultModel>>
            {
                Data = results
            };
        }

        public GenericResponse<int> CreatePerson(CreatePersonModel request)
        {

            var validatedPerson = CheckPerson(new PersonCheckModel
            {
                CountryOfBirthId = request.CountryOfBirth,
                GivenName = request.GivenName,
                OtherNames = request.OtherNames,
                DateOfBirth = request.DateOfBirth,
                FamilyName = request.FamilyName,
                PrimaryEmail = request.PrimaryEmail,
                Gender = request.Gender

            }).Data;

            if (validatedPerson.Any())
            {
                throw new UserFriendlySamException(Naati.Resources.Person.PersonAlreadyExists);
            }

            CreatePersonResponse serviceResponse = null;

            serviceResponse = _personQueryService.CreatePerson(new CreatePersonRequest
            {
                BirthCountryId = request.CountryOfBirth,
                DateOfBirth = request.DateOfBirth,
                SurName = request.FamilyName,
                GivenName = request.GivenName,
                OtherNames = request.OtherNames,
                PrimaryEmail = request.PrimaryEmail,
                Gender = request.Gender,
                AllowAutoRecertification = true
            });

            if (!string.IsNullOrWhiteSpace(serviceResponse.ErrorMessage))
            {
                throw new UserFriendlySamException(serviceResponse.ErrorMessage);
            }

            return serviceResponse.NaatiNumber;
        }

        public GenericResponse<IEnumerable<PersonCheckModel>> CheckPerson(PersonCheckModel request)
        {
            CheckPersonResponse serviceResponse = null;
            serviceResponse = _personQueryService.CheckPerson(new CreatePersonRequest
            {
                BirthCountryId = request.CountryOfBirthId,
                DateOfBirth = request.DateOfBirth,
                SurName = request.FamilyName,
                GivenName = request.GivenName,
                OtherNames = request.OtherNames,
                PrimaryEmail = request.PrimaryEmail,
                Gender = request.Gender,
                AllowAutoRecertification = true
            });

            if (!string.IsNullOrWhiteSpace(serviceResponse.ErrorMessage))
            {
                throw new UserFriendlySamException(serviceResponse.ErrorMessage);
            }
            if (serviceResponse.Results != null)
            {
                return new GenericResponse<IEnumerable<PersonCheckModel>>(serviceResponse.Results.Select(x => new PersonCheckModel
                {
                    GivenName = x.GivenName,
                    OtherNames = x.OtherNames,
                    FamilyName = x.SurName,
                    PrimaryEmail = x.PrimaryEmail,
                    CountryOfBirth = x.BirthCountry,
                    DateOfBirth = x.DateOfBirth
                }));
            }

            return new GenericResponse<IEnumerable<PersonCheckModel>>(new List<PersonCheckModel>());
        }
        
        public GenericResponse<EntitySearchResultModel> GetEntity(int naatiNumber)
        {
            var users = NaatiEntitySearch(new SearchRequest { Term = naatiNumber.ToString() });

            var response = new GenericResponse<EntitySearchResultModel>();
            if (users == null || !users.Any())
            {
                response.Warnings.Add($"No entity was found for NAATI number {naatiNumber}.");
            }
            else
            {
                response.Data = users.First();
            }

            return response;
        }


        public GenericResponse<IEnumerable<EntitySearchResultModel>> InstitutionSearch(QueryRequest request)
        {
            var searchRequest = new GetInstituteSearchRequest
            {
                Skip = 0,
                Take = 100,
                Filters = new List<InstituteSearchCriteria>() { new InstituteSearchCriteria
                {
                    Filter = InstituteFilterType.AnythingString, Values = new List<string>{ request.Term }
                } }
            };
            var response = _institutionQueryService.SearchInstitute(searchRequest);

            var models = response.Results.Select(MapInstitutionModelo);
            return new GenericResponse<IEnumerable<EntitySearchResultModel>> { Data = models };
        }

        private EntitySearchResultModel MapInstitutionModelo(InstituteSearchDto searchDto)
        {
            return new EntitySearchResultModel
            {
                EntityId = searchDto.EntityId,
                NaatiNumber = searchDto.NaatiNumber,
                NaatiNumberDisplay = searchDto.NaatiNumber.ToString(),
                EntityTypeId = 1,
                Name = searchDto.Name,
                InstitutionId = searchDto.InstituteId
            };
        }

        public GenericResponse<IEnumerable<EntitySearchResultModel>> PersonAndAndInstitutionSearch(QueryRequest query)
        {
            var result = new List<EntitySearchResultModel>();
            var persons = PersonSearch(query);
            var response = new GenericResponse<IEnumerable<EntitySearchResultModel>>();

            if (!persons.Success)
            {
                response.Success = false;
                response.Errors.AddRange(persons.Errors);
            }
            else if (persons.Data != null)
            {
                result.AddRange(persons.Data);
            }

            var institutions = InstitutionSearch(new QueryRequest { Term = query.Term });
            if (!institutions.Success)
            {
                response.Success = false;
                response.Errors.AddRange(institutions.Errors);
            }
            else if (institutions.Data != null)
            {
                result.AddRange(institutions.Data);
            }

            response.Data = result;
            return response;
        }

        public GenericResponse<PersonModel> GetPerson(int naatiNumber)
        {
            GetPersonDetailsResponse serviceResponse = null;
            serviceResponse = _personQueryService.GetPersonDetails(new GetPersonDetailsRequest { NaatiNumber = naatiNumber });

            var response = new GenericResponse<PersonModel>();
            if (serviceResponse.Results == null || !serviceResponse.Results.Any())
            {
                response.Warnings.Add($"No person was found for NAATI number {naatiNumber}.");
            }
            else
            {
                
                response.Data = _autoMapperHelper.Mapper.Map<PersonModel>(serviceResponse.Results.First());
                response.Data.ContactDetails = GetContactDetails(response.Data.EntityId);
                response.Data.NameHistory = GetNameHistory(response.Data.PersonId).ToList();

                var rolePlayerSettings = _examinerToolsService.GetRolePlayerSettings(new GetRolePlayerSettingsRequest() { NaatiNumber = naatiNumber }).Settings ?? new RolePlayerSettingsDto(){ RolePlayLocations = new int[0]};
                response.Data.RolePlayLocations = rolePlayerSettings.RolePlayLocations;

                response.Data.Rating = rolePlayerSettings.Rating;
                response.Data.MaximumRolePlaySessions = rolePlayerSettings.MaximumRolePlaySessions;
                response.Data.Senior = rolePlayerSettings.Senior;
                response.Data.ShowAllowAutoRecertification = response.Data.IsPractitioner || response.Data.IsFormerPractitioner || response.Data.IsFuturePractitioner;

                var result = serviceResponse.Results.First();
                response.Data.MfaModeIsSet = !string.IsNullOrEmpty(result.MfaCode);
            }
            return response;
        }

        public GenericResponse<PersonModel> GetLogBook(int naatiNumber, bool showAllCredential = true)
        {
            var serviceResponse = _personQueryService.GetPersonDetails(new GetPersonDetailsRequest { NaatiNumber = naatiNumber });

            var response = new GenericResponse<PersonModel>();
            if (serviceResponse.Results == null || !serviceResponse.Results.Any())
            {
                response.Warnings.Add($"No person was found for NAATI number {naatiNumber}.");
            }
            else
            {

                var credentials = _logbookQueryService.GetCredentials(naatiNumber).List;
                var credentialModelList = credentials.Select(_autoMapperHelper.Mapper.Map<CredentialDetailsModel>).ToList();
                credentialModelList.ForEach(x =>
                    x.RecertificationStatus =
                        _applicationBusinessLogicService.CalculateCredentialRecertificationStatus(x.Id).ToString());

                if (!showAllCredential)
                {
                    var statusList = new[] { CredentialStatusTypeName.Active.ToString(), CredentialStatusTypeName.Future.ToString() };
                    var reStatusList = new[]
                    {
                        RecertificationStatus.BeingAssessed.ToString(), RecertificationStatus.EligibleForExisting.ToString(),
                        RecertificationStatus.EligibleForNew.ToString()
                    };
                    credentialModelList = credentialModelList.Where(x =>
                        statusList.Contains(x.CredentialStatus) || reStatusList.Contains(x.RecertificationStatus)).ToList();
                }
                response.Data = _autoMapperHelper.Mapper.Map<PersonModel>(serviceResponse.Results.First());
                response.Data.Credentials = credentialModelList;
            }
            return response;
        }

        public GenericResponse<IList<CredentialModel>> GetPersonCredentials(int naatiNumber, bool checkSkillAvailability)
        {
            var credentials = _personQueryService.GetPersonCredentials(naatiNumber).Data;
            var models = credentials.Select(_autoMapperHelper.Mapper.Map<CredentialModel>).ToList();
            models.ForEach(x => x.RecertificationStatus = _applicationBusinessLogicService.CalculateCredentialRecertificationStatus(x.Id, checkSkillAvailability));
            return new GenericResponse<IList<CredentialModel>>(models);
        }

        public GenericResponse<PdActivityPoints> GetProfessionalDevelopmentActivityPoints(int naatiNumber, int certificationPeriodId)
        {
            return _activityPointsCalculatorService.CaluculatePointsFor(naatiNumber, certificationPeriodId);
        }

        public GenericResponse<IEnumerable<CertificationPeriodCredentialRequests>> GetCertificationPeriodsRequests(int naatiNumber)
        {
            var certificationPeriods = GetCertificationPeriodDetails(naatiNumber).Data;
            var applicationIds = certificationPeriods.Where(x => x.SubmittedRecertificationApplicationId.HasValue)
                .Select(y => y.SubmittedRecertificationApplicationId.GetValueOrDefault());

            var results = _logbookQueryService.GetSubmittedRecertificationCredentialRequests(
                    new GetCerfiationPeriodRequest() { ApplicationIds = applicationIds.ToArray() })
                .GroupBy(y => y.CertificationPeriodId)
                .Select(z => new CertificationPeriodCredentialRequests
                {
                    CertificationPeriodId = z.Key,
                    Requests = z.Select(w => new CertificationPeriodCredentialRequest { ExternalName = w.ExternalName, Skill = w.Skill })

                });

            return new GenericResponse<IEnumerable<CertificationPeriodCredentialRequests>>(results);
        }

        public GenericResponse<IEnumerable<CertificationPeriodDetailsDto>> GetCertificationPeriodDetails(int naatiNumber)
        {
            return new GenericResponse<IEnumerable<CertificationPeriodDetailsDto>>(_activityPointsCalculatorService.GetCertificationPeriodDetails(naatiNumber));
        }

        public GenericResponse<IEnumerable<ProfessionalDevelopmentRequirementResponse>> GetProfessionalDevelopmentRequirements(int categoryId)
        {
            if (categoryId <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(categoryId));
            }

            return new GenericResponse<IEnumerable<ProfessionalDevelopmentRequirementResponse>>(_logbookQueryService.GetProfessionalDevelopmentRequirements(categoryId));
        }

        public GenericResponse<IEnumerable<ProfessionalDevelopmentCategoryResponse>> GetProfessionalDevelopmentCategories()
        {
            return new GenericResponse<IEnumerable<ProfessionalDevelopmentCategoryResponse>>(_logbookQueryService.GetProfessionalDevelopmentCategories());
        }

    

        private List<PersonSearchDto> GetPeople(IEnumerable<PersonSearchCriteria> filters)
        {
            var searchRequest = new GetPersonSearchRequest
            {
                Filters = filters,
                Skip = 0,
                Take = 20,
            };

            return _personQueryService.SearchPerson(searchRequest).Results.ToList();
        }


        public GenericResponse<PersonBasicModel> GetPersonBasic(int naatiNumber)
        {
            var serviceResponse = _personQueryService.GetPersonDetailsBasic(new GetPersonDetailsRequest { NaatiNumber = naatiNumber });

            return serviceResponse.ConvertServiceResponse(x => _autoMapperHelper.Mapper.Map<PersonBasicModel>(x.PersonDetails));
        }

        public void UpdateDetails(PersonModel person)
        {
            var request = _autoMapperHelper.Mapper.Map<UpdatePersonDetailsRequest>(person);
            if (request.BirthDate < MinDate.Value)
            {
                throw new UserFriendlySamException("Birth date is invalid.");
            }

            _personQueryService.UpdatePersonDetails(request);
        }

        public void UpdateSettings(PersonModel person)
        {
            var request = _autoMapperHelper.Mapper.Map<UpdatePersonSettingsRequest>(person);

            if (person.IsRolePlayer)
            {
                request.RolePlayerSettingsRequest = new RolePlayerSettingsRequest
                {
                    Settings = new RolePlayerSettingsDto() {   RolePlayLocations = person.RolePlayLocations, MaximumRolePlaySessions =  person.MaximumRolePlaySessions, Rating = person.Rating, Senior = person.Senior, NaatiNumber = person.NaatiNumber.GetValueOrDefault()}
                };
            }


            if (person.Deceased.HasValue && person.Deceased.Value)
            {
                var personResponse = _personQueryService.GetPersonDetailsBasic(new GetPersonDetailsRequest { EntityId =person.EntityId });
                if (personResponse != null && personResponse.PersonDetails != null && personResponse.PersonDetails.IsMyNaatiRegistered)
                {
                    //lastly only delete if the user has permission PersonMyNaatiRegistration.Delete
                    if (_userService.HasPermission(SecurityNounName.PersonMyNaatiRegistration, SecurityVerbName.Delete))
                    {
                        DeleteMyNaatiUser(personResponse.PersonDetails.PrimaryEmail);
                    }
                }
            }


            _personQueryService.UpdatePersonSettings(request);
        }

        public ContactDetailsModel GetContactDetails(int entityId)
        {
            GetContactDetailsResponse response = null;
            response = _personQueryService.GetPersonContactDetails(new GetContactDetailsRequest { EntityId = entityId });

            if (response == null)
            {
                return new ContactDetailsModel();
            }

            return new ContactDetailsModel
            {
                Addresses = MapAddresses(response.Addresses),
                Phones = MapPhones(response.Phones),
                Emails = MapEmails(response.Emails),
                Websites = MapWebSites(response.Websites),
                ShowWebsite = response.ShowWebsite,
                IsMyNaatiRegistered = response.IsMyNaatiRegistered
            };



            //return new ContactDetailsModel
            //{
            //    Addresses = response.Addresses.Where(x => !x.Invalid).Select(AutoMapper.Mapper.DynamicMap<AddressModel>),
            //    Phones = response.Phones.Where(x => !x.Invalid).Select(AutoMapper.Mapper.DynamicMap<PhoneModel>),
            //    Emails = response.Emails.Where(x => !x.Invalid).Select(AutoMapper.Mapper.DynamicMap<EmailModel>),
            //    Websites = response.Websites.Select(AutoMapper.Mapper.DynamicMap<WebsiteModel>),
            //    ShowWebsite = response.ShowWebsite,
            //    IsMyNaatiRegistered = response.IsMyNaatiRegistered
            //};
        }

        private IEnumerable<WebsiteModel> MapWebSites(IEnumerable<WebsiteDetailsDto> websitesIn)
        {
            var webSitesOut = new List<WebsiteModel>();
            foreach(var webSite in websitesIn)
            {
                webSitesOut.Add(new WebsiteModel()
                {
                    EntityId = webSite.EntityId,
                    IncludeInPd = webSite.IncludeInPd,
                    Url = webSite.Url
                });
            }
            return webSitesOut;
        }

        private IEnumerable<EmailModel> MapEmails(IEnumerable<EmailDetailsDto> emailsIn)
        {
            var emailsOut = new List<EmailModel>();
            foreach(var emailIn in emailsIn.Where(x => !x.Invalid))
            {
                emailsOut.Add(new EmailModel()
                {
                    Email = emailIn.Email,
                    EmailId = emailIn.EmailId,
                    EntityId = emailIn.EntityId,
                    ExaminerCorrespondence = emailIn.ExaminerCorrespondence,
                    IncludeInPd = emailIn.IncludeInPd,
                    IsExaminer = emailIn.IsExaminer,
                    IsPreferredEmail = emailIn.IsPreferredEmail,
                    Note = emailIn.Note,
                    //ShowExaminer = no mapping
                });
            }
            return emailsOut;
        }

        private IEnumerable<PhoneModel> MapPhones(IEnumerable<PhoneDetailsDto> phonesIn)
        {
            var phonesOut = new List<PhoneModel>();
            foreach(var phoneIn in phonesIn.Where(x=>!x.Invalid))
            {
                phonesOut.Add(new PhoneModel()
                {
                    AllowSmsNotification = phoneIn.AllowSmsNotification,
                    EntityId = phoneIn.EntityId,
                    ExaminerCorrespondence = phoneIn.ExaminerCorrespondence,
                    IncludeInPd = phoneIn.IncludeInPd,
                    IsExaminer = phoneIn.IsExaminer,
                    LocalNumber = phoneIn.LocalNumber,
                    Note = phoneIn.Note,
                    PhoneId = phoneIn.PhoneId,
                    PrimaryContact = phoneIn.PrimaryContact
                });
            }
            return phonesOut;
        }

        private IEnumerable<AddressModel> MapAddresses(IEnumerable<AddressDetailsDto> addressesIn)
        {
            var addressesOut = new List<AddressModel>();
            foreach(var addressIn in addressesIn.Where(x => !x.Invalid))
            {
                addressesOut.Add(new AddressModel()
                {
                    AddressId = addressIn.AddressId,
                    CountryId = addressIn.CountryId,
                    CountryName = addressIn.CountryName,
                    EntityId = addressIn.EntityId,
                    ExaminerCorrespondence = addressIn.ExaminerCorrespondence,
                    IsExaminer = addressIn.IsExaminer,
                    IsOrganisation = addressIn.IsOrganisation,
                    Note = addressIn.Note,
                    OdAddressVisibilityTypeId = addressIn.OdAddressVisibilityTypeId,
                    OdAddressVisibilityTypeName = addressIn.OdAddressVisibilityTypeName,
                    Postcode = addressIn.Postcode,
                    PostcodeId = addressIn.PostcodeId,
                    PrimaryContact = addressIn.PrimaryContact,
                    StateAbbreviation = addressIn.StateAbbreviation,
                    StreetDetails = addressIn.StreetDetails,
                    Suburb = addressIn.Suburb,
                    SuburbId = addressIn.SuburbId,
                    SuburbName = addressIn.SuburbName,
                    ValidateInExternalTool = addressIn.ValidateInExternalTool

                });
            }
            return addressesOut;
        }

        public IEnumerable<PersonNameModel> GetNameHistory(int personId)
        {
            GetPersonNameResponse response = null;
            response = _personQueryService.GetPersonName(new GetPersonNameRequest { PersonId = personId });

            if (response == null)
            {
                return Enumerable.Empty<PersonNameModel>();
            }

            return response.Names.Select(n => _autoMapperHelper.Mapper.Map<PersonNameModel>(n));
        }
        public GenericResponse<bool> CheckExaminerRole(int entityId)
        {
            var result = _personQueryService.HasExminerRoleByEntityId(new GetExminerRoleFlagRequest { EntityId = entityId }).HasExaminerRole;
            return result;
        }

        public GetPersonAndContactDetailsResponse GetPersonInfoResponse(int naatiNumber)
        {

            var response = new GetPersonAndContactDetailsResponse();
            List<PersonEntityDto> personEntityList = new List<PersonEntityDto>();

            response.PersonDetails = new GetPersonDetailsResponse();
            response.ContactDetails = new GetContactDetailsResponse();

            var personData = GetPerson(naatiNumber).Data;
            var personEntityDto = new PersonEntityDto
            {
                Title = personData.Title,
                GivenName = personData.GivenName,
                OtherNames = personData.OtherNames,
                Surname = personData.Surname,
                Abn = personData.Abn,
                AllowVerifyOnline = personData.AllowVerifyOnline,
                BirthCountryId = personData.BirthCountryId,
                BirthDate = personData.BirthDate,
                WebAccountCreateDate = personData.WebAccountCreateDate,
                Deceased = personData.Deceased != null,
                DoNotInviteToDirectory = personData.DoNotInviteToDirectory,
                IsEportalActive = personData.IsEportalActive,
                DoNotSendCorrespondence = personData.DoNotSendCorrespondence,
                EntityId = personData.EntityId,
                EntityTypeId = personData.EntityTypeId,
                Gender = personData.Gender,
                GstApplies = personData.GstApplies,
                HasPhoto = personData.HasPhoto,
                PhotoDate = personData.PhotoDate,
                NaatiNumber = personData.NaatiNumber,
                NaatiNumberDisplay = personData.NaatiNumberDisplay,
                PractitionerNumber = personData.PractitionerNumber,
                Name = personData.Name,
                PersonId = personData.PersonId,
                ReleaseDetails = personData.ReleaseDetails,
                RevalidationScheme = personData.RevalidationScheme,
                ShowPhotoOnline = personData.ShowPhotoOnline,
                InterculturalCompetency = personData.InterculturalCompetency,
                EthicalCompetency = personData.EthicalCompetency,
                UseEmail = personData.UseEmail,
                AccountNumber = personData.AccountNumber,
                ExaminerTrackingCategory = personData.ExaminerTrackingCategory,
                IsFormerPractitioner = personData.IsFormerPractitioner,
                IsPractitioner = personData.IsPractitioner,
                IsApplicant = personData.IsApplicant,
                IsExaminer = personData.IsExaminer,
                IsFuturePractitioner = personData.IsFuturePractitioner,
                MaxCertificationPeriodEndDate = personData.MaxCertificationPeriodEndDate,

                PostcodeId = personData.PostcodeId,
                StateId = personData.StateId,
                Email = personData.Email,
                EmailId = personData.EmailId,
                Number = personData.Number,
                PersonAddress = personData.PersonAddress,
                StateName = personData.StateName,
                SecondaryEmail = personData.SecondaryEmail,
                SecondaryAddress = personData.SecondaryAddress,
                SecondaryContactNumber = personData.SecondaryContactNumber,
                InOdEmails = personData.InOdEmails,
                InOdPhones = personData.InOdPhones,
                InOdAddresses = personData.InOdAddresses
            };

            personEntityList.Add(personEntityDto);

            response.PersonDetails.Results = personEntityList;

            GetContactDetailsResponse contactDetailsResponse = new GetContactDetailsResponse
            {
                Addresses = personData.ContactDetails.Addresses.Select(_autoMapperHelper.Mapper.Map<AddressDetailsDto>),
                Phones = personData.ContactDetails.Phones.Select(_autoMapperHelper.Mapper.Map<PhoneDetailsDto>),
                Emails = personData.ContactDetails.Emails.Select(_autoMapperHelper.Mapper.Map<EmailDetailsDto>),
                Websites = personData.ContactDetails.Websites.Select(_autoMapperHelper.Mapper.Map<WebsiteDetailsDto>),
                ShowWebsite = personData.ContactDetails.ShowWebsite
            };

            response.ContactDetails = contactDetailsResponse;

            return response;
        }

        public AddressModel GetAddress(int entityId, int addressId)
        {
            var addressModel = _personQueryService.GetPersonAddress(new GetAddressRequest { EntityId = entityId, AddressId = addressId });

            if (addressModel?.Address == null)
            {
                throw new UserFriendlySamException("Address record not found or please refresh the browser.");
            }

            addressModel.Address.IsExaminer = _personQueryService.HasExminerRoleByEntityId(new GetExminerRoleFlagRequest { EntityId = entityId }).HasExaminerRole;

            return _autoMapperHelper.Mapper.Map<AddressModel>(addressModel.Address);
        }

        public PhoneModel GetPhone(int entityId, int phoneId)
        {
            var phoneModel = _personQueryService.GetPersonPhone(new GetPhoneRequest { EntityId = entityId, PhoneId = phoneId });
            phoneModel.Phone.IsExaminer = _personQueryService.HasExminerRoleByEntityId(new GetExminerRoleFlagRequest { EntityId = entityId }).HasExaminerRole;

            if (phoneModel?.Phone == null)
            {
                throw new UserFriendlySamException("Phone record not found.");
            }

            return _autoMapperHelper.Mapper.Map<PhoneModel>(phoneModel.Phone);
        }

        public PhoneModel GetPersonPrimaryPhone(int entityId)
        {
            var response =
                _personQueryService.GetPersonPhones(new GetPhonesRequest() { EntityId = entityId })
                    .Phones.SingleOrDefault(x => x.PrimaryContact);
            return _autoMapperHelper.Mapper.Map<PhoneModel>(response);
        }

        public EmailModel GetEmail(int entityId, int emailId)
        {
            GetPersonEmailResponse response = null;
            response = _personQueryService.GetPersonEmail(new GetEmailRequest { EntityId = entityId, EmailId = emailId });

            if (response == null || response.Email == null)
            {
                throw new UserFriendlySamException("Email record not found.");
            }
            var emailModel = _autoMapperHelper.Mapper.Map<EmailModel>(response.Email);

            GetExminerRoleFlagResponse exminerRoleFlag = null;

            exminerRoleFlag = _personQueryService.HasExminerRoleByEntityId(new GetExminerRoleFlagRequest { EntityId = entityId });


            emailModel.ShowExaminer = exminerRoleFlag.HasExaminerRole;

            return emailModel;
        }

        public WebsiteModel GetWebsite(int entityId)
        {
            GetPersonWebsiteResponse response = null;
            response = _personQueryService.GetPersonWebsite(new GetWebsiteRequest { EntityId = entityId });

            return _autoMapperHelper.Mapper.Map<WebsiteModel>(response.Website);
        }


        public IEnumerable<SuburbModel> GetSuburbs()
        {
            GetSuburbsResponse response = null;
            response = _personQueryService.GetSuburbs();

            if (response == null || response.Suburbs == null || !response.Suburbs.Any())
            {
                return Enumerable.Empty<SuburbModel>();
            }

            return response.Suburbs.Select(_autoMapperHelper.Mapper.Map<SuburbModel>);
        }

        public void UpdateAddress(AddressModel model)
        {
            var request = new UpdatePersonAddressRequest
            {
                Address = _autoMapperHelper.Mapper.Map<AddressDetailsDto>(model)
            };

            UpdateObjectResponse updateResponse = null;

            updateResponse = _personQueryService.UpdatePersonAddress(request);

            model.AddressId = updateResponse.ObjectId;

            // there can be only one Primary (SD-SAM-3001.1 BR4)
            if (request.Address.PrimaryContact)
            {
                var addressesRequest = new GetAddressRequest { EntityId = model.EntityId };
                GetAddressesResponse response = null;

                response = _personQueryService.GetPersonAddresses(addressesRequest);


                if (response != null)
                {
                    foreach (var address in response.Addresses)
                    {
                        if (address.AddressId != model.AddressId && address.PrimaryContact)
                        {
                            address.PrimaryContact = false;
                            request = new UpdatePersonAddressRequest
                            {
                                Address = address
                            };

                            _personQueryService.UpdatePersonAddress(request);
                        }
                    }
                }
            }
        }

        public void UpdatePhone(PhoneModel model)
        {
            var request = new UpdatePersonPhoneRequest
            {
                Phone = _autoMapperHelper.Mapper.Map<PhoneDetailsDto>(model)
            };

            model.PhoneId = _personQueryService.UpdatePersonPhone(request).ObjectId;

            if (request.Phone.PrimaryContact)
            {
                var phonesRequest = new GetPhonesRequest { EntityId = model.EntityId };
                GetPersonPhonesResponse response = null;
                response = _personQueryService.GetPersonPhones(phonesRequest);
                if (response != null)
                {
                    foreach (var phone in response.Phones)
                    {
                        if (!phone.Invalid && phone.PhoneId != model.PhoneId && phone.PrimaryContact)
                        {
                            phone.PrimaryContact = false;
                            request = new UpdatePersonPhoneRequest
                            {
                                Phone = phone
                            };

                            _personQueryService.UpdatePersonPhone(request);
                        }
                    }
                }
            }
        }

        public BusinessServiceResponse UpdateEmail(EmailModel model)
        {
            var response = new BusinessServiceResponse();

            var isAdmin = _userService.HasPermission(
                SecurityNounName.PersonMyNaatiRegistration,
                SecurityVerbName.Update);

            EmailDetailsDto oldPrimary = null;
            EmailDetailsDto oldEmail = null;

            var currentEmails = _personQueryService.GetPersonEmails(
                    new GetEmailRequest
                    {
                        EntityId = model.EntityId
                    })
                ?.Emails.ToList();

            if (model.EmailId.HasValue)
            {
                oldEmail = currentEmails?.Single(x => x.EmailId == model.EmailId.Value);
            }

            var request = new UpdatePersonEmailRequest
            {
                Email = _autoMapperHelper.Mapper.Map<EmailDetailsDto>(model),
                AllowUpdateWhenMyNaatiRegistered = isAdmin
            };

            var ncmsResponse = _personQueryService.UpdatePersonEmail(request);

            if (!string.IsNullOrWhiteSpace(ncmsResponse.ErrorMessage))
            {
                throw new UserFriendlySamException(ncmsResponse.ErrorMessage);
            }

            // there can be only one Primary (SD-SAM-3001.1 BR4)
            if (request.Email.IsPreferredEmail)
            {
                if (currentEmails != null)
                {
                    foreach (var email in currentEmails)
                    {
                        if (!email.Invalid && email.EmailId != model.EmailId && email.IsPreferredEmail)
                        {
                            oldPrimary = email;
                            email.IsPreferredEmail = false;
                            request = new UpdatePersonEmailRequest
                            {
                                Email = email,
                                AllowUpdateWhenMyNaatiRegistered = isAdmin
                            };

                            _personQueryService.UpdatePersonEmail(request);
                        }
                    }
                }
            }

            // if the person is registered in myNAATI, we will need to update their myNAATI username if we changed their primary email
            var personResponse = _personQueryService.GetPersonDetailsBasic(new GetPersonDetailsRequest { EntityId = model.EntityId });

            // an error response most likely indicates this is an Organisation, not a person
            if (!personResponse.Error)
            {
                var primaryEmailChanged = model.IsPreferredEmail && oldEmail != null && !model.Email.Equals(oldEmail.Email, StringComparison.Ordinal);
                var isNewPrimary = model.IsPreferredEmail && oldPrimary != null;

                var myNaatiUsernameUpdateRequired =
                    personResponse.PersonDetails.IsMyNaatiRegistered &&
                    (primaryEmailChanged || isNewPrimary) &&
                    isAdmin;

                if (myNaatiUsernameUpdateRequired)
                {
                    var currentUserName = oldPrimary?.Email ?? oldEmail?.Email;
                    currentUserName.NotNull("Don't know old myNAATI username. Logic error.");

                    try
                    {
                        var myNaatiResponse = _myNaatiIntegrationService.RenameUser(currentUserName, model.Email);

                        if (myNaatiResponse.Success)
                        {
                            response.Messages.Add($"myNAATI username changed from {currentUserName} to {model.Email}.");
                        }
                       
                    }
                    catch (Exception ex)
                    {
                        response.Errors.Add("An error occured while changing the myNAATI username. Please contact Support.");
                        LoggingHelper.LogException(ex, "Error changing myNAATI username from {OldUsername} to {NewUsername}. {Message}", currentUserName, model.Email, ex.Message);
                    }
                }
            }
            model.EmailId = ncmsResponse.ObjectId;

            return response;
        }

        public GetPersonDetailsBasicResponse GetPersonDetailsByEntityId(int entityId)
        {
            return _personQueryService.GetPersonDetailsBasic(new GetPersonDetailsRequest { EntityId = entityId });
        }

        public void UpdateWebsite(WebsiteModel model)
        {
            var request = new UpdatePersonWebsiteRequest
            {
                Website = _autoMapperHelper.Mapper.Map<WebsiteDetailsDto>(model)
            };

            _personQueryService.UpdatePersonWebsite(request);
        }

        public void DeleteAddress(int addressId)
        {
            var request = new DeleteObjectRequest { ObjectId = addressId };
            _personQueryService.DeletePersonAddress(request);
        }

        public DeleteResponseModel DeletePhone(DeleteRequestModel request)
        {
            var serviceRequest = new DeleteObjectRequest
            {
                ObjectId = request.ObjectId,
                FlowAnswers = request.FlowAnswers
            };

            DeleteObjectResponse response = null;
            response = _personQueryService.DeletePersonPhone(serviceRequest);

            return new DeleteResponseModel
            {
                Success = response.Success,
                FlowMessage = response.FlowMessage
            };
        }

        public DeleteResponseModel DeleteEmail(DeleteRequestModel request)
        {
            var serviceRequest = new DeleteObjectRequest
            {
                ObjectId = request.ObjectId,
                FlowAnswers = request.FlowAnswers
            };

            DeleteObjectResponse response = null;
            response = _personQueryService.DeletePersonEmail(serviceRequest);

            return new DeleteResponseModel
            {
                Success = response.Success,
                FlowMessage = response.FlowMessage
            };
        }

        public void DeleteWebsite(int entityId)
        {
            UpdateWebsite(new WebsiteModel
            {
                EntityId = entityId,
                Url = String.Empty
            });
        }

        public void AddName(int naatiNumber, PersonNameModel name)
        {
            var request = new AddNameRequest
            {
                NaatiNumber = naatiNumber,
                Name = _autoMapperHelper.Mapper.Map<PersonNameDto>(name)
            };

            _personQueryService.AddName(request);
        }

        public void AssignPractitionerNumber(int personId, string practitionerNumber)
        {
            if (String.IsNullOrEmpty(practitionerNumber))
            {
                var pnHelper = new PractitionerNumberHelper(_systemService, _personQueryService);
                practitionerNumber = pnHelper.GetNewPractitionerNumber();
            }

            var response = _personQueryService.AssignPractitionerNumber(new AssignPractitionerNumberRequest
            {
                PersonId = personId,
                PractitionerNumber = practitionerNumber
            });
            if (response.Error)
            {
                throw new Exception(response.ErrorMessage);
            }
        }

        public GenericResponse<IEnumerable<CertificationPeriodModel>> GetCertificationPeriods(GetCertificationPeriodsRequestModel request)
        {
            var serviceRequest = _autoMapperHelper.Mapper.Map<GetCertificationPeriodsRequest>(request);
            var response = _personQueryService.GetCertificationPeriods(serviceRequest);
            return new GenericResponse<IEnumerable<CertificationPeriodModel>>
            {
                Data = response.Results.Select(p => _autoMapperHelper.Mapper.Map<CertificationPeriodModel>(p)).ToList()
            };
        }

        public void SetCertificationEndDate(SetCertificationEndDateRequestModel request)
        {
            var serviceRequest = _autoMapperHelper.Mapper.Map<SetCertificationEndDateRequest>(request);
            serviceRequest.UserId = _userService.Get().Id;
            serviceRequest.CertificationPeriodId = request.Id;
            _personQueryService.SetCertificationEndDate(serviceRequest);
        }

        public void SaveCertificationPeriod(CertificationPeriodModel request)
        {
            var serviceRequest = new CertificationPeriodDto
            {
                OriginalEndDate = request.OriginalEndDate,
                CertificationPeriodStatus = request.CertificationPeriodStatus,
                CredentialApplicationId = request.CredentialApplicationId,
                EndDate = request.EndDate,
                Id = request.Id,
                NaatiNumber = request.NaatiNumber,
                Notes = request.Notes,
                StartDate = request.StartDate,
                UserId = _userService.Get().Id
            };
            
            _personQueryService.SaveCertificationPeriod(serviceRequest);
        }

        public CertificationPeriodModel CreateCertificationPeriod(CreateCertificationPeriodModel request)
        {
            var dto = _autoMapperHelper.Mapper.Map<CreateCertificationPeriodRequest>(request);
            var certificationPeriod = _personQueryService.CreateCertificationPeriod(dto);

            return _autoMapperHelper.Mapper.Map<CertificationPeriodModel>(certificationPeriod);
        }

        public void SetCredentialTerminateDate(SetCredentialTerminateDateModel request)
        {
            var serviceRequest = _autoMapperHelper.Mapper.Map<SetCredentialTerminateDateRequest>(request);
            serviceRequest.UserId = _userService.Get().Id;
            serviceRequest.CredentialId = request.Id;
            _personQueryService.SetCredentialTerminateDate(serviceRequest);
        }

        public GenericResponse<IEnumerable<ActivityModel>> GetProfessionalDevelopmentActivities(int naatiNumber, int certificationPeriodId)
        {
            if (naatiNumber <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(naatiNumber));
            }

            var results = _activityPointsCalculatorService.GetAllCertificationPeriodActivities(naatiNumber, certificationPeriodId);
            return new GenericResponse<IEnumerable<ActivityModel>>(results.Select(_autoMapperHelper.Mapper.Map<ActivityModel>));
        }

        public GenericResponse<DateTime?> RecalculateCertificationPeriodStartDate(int periodId)
        {
            return _personQueryService.GetCertificationPeriodEarliestDate(periodId);
        }

        public GenericResponse<CheckCertificationPeriodResult> CheckCertificationPeriod(CertificationPeriodModel request)
        {
            var errors = new List<string>();
            var certificationPeriodRecertify = Convert.ToInt32(_systemService.GetSystemValue("CertificationPeriodRecertify"));

            if (request.OriginalEndDate <= request.StartDate)
            {
                errors.Add("The Original End Date must be after the Start Date.");
            }

            if (request.EndDate < request.OriginalEndDate)
            {
                errors.Add("The End Date must be after or the same as the Original End Date.");
            }

            if (request.StartDate.AddMonths(certificationPeriodRecertify) >= request.OriginalEndDate)
            {
                errors.Add($"The Certification Period must be longer than {certificationPeriodRecertify} months.");
            }

            var minStartDate = request.Credentials.Min(x => x.StartDate);

            if (request.StartDate > minStartDate)
            {
                errors.Add($"The Start Date must be the same as the existing earliest Start Date of the Credentials.");
            }

            var periods = GetCertificationPeriods(new GetCertificationPeriodsRequestModel
            {
                NaatiNumber = request.NaatiNumber,
                CertificationPeriodStatus = new[] {
                    CertificationPeriodStatusModel.Expired,
                    CertificationPeriodStatusModel.Current,
                    CertificationPeriodStatusModel.Future,
                }
            });

            /// TODO: Future
            //var overlaps = periods.Data
            //	.Where(p =>
            //		(p.StartDate <= request.StartDate && request.StartDate <= p.OriginalEndDate) ||
            //		(request.StartDate <= p.StartDate && p.StartDate <= request.OriginalEndDate) ||
            //		(p.StartDate <= request.OriginalEndDate && request.OriginalEndDate <= p.OriginalEndDate) ||
            //		(request.StartDate <= p.OriginalEndDate && p.OriginalEndDate <= request.OriginalEndDate))
            //	.Select(p => p.Id.ToString());

            //if (overlaps.Any())
            //{
            //	errors.Add($"Certification periods must not overlap.  The following certification periods will overlap if this change is applied: {String.Join(",", overlaps)}.");
            //}

            var warnings = new List<string>();
            if (DateTime.Compare(request.OriginalEndDate, request.EndDate) != 0)
            {
                warnings.Add("You should use the Extend Certification screen to provide an extension to a practitioner instead of using this screen.");
            }

            return new CheckCertificationPeriodResult { Warnings = warnings, Errors = errors };
        }

        public GenericResponse<MyNaatiUserDetailsModel> GetMyNaatiDetails(string username)
        {
            var userDetails = _myNaatiIntegrationService.GetUser(username);

            // we had issues with AutoMapper here
            var model = new MyNaatiUserDetailsModel();
            if (userDetails.Data != null)
            {
                model.IsActive = userDetails.Data.IsActive;
                model.CreationDate = userDetails.Data.CreationDate;
                model.IsLocked = userDetails.Data.IsLocked;
                model.LastLoginDate = userDetails.Data.LastLoginDate;
                model.LastPasswordChangedDate = userDetails.Data.LastPasswordChangedDate;
                model.Username = userDetails.Data.Username;
            }

            return model;
        }

        public int CreateOrReplaceAttachment(PersonAttachmentModel request)
        {
            var serviceRequest = _autoMapperHelper.Mapper.Map<CreateOrReplacePersonAttachmentRequest>(request);
           
            var documentTypes = GetAllowedDocumentTypesToUpload();
            CreateOrReplacePersonAttachmentResponse response = null;
            
            if (documentTypes.All(x => x.Id != (int)serviceRequest.Type))
            {
                throw new Exception($"User does not have permissions to upload {request.Type} documents");
            }
            try
            {
                response = _personQueryService.CreateOrReplaceAttachment(serviceRequest);
            }
            catch (WebServiceException e)
            {
                throw new UserFriendlySamException(e.Message);
            }

            return response.StoredFileId;
        }

        public IEnumerable<PersonAttachmentModel> ListAttachments(int personId)
        {
            var userId = _userService.Get().Id;
            var request = new GetPersonAttachmentsRequest
            {
                PersonId = personId,
                UserRestriction = new DocumentTypeRoleRequest() { UserId = userId, Download = true }
            };

            GetPersonAttachmentsResponse response = null;

            try
            {
                response = _personQueryService.GetAttachments(request);
            }
            catch (WebServiceException e)
            {
                throw new UserFriendlySamException(e.Message);
            }

            return response?.Attachments.Select(a =>
            {
                var model = _autoMapperHelper.Mapper.Map<PersonAttachmentModel>(a);
                model.FileType = Path.GetExtension(a.FileName)?.Trim('.');
                model.Title = a.Description;
                return model;
            }).ToArray() ?? new PersonAttachmentModel[0];
        }

        public IEnumerable<DocumentTypeDto> GetAllowedDocumentTypesToUpload()
        {
            var documentTypes = _fileService.ListDocumentTypes(new ListDocumentTypesRequest
            {
                Category = DocumentTypeCategoryTypeName.Person,
                UserRestriction = new DocumentTypeRoleRequest
                {
                    UserId = _userService.Get().Id,
                    Upload = true,
                }
            });

            return documentTypes.Types;
        }


        public GenericResponse<IEnumerable<string>> GetDocumentTypesToUpload()
        {
            var documentTypes = GetAllowedDocumentTypesToUpload();
            return new GenericResponse<IEnumerable<string>>(documentTypes.Select(x => x.Name));
        }

        public void DeleteAttachment(int storedFileId)
        {
            var serviceRequest = new DeletePersonAttachmentRequest
            {
                StoredFileId = storedFileId
            };
             
            var documentTypes = GetAllowedDocumentTypesToUpload();

            var document = _personQueryService.GetAttachments(new GetPersonAttachmentsRequest() {StoredFileId = storedFileId}).Attachments.First();
           
            if (documentTypes.All(x => x.Id != (int)document.Type))
            {
                throw new Exception($"User does not have permissions to remove {document.Type} documents");
            }
            try
            {
                var response = _personQueryService.DeleteAttachment(serviceRequest);
            }
            catch (WebServiceException e)
            {
                throw new UserFriendlySamException(e.Message);
            }
        }

        public GenericResponse<List<QrCodeSummaryModel>> GetQrCodeSummary(int personId)
        {
            var result = new GenericResponse<List<QrCodeSummaryModel>>()
            {
                Data = new List<QrCodeSummaryModel>()
            };
            var dalResult = _personQueryService.GetQrCodeSummary(personId);
            if (!dalResult.Success)
            {
                throw new Exception($"GetQrCodeSummary Failed:{result.Messages.ToString()}");
            }
            foreach(var qrCode in dalResult.Data)
            {
                result.Data.Add(new QrCodeSummaryModel()
                {
                    QrCode = qrCode.QrCode,
                    InactiveDate = qrCode.InactiveDate,
                    GeneratedDate = qrCode.GeneratedDate,
                    Credential = qrCode.Credential,
                    Skill = qrCode.Skill
                });
            }

            return result;
        }

        public GenericResponse<List<QrCodeSummaryModel>> GetQrCodes(int naatiNumber)
        {

            var result = new GenericResponse<List<QrCodeSummaryModel>>()
            {
                Data = new List<QrCodeSummaryModel>()
            };
            var dalResult = _personQueryService.GetQrCodes(naatiNumber);
            if (!dalResult.Success)
            {
                throw new Exception($"GetQrCodes Failed:{result.Messages.ToString()}");
            }
            foreach (var qrCode in dalResult.Data)
            {
                result.Data.Add(new QrCodeSummaryModel()
                {
                    GeneratedDate = qrCode.GeneratedDate,
                    InactiveDate = qrCode.InactiveDate,
                    PractitionerVerificationUrl = GetQrCode(qrCode.QrCode),
                    QrCode = qrCode.QrCode,
                    ModifiedDate = qrCode.ModifiedDate
                });
            }

            return result;
        }

        public GenericResponse<bool> ToggleQrCode(Guid qrCode)
        {
            return _personQueryService.ToggleQrCode(qrCode);
        }

        private string GetQrCode(string guid)
        {
            var qrCodeAccessUrlTemplate = ConfigurationManager.AppSettings["QrCodeAccessUrl"];
            var url = string.Format(qrCodeAccessUrlTemplate, guid);
            return url;
        }
    }
}
