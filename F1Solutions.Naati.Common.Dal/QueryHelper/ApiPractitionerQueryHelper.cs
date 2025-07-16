using System;
using System.Collections.Generic;
using System.Linq;
using F1Solutions.Global.Common;
using F1Solutions.Global.Common.Mapping;
using F1Solutions.Naati.Common.Contracts.Bl;
using F1Solutions.Naati.Common.Contracts.Dal;
using F1Solutions.Naati.Common.Contracts.Dal.DTO;
using F1Solutions.Naati.Common.Contracts.Dal.Enum;
using F1Solutions.Naati.Common.Contracts.Dal.Request;
using F1Solutions.Naati.Common.Contracts.Dal.Response;
using F1Solutions.Naati.Common.Dal.Domain;
using F1Solutions.Naati.Common.Dal.NHibernate.Configuration;
using NHibernate;
using NHibernate.Criterion;
using Order = NHibernate.Criterion.Order;

namespace F1Solutions.Naati.Common.Dal.QueryHelper
{
    internal class ApiPractitionerQueryHelper : QuerySearchHelper
    {
        private readonly IAutoMapperHelper _autoMapperHelper;

        private ApiPublicPractitionerSearchDto mPractitionerSearchDto => null;
        private ApiPubicPractitionerCredentialTypeDto mPractitionerCredentialTypeDto => null;
        private ApiPublicPractitionerAddressDto mApiPractitionerAddressDto => null;
        private ApiPublicContactDetailsDto mPractitionerContactDetailDto => null;

        public ApiPractitionerQueryHelper(IAutoMapperHelper autoMapperHelper)
        {
            _autoMapperHelper = autoMapperHelper;
        }
        private Junction GetJunction(IEnumerable<ApiPublicPractitionerFilterSearchCriteria> filters)
        {
          
            var practitionerFiltersDictionary = new Dictionary<ApiPublicPractitionerFilterType, Func<ApiPublicPractitionerFilterSearchCriteria, Junction, Junction>>
            {
                [ApiPublicPractitionerFilterType.CredentialTypeIntList] = (criteria, previousJunction) => GetCredentialTypeFilter(criteria, previousJunction),
                [ApiPublicPractitionerFilterType.PersonIdIntList] = (criteria, previousJunction) => GetPersonIdFilter(criteria, previousJunction),
                [ApiPublicPractitionerFilterType.CountryIntList] = (criteria, previousJunction) => GetCountryFilter(criteria, previousJunction),
                [ApiPublicPractitionerFilterType.StateIntList] = (criteria, previousJunction) => GetStateFilter(criteria, previousJunction),
                [ApiPublicPractitionerFilterType.PostcodeIntList] = (criteria, previousJunction) => GetPostCodeFilter(criteria, previousJunction),
                [ApiPublicPractitionerFilterType.FamilyNameStringList] = (criteria, previousJunction) => GetFamilyNameFilter(criteria, previousJunction),
                [ApiPublicPractitionerFilterType.SkillIntList] = (criteria, previousJunction) => GetSkillFilter(criteria, previousJunction),
            };

            Junction junction = Restrictions.Conjunction();
            junction = junction.Add(Restrictions.IsNotNull(Projections.Property(() => Person.Id))); // default junction
            junction = junction.Add(Restrictions.Eq(Projections.Property(() => Person.Deceased), false)); // default junction


            var validCriterias = filters.Where(x => x.Values.Any(v => v != null)).ToList();
            if (validCriterias.Any())
            {
                foreach (var criteria in validCriterias)
                {
                    var junctionFunc = practitionerFiltersDictionary[criteria.Filter];
                    junction = junctionFunc(criteria, junction);
                }
            }

            return junction;
        }

