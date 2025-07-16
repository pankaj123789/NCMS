using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using F1Solutions.Global.Common;
using F1Solutions.Naati.Common.Contracts.Dal;
using F1Solutions.Naati.Common.Contracts.Dal.DTO;
using F1Solutions.Naati.Common.Contracts.Dal.Enum;
using F1Solutions.Naati.Common.Contracts.Dal.Request;
using F1Solutions.Naati.Common.Dal.Domain;
using F1Solutions.Naati.Common.Dal.NHibernate.Configuration;
using NHibernate;
using NHibernate.Criterion;
using NHibernate.Event.Default;
using NHibernate.Transform;

namespace F1Solutions.Naati.Common.Dal.QueryHelper
{
    internal class CredentialQueryHelper : QuerySearchHelper
    {
        private CredentialSearchDto mCredentialSearchDto => null;
        public IList<CredentialSearchDto> SearchCredentials(GetCredentialSearchRequest request)
        {
            var filterDictionary = new Dictionary<CredentialFilterTypeName, Func<CredentialSearchCriteria, Junction, Junction>>
            {
                [CredentialFilterTypeName.NaatiNumberIntList] = GetNaatiNumberFilter,
                [CredentialFilterTypeName.PractitionerNumberString] = (criteria, previousJunction) => previousJunction.Add<Person>(p => Person.PractitionerNumber.IsLike(criteria.Values.FirstOrDefault())),
                [CredentialFilterTypeName.GivenNamesString] = (criteria, previousJunction) => previousJunction.Add<PersonName>(p => PersonName.GivenName.IsLike(criteria.Values.FirstOrDefault(), MatchMode.Exact)),
                [CredentialFilterTypeName.FamilyNameString] = (criteria, previousJunction) => previousJunction.Add<PersonName>(p => PersonName.Surname.IsLike(criteria.Values.FirstOrDefault(), MatchMode.Exact)),
                [CredentialFilterTypeName.PhoneNumberString] = GetPhoneFilter,
                [CredentialFilterTypeName.EmailString] = GetEmailFilter,
                [CredentialFilterTypeName.DateOfBirthFromString] = GetBirthDayFromFilter,
                [CredentialFilterTypeName.DateOfBirthToString] = GetBirthDayToFilter,
                [CredentialFilterTypeName.GenderStringList] = GetGenderFilter,
                [CredentialFilterTypeName.ApplicationTypeIntList] = GetApplicationTypeFilter,
                [CredentialFilterTypeName.CredentialStartDateFromString] = GetCredentialStartDateFromFilter,
                [CredentialFilterTypeName.CredentialStartDateToString] = GetCredentialStartDateToFilter,
                [CredentialFilterTypeName.CredentialEndDateFromString] = GetCredentialEndDateFromFilter,
                [CredentialFilterTypeName.CredentialEndDateToString] = GetCredentialEndDateToFilter,
                [CredentialFilterTypeName.CredentialTypeIntList] = GetCredentialRequestTypeFilter,
                [CredentialFilterTypeName.SkillIntList] = GetSkillFilter,
                [CredentialFilterTypeName.CredentialStatusTypeIntList] = GetCredentialStatusTypeFilter,
                [CredentialFilterTypeName.ShowInOnlineDirectoryBoolean] = GetShowInOnlineDirectoryBoolean,
                [CredentialFilterTypeName.CredentialIssueDateFromString] = GetCredentialIssueDateFromFilter,
                [CredentialFilterTypeName.CredentialIssueDateToString] = GetCredentialIssueDateToFilter,
                [CredentialFilterTypeName.ProductCardClaimBoolean] = GetProductCardClaimFilter,
                [CredentialFilterTypeName.CertificationCredentialTypeBoolean] = GetCertificationCredentialTypeFilter,
                [CredentialFilterTypeName.StateIntList] = GetStateFilter
            };

            Junction junction = Restrictions.Conjunction();

            var validCriteria = request.Filters.Where(x => x.Values.Any(v => v != null)).ToList();
            foreach (var criterion in validCriteria)
            {
                var junctionFunc = filterDictionary[criterion.Filter];
                junction = junctionFunc(criterion, junction);
            }

            var queryOver = BuildQuery();

            queryOver = queryOver.Where(junction);

            if (request.Skip.HasValue)
            {
                queryOver.Skip(request.Skip.Value);
            }

            if (request.Take.HasValue)
            {
                queryOver.Take(request.Take.Value);
            }

            queryOver.UnderlyingCriteria.AddOrder(Order.Asc(Projections.Property(() => Person.PractitionerNumber)));
            queryOver.UnderlyingCriteria.AddOrder(Order.Asc(Projections.Property(() => Person.Id)));

            var projections = BuildProjections();

            queryOver = queryOver.Select(projections.ToArray());

            var resultList = queryOver.TransformUsing(Transformers.AliasToBean<CredentialSearchDto>()).List<CredentialSearchDto>();

            return resultList;
        }

