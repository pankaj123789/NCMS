using System;
using System.Collections.Generic;
using System.Linq;
using F1Solutions.Global.Common;
using F1Solutions.Naati.Common.Contracts.Dal;
using F1Solutions.Naati.Common.Contracts.Dal.DTO;
using F1Solutions.Naati.Common.Contracts.Dal.Enum;
using F1Solutions.Naati.Common.Contracts.Dal.QueryServices;
using F1Solutions.Naati.Common.Contracts.Dal.Request;
using F1Solutions.Naati.Common.Dal.Domain;
using F1Solutions.Naati.Common.Dal.NHibernate.Configuration;
using NHibernate;
using NHibernate.Criterion;
using NHibernate.Transform;

namespace F1Solutions.Naati.Common.Dal.QueryHelper
{
    internal class PersonQueryHelper : QuerySearchHelper
    {
        private PersonSearchDto mPersonSearchDto => null;
        private PersonCredentialRequestDto mPersonCredentialRequestDto => null;

        public IList<PersonSearchDto> SearchPeople(GetPersonSearchRequest request)
        {
            var personFiltersDictionary = new Dictionary<PersonFilterType, Func<PersonSearchCriteria, Junction, Junction>>
            {
                [PersonFilterType.NaatiNumberIntList] = (criteria, previousJunction) => GetNaatiNumberFilter(criteria, previousJunction),
                [PersonFilterType.PractitionerNumberString] = (criteria, previousJunction) => previousJunction.Add<Person>(p => p.PractitionerNumber.IsLike(criteria.Values.FirstOrDefault())),
                [PersonFilterType.GivenNamesString] = (criteria, previousJunction) => previousJunction.Add<PersonName>(p => PersonName.GivenName.IsLike(criteria.Values.FirstOrDefault(), MatchMode.Exact)),
                [PersonFilterType.FamilyNameString] = (criteria, previousJunction) => previousJunction.Add<PersonName>(p => PersonName.Surname.IsLike(criteria.Values.FirstOrDefault(), MatchMode.Exact)),
                [PersonFilterType.PhoneNumberString] = (criteria, previousJunction) => GetPhoneFilter(criteria, previousJunction),
                [PersonFilterType.EmailString] = (criteria, previousJunction) => previousJunction.Add<Email>(e => Email.EmailAddress.IsLike(criteria.Values.FirstOrDefault())),
                [PersonFilterType.DateOfBirthFromString] = (criteria, previousJunction) => GetBirthDayFromFilter(criteria, previousJunction),
                [PersonFilterType.DateOfBirthToString] = (criteria, previousJunction) => GetBirthDayToFilter(criteria, previousJunction),
                [PersonFilterType.GenderStringList] = (criteria, previousJunction) => GetGenderFilter(criteria, previousJunction),
                [PersonFilterType.CredentialTypeIntList] = (criteria, previousJunction) => GetCredentialTypeFilter(criteria, previousJunction),
                [PersonFilterType.ActiveApplicationTypeIntList] = (criteria, previousJunction) => GetActiveApplicationTypeFilter(criteria, previousJunction),
                [PersonFilterType.CountryOfBirthIntList] = (criteria, previousJunction) => GetCountryOfBirthFilter(criteria, previousJunction),
                [PersonFilterType.PrimaryEmailString] = (criteria, previousJunction) => previousJunction.Add<Email>(e => Email.EmailAddress.IsLike(criteria.Values.FirstOrDefault()) && Email.IsPreferredEmail),
                [PersonFilterType.EmailsStringList] = (criteria, previousJunction) => GetEmailsFilter(criteria, previousJunction),
                [PersonFilterType.CredentialIntList] = (criteria, previousJunction) => GetCredentialFilter(criteria, previousJunction),
                [PersonFilterType.CredentialSkillIntList] = (criteria, previousJunction) => GetCredentialSkillFilter(criteria, previousJunction),
                [PersonFilterType.CredentialStatusTypeIntList] = (criteria, previousJunction) => GetCredentialStatusTypeFilter(criteria, previousJunction),
                [PersonFilterType.AnythingString] = (criteria, previousJunction) => GetAnythingFilter(criteria, previousJunction),
                [PersonFilterType.AddressStateIntList] = (criteria, previousJunction) => GetStateFilter(criteria, previousJunction),
                [PersonFilterType.CredentialIssueDateFromString] = (criteria, previousJunction) => GetCredentialIssueDateFromFilter(criteria, previousJunction),
                [PersonFilterType.CredentialIssueDateToString] = (criteria, previousJunction) => GetCredentialIssueDateToFilter(criteria, previousJunction),
                [PersonFilterType.ProductCardClaimBoolean] = (criteria, previousJunction) => GetProductCardClaimFilter(criteria, previousJunction),
                [PersonFilterType.DeceasedBoolean] = (criteria, previousJunction) => GetDeceasedFilter(criteria, previousJunction)

            };

            var groupedResultFiltersDictionary = new Dictionary<PersonFilterType, Func<PersonSearchCriteria, Junction, Junction>>
            {
                [PersonFilterType.PersonTypeIntList] = (criteria, previousJunction) => GetPersonTypeFilter(criteria, previousJunction),
                [PersonFilterType.RolePlayerCredentialTypeIntList] = (criteria, previousJunction) => GetPersonTypeCredentialTypeFilter(criteria, previousJunction, PersonType.RolePlayer),
                [PersonFilterType.RolePlayerPanelIntList] = (criteria, previousJunction) => GetPersonTypePanelFilter(criteria, previousJunction, PersonType.RolePlayer),
                [PersonFilterType.ExaminerCredentialTypeIntList] = (criteria, previousJunction) => GetPersonTypeCredentialTypeFilter(criteria, previousJunction, PersonType.Examiner),
                [PersonFilterType.ExaminerPanelIntList] = (criteria, previousJunction) => GetPersonTypePanelFilter(criteria, previousJunction, PersonType.Examiner),
            };
            

            Junction junction = null;
            Junction groupedJuction = new CustomGroupConjuction();

            var validCriterias = request.Filters.Where(x => x.Values.Any(v => v != null)).ToList();
            if (validCriterias.Any())
            {
                junction = Restrictions.Conjunction();
                foreach (var criteria in validCriterias)
                {
                    if (groupedResultFiltersDictionary.ContainsKey(criteria.Filter))
                    {
                        var groupJunctionFunc = groupedResultFiltersDictionary[criteria.Filter];
                        groupedJuction = groupJunctionFunc(criteria, groupedJuction);
                    }
                    else
                    {
                        var junctionFunc = personFiltersDictionary[criteria.Filter];
                        junction = junctionFunc(criteria, junction);
                    }
                    
                }
            }

            var queryOver = BuildQuery();

            var subquery = GetSubquery(queryOver, junction, groupedJuction, request.Take, request.Skip);

            queryOver = queryOver.Where(Restrictions.In(Projections.Property(() => Person.Id),
                subquery.List<int>().ToList()));

            var projections = BuildProjections();
            queryOver = queryOver.Select(projections.ToArray());

            var indices = projections.ToDictionary(k => k.Aliases.First(), v => projections.FindIndex(value => value == v));

            return queryOver.TransformUsing(new PersonSearchTransformer(indices)).List<PersonSearchDto>();
        }
        

