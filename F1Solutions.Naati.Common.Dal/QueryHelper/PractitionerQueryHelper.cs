using System;
using System.Collections.Generic;
using System.Linq;
using F1Solutions.Global.Common;
using F1Solutions.Naati.Common.Contracts.Dal;
using F1Solutions.Naati.Common.Contracts.Dal.DTO;
using F1Solutions.Naati.Common.Contracts.Dal.Enum;
using F1Solutions.Naati.Common.Contracts.Dal.Request;
using F1Solutions.Naati.Common.Dal.Domain;
using F1Solutions.Naati.Common.Dal.NHibernate.Configuration;
using NHibernate;
using NHibernate.Criterion;
using Order = NHibernate.Criterion.Order;

namespace F1Solutions.Naati.Common.Dal.QueryHelper
{
    internal class PractitionerQueryHelper : QuerySearchHelper
    {
        private PractitionerSearchDto mPractitionerSearchDto => null;
        private PractitionerAddressDto mPractitionerAddressDto => null;
        private PractitionerCredentialTypeDto mPractitionerCredentialTypeDto => null;

        private Junction GetJunction(GetPractitionerCountRequest request)
        {
          
            var personFiltersDictionary = new Dictionary<PractitionerFilterType, Func<PractitionerFilterSearchCriteria, Junction, Junction>>
            {
                [PractitionerFilterType.Language1IntList] = (criteria, previousJunction) => GetLanguage1Filter(criteria, previousJunction),
                [PractitionerFilterType.Language2IntList] = (criteria, previousJunction) => GetLanguage2Filter(criteria, previousJunction),
                [PractitionerFilterType.CredentialTypeAndSkillIntList] = (criteria, previousJunction) => GetCredentialTypeAndSkillFilter(criteria, previousJunction),
                [PractitionerFilterType.PersonIdIntList] = (criteria, previousJunction) => GetPersonIdFilter(criteria, previousJunction),
                [PractitionerFilterType.CountryIntList] = (criteria, previousJunction) => GetCountryFilter(criteria, previousJunction),
                [PractitionerFilterType.StateIntList] = (criteria, previousJunction) => GetStateFilter(criteria, previousJunction),
                [PractitionerFilterType.PostcodeString] = (criteria, previousJunction) => GetPostCodeFilter(criteria, previousJunction),
                [PractitionerFilterType.FamilyNameString] = (criteria, previousJunction) => previousJunction.Add<PersonName>(p => PersonName.Surname.IsLike(criteria.Values.FirstOrDefault(), MatchMode.Anywhere)),
                [PractitionerFilterType.SkillIntList] = (criteria, previousJunction) => GetSkillFilter(criteria, previousJunction),
            };

            Junction junction = Restrictions.Conjunction();
            junction = junction.Add(Restrictions.IsNotNull(Projections.Property(() => Person.Id))); // default junction

            var validCriterias = request.Filters.Where(x => x.Values.Any(v => v != null)).ToList();
            if (validCriterias.Any())
            {
                foreach (var criteria in validCriterias)
                {
                    var junctionFunc = personFiltersDictionary[criteria.Filter];
                    junction = junctionFunc(criteria, junction);
                }
            }
            

            return junction;
        }

