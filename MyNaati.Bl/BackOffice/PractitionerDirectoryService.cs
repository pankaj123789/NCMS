//using System;
//using System.Collections.Generic;
//using System.Linq;
//using F1Solutions.Naati.Common.Contracts.Dal;
//using F1Solutions.Naati.Common.Contracts.Dal.DTO;
//using F1Solutions.Naati.Common.Contracts.Dal.Enum;
//using F1Solutions.Naati.Common.Contracts.Dal.Portal;
//using F1Solutions.Naati.Common.Contracts.Dal.Portal.Common;
//using F1Solutions.Naati.Common.Contracts.Dal.QueryServices;
//using F1Solutions.Naati.Common.Contracts.Dal.Request;
//using F1Solutions.Naati.Common.Dal.Portal.Repositories;
//using F1Solutions.Naati.Common.Dal.Portal.Repositories.PractitionersDirectory;
//using MyNaati.Contracts.BackOffice.PractitionerDirectory;
//using PersonImage = F1Solutions.Naati.Common.Dal.Domain.PersonImage;
//using SortDirection = F1Solutions.Naati.Common.Contracts.Dal.Enum.SortDirection;

//namespace MyNaati.Bl.BackOffice
//{
//    public class PractitionerDirectoryService : IPractitionerDirectoryService
//    {
//        private IPractitionersRepository mPractitionersRepository;
//        private IPersonRepository mPersonRepository;
//        private ICredentialRepository mAccreditationResultRepository;
//        private IPersonImageRepository mPersonImageRepository;
//        private IPractitionerQueryService mPractitionerQueryService;

//        public PractitionerDirectoryService(
//            IPractitionersRepository practitionersRepository, 
//            IPersonRepository personRepository,
//            ICredentialRepository accreditationResultRepository,
//            IPractitionerSearchRepository searchRepository,
//            IPersonImageRepository imageRepository, 
//            IPractitionerQueryService practitionerQueryService)
//        {
//            mPractitionersRepository = practitionersRepository;
  
//            mPersonRepository = personRepository;
//            mAccreditationResultRepository = accreditationResultRepository;
//            mPersonImageRepository = imageRepository;
//            mPractitionerQueryService = practitionerQueryService;
//        }

//        public SearchResults<Practitioner> SearchPractitioners(PractitionerSearchCriteria criteria)
//        {
//            var request = GetPractitionerSearchRequest(criteria);
//            var response = mPractitionerQueryService.SearchPractitioner(request);

//            return new SearchResults<Practitioner>
//            {
//                Results = response.Results.Select(MapPractitioner),
//                TotalResultsCount = response.Total,
//                PageNumber = criteria.PageNumber,
//                PageSize = criteria.PageSize,
//                ResultsLastUpdated = DateTime.Now.ToString("dd MMM yyyy hh:mm tt")
//            };
//        }

//        private Practitioner MapPractitioner(PractitionerSearchDto practioner)
//        {
//            var mappedEntity = new Practitioner
//            {
//                Id = practioner.PersonId,
//                Title = practioner.Title ?? string.Empty,
//                GivenName = practioner.GivenName,
//                OtherNames = practioner.OtherNames,
//                Surname = practioner.Surname,
//                Skills = string.Join(", ", practioner.CredentialTypes
//                .OrderByDescending(y => y.DisplayOrder).Select(x => string.Concat(x.ExternalName, " (", x.Direction.Trim(), ")"))),
//                ContactDetails = GetContactDetails(practioner),
//                ShowPhotoOnline = practioner.ShowPhotoOnline,
//                Deceased = practioner.Deceased,
//                Address = GetAddress(practioner),
//                Hash = practioner.Hash
//            };

//            mappedEntity.Location = GetLocation(mappedEntity.Address);

//            return mappedEntity;
//        }

//        public AddressDetail GetAddress(PractitionerSearchDto practitioner)
//        {
//            var address = new AddressDetail() { DefaultContryId = 13 };
//            var item = practitioner.DefaultAddress;
//            if (item == null)
//            {
//                return address;
//            }

//            address.Title = practitioner.Title ?? string.Empty;
//            address.Surname = practitioner.Surname;
//            address.OtherNames = practitioner.OtherNames ?? string.Empty;
//            address.CountryId = item.CountryId;
//            address.Country = item.Country;
//            address.OdAddressVisibilityTypeId = item.OdAddressVisibilityTypeId;
//            address.StreetDetails = item.StreetDetails;
//            address.State = item.State ?? string.Empty;
//            address.Suburb = item.Suburb ?? string.Empty;
//            address.Postcode = item.Postcode ?? string.Empty;
//            return address;
//        }

//        public string GetLocation(AddressDetail address)
//        {
//            return string.IsNullOrWhiteSpace(address.State) ? address.Country : address.State;
//        }

