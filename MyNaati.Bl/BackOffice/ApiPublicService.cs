using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using F1Solutions.Global.Common.Logging;
using F1Solutions.Naati.Common.Contracts.Dal;
using F1Solutions.Naati.Common.Contracts.Dal.DTO;
using F1Solutions.Naati.Common.Contracts.Dal.Enum;
using F1Solutions.Naati.Common.Contracts.Dal.QueryServices;
using F1Solutions.Naati.Common.Contracts.Dal.Request;
using F1Solutions.Naati.Common.Contracts.Dal.Response;
using F1Solutions.Naati.Common.Dal.Domain;
using F1Solutions.Naati.Common.Dal.Portal.Repositories;
using MyNaati.Contracts.BackOffice;
using MyNaati.Contracts.BackOffice.PersonalDetails;
using Credential = MyNaati.Contracts.BackOffice.AccreditationResults.Credential;
using PersonCredentialsResponse = MyNaati.Contracts.BackOffice.AccreditationResults.PersonCredentialsResponse;

namespace MyNaati.Bl.BackOffice
{

    public class ApiPublicService : IApiPublicService
    {
        private IPractitionerQueryService mPractitionerService;
        private ITestSessionQueryService mTestSessionService;
        private IInstitutionQueryService mInstitutionQueryService;
        private IPersonQueryService mPersonQueryService;
        private readonly IPersonRepository mPersonRepository;
        private readonly IAccreditationResultRepository mAccreditationResultRepository;
        private readonly ICredentialQrCodeDalService mCredentialQrCodeDalService;
        private readonly ICredentialQueryService mCredentialQueryService;

        private readonly Dictionary<int, Tuple<ApiPublicPractitionerFilterType, Func<string, bool>>> mAvailablePractitionerFilters;

        public ApiPublicService(
            IPractitionerQueryService practitionerQueryService,
            ITestSessionQueryService testSessionQueryService,
            IInstitutionQueryService institutionQueryService,
            IPersonQueryService personQueryService,
            IPersonRepository personRepository,
            IAccreditationResultRepository accreditationResultRepository,
            ICredentialQrCodeDalService credentialQrCodeDalService,
            ICredentialQueryService credentialQueryService)
        {
            mPractitionerService = practitionerQueryService;
            mTestSessionService = testSessionQueryService;
            mInstitutionQueryService = institutionQueryService;
            mPersonQueryService = personQueryService;
            mPersonRepository = personRepository;
            mAccreditationResultRepository = accreditationResultRepository;
            mCredentialQrCodeDalService = credentialQrCodeDalService;
            mCredentialQueryService = credentialQueryService;

            mAvailablePractitionerFilters = new Dictionary<int, Tuple<ApiPublicPractitionerFilterType, Func<string, bool>>>
            {
                {
                    1,
                    Tuple.Create<ApiPublicPractitionerFilterType, Func<string, bool>>(ApiPublicPractitionerFilterType.CountryIntList, (filterValue)=> int.TryParse(filterValue, out int parsedValue))
                },
                {
                    2,
                    Tuple.Create<ApiPublicPractitionerFilterType, Func<string, bool>>(ApiPublicPractitionerFilterType.StateIntList, (filterValue)=> int.TryParse(filterValue, out int parsedValue))
                },
                {
                    3,
                    Tuple.Create<ApiPublicPractitionerFilterType, Func<string, bool>>(ApiPublicPractitionerFilterType.PostcodeIntList, (filterValue)=>   int.TryParse(filterValue, out int parsedValue))
                },
                {
                    4,
                    Tuple.Create<ApiPublicPractitionerFilterType, Func<string, bool>>(ApiPublicPractitionerFilterType.FamilyNameStringList, (filterValue)=>  filterValue != null)
                },
                {
                    5,
                    Tuple.Create<ApiPublicPractitionerFilterType, Func<string, bool>>(ApiPublicPractitionerFilterType.PersonIdIntList, (filterValue)=>  int.TryParse(filterValue, out int parsedValue))
                },
                {
                    6,
                    Tuple.Create<ApiPublicPractitionerFilterType, Func<string, bool>>(ApiPublicPractitionerFilterType.SkillIntList, (filterValue)=>  int.TryParse(filterValue, out int parsedValue))
                },
                {
                    7,
                    Tuple.Create<ApiPublicPractitionerFilterType, Func<string, bool>>(ApiPublicPractitionerFilterType.CredentialTypeIntList, (filterValue)=>  int.TryParse(filterValue, out int parsedValue))
                }
            };

        }