        public IList<ApiPublicPractitionerSearchDto> SearchPractitioner(GetApiPublicPractitionerSearchRequest request)
        {
            var junction = GetJunction(request.Filters);
            var queryOver = BuildQuery();

            var orderingProjections = new List<KeyValuePair<IProjection, ApiPublicSortDirection>>();

            foreach (var sortingOption in request.SortingOptions)
            {
                var ordering = GetOrderingProjection(sortingOption.SortTypeId, request);
                orderingProjections.Add(new KeyValuePair<IProjection, ApiPublicSortDirection>(ordering, sortingOption.SortDirectionId));
                if (sortingOption.SortTypeId == ApiPublicSortType.RandomSeed)
                { // add twice, this improve performance, pending to investigate why
                    orderingProjections.Add(new KeyValuePair<IProjection, ApiPublicSortDirection>(ordering, sortingOption.SortDirectionId));
                }
            }

            var subquery = GetSubquery(queryOver, junction, request.Take, request.Skip, orderingProjections);

            var personIds = subquery.List<int>().ToList();
            queryOver = queryOver.Where(Restrictions.In(Projections.Property(() => Person.Id), personIds));

            foreach (var orderingProjection in orderingProjections)
            {
                var order = GetOrdering(orderingProjection.Key, orderingProjection.Value);
                queryOver.UnderlyingCriteria.AddOrder(order);
            }

            var projections = BuildPractitionerSearchProjections(request.RandomSeed);
            queryOver = queryOver.Select(projections.ToArray());

            var indices = projections.ToDictionary(k => k.Aliases.First(), v => projections.FindIndex(value => value == v));

            var results = queryOver.TransformUsing(new ApiPublicPractitionerSearchTransformer(indices)).List<ApiPublicPractitionerSearchDto>();

            return results;
        }

        private int CountTotalPeople(IQueryOver<Person, Person> queryOver, Junction junction)
        {
            var personId = Projections.Property(() => Person.Id);
            queryOver = queryOver.Select(Projections.Count(Projections.Distinct(personId)));
            queryOver = queryOver.Where(junction);
            var result = queryOver.List<int>()[0];
            return result;
        }

        private ICriterion GetValidOdCredentialRestriction()
        {
            var activeCredentialRestriction = GetActiveCredentialFilter();
            var showInOdRestriction = Restrictions.Eq(Projections.Property(() => Credential.ShowInOnlineDirectory), true);
            var certificationRestriction = Restrictions.Eq(Projections.Property(() => CredentialType.Certification), true);

            return Restrictions.And(Restrictions.And(activeCredentialRestriction, showInOdRestriction),
                certificationRestriction);
        }