        private IQueryOver<IssuedCredentialCredentialRequest, IssuedCredentialCredentialRequest> BuildQuery()
        {
            var primaryAddressRestriction = Restrictions.Conjunction()
                .Add(Restrictions.Eq(Projections.Property(() => Address.Invalid), false))
                .Add(Restrictions.Eq(Projections.Property(() => Address.PrimaryContact), true));

            var primaryEmailRestriction = Restrictions.Conjunction()
                .Add(Restrictions.Eq(Projections.Property(() => Email.Invalid), false))
                .Add(Restrictions.Eq(Projections.Property(() => Email.IsPreferredEmail), true));

            var queryOver = NHibernateSession.Current.QueryOver(() => IssuedCredentialCredentialRequest)
                .Inner.JoinAlias(x => IssuedCredentialCredentialRequest.Credential, () => Credential)
                .Inner.JoinAlias(x => IssuedCredentialCredentialRequest.CredentialRequest, () => CredentialRequest)
                .Inner.JoinAlias(x => CredentialRequest.CredentialType, () => CredentialType)
                .Inner.JoinAlias(x => CredentialType.CredentialCategory, () => CredentialCategory)
                .Inner.JoinAlias(x => CredentialRequest.Skill, () => Skill)
                .Inner.JoinAlias(x => Skill.Language1, () => Language1)
                .Inner.JoinAlias(x => Skill.Language2, () => Language2)
                .Inner.JoinAlias(x => Skill.DirectionType, () => DirectionType)
                .Inner.JoinAlias(x => CredentialRequest.CredentialApplication, () => CredentialApplication)
                .Inner.JoinAlias(x => CredentialApplication.CredentialApplicationType, () => CredentialApplicationType)
                .Inner.JoinAlias(x => CredentialApplication.Person, () => Person)
                .Inner.JoinAlias(x => Person.Entity, () => Entity)
                .Inner.JoinAlias(x => Person.LatestPersonName, () => LatestPersonName)
                .Inner.JoinAlias(x => LatestPersonName.PersonName, () => PersonName)
                .Left.JoinAlias(x => Credential.CertificationPeriod, () => CertificationPeriod)
                .Left.JoinAlias(x => Language1.LanguageGroup, () => LanguageGroup1)
                .Left.JoinAlias(x => Language2.LanguageGroup, () => LanguageGroup2)
                .Left.JoinAlias(x => Entity.Addresses, () => Address, primaryAddressRestriction)
                .Left.JoinAlias(x => Address.Postcode, () => Postcode)
                .Left.JoinAlias(x => Postcode.Suburb, () => Suburb)
                .Left.JoinAlias(x => Suburb.State, () => State)
                .Left.JoinAlias(x => PersonName.Title, () => Title)
                .Left.JoinAlias(x => Address.Country, () => AddressCountry)
                .Left.JoinAlias(x => CredentialRequest.CredentialRequestStatusType, () => CredentialRequestStatusType)
                .Left.JoinAlias(x => Entity.Emails, () => Email, primaryEmailRestriction);

            return queryOver;
        }