        public ApiPublicPractitionerSearchResponse SearchPractitioner(ApiPublicPractitionerSearchRequest request)
        {
            var response = new ApiPublicPractitionerSearchResponse();

            if (request.SortingOptions == null)
            {
                response.ErrorCode = ApiPublicErrorCode.ErrorParsingRequest;
                response.Message = $"SortingOptions is missing from the request";
                return response;
            }

            if (!request.SortingOptions.Any())
            {
                request.SortingOptions = new[]
                {
                    new ApiPublicSortingOption()
                    {
                        SortTypeId = ApiPublicSortType.None,
                        SortDirectionId = ApiPublicSortDirection.Ascending
                    }
                };
            }

            var practitionerFilterSearchCriteria = new List<ApiPublicPractitionerFilterSearchCriteria>();

            foreach (var filter in request.Filters)
            {
                if (!mAvailablePractitionerFilters.TryGetValue(filter.FilterId, out var filterData))
                {
                    response.ErrorCode = ApiPublicErrorCode.InvalidFilter;
                    response.Message = $"Invalid Filter: {filter.FilterId}";
                    return response;
                }

                if (!ValidateFilter(filter.Values, filterData.Item2))
                {
                    response.ErrorCode = ApiPublicErrorCode.InvalidFilterValue;
                    response.Message = $"Invalid Filter Value: {string.Join(",", filter.Values)}";
                    return response;
                }

                var practitionerSearchRequest = new ApiPublicPractitionerFilterSearchCriteria
                {
                    Filter = filterData.Item1,
                    Values = filter.Values
                };
                practitionerFilterSearchCriteria.Add(practitionerSearchRequest);
            }

            var practitionerRequest = new GetApiPublicPractitionerSearchRequest
            {
                Skip = request.Skip,
                Take = request.Take,
                Filters = practitionerFilterSearchCriteria,
                RandomSeed = request.RandomSeed,
                SortingOptions = request.SortingOptions
            };

            var practitioners = mPractitionerService.ApiSearchPractitioner(LimitSearch(practitionerRequest));

            foreach (var result in practitioners.Results)
            {
                result.CredentialTypes = result.CredentialTypes.OrderByDescending(x => x.DisplayOrder).ToList();
            }

            response.Results = practitioners.Results;
            return response;
        }

        public ApiPublicPractitionerCountResponse PractitionersCount(PractitionerCountRequest request)
        {
            var response = new ApiPublicPractitionerCountResponse();

            var practitionerSearchFilters = new List<ApiPublicPractitionerFilterSearchCriteria>();

            foreach (var filter in request.Filters)
            {
                if (!mAvailablePractitionerFilters.TryGetValue(filter.FilterId, out var filterData))
                {
                    response.ErrorCode = ApiPublicErrorCode.InvalidFilter;
                    response.Message = $"Invalid Filter: {filter.FilterId}";
                    return response;
                }

                if (!ValidateFilter(filter.Values, filterData.Item2))
                {
                    response.ErrorCode = ApiPublicErrorCode.InvalidFilterValue;
                    response.Message = $"Invalid Filter Value: {string.Join(",", filter.Values)}";
                    return response;
                }

                var practitionerSearchFilter = new ApiPublicPractitionerFilterSearchCriteria
                {
                    Filter = filterData.Item1,
                    Values = filter.Values
                };

                practitionerSearchFilters.Add(practitionerSearchFilter);
            }

            var countRequest = new GetAPiPublicPractitionerCountRequest
            {
                Filters = practitionerSearchFilters
            };

            var reuslt = mPractitionerService.ApiCountPractitioners(countRequest);
            response = MapApiPublicPractitionerCountResponse(reuslt);

            return response;
        }


        private bool ValidateFilter(IEnumerable<string> values, Func<string, bool> validator)
        {
            foreach (var value in values)
            {
                if (!validator(value))
                {
                    return false;
                }
            }

            return true;
        }

        private bool ValidateFilter(string value, Func<string, bool> validator)
        {
            if (!validator(value))
            {
                return false;
            }
            return true;
        }