        public IEnumerable<ApiPublicItemCountDto> GetCount(GetAPiPublicPractitionerCountRequest request)
        {
            var junction = GetJunction(request.Filters);
            var queryOver = BuildQuery();
            queryOver = queryOver.Where(junction);
            
            // Properties that are going to be used for the grouping of the query
            //var language1 = Projections.Property(() => Language1.Id);
            //var language2 = Projections.Property(() => Language2.Id);
            //var skillId = Projections.Property(() => Skill.Id);

            var addressCountry = Projections.Property(() => AddressCountry.Id);
            var stateId = Projections.Property(() => State.Id);
            var credentialTypeId = Projections.Property(() => CredentialType.Id);
            var personId = Projections.Property(() => Person.Id);
            
            // Columns that are going to be selected Todo Fix this:(Order matters. It will be use to build the dtos)
            var projections = new IProjection[]
            {
               // Projections.GroupProperty(language1),
               // Projections.GroupProperty(language2),
                Projections.GroupProperty(addressCountry),
                Projections.GroupProperty(stateId),
                Projections.GroupProperty(credentialTypeId),
               // Projections.GroupProperty(skillId),
                Projections.GroupProperty(Projections.GroupProperty(personId))
            };

            // Add ordering to the query
            queryOver = queryOver.Select(projections);
           // queryOver.UnderlyingCriteria.AddOrder(Order.Asc(language1));
          //  queryOver.UnderlyingCriteria.AddOrder(Order.Asc(language2));
            queryOver.UnderlyingCriteria.AddOrder(Order.Asc(addressCountry));
            queryOver.UnderlyingCriteria.AddOrder(Order.Asc(stateId));
            queryOver.UnderlyingCriteria.AddOrder(Order.Asc(credentialTypeId));
           // queryOver.UnderlyingCriteria.AddOrder(Order.Asc(skillId));
            
            //Gets the result from db
            var result = queryOver.List<object[]>();

            // Item count dto for alls count
            //var language1AllItemdDto = new ItemCountValueDto();
            //var language2AllItemdDto = new ItemCountValueDto();
            var countryAllItemDto = new ItemCountValueDto();
            var stateAllItemDto = new ItemCountValueDto();
            var credentialTypeAllItemDto = new ItemCountValueDto();

            //var language1Dtos = new Dictionary<int, ApiItemCountValueBuilder> { { 0, new ApiItemCountValueBuilder(language1AllItemdDto) } };
            //var language2Dtos = new Dictionary<int, ApiItemCountValueBuilder> { { 0, new ApiItemCountValueBuilder(language2AllItemdDto) } };
            var countriesDtos = new Dictionary<int, ApiItemCountValueBuilder> { { 0, new ApiItemCountValueBuilder(countryAllItemDto) } };
            var statesDtos = new Dictionary<int, ApiItemCountValueBuilder> { { 0, new ApiItemCountValueBuilder(stateAllItemDto) } };
            var credentialTypeDtos = new Dictionary<int, ApiItemCountValueBuilder> { { 0, new ApiItemCountValueBuilder(credentialTypeAllItemDto) } };
            
            // increments the count for each dto
            foreach (var item in result)
            {
                //IncrementDtoCount(language1Dtos, 0, item);
                //IncrementDtoCount(language2Dtos,1, item);
                IncrementDtoCount(countriesDtos,0, item);
                IncrementDtoCount(statesDtos, 1, item);
                IncrementDtoCount(credentialTypeDtos, 2, item);
               // IncrementCredentialTypeCount(credentialTypeDtos, item);
            }
            
            return new[]
            {
                new ApiPublicItemCountDto { Type = ApiPublicPractitionerCountLookupType.ByCredentialTypeId, Values = credentialTypeDtos.Values.Select(v=> v.Dto).OrderBy(x=> x.Id) },
                new ApiPublicItemCountDto { Type = ApiPublicPractitionerCountLookupType.ByCountryId, Values = countriesDtos.Values.Select(v=> v.Dto).OrderBy(x=> x.Id) },
                new ApiPublicItemCountDto { Type = ApiPublicPractitionerCountLookupType.ByStateId, Values = statesDtos.Values.Select(v=> v.Dto).OrderBy(x=> x.Id) },
            };
        }

        private void IncrementDtoCount(IDictionary<int, ApiItemCountValueBuilder> dictionary, int columnId,  object[] data)
        {
            var columnValue = data[columnId] as int?;
            var personId = (int) data[3];
            if (columnValue == null)
            {
                return;
            }

            ApiItemCountValueBuilder dtoBuilder;
            if (!dictionary.TryGetValue(columnValue.GetValueOrDefault(), out dtoBuilder))
            {
                dtoBuilder = new ApiItemCountValueBuilder(new ItemCountValueDto {Id = columnValue.GetValueOrDefault()});
                dictionary[dtoBuilder.Dto.Id] = dtoBuilder;
            }

            dictionary[0].AddPersonToCount(personId);
            dtoBuilder.AddPersonToCount(personId);
        }

        protected IQueryOver<Person, Person> GetSubquery(IQueryOver<Person, Person> queryOver, Junction junction,
            int? take, int? skip, IEnumerable<KeyValuePair<IProjection, ApiPublicSortDirection>> orderProjections)
        {
            var subQueryOver = queryOver.Clone();

            if (junction != null)
            {
                subQueryOver = subQueryOver.Where(junction);
            }

            var personId = Projections.Property(() => Person.Id);
            var grouping = Projections.GroupProperty(personId);

            subQueryOver = subQueryOver.Select(grouping);

            foreach (var orderProjection in orderProjections)
            {
                var order = GetOrdering(Projections.Max(orderProjection.Key), orderProjection.Value);
                subQueryOver.UnderlyingCriteria.AddOrder(order);
            }


            if (skip.HasValue)
            {
                subQueryOver.Skip(skip.Value);
            }

            if (take.HasValue)
            {
                subQueryOver.Take(take.Value);
            }

            return subQueryOver;
        }