        protected IQueryOver<Person, Person> GetSubquery(IQueryOver<Person, Person> queryOver, Junction junction, ICriterion groupedJunction, int? take, int? skip)
        {
            var subQueryOver = queryOver.Clone();

            var junc = junction ?? Restrictions.Conjunction();
          

            var personId = Projections.Property(() => Person.Id);
            var personIdGroup = Projections.GroupProperty(personId);

            subQueryOver = subQueryOver.Where(junc).OrderBy(x => Person.Id).Asc; 
            subQueryOver.Select(personIdGroup);
           
            subQueryOver.Where(groupedJunction);

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

        public IList<PersonCredentialRequestDto> GetPersonCredentialRequests(int personId)
        {
            var queryOver = NHibernateSession.Current.QueryOver(() => CredentialRequest)
                .Inner.JoinAlias(x => CredentialRequest.CredentialApplication, () => CredentialApplication)
                .Inner.JoinAlias(x => CredentialApplication.Person, () => Person)
                .Inner.JoinAlias(x => CredentialRequest.CredentialRequestStatusType, () => CredentialRequestStatusType)
                .Inner.JoinAlias(x => CredentialRequest.CredentialType, () => CredentialType)
                .Inner.JoinAlias(x => CredentialRequest.Skill, () => Skill)
                .Inner.JoinAlias(x => Skill.DirectionType, () => DirectionType)
                .Inner.JoinAlias(x => Skill.Language1, () => Language1)
                .Inner.JoinAlias(x => Skill.Language2, () => Language2)
                .Where(() => Person.Id == personId);

            queryOver = queryOver.Where(GetValidCredentialRequestStatusRestriction());

            queryOver.Select(
                    Projections.Property(() => Person.Id).WithAlias(() => mPersonCredentialRequestDto.PersonId),
                    Projections.Property(() => CredentialType.InternalName).WithAlias(() => mPersonCredentialRequestDto.CredentialType),
                    GetDirectionProjection().WithAlias(() => mPersonCredentialRequestDto.Direction),
                    Projections.Property(() => CredentialRequestStatusType.DisplayName).WithAlias(() => mPersonCredentialRequestDto.CredentialStatus))
                .TransformUsing(Transformers.AliasToBean<PersonCredentialRequestDto>());

            return queryOver.List<PersonCredentialRequestDto>();
        }
        private IQueryOver<Person, Person> BuildQuery()
        {
            var queryOver = NHibernateSession.Current.QueryOver(() => Person)
                .Inner.JoinAlias(x => Person.Entity, () => Entity)
                .Inner.JoinAlias(x => Person.LatestPersonName, () => LatestPersonName)
                .Left.JoinAlias(x => Person.CredentialApplications, () => CredentialApplication)
                .Inner.JoinAlias(x => LatestPersonName.PersonName, () => PersonName)
                .Left.JoinAlias(x => Entity.Emails, () => Email)
                .Left.JoinAlias(x => Entity.Phones, () => Phone)
                .Left.JoinAlias(x => CredentialApplication.CredentialRequests, () => CredentialRequest)
                .Left.JoinAlias(x => CredentialRequest.CredentialRequestStatusType, () => CredentialRequestStatusType)
                .Left.JoinAlias(x => CredentialRequest.CredentialType, () => CredentialType)
                .Left.JoinAlias(x => CredentialRequest.Skill, () => Skill)
                .Left.JoinAlias(x => CredentialApplication.CredentialApplicationStatusType, () => CredentialApplicationStatusType)
                .Left.JoinAlias(x => CredentialRequest.CredentialCredentialRequests, () => CredentialCredentialRequest)
                .Left.JoinAlias(x => CredentialCredentialRequest.Credential, () => Credential)
                .Left.JoinAlias(x => Credential.CertificationPeriod, () => CertificationPeriod)
                .Left.JoinAlias(x => CredentialApplication.SponsorInstitution, () => SponsorInstitution)
                .Left.JoinAlias(x => SponsorInstitution.Entity, () => SponsorEntity)
                .Left.JoinAlias(x => Person.PanelMemberships, () => PanelMembership)
                .Left.JoinAlias(x => PanelMembership.Panel, () => Panel)
                .Left.JoinAlias(x => PanelMembership.PanelRole, () => PanelRole)
                .Left.JoinAlias(x => PanelRole.PanelRoleCategory, () => PanelRoleCategory)
                .Left.JoinAlias(x => Panel.PanelType, () => PanelType)
                .Left.JoinAlias(x => CredentialApplication.CredentialApplicationType,() => CredentialApplicationType)
                .Left.JoinAlias(x=> Entity.Addresses, ()=> Address)
                .Left.JoinAlias(x=> Address.Postcode, ()=> Postcode)
                .Left.JoinAlias(x=> Postcode.Suburb, ()=>  Suburb)
                .Left.JoinAlias(x=> Suburb.State, ()=>  State)
                .Left.JoinAlias(x=> PanelMembership.PanelMembershipCredentialTypes, ()=> PanelMembershipCredentialType)
                .Left.JoinAlias(x=> PanelMembershipCredentialType.CredentialType, ()=> MembershipCredentialType);

            return queryOver;
        }

        private List<IProjection> BuildProjections()
        {
            PersonSearchTransformer personSearchTransformer = null;
            return new List<IProjection>
            {
                Projections.Property(() => Person.Id).WithAlias(() => mPersonSearchDto.PersonId),
                Projections.Property(() => Entity.NaatiNumber).WithAlias(() => mPersonSearchDto.NaatiNumber),
                GetEmailProjection().WithAlias(()=> mPersonSearchDto.PrimaryEmail),
                GetPhoneProjection().WithAlias(()=> mPersonSearchDto.PrimaryContactNumber),
                GetNameProjection().WithAlias(()=>mPersonSearchDto.Name),
                Projections.Property(() => Person.PractitionerNumber).WithAlias(() => mPersonSearchDto.PractitionerNumber),
                GetActiveApplicationProjection().WithAlias(()=> personSearchTransformer.ActiveApplicationProperty),
                GetCredentialStatusTypeIdProjectionIfAvailable().WithAlias(()=> personSearchTransformer.CredentialStatusProperty),
                GetCredentialCertficationFlagIfAvailable().WithAlias(()=> personSearchTransformer.CredentialCertificationProperty),
                Projections.Property(() => Person.IsEportalActive).WithAlias(() => mPersonSearchDto.IsEportalActive),
                GetIsPersonExaminerProjection().WithAlias(()=> personSearchTransformer.IsExaminer),
                GetIsRolePlayerProjection().WithAlias(()=> personSearchTransformer.IsRolePlayer),
                Projections.Property(() => Entity.Id).WithAlias(() => mPersonSearchDto.EntityId),
            };
        }

        private IProjection GetCredentialStatusTypeIdProjectionIfAvailable()
        {
            var statusProjection = GetCredentialStatusProjection();
            return Projections.Conditional(Restrictions.IsNotNull(Projections.Property(() => Credential.Id)),
                statusProjection, GetCredentialStatusTypeIdProjection(CredentialStatusTypeName.Unknown));
        }

        private IProjection GetCredentialCertficationFlagIfAvailable()
        {
            return Projections.Conditional(Restrictions.IsNotNull(Projections.Property(() => CredentialType.Id)),
                Projections.Property(() => CredentialType.Certification), Projections.Constant(false, NHibernateUtil.Boolean));
        }


        private IProjection GetEmailProjection()
        {
            return Projections.Conditional(
                Restrictions.And(
                    Restrictions.Eq(Projections.Property(() => Email.Invalid), false),
                    Restrictions.Eq(Projections.Property(() => Email.IsPreferredEmail), true)
                ),
                Projections.Property(() => Email.EmailAddress),
                Projections.Constant(null, NHibernateUtil.String));
        }

        private Junction GetCredentialTypeFilter<S>(ISearchCriteria<S> criteria, Junction junction)
        {
            var validCredentialRequest = GetValidCredentialRequestStatusRestriction();
            var credentialTypeId = Projections.Property(() => CredentialType.Id);
            var typeList = criteria.ToList<S, int>();

            var filter = Restrictions.And(validCredentialRequest, Restrictions.In(credentialTypeId, typeList));

            junction.Add(filter);
            return junction;
        }

        private Junction GetStateFilter<S>(ISearchCriteria<S> criteria, Junction junction)
        {
            var credentialTypeId = Projections.Property(() => State.Id);
            var typeList = criteria.ToList<S, int>();
            
            var filter = Restrictions.Conjunction()
                    .Add(Restrictions.IsNotNull(Projections.Property(()=> Address.Id)))
                    .Add(Restrictions.IsNotNull(Projections.Property(()=> State.Id)))
                    .Add(Restrictions.Eq(Projections.Property(()=> Address.PrimaryContact), true))
                    .Add(Restrictions.Eq(Projections.Property(()=> Address.Invalid), false))
                    .Add(Restrictions.In(credentialTypeId, typeList));

            if (typeList.Contains(9)) // Overseas
            {
                filter = Restrictions.Disjunction().Add(filter);
                filter.Add(Restrictions.Conjunction()
                    .Add(Restrictions.IsNotNull(Projections.Property(() => Address.Id)))
                    .Add(Restrictions.Not(Restrictions.Eq(Projections.Property(() => Address.Country.Id),13))) // Not Australia
                    .Add(Restrictions.Eq(Projections.Property(() => Address.PrimaryContact), true))
                    .Add(Restrictions.Eq(Projections.Property(() => Address.Invalid), false)));

            }
            

            junction.Add(filter);
            return junction;
        }

        private Junction GetCredentialFilter<S>(ISearchCriteria<S> criteria, Junction junction)
        {
            var activeCredentialRestriction = GetActiveCredentialFilter();

            var credentialTypeId = Projections.Property(() => CredentialType.Id);
            var typeList = criteria.ToList<S, int>();

            var filter = Restrictions.Conjunction()
                .Add(Restrictions.IsNotNull(Projections.Property(() => Credential.Id)))
                .Add(activeCredentialRestriction)
                .Add(Restrictions.In(credentialTypeId, typeList));


            junction.Add(filter);
            return junction;
        }

        private Junction GetCredentialSkillFilter<S>(ISearchCriteria<S> criteria, Junction junction)
        {
            var skillId = Projections.Property(() => Skill.Id);
            var typeList = criteria.ToList<S, int>();

            var activeCredentialRestriction = GetActiveCredentialFilter();
            var filter = Restrictions.Conjunction()
                        .Add(Restrictions.IsNotNull(Projections.Property(() => Credential.Id)))
                        .Add(activeCredentialRestriction)
                        .Add(Restrictions.In(skillId, typeList));

            junction.Add(filter);
            return junction;
        }


        private ICriterion GetPersonTypeCriterion(IEnumerable<PersonType> typeList)
        {
            var filter = Restrictions.Disjunction();

            foreach (var filterValue in typeList)
            {
                var personType = filterValue;

                switch (personType)
                {
                    case PersonType.Applicant:
                        filter.Add(Restrictions.Gt(Projections.Sum(GetIntValueProjectionFor(GetActiveApplicationRestriction())), 0));
                        break;
                    case PersonType.Practitioner:
                        filter.Add(Restrictions.Gt(Projections.Sum(GetIntValueProjectionFor(GetHasActiveCertificationCredentials())), 0));
                        break;
                    case PersonType.FormerPractitioner:
                        filter.Add(
                            Restrictions.Conjunction()
                                .Add(Restrictions.Eq(Projections.Sum(GetIntValueProjectionFor(GetHasActiveCertificationCredentials())), 0))
                                .Add(Restrictions.Eq(Projections.Sum(GetIntValueProjectionFor(GetHasFutureCertificationCredentials())), 0))
                                .Add(Restrictions.Gt(Projections.Sum(GetIntValueProjectionFor(GetHasExpiredOrTerminatedCredentials())), 0)));
                        break;
                    case PersonType.FuturePractitioner:
                        filter.Add(Restrictions.Conjunction()
                            .Add(Restrictions.Eq(Projections.Sum(GetIntValueProjectionFor(GetHasActiveCertificationCredentials())), 0))
                            .Add(Restrictions.Gt(Projections.Sum(GetIntValueProjectionFor(GetHasFutureCertificationCredentials())), 0)));
                        break;
                    case PersonType.Examiner:
                        filter.Add(Restrictions.Gt(Projections.Sum(GetIntValueProjectionFor(GetIsExaminerRestriction())), 0));

                        break;
                    case PersonType.RolePlayer:
                        filter.Add(Restrictions.Gt(Projections.Sum(GetIntValueProjectionFor(GetIsRolePlayerRestriction())), 0));
                        break;
                }

            }

            return filter;
        }
        private Junction GetPersonTypeFilter<S>(ISearchCriteria<S> criteria, Junction junction)
        {
            var typeList = criteria.ToList<S, int>().Select(x=>(PersonType)x);
            var filter = GetPersonTypeCriterion(typeList);
            
            return junction.Add(filter);
        }

        private Junction GetPersonTypeCredentialTypeFilter<S>(ISearchCriteria<S> criteria, Junction junction, PersonType personType)
        {
            var credentialTypeIds = criteria.ToList<S, int>();

            var filter = Restrictions.Conjunction();
            filter.Add(GetPersonTypeCriterion(new[] { personType }));

            var credentialType = Projections.Property(() => MembershipCredentialType.Id);
            filter.Add(Restrictions.Gt(Projections.Sum(GetIntValueProjectionFor(Restrictions.In(credentialType, credentialTypeIds.ToArray()))), 0));

            return junction.Add(filter);
        }

        private Junction GetPersonTypePanelFilter<S>(ISearchCriteria<S> criteria, Junction junction, PersonType personType)
        {
            var panelIds = criteria.ToList<S, int>().Select(x =>x);

            var filter = Restrictions.Conjunction();
            filter.Add(GetPersonTypeCriterion(new[]{ personType }));

            var panelProperty = Projections.Property(() => Panel.Id);
            filter.Add(Restrictions.Gt(Projections.Sum(GetIntValueProjectionFor(Restrictions.In(panelProperty, panelIds.ToArray()))), 0));

            return junction.Add(filter);
        }


        private ICriterion GetCertificationRestriction()
        {
            return Restrictions.Eq(Projections.Property(() => CredentialType.Certification), true);
        }
        
        private ICriterion GetHasActiveCertificationCredentials()
        {
            var credentialStatusTypeIdProjection = GetCredentialStatusTypeIdProjectionIfAvailable();
            var typeList = new List<int>() { (int)CredentialStatusTypeName.Active };
            
            var filter = Restrictions.Disjunction();
            // Note: Restrictions.In didnt doesnt work for this case.
            typeList.ForEach(v => filter.Add(Restrictions.Eq(credentialStatusTypeIdProjection, v)));
          
            return Restrictions.Conjunction().Add(GetCertificationRestriction()).Add(filter);
        }

        private ICriterion GetHasExpiredOrTerminatedCredentials()
        {
            var credentialStatusTypeIdProjection = GetCredentialStatusTypeIdProjectionIfAvailable();
            var typeList = new List<int>() { (int)CredentialStatusTypeName.Terminated, (int)CredentialStatusTypeName.Expired };
            
            var filter = Restrictions.Disjunction();
            // Note: Restrictions.In didnt doesnt work for this case.
            typeList.ForEach(v => filter.Add(Restrictions.Eq(credentialStatusTypeIdProjection, v)));
  
            return Restrictions.Conjunction().Add(GetCertificationRestriction()).Add(filter);
        }

        private ICriterion GetHasFutureCertificationCredentials()
        {
            var credentialStatusTypeIdProjection = GetCredentialStatusTypeIdProjectionIfAvailable();
            var typeList = new List<int>() { (int)CredentialStatusTypeName.Future };

            var filter = Restrictions.Conjunction();
            // Note: Restrictions.In didnt doesnt work for this case.
            typeList.ForEach(v => filter.Add(Restrictions.Eq(credentialStatusTypeIdProjection, v)));
     
            return Restrictions.Conjunction().Add(GetCertificationRestriction()).Add(filter);
        }

        private Junction GetAnythingFilter<S>(ISearchCriteria<S> criteria, Junction junction)
        {
            var stringValue = criteria.Values.FirstOrDefault()?.Trim();
            if (stringValue != null)
            {
                var filters = Restrictions.Disjunction();
                if (int.TryParse(stringValue, out int intValue))
                {
                    filters.Add<Entity>(e => Entity.NaatiNumber.IsIn(new[] { intValue }) || SponsorEntity.NaatiNumber.IsIn((new[] { intValue })));
                }

                filters.Add<Person>(p => p.PractitionerNumber == stringValue);
                filters.Add<PersonName>(p => PersonName.GivenName.IsLike(stringValue, MatchMode.Anywhere));

                // if the search string has spaces, take the last part and treat it as the surname, and treat the rest as the given name
                var lastSpace = stringValue.LastIndexOf(" ", StringComparison.Ordinal);
                if (lastSpace > 0)
                {
                    var term1 = stringValue.Substring(0, lastSpace).Trim();
                    var term2 = stringValue.Substring(lastSpace, stringValue.Length - lastSpace).Trim();
                    filters.Add<PersonName>(p => PersonName.GivenName.IsLike(term1, MatchMode.Anywhere) && PersonName.Surname.IsLike(term2, MatchMode.Anywhere));
                    filters.Add<PersonName>(p => PersonName.GivenName.IsLike(term2, MatchMode.Anywhere) && PersonName.Surname.IsLike(term1, MatchMode.Anywhere));
                }
                else
                {
                    filters.Add<PersonName>(p => PersonName.Surname.IsLike(stringValue, MatchMode.Anywhere));
                    // many people have numeric email addreses, so to prevent finding a lot of bad matches when the search term is a 
                    // NAATI number, we only search for emails when @ or . is included
                    if (stringValue.Contains('@') || stringValue.Contains('.'))
                    {
                        filters.Add<PersonName>(p => Email.EmailAddress.IsLike(stringValue, MatchMode.Anywhere));
                    }
                }

                filters.Add(GetPhoneRestriction(criteria));
                filters.Add<Email>(e => Email.EmailAddress == stringValue);

                junction.Add(filters);
            }
            return junction;
        }
        private Junction GetCredentialStatusTypeFilter<S>(ISearchCriteria<S> criteria, Junction junction)
        {
            var credentialStatusTypeIdProjection = GetCredentialStatusTypeIdProjectionIfAvailable();
            var typeList = criteria.ToList<S, int>();

            var filter = Restrictions.Disjunction();
            // Note: Restrictions.In didnt doesnt work for this case.
            typeList.ForEach(v => filter.Add(Restrictions.Eq(credentialStatusTypeIdProjection, v)));
            junction.Add(filter);

            return junction;
        }

        private Junction GetEmailsFilter<S>(ISearchCriteria<S> criteria, Junction junction)
        {
            var typeList = criteria.Values.ToList();
            junction.Add<Person>(p => Email.EmailAddress.IsIn(typeList));
            return junction;
        }

        private ICriterion GetValidCredentialRequestStatusRestriction()
        {
            var statusIdProjection = Projections.Property(() => CredentialRequestStatusType.Id);
            var statuses = new List<int> { (int)CredentialRequestStatusTypeName.Cancelled, (int)CredentialRequestStatusTypeName.Deleted };
            return Restrictions.Or(Restrictions.IsNull(statusIdProjection), Restrictions.Not(Restrictions.In(statusIdProjection, statuses)));
        }

        private Junction GetGenderFilter<S>(ISearchCriteria<S> criteria, Junction junction)
        {
            var typeList = criteria.Values.ToList();
            junction.Add<Person>(p => Person.Gender.IsIn(typeList));
            return junction;
        }

        public Junction GetCountryOfBirthFilter<S>(ISearchCriteria<S> criteria, Junction junction)
        {
            var typeList = criteria.ToList<S, int>();
            junction.Add<Person>(p => Person.BirthCountry.Id.IsIn(typeList));
            return junction;
        }

        private IProjection GetActiveApplicationProjection()
        {
            var activeApplicationRestriction = GetActiveApplicationRestriction();
            return GetBooleanProjectionFor(activeApplicationRestriction);
        }

        private IProjection GetIsPersonExaminerProjection()
        {
            var restriction = GetIsExaminerRestriction();
            return GetBooleanProjectionFor(restriction);
        }
        private IProjection GetIsRolePlayerProjection()
        {
            var restriction = GetIsRolePlayerRestriction();
            return GetBooleanProjectionFor(restriction);
        }

        private ICriterion GetIsExaminerRestriction()
        {
            var panelTypeIdProperty = Projections.Property(() => PanelType.Id);
            var roleIdProperty = Projections.Property(() => PanelRole.Id);
            var roleCategoryProperty = Projections.Property(() => PanelRoleCategory.Id);
            var panelMembershipStart = Projections.Property(() => PanelMembership.StartDate);
            var panelMembershipEnd = Projections.Property(() => PanelMembership.EndDate);

            var restriction = Restrictions.Conjunction()
                .Add(Restrictions.IsNotNull(panelTypeIdProperty))
                .Add(Restrictions.IsNotNull(roleIdProperty))
                .Add(Restrictions.IsNotNull(panelMembershipStart))
                .Add(Restrictions.IsNotNull(panelMembershipEnd))
                .Add(Restrictions.IsNotNull(roleCategoryProperty))
                .Add(Restrictions.In(roleCategoryProperty, new[] { (int)PanelRoleCategoryName.Examiner }))
                .Add(Restrictions.Le(panelMembershipStart, DateTime.Now))
                .Add(Restrictions.Ge(panelMembershipEnd, DateTime.Now));

            return restriction;
        }

        private ICriterion GetIsRolePlayerRestriction()
        {
            var panelTypeIdProperty = Projections.Property(() => PanelType.Id);
            var roleIdProperty = Projections.Property(() => PanelRole.Id);
            var roleCategoryProperty = Projections.Property(() => PanelRoleCategory.Id);
            var panelMembershipStart = Projections.Property(() => PanelMembership.StartDate);
            var panelMembershipEnd = Projections.Property(() => PanelMembership.EndDate);

            var restriction = Restrictions.Conjunction()
                .Add(Restrictions.IsNotNull(panelTypeIdProperty))
                .Add(Restrictions.IsNotNull(roleIdProperty))
                .Add(Restrictions.IsNotNull(panelMembershipStart))
                .Add(Restrictions.IsNotNull(panelMembershipEnd))
                .Add(Restrictions.IsNotNull(roleCategoryProperty))
                .Add(Restrictions.In(roleCategoryProperty, new[] { (int)PanelRoleCategoryName.RolePlayer }))
                .Add(Restrictions.Le(panelMembershipStart, DateTime.Now))
                .Add(Restrictions.Ge(panelMembershipEnd, DateTime.Now));

            return restriction;
        }

        public Junction GetActiveApplicationTypeFilter<S>(ISearchCriteria<S> criteria, Junction junction)
        {
            junction = GetApplicationTypeFilter(criteria, junction);
            var activeCriteria = FilterExtensionHelper.CreateCriteria<ApplicationSearchCriteria, ApplicationFilterType>
                (ApplicationFilterType.ActiveApplicationBoolean, new[] { true.ToString() });
            return GetActiveApplicationFilter(activeCriteria, junction);
        }

        private Junction GetBirthDayFromFilter<S>(ISearchCriteria<S> criteria, Junction junction)
        {
            if (criteria.Values.FirstOrDefault() == string.Empty)
            {
                return junction;

            }
            DateTime dateTime;
            if (!DateTime.TryParse(criteria.Values.FirstOrDefault(), out dateTime))
            {
                dateTime = DateTime.Now;
            }

            var birthDate = GetDateProjectionFrom(Projections.Property(() => Person.BirthDate));
            junction.Add(Restrictions.Ge(birthDate, dateTime.Date));

            return junction;
        }

        private Junction GetBirthDayToFilter<S>(ISearchCriteria<S> criteria, Junction junction)
        {
            if (criteria.Values.FirstOrDefault() == string.Empty)
            {
                return junction;

            }
            DateTime dateTime;
            if (!DateTime.TryParse(criteria.Values.FirstOrDefault(), out dateTime))
            {
                dateTime = new DateTime(1900, 12, 01);
            }

            var birthDate = GetDateProjectionFrom(Projections.Property(() => Person.BirthDate));
            junction.Add(Restrictions.Le(birthDate, dateTime));

            return junction;
        }

        private Junction GetCredentialIssueDateFromFilter<S>(ISearchCriteria<S> criteria, Junction junction)
        {
            if (criteria.Values.FirstOrDefault() == string.Empty)
            {
                return junction;

            }
            DateTime dateTime;
            if (!DateTime.TryParse(criteria.Values.FirstOrDefault(), out dateTime))
            {
                dateTime = DateTime.Now;
            }

            var filters = Restrictions.Conjunction();

            var credentialRequestStatusTypeId = Projections.Property(() => CredentialRequestStatusType.Id);

            filters.Add(Restrictions.Eq(credentialRequestStatusTypeId, (int) CredentialRequestStatusTypeName.CertificationIssued));
          

            var statusChangeDate = GetDateProjectionFrom(Projections.Property(() => CredentialRequest.StatusChangeDate));
            filters.Add(Restrictions.Ge(statusChangeDate, dateTime.Date));

            junction.Add(filters);
            return junction;
        }

        private Junction GetCredentialIssueDateToFilter<S>(ISearchCriteria<S> criteria, Junction junction)
        {
            if (criteria.Values.FirstOrDefault() == string.Empty)
            {
                return junction;

            }
            DateTime dateTime;
            if (!DateTime.TryParse(criteria.Values.FirstOrDefault(), out dateTime))
            {
                dateTime = new DateTime(1900, 12, 01);
            }

            var filters = Restrictions.Conjunction();

            var credentialRequestStatusTypeId = Projections.Property(() => CredentialRequestStatusType.Id);

            filters.Add(Restrictions.Eq(credentialRequestStatusTypeId, (int)CredentialRequestStatusTypeName.CertificationIssued));


            var statusChangeDate = GetDateProjectionFrom(Projections.Property(() => CredentialRequest.StatusChangeDate));
            filters.Add(Restrictions.Le(statusChangeDate, dateTime.Date));

            junction.Add(filters);
            return junction;
        }

        protected Junction GetProductCardClaimFilter<S>(ISearchCriteria<S> criteria, Junction junction)
        {
            var claim = criteria.ToList<S, bool>().First();

            int[] credentialApplicationTypeCategories = {(int)CredentialApplicationTypeCategoryName.Transition, (int)CredentialApplicationTypeCategoryName.Recertification };

         

            var transitionOrRecertificationClaimed = Restrictions.And(
                Restrictions.In(Projections.Property(() => CredentialApplicationType.CredentialApplicationTypeCategory.Id), credentialApplicationTypeCategories),
                Subqueries.WhereProperty<CredentialApplication>(e => CredentialApplication.Id).In(GetClaimFieldCredentialApplicationIds()));

            var notTransitionOrRecertification = Restrictions.Not(
                Restrictions.In(Projections.Property(() => CredentialApplicationType.CredentialApplicationTypeCategory.Id), credentialApplicationTypeCategories));

            var combinedRestriction = Restrictions.Or(transitionOrRecertificationClaimed, notTransitionOrRecertification);
            if(!claim)
            {
                var notTransitionOrRecertificationClaimed = Restrictions.And(
                    Restrictions.In(Projections.Property(() => CredentialApplicationType.CredentialApplicationTypeCategory.Id), credentialApplicationTypeCategories),
                    Subqueries.WhereProperty<CredentialApplication>(e => CredentialApplication.Id).NotIn(GetClaimFieldCredentialApplicationIds()));

              
                combinedRestriction = notTransitionOrRecertificationClaimed;
            }

            return junction.Add(combinedRestriction);
        }


        private QueryOver<CredentialApplicationFieldData, CredentialApplicationFieldData> GetClaimFieldCredentialApplicationIds()
        {
            CredentialApplicationFieldData mCredentialApplicationFieldData = null;
            CredentialApplicationField mCredentialApplicationField = null;

            var subquery = QueryOver.Of(() => mCredentialApplicationFieldData)
                .Inner.JoinAlias(x => mCredentialApplicationFieldData.CredentialApplicationField, () => mCredentialApplicationField)
                .Where(
                    Restrictions.Conjunction()
                        .Add(Restrictions.Eq(Projections.Property(() => mCredentialApplicationField.CredentialApplicationFieldCategory.Id), (int)CredentialApplicationFieldCategoryTypeName.Claim))//Is '1' the correct thing here?
                        .Add(Restrictions.Eq(Projections.Property(() => mCredentialApplicationFieldData.Value), "True"))
                )
                .Select(Projections.Property(() => mCredentialApplicationFieldData.CredentialApplication.Id));
            return subquery;
        }

        public int? GetPersonHashByNaatiNumber(int naatiNumber)
        {
            var query = NHibernateSession.Current.QueryOver(() => Person)
                .Inner.JoinAlias(x => Person.Entity, () => Entity)
                .Where(Restrictions.Eq(Projections.Property(() => Entity.NaatiNumber), naatiNumber))
                .Select(GetPersonHashProjection());

            var result = query.SingleOrDefault<int?>();
            return result;
        }

        public int? GetPersonHashByApplicationId(int applicationId)
        {
            var query = NHibernateSession.Current.QueryOver(() => CredentialApplication)
                .Inner.JoinAlias(x => CredentialApplication.Person, () => Person)
                .Inner.JoinAlias(x => Person.Entity, () => Entity)
                .Where(Restrictions.Eq(Projections.Property(() => CredentialApplication.Id), applicationId))
                .Select(GetPersonHashProjection());

            var result = query.SingleOrDefault<int?>();
            return result;
        }

        public IProjection GetPersonHashProjection()
        {
            var naatiNumberProjection = Projections.Property(() => Entity.NaatiNumber);
            var personIdProjection = Projections.Property(() => Person.Id);
            var entityProjection = Projections.Property(() => Entity.Id);
            var sum = SumProjections(personIdProjection, entityProjection, naatiNumberProjection);
            var sumString = Projections.Cast(NHibernateUtil.String, sum);
            var hash = GetMd5HashProjectionFor(sumString);
            var integerHashProjection = Projections.Cast(NHibernateUtil.Int32, hash);

            return integerHashProjection;
        }

        protected Junction GetDeceasedFilter<S>(ISearchCriteria<S> criteria, Junction junction)
        {
            var deceased = criteria.ToList<S, bool>().First();

            var filter = Restrictions.Eq(Projections.Property(() => Person.Deceased), deceased);
            return junction.Add(filter);
        }


    }
}