        public ApiTestSessionAvailabilityResponse GetTestSessionAvailability(ApiTestSessionAvailabilityRequest request)
        {
            var response = new ApiTestSessionAvailabilityResponse();
            if (request.CredentialTypeId <= 0)
            {
                return GetInvalidParameterValue(
                    response,
                    nameof(request.CredentialTypeId),
                    request.CredentialTypeId.ToString());
            }

            if (request.PreferredTestLocationId < 0)
            {
                return GetInvalidParameterValue(
                    response,
                    nameof(request.PreferredTestLocationId),
                    request.PreferredTestLocationId.ToString());
            }

            if (request.SkillId <= 0)
            {
                return GetInvalidParameterValue(
                    response,
                    nameof(request.SkillId),
                    request.SkillId.ToString());
            }

            //if ((request.FromTestDate != null) & !TryParseDate(request.FromTestDate))
            //{
            //    return GetInvalidParameterValue(
            //        response,
            //        nameof(request.FromTestDate),
            //        request.FromTestDate);
            //}

            //if ((request.ToTestDate != null) & !TryParseDate(request.ToTestDate))
            //{
            //    return GetInvalidParameterValue(
            //        response,
            //        nameof(request.FromTestDate),
            //        request.ToTestDate);
            //}

            var sessionRequest = new ApiSessionAvailabilityRequest()
            {
                SkillId = request.SkillId,
                PreferredTestLocationId = request.PreferredTestLocationId,
                CredentialTypeId = request.CredentialTypeId,
                IncludeBacklog = request.IncludeBacklog,
                //FromTestDate = request.FromTestDate != null ? DateTime.Parse(request.FromTestDate) : (DateTime?)null,
                //ToTestDate = request.ToTestDate != null ? DateTime.Parse(request.ToTestDate) : (DateTime?)null
            };

            var results = mTestSessionService.ApiGetTestSessionAvailability(sessionRequest).AvailableTestSessions;
            var resultList = new List<ApiTestSessionAvailabilityContract>();

            foreach (var result in results)
            {
                resultList.Add(MapSessionAvailability(result));
            }

            response.Results = resultList;
            return response;
        }

        public ApiPublicCertificationsResponse GetCertifications(GetCertificationsRequest request)
        {

            var response = new ApiPublicCertificationsResponse();

            if (string.IsNullOrWhiteSpace(request.PractitionerId))
            {
                response.ErrorCode = ApiPublicErrorCode.InvalidPractitionerParameterValue;
                response.Message = $"A Public API call was received with no Practitioner ID.";
                return response;
            }

            var person = mPersonRepository.FindByPractitionerNo(request.PractitionerId);

            if (person?.Entity?.NaatiNumber == null || person?.Entity?.NaatiNumber <= 0)
            {
                response.ErrorCode = ApiPublicErrorCode.PractitionerNotExist;
                response.Message = $"A Public API call was received with Practitioner ID :{request.PractitionerId} does not exist in the system.";
                return response;
            }

            if (!person.AllowVerifyOnline)
            {
                response.ErrorCode = ApiPublicErrorCode.PractitionerNotAllowVerifyOnline;
                response.Message = $"A Public API call was received with Practitioner ID :{request.PractitionerId} cannot verify the details of this practitioner. Please contact NAATI if you require verification. ";
                return response;
            }

            var personDetails = GetPersonDetails(person.Entity.NaatiNumber);
            var currentCredentials = GetCurrentCredentialsForPerson(person.Entity.NaatiNumber);
            var previousCredentials = GetPreviousCredentialsForPerson(person.Entity.NaatiNumber);

            if (!currentCredentials.Credentials.Any() && !previousCredentials.Credentials.Any())
            {
                response.ErrorCode = ApiPublicErrorCode.PractitionerNotExist;
                response.Message = $"A Public API call was received with Practitioner ID :{request.PractitionerId} does not exist in the system.";
                return response;
            }

            response.Practitioner = new ApiPublicPractitioner
            {
                PractitionerId = personDetails.PractitionerNumber,
                GivenName = personDetails.GivenName,
                FamilyName = personDetails.Surname,
                Country = personDetails.Country
            };
            response.CurrentCertifications = currentCredentials.Credentials?.Select(MapPublicCredentials);
            response.PreviousCertifications = previousCredentials.Credentials?.Select(MapPublicCredentials);

            return response;
        }