        private IProjection GetLevelOrderingProjection(GetApiPublicPractitionerSearchRequest request)
        {
            var levelRestriction = Restrictions.Conjunction();

            var skillIds = request.Filters.FirstOrDefault(x => x.Filter == ApiPublicPractitionerFilterType.SkillIntList)?.ToList<ApiPublicPractitionerFilterType, int>().Where(x=> x != 0).ToList();
            if (skillIds != null && skillIds.Any())
            {
                levelRestriction.Add(Restrictions.In(Projections.Property(() => Skill.Id), skillIds));
            }

            var credentialTypesIds = request.Filters
                .FirstOrDefault(x => x.Filter == ApiPublicPractitionerFilterType.CredentialTypeIntList)
                ?.ToList<ApiPublicPractitionerFilterType, int>()
                .Where(x => x != 0).ToList();

            if (credentialTypesIds != null && credentialTypesIds.Any())
            {

                levelRestriction.Add(Restrictions.In(Projections.Property(() => CredentialType.Id), credentialTypesIds));
            }

            return Projections.Conditional(levelRestriction, Projections.Property(() => CredentialType.Level), Projections.Constant(-1, NHibernateUtil.Int32));

        }

        private IProjection GetOrderingProjection(ApiPublicSortType type, GetApiPublicPractitionerSearchRequest request)
        {
            switch (type)
            {
                case ApiPublicSortType.Level:
                    return GetLevelOrderingProjection(request);
                case ApiPublicSortType.State:
                    return Projections.Property(() => State.Name);
                case ApiPublicSortType.RandomSeed:
                    return GetPseudoRandomProjection(request.RandomSeed);
                default:
                    return Projections.Property(() => Person.Id);
            }

            //switch (type)
            //{
            //    case ApiPublicSortType.Level:
            //        return GetLevelOrderingProjection(request);
            //    case ApiPublicSortType.State:
            //        return Projections.Property(() => State.Id);
            //    case ApiPublicSortType.RandomSeed:
            //        return GetPseudoRandomProjection(request.RandomSeed);
            //    default:
            //        return Projections.Property(() => Person.Id);
            //}
        }

        private IProjection GetSearchHashProjection(int seed)
        {
            var hash = GetPseudoRandomProjection(seed);
            return Projections.Cast(NHibernateUtil.Int32, hash);
        }

        private IProjection GetPseudoRandomProjection(int seed)
        {
            var personIdProjection = Projections.Property(() => Entity.Id);
            var seedProjection = Projections.Constant(seed, NHibernateUtil.Int32);
            var sumProjection = SumProjections(personIdProjection, seedProjection);
            var sumString = Projections.Cast(NHibernateUtil.String, sumProjection);

            return GetMd5HashProjectionFor(sumString);
        }


        private Order GetOrdering(IProjection projection, ApiPublicSortDirection sortDirection)
        {
            if (sortDirection == ApiPublicSortDirection.Descending)
            {
                return Order.Desc(projection);
            }

            return Order.Asc(projection);
        }