        private List<IProjection> BuildProjections()
        {
            return new List<IProjection>
            {
                Projections.Property(() => Entity.NaatiNumber).WithAlias(() => mCredentialSearchDto.NaatiNumber),
                Projections.Property(() => Person.PractitionerNumber).WithAlias(() => mCredentialSearchDto.PractitionerNumber),
                Projections.Property(() => Title.TitleName).WithAlias(() => mCredentialSearchDto.Title),
                Projections.Property(() => PersonName.GivenName).WithAlias(() => mCredentialSearchDto.GivenName),
                Projections.Property(() => PersonName.Surname).WithAlias(() => mCredentialSearchDto.FamilyName),
                Projections.Property(() => Address.StreetDetails).WithAlias(() => mCredentialSearchDto.Address),
                Projections.Property(() => Suburb.Name).WithAlias(() => mCredentialSearchDto.Suburb),
                Projections.Property(() => State.Abbreviation).WithAlias(() => mCredentialSearchDto.State),
                Projections.Property(() => Postcode.PostCode).WithAlias(() => mCredentialSearchDto.Postcode),
                Projections.Property(() => AddressCountry.Name).WithAlias(() => mCredentialSearchDto.Country),
                Projections.Property(() => Email.EmailAddress).WithAlias(() => mCredentialSearchDto.PrimaryEmail),
                Projections.Property(() => Credential.Id).WithAlias(() => mCredentialSearchDto.CredentialId),
                Projections.Property(() => CredentialApplicationType.DisplayName).WithAlias(() => mCredentialSearchDto.ApplicationType),
                Projections.Property(() => CredentialType.InternalName).WithAlias(() => mCredentialSearchDto.CredentialTypeInternalName),
                Projections.Property(() => CredentialType.ExternalName).WithAlias(() => mCredentialSearchDto.CredentialTypeExternalName),
                Projections.Property(() => Language1.Name).WithAlias(() => mCredentialSearchDto.Language1),
                Projections.Property(() => Language1.Code).WithAlias(() => mCredentialSearchDto.Language1Code),
                Projections.Property(() => LanguageGroup1.Name).WithAlias(() => mCredentialSearchDto.Language1Group),
                Projections.Property(() => Language2.Name).WithAlias(() => mCredentialSearchDto.Language2),
                Projections.Property(() => Language2.Code).WithAlias(() => mCredentialSearchDto.Language2Code),
                Projections.Property(() => LanguageGroup2.Name).WithAlias(() => mCredentialSearchDto.Language2Group),
                GetDirectionProjection().WithAlias(()=> mCredentialSearchDto.Direction),
                Projections.Property(() => Credential.StartDate).WithAlias(() => mCredentialSearchDto.StartDate),
                GetExpiryDateForCredential().WithAlias(() => mCredentialSearchDto.EndDate),
                Projections.Property(() => CredentialRequestStatusType.DisplayName).WithAlias(() => mCredentialSearchDto.Status),
                Projections.Property(() => CredentialRequest.StatusChangeDate).WithAlias(() => mCredentialSearchDto.IssuedDate),

                Projections.Property(() => CredentialRequest.Id).WithAlias(() => mCredentialSearchDto.CredentialRequestId),
                Projections.Property(() => CredentialApplication.Id).WithAlias(() => mCredentialSearchDto.CredentialApplicationId),
                Projections.Property(() => CredentialCategory.DisplayName).WithAlias(() => mCredentialSearchDto.Category),
                GetCredentialStatusTypeIdProjectionIfAvailable().WithAlias(() => mCredentialSearchDto.StatusId),
                Projections.Property(() => Credential.ShowInOnlineDirectory).WithAlias(() => mCredentialSearchDto.ShowInOnlineDirectory)
            };
        }

        protected virtual Junction GetNaatiNumberFilter<S>(ISearchCriteria<S> criteria, Junction junction)
        {
            var naatiNumberList = criteria.ToList<S, int>();
            junction.Add<Entity>(
                e => Entity.NaatiNumber.IsIn(naatiNumberList));
            return junction;
        }

        protected Junction GetCredentialStartDateFromFilter<S>(ISearchCriteria<S> criteria, Junction junction)
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