        public KeyValuePair<int, IList<PractitionerSearchDto>> SearchPractitioner(GetPractitionerSearchRequest request)
        {
            var junction = GetJunction(request);
            var queryOver = BuildQuery();
            var total = CountTotalPeople(queryOver.Clone(), junction);

            var orderingProjections = new List<KeyValuePair<IProjection, SortDirection>>();

            foreach (var sortingOption in request.SortingOptions)
            {
                var ordering = GetOrderingProjection(sortingOption.SortType, request);
                orderingProjections.Add(new KeyValuePair<IProjection, SortDirection>(ordering, sortingOption.SortDirection));
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

            var indices = projections.ToDictionary(k => k.Aliases.First(),
                v => projections.FindIndex(value => value == v));

            var results = queryOver.TransformUsing(new PractitionerSearchTransformer(indices))
                .List<PractitionerSearchDto>();
            return new KeyValuePair<int, IList<PractitionerSearchDto>>(total, results);
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
            var deceasedRestriction = Restrictions.Eq(Projections.Property(() => Person.Deceased), false);

            return Restrictions.Conjunction()
                .Add(activeCredentialRestriction)
                .Add(showInOdRestriction)
                .Add(certificationRestriction)
                .Add(deceasedRestriction);
        }



        public IEnumerable<ItemCountDto> GetCount(GetPractitionerCountRequest request)
        {
            // Get filter for the query
            var junction = GetJunction(request);
            //Build query
            var queryOver = BuildQuery();
            //Add filter to the query
            queryOver = queryOver.Where(junction);

            // Properties that are going to be used for the grouping of the query
            var language1 = Projections.Property(() => Language1.Id);
            var language2 = Projections.Property(() => Language2.Id);
            var addressCountry = Projections.Property(() => AddressCountry.Id);
            var stateId = Projections.Property(() => State.Id);
            var credentialTypeId = Projections.Property(() => CredentialType.Id);
            var skillId = Projections.Property(() => Skill.Id);
            var personId = Projections.Property(() => Person.Id);
            
            // Columns that are going to be selected Todo Fix this:(Order matters. It will be use to build the dtos)
            var projections = new IProjection[]
            {
                Projections.GroupProperty(language1),
                Projections.GroupProperty(language2),
                Projections.GroupProperty(addressCountry),
                Projections.GroupProperty(stateId),
                Projections.GroupProperty(credentialTypeId),
                Projections.GroupProperty(skillId),
                Projections.GroupProperty(Projections.GroupProperty(personId))
            };

            // Add ordering to the query
            queryOver = queryOver.Select(projections);
            queryOver.UnderlyingCriteria.AddOrder(Order.Asc(language1));
            queryOver.UnderlyingCriteria.AddOrder(Order.Asc(language2));
            queryOver.UnderlyingCriteria.AddOrder(Order.Asc(addressCountry));
            queryOver.UnderlyingCriteria.AddOrder(Order.Asc(stateId));
            queryOver.UnderlyingCriteria.AddOrder(Order.Asc(credentialTypeId));
            queryOver.UnderlyingCriteria.AddOrder(Order.Asc(skillId));
            
            //Gets the result from db
            var result = queryOver.List<object[]>();

            // Item count dto for alls count
            var language1AllItemdDto = new ItemCountValueDto();
            var language2AllItemdDto = new ItemCountValueDto();
            var countryAllItemDto = new ItemCountValueDto();
            var stateAllItemDto = new ItemCountValueDto();
            var credentialTypeAllItemDto = new ItemCountValueDto();

            var language1Dtos = new Dictionary<int, ItemCountValueBuilder> { { 0, new ItemCountValueBuilder(language1AllItemdDto) } };
            var language2Dtos = new Dictionary<int, ItemCountValueBuilder> { { 0, new ItemCountValueBuilder(language2AllItemdDto) } };
            var countriesDtos = new Dictionary<int, ItemCountValueBuilder> { { 0, new ItemCountValueBuilder(countryAllItemDto) } };
            var statesDtos = new Dictionary<int, ItemCountValueBuilder> { { 0, new ItemCountValueBuilder(stateAllItemDto) } };
            var credentialTypeDtos = new Dictionary<int, ItemCountValueBuilder> { { 0, new ItemCountValueBuilder(credentialTypeAllItemDto) } };
            
            // increments the count for each dto
            foreach (var item in result)
            {
                IncrementDtoCount(language1Dtos, 0, item);
                IncrementDtoCount(language2Dtos,1, item);
                IncrementDtoCount(countriesDtos,2, item);
                IncrementDtoCount(statesDtos, 3, item);
                IncrementCredentialTypeCount(credentialTypeDtos, item);
            }
            
            return new[]
            {
                new ItemCountDto { Type = PractitionerLookupType.ActiveCredentialsLanguages1, Values = language1Dtos.Values.Select(v=> v.Dto).OrderBy(x=> x.Id) },
                new ItemCountDto { Type = PractitionerLookupType.ActiveCredentialsLanguages2, Values = language2Dtos.Values.Select(v=> v.Dto).OrderBy(x=> x.Id) },
                new ItemCountDto { Type = PractitionerLookupType.ActiveCredentialsCountries, Values = countriesDtos.Values.Select(v=> v.Dto).OrderBy(x=> x.Id) },
                new ItemCountDto { Type = PractitionerLookupType.ActiveCredentialsStates, Values = statesDtos.Values.Select(v=> v.Dto).OrderBy(x=> x.Id) },
                new ItemCountDto { Type = PractitionerLookupType.ActiveCredentialsTypesByRequest, Values = credentialTypeDtos.Values.Select(v=> v.Dto).OrderBy(x=> x.Id) }
            };
        }

        private void IncrementDtoCount(IDictionary<int, ItemCountValueBuilder> dictionary, int columnId,  object[] data)
        {
            var columnValue = data[columnId] as int?;
            var personId = (int) data[6];
            if (columnValue == null)
            {
                return;
            }

            ItemCountValueBuilder dtoBuilder;
            if (!dictionary.TryGetValue(columnValue.GetValueOrDefault(), out dtoBuilder))
            {
                dtoBuilder = new ItemCountValueBuilder(new ItemCountValueDto {Id = columnValue.GetValueOrDefault()});
                dictionary[dtoBuilder.Dto.Id] = dtoBuilder;
            }

            dictionary[0].AddPersonToCount(personId);
            dtoBuilder.AddPersonToCount(personId);
        }

        private void IncrementCredentialTypeCount(IDictionary<int, ItemCountValueBuilder> dictionary, object[] data)
        {
            var skillId = (int)data[5];
            var credentialTypeId = (int)data[4];
            var personId = (int)data[6];

            ItemCountValueBuilder dtoBuilder;

            // Note: Uses union of credentialtype-skill Id as key of the dictionary
            var identifier = int.Parse(credentialTypeId.ToString().PadLeft(5, '0') + skillId.ToString().PadLeft(5, '0'));
            if (!dictionary.TryGetValue(identifier, out dtoBuilder))

            {   
                dtoBuilder = new ItemCountValueBuilder(new ItemCountValueDto { Id = identifier });
                dictionary[identifier] = dtoBuilder;
            }

            dictionary[0].AddPersonToCount(personId);
            dtoBuilder.AddPersonToCount(personId);
        }

        protected IQueryOver<Person, Person> GetSubquery(IQueryOver<Person, Person> queryOver, Junction junction,
            int? take, int? skip, IEnumerable<KeyValuePair<IProjection, SortDirection>> orderProjections)
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

        private IProjection GetLevelOrderingProjection(GetPractitionerSearchRequest request)
        {
            var levelRestriction = Restrictions.Conjunction();

            var skillIds = request.Filters.FirstOrDefault(x => x.Filter == PractitionerFilterType.SkillIntList)?.ToList<PractitionerFilterType, int>().Where(x=> x != 0).ToList();
            if (skillIds != null && skillIds.Any())
            {
                levelRestriction.Add(Restrictions.In(Projections.Property(() => Skill.Id), skillIds));
            }
            var credentialTypesIds = request.Filters.FirstOrDefault(x => x.Filter == PractitionerFilterType.CredentialTypeAndSkillIntList)?.ToList<PractitionerFilterType, int>().
                Select(x => x == 0 ? 0 : int.Parse(x.ToString().
                Remove(x.ToString().Length - 5))).Where(x => x != 0).ToList().ToList();

            if (credentialTypesIds != null && credentialTypesIds.Any())
            {

                levelRestriction.Add(Restrictions.In(Projections.Property(() => CredentialType.Id), credentialTypesIds));
            }

            return Projections.Conditional(levelRestriction, Projections.Property(() => CredentialType.Level), Projections.Constant(-1, NHibernateUtil.Int32));

        }

        private IProjection GetOrderingProjection(SortType type, GetPractitionerSearchRequest request)
        {
            switch (type)
            {
                case SortType.Level:

                    return GetLevelOrderingProjection(request);

                case SortType.State:
                    return Projections.Property(() => State.Name);
                case SortType.Suburb:
                    return Projections.Property(() => Suburb.Name);
                default:
                    return GetPseudoRandomProjection(request.RandomSeed);
            }
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


        private Order GetOrdering(IProjection projection, SortDirection sortDirection)
        {
            if (sortDirection == SortDirection.Descending)
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
                Projections.Property(() => Title.TitleName).WithAlias(() => mPractitionerSearchDto.Title),
                Projections.Property(() => PersonName.GivenName).WithAlias(() => mPractitionerSearchDto.GivenName),
                Projections.Property(() => PersonName.OtherNames).WithAlias(() => mPractitionerSearchDto.OtherNames),
                GetSurnameProjection().WithAlias(() => mPractitionerSearchDto.Surname),
                Projections.Property(() => CredentialType.Id).WithAlias(() => mPractitionerCredentialTypeDto.CredentialTypeId),
                Projections.Property(() => CredentialType.ExternalName).WithAlias(() => mPractitionerCredentialTypeDto.ExternalName),
                //Projections.Property(() => CredentialType.UpgradePath).WithAlias(() => mPractitionerCredentialTypeDto.UpgradePath),
                Projections.Property(() => CredentialType.DisplayOrder).WithAlias(() => mPractitionerCredentialTypeDto.DisplayOrder),
                GetDirectionProjection().WithAlias(() => mPractitionerCredentialTypeDto.Direction),
                Projections.Property(() => Address.Id).WithAlias(() => mPractitionerAddressDto.AddressId),
                Projections.Property(() => AddressCountry.Id).WithAlias(() => mPractitionerAddressDto.CountryId),
                Projections.Property(() => Address.StreetDetails).WithAlias(() => mPractitionerAddressDto.StreetDetails),
                Projections.Property(() => AddressCountry.Name).WithAlias(() => mPractitionerAddressDto.Country),
                Projections.Property(() => Suburb.Name).WithAlias(() => mPractitionerAddressDto.Suburb),
                Projections.Property(() => State.Name).WithAlias(() => mPractitionerAddressDto.State),
                Projections.Property(() => State.Id).WithAlias(() => mPractitionerAddressDto.StateId),
                Projections.Property(() => Postcode.PostCode).WithAlias(() => mPractitionerAddressDto.Postcode),
                Projections.Property(() => Address.PrimaryContact).WithAlias(() => mPractitionerAddressDto.IsPrimaryAddress),
                Projections.Property(() => OdAddressVisibilityType.Id).WithAlias(() => mPractitionerAddressDto.OdAddressVisibilityTypeId),
                GetPhoneProjection().WithAlias(() => mPractitionerSearchDto.PhonesInPd),
                GetEmailProjection().WithAlias(() => mPractitionerSearchDto.EmailsInPd),
                Projections.Property(() => Person.ShowPhotoOnline).WithAlias(() => mPractitionerSearchDto.ShowPhotoOnline),
                Projections.Property(() => Person.Deceased).WithAlias(() => mPractitionerSearchDto.Deceased),
                Projections.Property(() => Language1.Id).WithAlias(()=> mPractitionerSearchDto.Language1Ids),
                Projections.Property(() => Language2.Id).WithAlias(()=> mPractitionerSearchDto.Language2Ids),
                Projections.Property(() => Entity.NaatiNumber).WithAlias(()=> mPractitionerSearchDto.NaatiNumber),
                Projections.Property(() => Skill.Id).WithAlias(()=> mPractitionerCredentialTypeDto.SkillId),
                Projections.Property(() => Entity.WebsiteUrl).WithAlias(()=> mPractitionerSearchDto.Website),
                GetSearchHashProjection(seed).WithAlias(()=> mPractitionerSearchDto.Hash),

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

        private Junction GetCredentialTypeAndSkillFilter<S>(ISearchCriteria<S> criteria, Junction junction)
        {
            var typeList = criteria.ToList<S, int>();
            
            var credentialTypesIds = typeList.Select(x => x == 0 ? 0 : int.Parse(x.ToString().Remove(x.ToString().Length - 5))).ToList();

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

            var postalCodeProjection = Projections.Property(() => Postcode.PostCode);
            var postalCodeRestriction = Restrictions.And(Restrictions.IsNotNull(postalCodeProjection), Restrictions.Like(postalCodeProjection, criteria.Values.FirstOrDefault(), MatchMode.Start));
            
            junction.Add(postalCodeRestriction);
            junction.Add(Restrictions.Or(
                Restrictions.Eq(odAddressVisibilityTypeProjection, (int)OdAddressVisibilityTypeName.StateAndSuburb),
                Restrictions.Eq(odAddressVisibilityTypeProjection, (int)OdAddressVisibilityTypeName.FullAddress)));
            return junction;
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

        private IEnumerable<LookupTypeDto> GetLanguages(IList<int> filteredIds)
        {
            var languages = NHibernateSession.Current.Query<Language>().Where(x => filteredIds.Contains(x.Id)).ToList()
                .Select(x =>
                {
                    var groupName = x.LanguageGroup != null ? " (" + x.LanguageGroup.Name + ")" : string.Empty;
                    return new LookupTypeDto { Id = x.Id, DisplayName = $"{x.Name}{groupName}" };
                }).OrderBy(x => x.DisplayName).ToList();

            return languages;
        }

        private IEnumerable<LookupTypeDto> GetCountries(IList<int> filteredIds)
        {
            var countries = NHibernateSession.Current.Query<Country>().Where(x => filteredIds.Contains(x.Id)).OrderBy(x => x.Name)
                .Select(x => new LookupTypeDto { Id = x.Id, DisplayName = x.Name }).ToList();
            return countries;
        }

        private IEnumerable<LookupTypeDto> GetStates(IList<int> filteredIds)
        {
            var countries = NHibernateSession.Current.Query<State>().Where(x => filteredIds.Contains(x.Id)).OrderBy(x => x.Name)
                .Select(x => new LookupTypeDto { Id = x.Id, DisplayName = x.Name }).ToList();
            return countries;
        }

        private IEnumerable<LookupTypeDto> GetCredentialTypesBySkills(IList<int> filteredIds)
        {
            var skillIds = filteredIds.Select(x => x== 0 ? 0 : int.Parse(x.ToString().Substring(x.ToString().Length - 5))).ToList();
            var credentialTypesIds = filteredIds.Select(x => x == 0 ? 0 : int.Parse(x.ToString().Remove(x.ToString().Length - 5))).ToList();

            var skills = NHibernateSession.Current.Query<Skill>().Where(x => skillIds.Contains(x.Id)).ToDictionary(x=> x.Id, y=> y.DisplayName);
            var credentialTypes= NHibernateSession.Current.Query<CredentialType>().Where(x => credentialTypesIds.Contains(x.Id)).ToDictionary(x => x.Id, y => y.ExternalName);
            var dtos = new List<LookupTypeDto>();

            for (var i = 1; i < filteredIds.Count; i++)
            {
                dtos.Add(new LookupTypeDto { Id = filteredIds[i], DisplayName = string.Concat(credentialTypes[credentialTypesIds[i]], " ", skills[skillIds[i]]), ExtraData = skills[skillIds[i]] });
            }
  
            return dtos.OrderBy(x=> x.DisplayName );
        }

        public IEnumerable<LookupTypeDto> GetLookup(PractitionerLookupType lookupType)
        {
            var response = GetCount(new GetPractitionerCountRequest { Filters = Enumerable.Empty<PractitionerFilterSearchCriteria>() });
            var filterItemsIds = response.First(v => v.Type == lookupType).Values.Select(x => x.Id).ToList();

            switch (lookupType)
            {
                case PractitionerLookupType.ActiveCredentialsLanguages1:
                case PractitionerLookupType.ActiveCredentialsLanguages2:
                    return GetLanguages(filterItemsIds);
                case PractitionerLookupType.ActiveCredentialsCountries:
                    return GetCountries(filterItemsIds);
                case PractitionerLookupType.ActiveCredentialsStates:
                    return GetStates(filterItemsIds);
                case PractitionerLookupType.ActiveCredentialsTypesByRequest:
                    return GetCredentialTypesBySkills(filterItemsIds);

                default:
                    throw new NotSupportedException();
            }
        }
    }

    internal class ItemCountValueBuilder
    {
        public ItemCountValueDto Dto { get; }
        private readonly HashSet<int> mPersonIds = new HashSet<int>();

        public void AddPersonToCount(int personId)
        {
            mPersonIds.Add(personId);
            Dto.Count = mPersonIds.Count;
        }

        public ItemCountValueBuilder(ItemCountValueDto dto)
        {
            Dto = dto;
        }
    }

}