//        private List<ContactDetail> GetContactDetails(PractitionerSearchDto practioner)
//        {
//            var contactDetails = new List<ContactDetail>();
//            contactDetails.AddRange(practioner.PhonesInPd.Select(x => new ContactDetail { Type = "Phone", Contact = x }));
//            contactDetails.AddRange(practioner.EmailsInPd.Select(x => new ContactDetail { Type = "Email", Contact = x }));

//            return contactDetails;
//        }

//        private GetPractitionerSearchRequest GetPractitionerSearchRequest(PractitionerSearchCriteria criteria)
//        {
//            var practitionersFilter = GetFilter(criteria);
//            var skipCount = (criteria.PageNumber - 1) * criteria.PageSize;

//            SortType sortType;
//            Enum.TryParse(criteria.SortMember.ToString(), true, out sortType);
//            SortDirection sortDirection;
//            Enum.TryParse(criteria.SortOrder.ToString(), true, out sortDirection);

//            var sortingOptions = new List<SortingOption>()
//            {
//                new SortingOption()
//                {
//                    SortType = SortType.Level,// default sort direction
//                    SortDirection = SortDirection.Descending
//                },
//                new SortingOption()
//                {
//                    SortType = SortType.None,// default sort direction
//                    SortDirection = SortDirection.Ascending
//                }
//            };

//            if (sortingOptions.All(x => x.SortType != sortType))
//            {
//                sortingOptions.Insert(0,new SortingOption()
//                {
//                    SortType = sortType,
//                    SortDirection = sortDirection
//                });
//            }
//            return new GetPractitionerSearchRequest
//            {
//                Filters = practitionersFilter,
//                Skip = skipCount,
//                Take = criteria.PageSize,
//                SortingOptions = sortingOptions,
//                RandomSeed = criteria.RandomSearchSeed
//            };
//        }

//        private IEnumerable<PractitionerFilterSearchCriteria> GetFilter(PractitionerSearchCriteria criteria)
//        {
//            var firstLanguageValue = criteria.FirstLanguageId.GetValueOrDefault() != 0 ? criteria.FirstLanguageId.ToString() : null;
//            var secondLanguageValue = criteria.SecondLanguageId.GetValueOrDefault() != 0 ? criteria.SecondLanguageId.ToString() : null;
//            var skillsValue = criteria.Skills?.Any() ?? false ? criteria.Skills.Select(s => s.ToString()) : new[] { (string)null };
//            var credentialAndSkillValue = criteria.AccreditationLevelId.GetValueOrDefault() != 0 ? criteria.AccreditationLevelId.ToString() : null;
//            var familyNameValue = !string.IsNullOrWhiteSpace(criteria.Surname) ? criteria.Surname : null;
//            var countryValue = criteria.CountryId.GetValueOrDefault() != 0 ? criteria.CountryId.ToString() : null;
//            var stateValue = criteria.StateId.GetValueOrDefault() != 0 ? criteria.StateId.ToString() : null;
//            var postCodeValue = !string.IsNullOrWhiteSpace(criteria.Postcode) ? criteria.Postcode : null;

//            return new[]
//            {
//                new PractitionerFilterSearchCriteria { Filter = PractitionerFilterType.Language1IntList, Values = new []{ firstLanguageValue } },
//                new PractitionerFilterSearchCriteria { Filter = PractitionerFilterType.Language2IntList, Values = new []{ secondLanguageValue } },
//                new PractitionerFilterSearchCriteria { Filter = PractitionerFilterType.SkillIntList, Values = skillsValue },
//                new PractitionerFilterSearchCriteria { Filter = PractitionerFilterType.CredentialTypeAndSkillIntList, Values = new []{ credentialAndSkillValue } },
//                new PractitionerFilterSearchCriteria { Filter = PractitionerFilterType.CountryIntList, Values = new []{ countryValue } },
//                new PractitionerFilterSearchCriteria { Filter = PractitionerFilterType.PostcodeString, Values = new []{ postCodeValue } },
//                new PractitionerFilterSearchCriteria { Filter = PractitionerFilterType.StateIntList, Values = new []{ stateValue } },
//                new PractitionerFilterSearchCriteria { Filter = PractitionerFilterType.FamilyNameString, Values = new []{ familyNameValue } },
//            };
//        }

//        private IEnumerable<PractitionerFilterSearchCriteria> GetCountFilterFor(PractitionerSearchCriteria criteria, PractitionerLookupType lookupType)
//        {
//            var firstLanguageValue = criteria.FirstLanguageId.GetValueOrDefault() != 0 ? criteria.FirstLanguageId.ToString() : null;
//            var skillsValue = criteria.Skills?.Any() ?? false ? criteria.Skills.Select(s => s.ToString()) : new[] { (string)null };
//            var secondLanguageValue = criteria.SecondLanguageId.GetValueOrDefault() != 0 ? criteria.SecondLanguageId.ToString() : null;
//            var credentialAndSkillValue = criteria.AccreditationLevelId.GetValueOrDefault() != 0 ? criteria.AccreditationLevelId.ToString() : null;
//            var countryValue = criteria.CountryId.GetValueOrDefault() != 0 ? criteria.CountryId.ToString() : null;