        public ApiPersonImageResponse GetPersonPhoto(ApiPublicPersonPhotoRequest request)
        {

            var availablePhotoRequestPropertyType = new Dictionary<int, Tuple<ApiPublicPhotoRequestPropertyType, Func<string, bool>>>()
            {
                {
                    1,
                    Tuple.Create<ApiPublicPhotoRequestPropertyType, Func<string, bool>>(ApiPublicPhotoRequestPropertyType.PersonId, (propertyTypeValue)=> int.TryParse(propertyTypeValue, out int propertyTypeParseValue))
                },
                {
                    2,
                    Tuple.Create<ApiPublicPhotoRequestPropertyType, Func<string, bool>>(ApiPublicPhotoRequestPropertyType.NaatiNumber, (propertyTypeValue)=> int.TryParse(propertyTypeValue, out int propertyTypeParseValue))
                },
                {
                    3,
                    Tuple.Create<ApiPublicPhotoRequestPropertyType, Func<string, bool>>(ApiPublicPhotoRequestPropertyType.PractitionerId, (propertyTypeValue)=> propertyTypeValue != null)
                }
            };
            var response = new ApiPersonImageResponse();
            if (!availablePhotoRequestPropertyType.TryGetValue(request.PropertyType, out var propertyTypeData))
            {
                response.ErrorCode = ApiPublicErrorCode.InvalidPhotoPropertyType;
                response.Message = $"Invalid Photo PropertyType: {request.PropertyType}";
                return response;
            }

            if (!ValidateFilter(request.Value, propertyTypeData.Item2))
            {
                response.ErrorCode = ApiPublicErrorCode.InvalidPhotoPropertyTypeValue;
                response.Message = $"Invalid Photo PropertyType Value: {request.Value}";
                return response;
            }

            var serviceRequest = new GetPublicPersonPhotoRequest();
            switch (request.PropertyType)
            {
                case 1:
                    serviceRequest.PropertyType = ApiPublicPhotoRequestPropertyType.PersonId;
                    break;
                case 2:
                    serviceRequest.PropertyType = ApiPublicPhotoRequestPropertyType.NaatiNumber;
                    break;
                case 3:
                    serviceRequest.PropertyType = ApiPublicPhotoRequestPropertyType.PractitionerId;
                    break;
            }
            serviceRequest.Value = request.Value;

            response = mPersonQueryService.GetApiPersonImage(serviceRequest);

            if (!response.IsPersonExist)
            {
                response.ErrorCode = ApiPublicErrorCode.PersonNotExist;
                response.Message = $"Referenced person does not exist in the system.";
                return response;
            }

            if (response.IsPersonExist && response.PersonImageData.Length == 0)
            {
                response.ErrorCode = ApiPublicErrorCode.PersonPhotoNotExist;
                response.Message = $"Referenced person photo does not exist in the system.";
                return response;
            }

            if (response.IsPersonExist && response.PersonImageData.Length > 0 && !response.ShowPhotoOnline)
            {
                response.ErrorCode = ApiPublicErrorCode.PersonPhotoNotAvailable;
                response.Message = $"Verification of the person's photo is not available in the system.";
                return response;
            }

            if (response.IsDeceased)
            {
                response.ErrorCode = ApiPublicErrorCode.PersonPhotoNotAvailable;
                response.Message = $"Person Photo not allowed.";
                LoggingHelper.LogWarning($"Attempt to access photo of deceased. Naati Number: {request.Value}");
                return response;
            }

            return response;
        }