        private IQueryOver<Person, Person> BuildQuery()
        {
            var queryOver = NHibernateSession.Current.QueryOver(() => Person)
                .Inner.JoinAlias(x => Person.Entity, () => Entity)
                .Inner.JoinAlias(x => Person.LatestPersonName, () => LatestPersonName)
                .Inner.JoinAlias(x => Person.CredentialApplications, () => CredentialApplication)
                .Inner.JoinAlias(x => LatestPersonName.PersonName, () => PersonName)
                .Left.JoinAlias(x => Entity.Emails, () => Email)
                .Left.JoinAlias(x => Entity.Phones, () => Phone)
                .Left.JoinAlias(x => Entity.Addresses, () => Address)
                .Left.JoinAlias(x => Address.OdAddressVisibilityType, () => OdAddressVisibilityType)
                .Left.JoinAlias(x => Address.Country, () => AddressCountry)
                .Left.JoinAlias(x => Address.Postcode, () => Postcode)
                .Left.JoinAlias(x => Postcode.Suburb, () => Suburb)
                .Left.JoinAlias(x => Suburb.State, () => State)
                .Left.JoinAlias(x => PersonName.Title, () => Title)
                .Inner.JoinAlias(x => CredentialApplication.CredentialRequests, () => CredentialRequest)
                .Inner.JoinAlias(x => CredentialRequest.CredentialType, () => CredentialType)
                .Inner.JoinAlias(x => CredentialRequest.CredentialCredentialRequests, () => CredentialCredentialRequest)
                .Inner.JoinAlias(x => CredentialCredentialRequest.Credential, () => Credential)
                .Inner.JoinAlias(x => CredentialRequest.Skill, () => Skill)
                .Inner.JoinAlias(x => Skill.Language1, () => Language1)
                .Inner.JoinAlias(x => Skill.Language2, () => Language2)
                .Inner.JoinAlias(x => Skill.DirectionType, () => DirectionType)
                .Left.JoinAlias(x => Credential.CertificationPeriod, () => CertificationPeriod)
               .Where(GetValidOdCredentialRestriction())
               .Where(GetValidAddressRestriction());

            return queryOver;
        }
        private ICriterion GetValidAddressRestriction()
        {
            var validAddress = Restrictions.Eq(Projections.Property(() => Address.Invalid), false);
            var pdAddress = Restrictions.Not(Restrictions.Eq(Projections.Property(() => Address.OdAddressVisibilityType.Id), (int)OdAddressVisibilityTypeName.DoNotShow));

            var primaryAddress = Restrictions.Eq(Projections.Property(() => Address.PrimaryContact), true);

            var entitiesWithoutPd = Subqueries.WhereProperty<NaatiEntity>(e => Entity.Id).In(GetEntitiesIdsWithoutPdAddress());
            var primaryEntityAddress = Restrictions.And(primaryAddress, entitiesWithoutPd);
            return Restrictions.And(validAddress, Restrictions.Or(pdAddress, primaryEntityAddress));
        }
        
        private QueryOver<NaatiEntity, NaatiEntity> GetEntitiesIdsWithoutPdAddress()
        {
            NaatiEntity subQueryEntity = null;
            Address subQueryAddress = null;
            OdAddressVisibilityType visibilityType = null;

            //Todo: Review if this is really need with the new implementation.
            var addressOdType = Projections.Property(() => visibilityType.Id);
            var includeInOd =
                Projections.Conditional(Restrictions.Eq(addressOdType, (int) OdAddressVisibilityTypeName.DoNotShow),
                    Projections.Constant(0, NHibernateUtil.Int32), Projections.Constant(1, NHibernateUtil.Int32));
          
            var totalAddress = Projections.Sum(includeInOd);

            return QueryOver.Of(() => subQueryEntity)
                .Inner.JoinAlias(() => subQueryEntity.Addresses, () => subQueryAddress)
                .Inner.JoinAlias(() => subQueryAddress.OdAddressVisibilityType, () => visibilityType)

                .Where(e => e.EntityTypeId == 1)
                .Where(Restrictions.Eq(Projections.Property(() => subQueryAddress.Invalid), false))
                .Where(Restrictions.Eq(totalAddress, 0))
                .Select(Projections.GroupProperty(Projections.Property(() => subQueryEntity.Id)));
        }