//            var firstLanguageFilter = new PractitionerFilterSearchCriteria { Filter = PractitionerFilterType.Language1IntList, Values = new[] { firstLanguageValue } };
//            var skillFilter = new PractitionerFilterSearchCriteria { Filter = PractitionerFilterType.SkillIntList, Values = skillsValue };
//            var credentialtTypeFilter = new PractitionerFilterSearchCriteria { Filter = PractitionerFilterType.CredentialTypeAndSkillIntList, Values = new[] { credentialAndSkillValue } };
//            var countryFilter = new PractitionerFilterSearchCriteria { Filter = PractitionerFilterType.CountryIntList, Values = new[] { countryValue } };
//            var secondLanguageFilter = new PractitionerFilterSearchCriteria { Filter = PractitionerFilterType.Language2IntList, Values = new[] { secondLanguageValue } };

//            var filters = new List<PractitionerFilterSearchCriteria>();
//            if (lookupType == PractitionerLookupType.ActiveCredentialsTypesByRequest)
//            {
//                filters.Add(firstLanguageFilter);
//                filters.Add(secondLanguageFilter);
//                filters.Add(skillFilter);
//                return filters;
//            }

//            if (lookupType == PractitionerLookupType.ActiveCredentialsCountries)
//            {
//                filters.Add(skillFilter);
//                filters.Add(firstLanguageFilter);
//                filters.Add(credentialtTypeFilter);
//                filters.Add(secondLanguageFilter);
//                return filters;
//            }

//            if (lookupType == PractitionerLookupType.ActiveCredentialsStates)
//            {
//                filters.Add(skillFilter);
//                filters.Add(firstLanguageFilter);
//                filters.Add(credentialtTypeFilter);
//                filters.Add(countryFilter);
//                filters.Add(secondLanguageFilter);
//                return filters;
//            }

//            throw new NotSupportedException(lookupType.ToString());
//        }

//        private ItemCountDto GetItemCountFor(PractitionerLookupType type, PractitionerSearchCriteria criteria)
//        {
//            var request = new GetPractitionerCountRequest
//            {
//                Filters = GetCountFilterFor(criteria, type)
//            };
//            var response = mPractitionerQueryService.CountPractitioners(request);
//            return response.Results.First(x => x.Type == type);
//        }

//        public CountResults<ValCount> CountPractitioners(PractitionerSearchCriteria criteria)
//        {
//            var resultList = new List<ItemCountDto>
//            {
//                GetItemCountFor(PractitionerLookupType.ActiveCredentialsTypesByRequest, criteria),
//                GetItemCountFor(PractitionerLookupType.ActiveCredentialsCountries, criteria),
//                GetItemCountFor(PractitionerLookupType.ActiveCredentialsStates, criteria)
//            };

//            int total;
//            var results = MapItemCount(resultList, out total);
//            return new CountResults<ValCount>()
//            {
//                Results = results,
//                TotalResultsCount = total,
//                PageNumber = criteria.PageNumber,
//                PageSize = criteria.PageSize,
//                ResultsLastUpdated = DateTime.Now.ToString("dd MMM yyy hh:mm tt")
//            };
//        }


//        private IEnumerable<ValCount[]> MapItemCount(IEnumerable<ItemCountDto> items, out int total)
//        {
//            IEnumerable<ItemCountValueDto> credentialTypeCount = null;
//            IEnumerable<ItemCountValueDto> countriesCount = null;
//            IEnumerable<ItemCountValueDto> statesCount = null;
//            total = 0;

//            foreach (var item in items)
//            {
//                switch (item.Type)
//                {
//                    case PractitionerLookupType.ActiveCredentialsTypesByRequest:
//                        credentialTypeCount = item.Values;
//                        break;
//                    case PractitionerLookupType.ActiveCredentialsCountries:
//                        countriesCount = item.Values;
//                        total = item.Values.ElementAt(0).Count;
//                        break;
//                    case PractitionerLookupType.ActiveCredentialsStates:
//                        statesCount = item.Values;
//                        break;
//                }
//            }

//            var list = new List<ValCount[]>
//            {
//                credentialTypeCount?.Select(v=> new ValCount { Val = v.Id, Count = v.Count}).ToArray()?? new ValCount[0],
//                countriesCount?.Select(v=> new ValCount { Val = v.Id, Count = v.Count}).ToArray()?? new ValCount[0],
//                statesCount?.Select(v=> new ValCount { Val = v.Id, Count = v.Count}).ToArray()?? new ValCount[0],
//            };