        public ApiTestSessionSearchResponse SearchTestSession(ApiTestSessionSearchRequest request)
        {
            var response = new ApiTestSessionSearchResponse();

            var testSessionFilters = new Dictionary<int, Tuple<TestSessionFilterType, Func<string, bool>>>()
            {
                {1,Tuple.Create<TestSessionFilterType, Func<string, bool>>(TestSessionFilterType.CredentialIntList, (filterValue)=> int.TryParse(filterValue, out int parsedValue))},
                {2,Tuple.Create<TestSessionFilterType, Func<string, bool>>(TestSessionFilterType.CredentialSkillIntList, (filterValue)=> int.TryParse(filterValue, out int parsedValue))},
                {3,Tuple.Create<TestSessionFilterType, Func<string, bool>>(TestSessionFilterType.TestLocationIntList, (filterValue)=> int.TryParse(filterValue, out int parsedValue))},
                {4,Tuple.Create<TestSessionFilterType, Func<string, bool>>(TestSessionFilterType.TestVenueIntList, (filterValue)=> int.TryParse(filterValue, out int parsedValue))},

                {5,Tuple.Create<TestSessionFilterType, Func<string, bool>>(TestSessionFilterType.TestDateFromString, TryParseDate)},
                {6,Tuple.Create<TestSessionFilterType, Func<string, bool>>(TestSessionFilterType.TestDateToString, TryParseDate)},

                {7,Tuple.Create<TestSessionFilterType, Func<string, bool>>(TestSessionFilterType.SessionNameString, (filterValue)=> !string.IsNullOrWhiteSpace(filterValue))},
                {8,Tuple.Create<TestSessionFilterType, Func<string, bool>>(TestSessionFilterType.IncludeCompletedSessionsBoolean, (filterValue)=> bool.TryParse(filterValue, out bool parsedValue))},
                {9,Tuple.Create<TestSessionFilterType, Func<string, bool>>(TestSessionFilterType.TestSessionIntList, (filterValue)=> int.TryParse(filterValue, out int parsedValue))},
            };

            List<TestSessionSearchCriteria> filters = new List<TestSessionSearchCriteria>();

            foreach (var f in request.Filters)
            {
                if (!testSessionFilters.TryGetValue(f.FilterId, out var filterData))
                {
                    response.ErrorCode = ApiPublicErrorCode.InvalidFilter;
                    response.Message = $"Invalid Filter: {f.FilterId}";
                    return response;
                }

                if (!ValidateFilter(f.Values, filterData.Item2))
                {
                    response.ErrorCode = ApiPublicErrorCode.InvalidFilterValue;
                    response.Message = $"Invalid Filter Value: {string.Join(",", f.Values)}"; ;
                    return response;
                }

                var searchCriteria = new TestSessionSearchCriteria
                {
                    Filter = filterData.Item1,
                    Values = f.Values
                };

                filters.Add(searchCriteria);
            }

            filters.Add(new TestSessionSearchCriteria
            {
                Filter = TestSessionFilterType.AllowSelfAssignBoolean,
                Values = new[] { true.ToString() }
            });

            filters.Add(new TestSessionSearchCriteria
            {
                Filter = TestSessionFilterType.IsActiveBoolean,
                Values = new[] { true.ToString() }
            });

            GetTestSessionSearchRequest sessionRequest = new GetTestSessionSearchRequest
            {
                Skip = request.Skip,
                Take = request.Take,
                Filters = filters
            };

            var searchData = mTestSessionService.Search(LimitSearch(sessionRequest));

            List<ApiTestSessionContract> resultsList = new List<ApiTestSessionContract>();

            foreach (var t in searchData.TestSessions)
            {
                resultsList.Add(new ApiTestSessionContract
                {
                    TestDateTime = t.TestDate.HasValue ? t.TestDate.Value.ToString("dd-MM-yyyy hh:mm tt") : string.Empty,
                    Venue = t.Venue,
                    TestSessionId = t.TestSessionId,
                    CredentialTypeId = t.CredentialTypeId,
                    CredentialType = t.CredentialTypeExternalName,
                    Capacity = t.Capacity,
                    Completed = t.Completed,
                    TestLocationName = t.TestLocationName,
                    TestLocationStateName = t.TestLocationStateName,
                    SessionName = t.SessionName,
                    DurationInMinute = t.Duration,
                });
            }

            return new ApiTestSessionSearchResponse
            {
                Results = resultsList
            };
        }

        public PublicLookupResponse GetLookups(LookupRequest request)
        {
            var response = new PublicLookupResponse();
            ApiPublicLookupType lookupType;
            try
            {
                lookupType = (ApiPublicLookupType)request.LookupId;
            }
            catch
            {
                response.ErrorCode = ApiPublicErrorCode.InvalidParameterValue;
                response.Message = $"Invalid Parameter Value: {nameof(request.LookupId)}. Value: {request.LookupId}";
                return response;
            }

            response = mPractitionerService.ApiGetLookup(lookupType);
            return response;
        }