        private List<IProjection> BuildPractitionerSearchProjections(int seed)
        {
            return new List<IProjection>
            {
                Projections.Property(() => Person.Id).WithAlias(() => mPractitionerSearchDto.PersonId),
                GetSurnameProjection().WithAlias(() => mPractitionerSearchDto.Surname),
                Projections.Property(() => PersonName.GivenName).WithAlias(() => mPractitionerSearchDto.GivenName),
                Projections.Property(() => PersonName.OtherNames).WithAlias(() => mPractitionerSearchDto.OtherNames),
                Projections.Property(() => Title.TitleName).WithAlias(() => mPractitionerSearchDto.Title),
                
                Projections.Property(() => CredentialType.ExternalName).WithAlias(() => mPractitionerCredentialTypeDto.ExternalName),
                
                Projections.Property(() => CredentialType.DisplayOrder).WithAlias(() => mPractitionerCredentialTypeDto.DisplayOrder),
                Projections.Property(() => Skill.Id).WithAlias(()=> mPractitionerCredentialTypeDto.SkillId),
                GetDirectionProjection().WithAlias(()=>mPractitionerCredentialTypeDto.Skill),

                Projections.Property(() => Address.Id).WithAlias(() => mApiPractitionerAddressDto.AddressId),
                Projections.Property(() => Address.PrimaryContact).WithAlias(() => mApiPractitionerAddressDto.IsPrimaryAddress),
                Projections.Property(() => Suburb.Name).WithAlias(() => mApiPractitionerAddressDto.Suburb),
                Projections.Property(() => State.Name).WithAlias(() => mApiPractitionerAddressDto.State),
                Projections.Property(() => Postcode.PostCode).WithAlias(() => mApiPractitionerAddressDto.Postcode),

                Projections.Property(() => Address.StreetDetails).WithAlias(() => mApiPractitionerAddressDto.StreetDetails),
                Projections.Property(() => AddressCountry.Name).WithAlias(() => mApiPractitionerAddressDto.Country),

                Projections.Property(() => OdAddressVisibilityType.Id).WithAlias(() => mApiPractitionerAddressDto.OdAddressVisibilityTypeId),
                GetPhoneProjection().WithAlias(() => mPractitionerContactDetailDto.PhonesInPd),
                GetEmailProjection().WithAlias(() => mPractitionerContactDetailDto.EmailsInPd),
                Projections.Property(() => Person.Deceased).WithAlias(() => mPractitionerSearchDto.Deceased),
                Projections.Property(() => Entity.WebsiteUrl).WithAlias(()=> mPractitionerContactDetailDto.WebsiteUrlInPd),
                
                //Projections.Property(() => Address.Id).WithAlias(() => mApiPractitionerAddressDto.AddressId),
                //Projections.Property(() => CredentialType.UpgradePath).WithAlias(() => mPractitionerCredentialTypeDto.UpgradePath),
                //Projections.Property(() => State.Id).WithAlias(() => mApiPractitionerAddressDto.StateId),
                //Projections.Property(() => AddressCountry.Id).WithAlias(() => mApiPractitionerAddressDto.CountryId),
                //Projections.Property(() => Entity.NaatiNumber).WithAlias(()=> mPractitionerSearchDto.NaatiNumber),
               
                //GetSearchHashProjection(seed).WithAlias(()=> mPractitionerSearchDto.Hash),
                //Projections.Property(() => Person.ShowPhotoOnline).WithAlias(() => mPractitionerSearchDto.ShowPhotoOnline),

            };
        }

        protected override IProjection GetPhoneProjection()
        {
            var phoneFunction = Projections.SqlFunction("CONCAT", NHibernateUtil.String,
                Projections.Property(() => Phone.CountryCode),
                Projections.Constant(" "),
                Projections.Property(() => Phone.AreaCode),
                Projections.Constant(" "),
                Projections.Property(() => Phone.LocalNumber));

            return Projections.Conditional(
                Restrictions.And(
                    Restrictions.Eq(Projections.Property(() => Phone.Invalid), false),
                    Restrictions.Eq(Projections.Property(() => Phone.IncludeInPD), true)
                ),

                phoneFunction,
                Projections.Constant(null, NHibernateUtil.String));
        }

        protected IProjection GetEmailProjection()
        {
            var emailProperty = Projections.Property(() => Email.EmailAddress);

            return Projections.Conditional(
                Restrictions.And(
                    Restrictions.Eq(Projections.Property(() => Email.Invalid), false),
                    Restrictions.Eq(Projections.Property(() => Email.IncludeInPD), true)
                ),
                emailProperty,
                Projections.Constant(null, NHibernateUtil.String));
        }

        private Junction GetCredentialTypeFilter<S>(ISearchCriteria<S> criteria, Junction junction)
        {
            var credentialTypesIds = criteria.ToList<S, int>();

            credentialTypesIds = credentialTypesIds.Concat(GetUpgradePathCredentialTypes(credentialTypesIds)).ToList();

            junction.Add<CredentialType>(p => CredentialType.Id.IsIn(credentialTypesIds));
            return junction;
        }