//            return list;
//        }

   
//        public byte[] GetProfilePicture(int naatiNumber)
//        {
//            PersonImage personImage = mPersonImageRepository.FindByNaatiNumber(naatiNumber);
//            if (personImage != null && personImage.Photo != null)
//            {
//                return personImage.Photo;
//            }

//            return null;
//        }

//        public SearchResults<Practitioner> ExportPractitioners(PractitionerSearchCriteria criteria)
//        {
//            return SearchPractitioners(criteria);
//        }


//        public IEnumerable<CredentialsDetailsDto> GetPractionerCredentials(int personId)
//        {
//            var naatiNo = mPersonRepository.FindByPersonId(personId).Entity.NaatiNumber;
//            return mAccreditationResultRepository.GetCurrentCertifiedCredentailsByEmail(naatiNo);
//        }

//        public PractitionerDirectoryGetContactDetailsResponse GetPractitionerContactDetails(PractitionerDirectoryGetContactDetailsRequest request)
//        {
//            var filter = new[]
//            {
//                new PractitionerFilterSearchCriteria
//                {
//                    Filter = PractitionerFilterType.PersonIdIntList,
//                    Values = new[] {request.Identifier.ToString()},
//                },
//            };

//            var response = mPractitionerQueryService.SearchPractitioner(new GetPractitionerSearchRequest { Filters = filter, Skip = 0, Take = 1, RandomSeed = request.Seed, SortingOptions = Enumerable.Empty<SortingOption>()});


//            var legacyAccreditations =
//                mPractitionerQueryService.GetLegacyAccreditions(
//                    new GetLegacyAccreditionsRequest { PersonId = request.Identifier });

//            var practitioner = response.Results.FirstOrDefault();

//            if (practitioner?.Hash != request.Hash)
//            {
//                throw new Exception($"Invalid practitioner details provided Identifier:{request.Identifier}, hash:{request.Hash} seed:{request.Seed}");
//            }

//            var detailsResponse = new PractitionerDirectoryGetContactDetailsResponse();
//            if (practitioner == null)
//            {
//                return detailsResponse;
//            }

//            var item = practitioner.Addresses.FirstOrDefault(x =>
//                           x.OdAddressVisibilityTypeId != (int)OdAddressVisibilityTypeName.DoNotShow) ??
//                       practitioner.Addresses.FirstOrDefault(x => x.IsPrimaryAddress);

//            if (item == null)
//            {
//                return detailsResponse;
//            }

//            detailsResponse.Title = practitioner.Title ?? string.Empty;
//            detailsResponse.GivenName = practitioner.GivenName;
//            detailsResponse.Surname = practitioner.Surname;
//            detailsResponse.OtherNames = practitioner.OtherNames ?? string.Empty;
//            detailsResponse.CountryId = item.CountryId;
//            detailsResponse.Country = item.Country;
//            detailsResponse.OdAddressVisibilityTypeId = item.OdAddressVisibilityTypeId;
//            detailsResponse.StreetDetails = item.StreetDetails;
//            detailsResponse.State = item.State ?? string.Empty;
//            detailsResponse.Suburb = item.Suburb ?? string.Empty;
//            detailsResponse.Postcode = item.Postcode ?? string.Empty;
//            detailsResponse.ContactDetails = GetContactDetails(practitioner);
//            detailsResponse.LegacyAccreditations = legacyAccreditations.Results.Select(MapLegacyAcreditation);
//            detailsResponse.Website = practitioner.Website;
//            return detailsResponse;
//        }

//        private AccreditationLegacy MapLegacyAcreditation(LegacyAccreditationDto acreditation)
//        {

//            var direction = string.Empty;
//            switch (acreditation.Direction?.ToUpper())
//            {
//                case "B":
//                    direction = $"{acreditation.Language1} to/from {acreditation.Language2}";
//                    break;
//                case "E":
//                    direction = $"{acreditation.Language1} to {acreditation.Language2}";
//                    break;
//                case "O":
//                    direction = $"{acreditation.Language2} to {acreditation.Language1}";
//                    break;
//            }

//            var result = new AccreditationLegacy
//            {
//                Level = acreditation.Level,
//                Category = acreditation.Category,
//                Direction = direction,
//                StartDate = acreditation.StartDate.ToString("dd MMM yyyy"),
//                EndDate = acreditation.ExpiryDate?.ToString("dd MMM yyyy")
//            };

//            return result;
//        }

//        public string GetPractitionerWorkAreas(PractitionerDirectoryGetContactDetailsRequest request)
//        {
//            return mPractitionersRepository.GetPractitionerWorkAreas(request.Identifier);
//        }
//    }
//}