        public LanguagesResponse GetLanguages(LanguagesRequest request)
        {
            var response = mPractitionerService.ApiGetLanguages(request);
            return response;
        }

        public PublicLegacyAccreditionsResponse GetLegacyAccreditions(GetLegacyAccreditionsRequest request)
        {
            var legacyAccreditionsresponse = mPractitionerService.ApiGetLegacyAccreditions(request);

            var results = legacyAccreditionsresponse.Results.Select(MapLegacyAcreditation);

            var response = new PublicLegacyAccreditionsResponse
            {
                Results = results
            };

            return response;
        }

        public ApiEndorsedQualificationResponse SearchEndorsedQualification(ApiEndorseQualificationSearchRequest request)
        {
            var response = new ApiEndorsedQualificationResponse();

            var endorsedQualificationFilters = new Dictionary<int, Tuple<EndorsedQualificationFilterType, Func<string, bool>>>()
            {
                {1,Tuple.Create<EndorsedQualificationFilterType, Func<string, bool>>(EndorsedQualificationFilterType.InstitutionIntList, (filterValue)=> int.TryParse(filterValue, out int parsedValue))},
                {2,Tuple.Create<EndorsedQualificationFilterType, Func<string, bool>>(EndorsedQualificationFilterType.LocationString, (filterValue)=> !string.IsNullOrWhiteSpace(filterValue))},
                {3,Tuple.Create<EndorsedQualificationFilterType, Func<string, bool>>(EndorsedQualificationFilterType.QualificationString, (filterValue)=> !string.IsNullOrWhiteSpace(filterValue))},
                {4,Tuple.Create<EndorsedQualificationFilterType, Func<string, bool>>(EndorsedQualificationFilterType.CredentialTypeIntList, (filterValue)=> int.TryParse(filterValue, out int parsedValue))},
                {5,Tuple.Create<EndorsedQualificationFilterType, Func<string, bool>>(EndorsedQualificationFilterType.EndorsementFromString, TryParseDate)},
                {6,Tuple.Create<EndorsedQualificationFilterType, Func<string, bool>>(EndorsedQualificationFilterType.EndorsementToString, TryParseDate)},
                {7,Tuple.Create<EndorsedQualificationFilterType, Func<string, bool>>(EndorsedQualificationFilterType.EndorsedQualificationIdIntList, (filterValue)=> int.TryParse(filterValue, out int parsedValue))}
            };

            var filters = new List<EndorsedQualificationSearchCriteria>();

            foreach (var filter in request.Filters)
            {
                if (!endorsedQualificationFilters.TryGetValue(filter.FilterId, out var filterData))
                {
                    response.ErrorCode = ApiPublicErrorCode.InvalidFilter;
                    response.Message = $"Invalid Filter: {filter.FilterId}";
                    return response;
                }

                if (!ValidateFilter(filter.Values, filterData.Item2))
                {
                    response.ErrorCode = ApiPublicErrorCode.InvalidFilterValue;
                    response.Message = $"Invalid Filter Value: {string.Join(",", filter.Values)}";
                    return response;
                }

                var searchCriteria = new EndorsedQualificationSearchCriteria
                {
                    Filter = filterData.Item1,
                    Values = filter.Values
                };

                filters.Add(searchCriteria);
            }

            var endorsedQualificationRequest = new GetEndorsedQualificationSearchRequest
            {
                Skip = request.Skip,
                Take = request.Take,
                Filters = filters
            };

            var searchData = mInstitutionQueryService.SearchEndorsedQualification(LimitSearch(endorsedQualificationRequest));

            List<ApiEndorsedQualificationContract> resultsList = new List<ApiEndorsedQualificationContract>();

            foreach (var eq in searchData.Data)
            {
                resultsList.Add(new ApiEndorsedQualificationContract
                {
                    InstitutionId = eq.InstitutionId,
                    InstitutionName = eq.InstitutionName,
                    Location = eq.Location,
                    Qualification = eq.Qualification,
                    CredentialTypeId = eq.CredentialTypeId,
                    CredentialType = eq.CredentialTypeExternalName,
                    EndorsementPeriodFrom = eq.EndorsementPeriodFrom.ToString("dd-MM-yyyy"),
                    EndorsementPeriodTo = eq.EndorsementPeriodTo.ToString("dd-MM-yyyy"),
                    Active = eq.Active
                });
            }

            response.Results = resultsList;
            return response;
        }