        public List<int> GetUpgradePathCredentialTypes(List<int> credentialTypesIds)
        {
            var result = new List<int>();
           // result.AddRange(credentialTypesIds);

            var list = NHibernateSession.Current.Query<CredentialTypeUpgradePath>().ToList();

            foreach (var typeId in credentialTypesIds)
            {
                var current = list.FirstOrDefault(x => x.CredentialTypeFrom.Id == typeId);

                if (current != null)
                {
                    if (!result.Contains(current.CredentialTypeTo.Id))
                        result.Add(current.CredentialTypeTo.Id);
                    while (current != null)
                    {
                        var index = list.FindIndex(x => x.CredentialTypeFrom.Id == current.CredentialTypeTo.Id);
                        if (index != -1)
                        {
                            current = list[index];
                            if (!result.Contains(current.CredentialTypeTo.Id))
                                result.Add(list[index].CredentialTypeTo.Id);
                        }
                        else
                        {
                            current = null;
                        }
                    }
                }
            }
            return result;
        }

        private Junction GetLanguage1Filter<S>(ISearchCriteria<S> criteria, Junction junction)
        {
            var typeList = criteria.ToList<S, int>();
            junction.Add<Language>(p => Language1.Id.IsIn(typeList));
            return junction;
        }

        private Junction GetLanguage2Filter<S>(ISearchCriteria<S> criteria, Junction junction)
        {
            var typeList = criteria.ToList<S, int>();
            junction.Add<Language>(p => Language2.Id.IsIn(typeList));
            return junction;
        }

        private Junction GetCountryFilter<S>(ISearchCriteria<S> criteria, Junction junction)
        {
            var typeList = criteria.ToList<S, int>();
            var countryProjection = Projections.Property(() => AddressCountry.Id);
            var countryRestriction = Restrictions.And(Restrictions.IsNotNull(countryProjection), Restrictions.In(countryProjection, typeList));
            // junction.Add(Restrictions.EqProperty(Projections.Property(()=> Address.Id), GetPdAddressProjection() ));
            junction.Add(countryRestriction);
            return junction;
        }

        private Junction GetPostCodeFilter<S>(ISearchCriteria<S> criteria, Junction junction)
        {
            var odAddressVisibilityTypeProjection = Projections.Property(() => OdAddressVisibilityType.Id);

            var postCodeList = criteria.Values.ToList();
            var postalCodeProjection = Projections.Property(() => Postcode.PostCode);
            var postcodeId = Projections.Property(() => Postcode.Id);

            var postalCodeRestriction = Restrictions.Conjunction();
            postalCodeRestriction.Add(Restrictions.IsNotNull(postcodeId));
            postalCodeRestriction.Add(Restrictions.In(postalCodeProjection, postCodeList));

            postalCodeRestriction.Add(Restrictions.Or(
                Restrictions.Eq(odAddressVisibilityTypeProjection, (int)OdAddressVisibilityTypeName.StateAndSuburb),
                Restrictions.Eq(odAddressVisibilityTypeProjection, (int)OdAddressVisibilityTypeName.FullAddress)));


            junction.Add(postalCodeRestriction);

            return junction;
        }

        private Junction GetFamilyNameFilter<S>(ISearchCriteria<S> criteria, Junction junction)
        {
            var familyNameList = criteria.Values.ToList();
            var familyNameProjection = Projections.Property(()=>Person.Surname);

            var filter = Restrictions.In(familyNameProjection, familyNameList);

            return junction.Add(filter);
        }

        private Junction GetStateFilter<S>(ISearchCriteria<S> criteria, Junction junction)
        {
            var typeList = criteria.ToList<S, int>();
            var stateIdProjection = Projections.Property(() => State.Id);
            var stateRestriction = Restrictions.And(Restrictions.IsNotNull(stateIdProjection), Restrictions.In(stateIdProjection, typeList));
            junction.Add(stateRestriction);
            return junction;
        }

        private Junction GetPersonIdFilter<S>(ISearchCriteria<S> criteria, Junction junction)
        {
            var typeList = criteria.ToList<S, int>();
            junction.Add<Person>(p => Person.Id.IsIn(typeList));
            return junction;
        }
        
