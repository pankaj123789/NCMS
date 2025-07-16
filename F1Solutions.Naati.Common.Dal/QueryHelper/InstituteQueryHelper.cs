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

namespace F1Solutions.Naati.Common.Dal.QueryHelper
{
    internal class InstituteQueryHelper : QuerySearchHelper
    {
        private InstituteSearchDto mInstituteSearchDto => null;

        public IList<InstituteSearchDto> SearchInstitute(GetInstituteSearchRequest request)
        {
            var instituteFiltersDictionary = new Dictionary<InstituteFilterType, Func<InstituteSearchCriteria, Junction, Junction>>
            {
                [InstituteFilterType.NaatiNumberIntList] = (criteria, previousJunction) => GetInstitutionEntityNaatiNumberFilter(criteria, previousJunction),
                [InstituteFilterType.NameString] = (criteria, previousJunction) => previousJunction.Add<InstitutionName>(p => InstitutionName.Name.IsLike(criteria.Values.FirstOrDefault(), MatchMode.Exact)),
                [InstituteFilterType.ContactNumberString] = (criteria, previousJunction) => GetInstitutionContactNumber(criteria, previousJunction),
                [InstituteFilterType.ContactNameString] = (criteria, previousJunction) => previousJunction.Add<ContactPerson>(p => (ContactPerson.Name.IsLike(criteria.Values.FirstOrDefault(), MatchMode.Exact) && ContactPerson.Inactive == false)),
                [InstituteFilterType.EmailString] = (criteria, previousJunction) => previousJunction.Add<Email>(e => (Email.EmailAddress.IsLike(criteria.Values.FirstOrDefault()) && Email.IsPreferredEmail) || (ContactPerson.Email.IsLike(criteria.Values.FirstOrDefault() ?? string.Empty) && ContactPerson.Inactive == false)),
                [InstituteFilterType.AnythingString] = (criteria, previousJunction) => GetAnythingFilter(criteria, previousJunction),
            };

            Junction junction = null;

            var validCriterias = request.Filters.Where(x => x.Values.Any(v => v != null)).ToList();
            if (validCriterias.Any())
            {
                junction = Restrictions.Conjunction();
                foreach (var criteria in validCriterias)
                {
                    var junctionFunc = instituteFiltersDictionary[criteria.Filter];
                    junction = junctionFunc(criteria, junction);
                }
            }

            var queryOver = BuildQuery();

            var subquery = GetSubquery(queryOver, junction, request.Take, request.Skip);

            var ids = subquery.List<int>().Concat(new[] { 0 }).ToList();
            queryOver = queryOver.Where(Restrictions.In(Projections.Property(() => Institution.Id), ids));

            var projections = BuildProjections();
            queryOver = queryOver.Select(projections.ToArray());

            var indices = projections.ToDictionary(k => k.Aliases.First(), v => projections.FindIndex(value => value == v));
            return queryOver.TransformUsing(new InstituteSearchTransformer(indices)).List<InstituteSearchDto>();
        }

        private Junction GetInstitutionEntityNaatiNumberFilter<S>(ISearchCriteria<S> criteria, Junction junction)
        {
            var naatiNumberList = criteria.ToList<S, int>();
            junction.Add<Entity>(
                e => Entity.NaatiNumber.IsIn(naatiNumberList));
            return junction;
        }

        private Junction GetAnythingFilter<S>(ISearchCriteria<S> criteria, Junction junction)
        {
            var stringValue = criteria.Values.FirstOrDefault();

            var filters = Restrictions.Disjunction();
            int intValue;
            if (int.TryParse(stringValue, out intValue))
            {
                var naatiNumberFilter = Restrictions.In(Projections.Property(() => Entity.NaatiNumber), new[] { intValue });
                filters.Add(naatiNumberFilter);
            }
            filters.Add<InstitutionName>(p => InstitutionName.Name.IsLike(stringValue, MatchMode.Anywhere));
            filters.Add<ContactPerson>(p => (ContactPerson.Name.IsLike(stringValue, MatchMode.Anywhere) && ContactPerson.Inactive == false));
            filters.Add<Email>(
                e => (Email.EmailAddress == stringValue && Email.IsPreferredEmail) ||
                     (ContactPerson.Email == (stringValue ?? string.Empty) &&
                      ContactPerson.Inactive == false));
            junction.Add(filters);

            return junction;
        }

        private Junction GetInstitutionContactNumber<S>(ISearchCriteria<S> criteria, Junction junction)
        {
            var numberRestrictioin = GetPhoneRestriction(criteria);
            var contactNumberRestriction = Restrictions.Like(Projections.Property(() => ContactPerson.Phone), criteria.Values.FirstOrDefault() ?? String.Empty);
            junction.Add(Restrictions.Or(numberRestrictioin, contactNumberRestriction));
            return junction;
        }

        protected IQueryOver<Institution, Institution> GetSubquery(IQueryOver<Institution, Institution> queryOver, Junction junction, int? take, int? skip)
        {
            var subQueryOver = queryOver.Clone();

            if (junction != null)
            {
                subQueryOver = subQueryOver.Where(junction);
            }

            var institutionId = Projections.Property(() => Institution.Id);

            subQueryOver = subQueryOver.Select(Projections.Distinct(institutionId))
                .OrderBy(() => Institution.Id)
                .Asc;

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

        private IQueryOver<Institution, Institution> BuildQuery()
        {
            var queryOver = NHibernateSession.Current.QueryOver(() => Institution)
                .Inner.JoinAlias(x => Institution.Entity, () => Entity)
                .Inner.JoinAlias(x => Institution.LatestInstitutionName, () => LatestInstitutionName)
                .Left.JoinAlias(x => Institution.LatestInstitutionName.InstitutionName, () => InstitutionName)
                .Left.JoinAlias(x => Institution.Entity.Emails, () => Email)
                .Left.JoinAlias(x => Institution.Entity.Phones, () => Phone)
                .Left.JoinAlias(x => Institution.ContactPersons, () => ContactPerson);


            return queryOver;
        }

        private List<IProjection> BuildProjections()
        {
            InstituteSearchTransformer instituteSearchTransformer = null;
            return new List<IProjection>
            {
                Projections.Property(() => Institution.Id).WithAlias(() => mInstituteSearchDto.InstituteId),
                Projections.Property(() => Entity.NaatiNumber).WithAlias(() => mInstituteSearchDto.NaatiNumber),
                GetEmailProjection().WithAlias(()=> mInstituteSearchDto.PrimaryEmail),
                GetPhoneProjection().WithAlias(()=> mInstituteSearchDto.PrimaryContactNo),
                Projections.Property(() => InstitutionName.Name).WithAlias(() => mInstituteSearchDto.Name),
                GetContactIdProjection().WithAlias(() => instituteSearchTransformer.ContactIdProperty),
                Projections.Property(() => Entity.Id).WithAlias(() => mInstituteSearchDto.EntityId),
            };
        }

        private IProjection GetEmailProjection()
        {
            return Projections.Conditional(
                Restrictions.Eq(Projections.Property(() => Email.IsPreferredEmail), true),
                Projections.Property(() => Email.EmailAddress),
                Projections.Constant(null, NHibernateUtil.String));
        }

        private IProjection GetContactIdProjection()
        {
            return Projections.Conditional(Restrictions.Eq(Projections.Property(() => ContactPerson.Inactive), false), Projections.Property(() => ContactPerson.Id), Projections.Constant(null, NHibernateUtil.Int32));
        }

    }
}