        public ApiPublicVerifyCertificationQrCodeResponse VerifyCertificationQrCode(ApiCredentialQrCodeRequest request)
        {
            LoggingHelper.LogInfo($"Request to verify : {request.CredentialQrCode} received");

            Guid qrCodeGuid;
            if (!Guid.TryParse(request.CredentialQrCode, out qrCodeGuid))
            {
                var response = new ApiPublicVerifyCertificationQrCodeResponse();
                response.ErrorCode = ApiPublicErrorCode.InvalidParameterValue;
                response.Message = $"Invalid Filter: {request}";
                LoggingHelper.LogError($"Request to verify : {request} failed. Invalid format");
                return response;
            }

            var result = mCredentialQrCodeDalService.VerifyCredentialQRCode(qrCodeGuid);

            if (!result.Success)
            {
                var response = new ApiPublicVerifyCertificationQrCodeResponse();
                response.ErrorCode = ApiPublicErrorCode.GenericError; //presuming that we don't give too much away here.
                response.Message = $"Invalid Filter: {request}";
                LoggingHelper.LogError($"Request to verify : {request} failed. {response.Message}");
                return response;
            }
            return new ApiPublicVerifyCertificationQrCodeResponse()
            {
                Certification = result.Data.CredentialTypeName,
                DateIssued = result.Data.IssueDate,
                PractionerNumber = result.Data.PractitionerNumber
            };
        }


        private ApiPublicPractitionerCountResponse MapApiPublicPractitionerCountResponse(ApiPublicPractitionerCountServiceResponse response)
        {
            var result = new ApiPublicPractitionerCountResponse();

            foreach (var item in response.Results.ToList())
            {
                switch (item.Type)
                {
                    case ApiPublicPractitionerCountLookupType.ByCredentialTypeId:
                        result.ByCredentialTypeId = item.Values;
                        break;
                    case ApiPublicPractitionerCountLookupType.ByStateId:
                        result.ByStateId = item.Values;
                        break;
                    case ApiPublicPractitionerCountLookupType.ByCountryId:
                        result.ByCountryId = item.Values;
                        break;
                }
            }

            result.TotalPractitinerCount = response.Results.ToList()[0].Values.ToList()[0].Count;
            return result;
        }

        private PublicAccreditationLegacy MapLegacyAcreditation(LegacyAccreditationDto acreditation)
        {

            var direction = string.Empty;
            switch (acreditation.Direction?.ToUpper())
            {
                case "B":
                    direction = $"{acreditation.Language1} to/from {acreditation.Language2}";
                    break;
                case "E":
                    direction = $"{acreditation.Language1} to {acreditation.Language2}";
                    break;
                case "O":
                    direction = $"{acreditation.Language2} to {acreditation.Language1}";
                    break;
            }

            var result = new PublicAccreditationLegacy
            {
                Level = acreditation.Level,
                Category = acreditation.Category,
                Direction = direction,
                StartDate = acreditation.StartDate.ToString("dd-MM-yyyy"),
                EndDate = acreditation.ExpiryDate != null ? acreditation.ExpiryDate?.ToString("dd-MM-yyyy") : string.Empty
            };

            return result;
        }

        private T LimitSearch<T>(T request) where T : ISearchPagingRequest
        {
            var maxResults = Convert.ToInt32(ConfigurationManager.AppSettings["MaxPublicApiResults"]);

            if (!request.Take.HasValue || request.Take > maxResults)
            {
                request.Take = maxResults;
            }

            return request;
        }

        private bool TryParseDate(string dateString)
        {
            if (string.IsNullOrWhiteSpace(dateString))
            {
                return false;
            }

            if (!DateTime.TryParse(dateString, out var dateTime))
            {
                return false;
            }

            if ((dateTime.Year < 1900) || (dateTime.Year > 2100))
            {
                return false;
            }

            return true;
        }