        private IEnumerable<LookupDto> GetCountries()
        {
            var countries = NHibernateSession.Current.Query<Country>().OrderBy(x => x.Name)
                .Select(x => new LookupDto { Id = x.Id, DisplayName = x.Name }).ToList();
            return countries;
        }

        private IEnumerable<LookupDto> GetStates()
        {
            var countries = NHibernateSession.Current.Query<State>().OrderBy(x => x.Name)
                .Select(x => new LookupDto { Id = x.Id, DisplayName = x.Name }).ToList();
            return countries;
        }
        
        private IEnumerable<LookupDto> GetPostcode()
        {
            var postcodes = NHibernateSession.Current.Query<Postcode>().Where(x=> x.PostCode != "") .OrderBy(x => x.PostCode)
                .Select(x => new LookupDto { Id = x.Id, DisplayName = x.PostCode }).ToList();
            return postcodes;
        }

        private IEnumerable<LookupDto> GetTestLocation()
        {
            var testLocations = NHibernateSession.Current.Query<TestLocation>().OrderBy(x => x.Name)
                .Select(x => new LookupDto { Id = x.Id, DisplayName = x.Name }).ToList();
            return testLocations;
        }

        private IEnumerable<LookupDto> GetCredentialTypes()
        {
            var credentialTypes = NHibernateSession.Current.Query<CredentialType>().OrderBy(x => x.ExternalName)
                .Select(x => new LookupDto { Id = x.Id, DisplayName = x.ExternalName}).ToList();
            return credentialTypes;
        }

        public IEnumerable<LookupDto> GetLookup(ApiPublicLookupType lookupType)
        {
            switch (lookupType)
            {
                case ApiPublicLookupType.Country:
                    return GetCountries();
                case ApiPublicLookupType.State:
                    return GetStates();
                case ApiPublicLookupType.CredentialType:
                    return GetCredentialTypes();
                case ApiPublicLookupType.Postcode:
                    return GetPostcode();

                case ApiPublicLookupType.TestLocation:
                    return GetTestLocation();

                default:
                    throw new NotSupportedException();
            }
        }

        public GetLegacyAccreditionsResponse GetLegacyAccreditions(GetLegacyAccreditionsRequest request)
        {
            var naatiNumber = NHibernateSession.Current.Get<Person>(request.PersonId)?.Entity.NaatiNumber ?? 0;
            var acreditations = NHibernateSession.Current.Query<LegacyAccreditation>()
                .Where(x => x.NAATINumber == naatiNumber).ToList();
      
            return new GetLegacyAccreditionsResponse { Results = acreditations.Select(_autoMapperHelper.Mapper.Map<LegacyAccreditationDto>) };
        }

        public LanguagesResponse GetLanguages(LanguagesRequest request)
        {
            var credentialIds = request.CredentialTypeIds.Distinct().ToList();

            var skillTypeIds = NHibernateSession.Current.Query<CredentialType>().Where(x=> credentialIds.Contains(x.Id)).Select(x => x.SkillType.Id).ToList();
            
            var apiLanguage = NHibernateSession.Current.Query<Skill>().Where(x => skillTypeIds.Contains(x.SkillType.Id))
                .Select(y => new { SkillId = y.Id, DirectionType = y.DirectionType.DisplayName, Language1 = y.Language1.Name,  Language2 = y.Language2.Name  }).ToList()
                .Select(z => new { DisplayName = z.DirectionType.Replace("[Language 1]", z.Language1 ).Replace("[Language 2]", z.Language2), SkillId = z.SkillId})
                .GroupBy(w=> w.DisplayName).Select(z => new ApiLanguage()
                {
                    DisplayName = z.Key,
                    SkillIds = z.Select(sk => sk.SkillId).ToList()
                });

            return new LanguagesResponse
            {
                Results = apiLanguage
            };
        }
    }

    internal class ApiItemCountValueBuilder
    {
        public ItemCountValueDto Dto { get; }
        private readonly HashSet<int> mPersonIds = new HashSet<int>();

        public void AddPersonToCount(int personId)
        {
            mPersonIds.Add(personId);
            Dto.Count = mPersonIds.Count;
        }

        public ApiItemCountValueBuilder(ItemCountValueDto dto)
        {
            Dto = dto;
        }
    }

}