            var startDate = GetDateProjectionFrom(Projections.Property(() => Credential.StartDate));
            junction.Add(Restrictions.Ge(startDate, dateTime));

            return junction;
        }

        protected Junction GetCredentialStartDateToFilter<S>(ISearchCriteria<S> criteria, Junction junction)
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

            var startDate = GetDateProjectionFrom(Projections.Property(() => Credential.StartDate));
            junction.Add(Restrictions.Le(startDate, dateTime));

            return junction;
        }

        protected Junction GetCredentialEndDateFromFilter<S>(ISearchCriteria<S> criteria, Junction junction)
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

            var endDate = GetDateProjectionFrom(GetExpiryDateForCredential());
            junction.Add(Restrictions.Ge(endDate, dateTime));

            return junction;
        }

        protected Junction GetCredentialEndDateToFilter<S>(ISearchCriteria<S> criteria, Junction junction)
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

            var endDate = GetDateProjectionFrom(GetExpiryDateForCredential());
            junction.Add(Restrictions.Le(endDate, dateTime));

            return junction;
        }

        protected Junction GetBirthDayFromFilter<S>(ISearchCriteria<S> criteria, Junction junction)
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

        protected Junction GetBirthDayToFilter<S>(ISearchCriteria<S> criteria, Junction junction)
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

        protected Junction GetGenderFilter<S>(ISearchCriteria<S> criteria, Junction junction)
        {
            var typeList = criteria.Values.ToList();
            junction.Add<Person>(p => Person.Gender.IsIn(typeList));
            return junction;
        }

        private IProjection GetCredentialStatusTypeIdProjectionIfAvailable()
        {
            var statusProjection = GetCredentialStatusProjection();
            return Projections.Conditional(Restrictions.IsNotNull(Projections.Property(() => Credential.Id)),
                statusProjection, GetCredentialStatusTypeIdProjection(CredentialStatusTypeName.Unknown));
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

        protected Junction GetShowInOnlineDirectoryBoolean<S>(ISearchCriteria<S> criteria, Junction junction)
        {
            var active = criteria.ToList<S, bool>().First();

            var showOdProperty = Projections.Property(() => Credential.ShowInOnlineDirectory);

            var showInRestriction = Restrictions.Eq(showOdProperty, true);
            if (active)
            {
                return junction.Add(showInRestriction);
            }

            return junction.Add(Restrictions.Not(showInRestriction));
        }

        protected Junction GetCertificationCredentialTypeFilter<S>(ISearchCriteria<S> criteria, Junction junction)
        {
            var trueSelected = criteria.ToList<S, bool>().First();

            var certificationCredentialTypeProperty = Projections.Property(() => CredentialType.Certification);

            var restriction = Restrictions.Eq(certificationCredentialTypeProperty, true);
            if (trueSelected)
            {
                return junction.Add(restriction);
            }

            return junction.Add(Restrictions.Not(restriction));
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

            filters.Add(Restrictions.Eq(credentialRequestStatusTypeId, (int)CredentialRequestStatusTypeName.CertificationIssued));
            
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

            var applicationTypesWithClaimFields = Subqueries.WhereProperty<CredentialApplicationType>(e => CredentialApplicationType.Id).In(GetCredentialApplicationTypesWithClaimFields());  
            var applicationsWithClaimRequested = Subqueries.WhereProperty<CredentialApplication>(e => CredentialApplication.Id).In(GetClaimFieldCredentialApplicationIds());

            var claimsRequested = Restrictions.And(applicationTypesWithClaimFields, applicationsWithClaimRequested);
            var applicationTypesWithoutClaims = Subqueries.WhereProperty<CredentialApplicationType>(e => CredentialApplicationType.Id).NotIn(GetCredentialApplicationTypesWithClaimFields());
            
            ICriterion filter;
            if (claim)
            {
                filter = Restrictions.Or(claimsRequested, applicationTypesWithoutClaims);
            }
            else
            {
                filter = Restrictions.And(Restrictions.Not(claimsRequested), Restrictions.Not(applicationTypesWithoutClaims));
            }

            junction = junction.Add(filter);
            return junction;
        }
        
        private QueryOver<CredentialApplicationField, CredentialApplicationField> GetCredentialApplicationTypesWithClaimFields()
        {
            CredentialApplicationType xCredentialApplicationType = null;
            CredentialApplicationField xCredentialApplicationField = null;
            CredentialApplicationFieldCategory xCredentialApplicationFieldCategory = null;

            var subQuery = QueryOver.Of(() => xCredentialApplicationField)
                .Inner.JoinAlias(x => xCredentialApplicationField.CredentialApplicationType, () => xCredentialApplicationType)
                .Inner.JoinAlias(x => xCredentialApplicationField.CredentialApplicationFieldCategory, () => xCredentialApplicationFieldCategory)
                .Where(Restrictions.Eq(Projections.Property(() => xCredentialApplicationFieldCategory.Id), (int)CredentialApplicationFieldCategoryTypeName.Claim))
                .Select(Projections.Distinct(Projections.Property(() => xCredentialApplicationType.Id)));
            return subQuery;

        }

        private QueryOver<CredentialApplicationFieldData, CredentialApplicationFieldData> GetClaimFieldCredentialApplicationIds()
        {
            CredentialApplicationFieldData mCredentialApplicationFieldData = null;
            CredentialApplicationField mCredentialApplicationField = null;

            var subQuery = QueryOver.Of(() => mCredentialApplicationFieldData)
                .Inner.JoinAlias(x => mCredentialApplicationFieldData.CredentialApplicationField, () => mCredentialApplicationField)
                .Where(
                    Restrictions.Conjunction()
                        .Add(Restrictions.Eq(Projections.Property(() => mCredentialApplicationField.CredentialApplicationFieldCategory.Id), (int)CredentialApplicationFieldCategoryTypeName.Claim))//Is '1' the correct thing here?
                        .Add(Restrictions.Eq(Projections.Property(() => mCredentialApplicationFieldData.Value), "True"))
                )
                .Select(Projections.Property(() => mCredentialApplicationFieldData.CredentialApplication.Id));
            return subQuery;
        }
        
        protected Junction GetEmailFilter<S>(ISearchCriteria<S> criteria, Junction junction)
        {
            if (criteria.Values.FirstOrDefault() == string.Empty)
            {
                return junction;
            }

            var email = criteria.Values.FirstOrDefault();
            var restriction = Subqueries.WhereProperty<Entity>(e => Entity.Id).In(GetEmailEntityIds(email));

            junction.Add(restriction);
            return junction;
        }

        private QueryOver<Email, Email> GetEmailEntityIds(string email)
        {
            Email feEntityEmail = null;
            NaatiEntity feNaatiEntity = null;

            var subQuery = QueryOver.Of(() => feEntityEmail)
                .Inner.JoinAlias(x => feEntityEmail.Entity, () => feNaatiEntity)
                .Where(Restrictions.Eq(Projections.Property(() => feEntityEmail.EmailAddress), email))
                .Select(Projections.Distinct(Projections.Property(() => feNaatiEntity.Id)));
            return subQuery;
        }

        protected override Junction GetPhoneFilter<S>(ISearchCriteria<S> criteria, Junction junction)
        {
            Phone mPhone = null;

            var numberProjection = Concatenate(Projections.Property(() => mPhone.CountryCode),
                Projections.Property(() => mPhone.AreaCode),
                Projections.Property(() => mPhone.LocalNumber));

            var filter = Restrictions.Conjunction();
            filter.Add(Restrictions.Eq(Projections.Property(() => mPhone.Invalid), false));
            filter.Add(Restrictions.Like(numberProjection, (criteria.Values.FirstOrDefault() ?? string.Empty).Replace(" ", string.Empty)));
            var subQuery = QueryOver.Of(() => mPhone)
                .Where(filter)
                .Select(Projections.Property(() => mPhone.Entity.Id));

            junction.Add(Subqueries.WhereProperty<NaatiEntity>(e => Entity.Id).In(subQuery));
                
            return junction;
        }
    }
}