        private ApiPublicCredentials MapPublicCredentials(Credential credential)
        {
            return new ApiPublicCredentials
            {
                CertificationType = credential.Skill,
                Skill = credential.Direction,
                StartDate = credential.StartDate?.ToString("dd-MM-yyyy"),
                EndDate = credential.ExpiryDate?.ToString("dd-MM-yyyy"),
            };
        }
        public PersonCredentialsResponse GetPreviousCredentialsForPerson(int naatiNumber)
        {
            var credentials = mAccreditationResultRepository.GetPreviousCertifiedCredentailsByEmail(naatiNumber);
            return new PersonCredentialsResponse() { Credentials = credentials.Select(ConvertToCredentialViewModel).ToArray() };
        }

        public ApiVerifyDocumentResponse VerifyDocument(ApiVerifyDocumentRequest request)
        {
            //assume format of <credential>-<application>-<documentNumber>
            var verifyDocumentRequest = new VerifyDocumentRequest();
            var splitRequest = request.DocumentNumber.Split('-');

            if(splitRequest.Length != 3)
            {
                LoggingHelper.LogError($"Bad format for {request.DocumentNumber}");
                throw new Exception("An error has occurred");
            }

            if (int.TryParse(splitRequest[0], out var credentialId))
            {
                verifyDocumentRequest.CredentialId = credentialId;
            }

            if (int.TryParse(splitRequest[1], out var credentialApplicationId))
            {
                verifyDocumentRequest.CredentialApplicationId = credentialApplicationId;
            }

            if (verifyDocumentRequest.CredentialId == 0  || verifyDocumentRequest.CredentialApplicationId == 0)
            {
                LoggingHelper.LogError($"Bad format for {request.DocumentNumber}");
                throw new Exception("An error has occurred");
            }

            var documentVerificationResponse = mCredentialQueryService.VerifyCredentialDocument(verifyDocumentRequest);
            if(!documentVerificationResponse.Success)
            {
                LoggingHelper.LogError(documentVerificationResponse.Messages.First());
                throw new Exception("An error has occurred");
            }

            var documentVerificationData = documentVerificationResponse.Data;
            return new ApiVerifyDocumentResponse()
            {
                Name = documentVerificationData.Name,
                PractitionerId = documentVerificationData.PractitionerId,
                CertificationType = documentVerificationData.CertificationType,
                Skill = documentVerificationData.Skill,
                DateIssued = documentVerificationData.DateIssued
            };
        }
        private PersonCredentialsResponse GetCurrentCredentialsForPerson(int naatiNumber)
        {
            var credentials = mAccreditationResultRepository.GetCurrentCertifiedCredentailsByEmail(naatiNumber);
            return new PersonCredentialsResponse() { Credentials = credentials.Select(ConvertToCredentialViewModel).ToArray() };
        }

        private Credential ConvertToCredentialViewModel(CredentialsDetailsDto model)
        {
            return new Credential()
            {
                AccreditationResultId = model.Id,
                ToLanguage = model.ToLanguage,
                ExpiryDate = model.TerminationDate ?? model.EndDate,
                StartDate = model.StartDate,
                Direction = model.Direction,
                Skill = model.Skill,
                Language = model.Language
            };
        }

        private PersonalEditPerson GetPersonDetails(int naatiNumber)
        {
            var personDetails = new PersonalEditPerson();
            var person = mPersonRepository.FindByNaatiNumber(naatiNumber);
            if (person != null)
            {
                personDetails = MapPerson(person);
            }
            return personDetails;
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
                Country = person.PrimaryAddress?.Country.Name
            };
        }

        private ApiTestSessionAvailabilityContract MapSessionAvailability(AvailableTestSessionDto dto)
        {
            return new ApiTestSessionAvailabilityContract
            {
                AvailableSeats = dto.AvailableSeats,
                DurationInMinute = dto.TestSessionDuration,
                IsPreferredLocation = dto.IsPreferedLocation,
                Name = dto.Name,
                TestDateTime = dto.TestDateTime.ToString("dd-MM-yyyy hh:mm tt"),
                TestLocation = dto.TestLocation,
                TestLocationId = dto.TestLocationId,
                TestSessionId = dto.TestSessionId,
                Venue = dto.VenueName,
                VenueAddress = dto.VenueAddress,
            };
        }

        private T GetInvalidParameterValue<T>(T response, string parameter, string value) where T : BaseResponse
        {
            response.ErrorCode = ApiPublicErrorCode.InvalidParameterValue;
            response.Message = $"Invalid Parameter Value {parameter}. Value: {value}";
            return response;
        }
    }
}