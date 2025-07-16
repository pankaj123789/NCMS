using System;
using System.Collections.Generic;
using System.Linq;
using F1Solutions.Global.Common;
using F1Solutions.Naati.Common.Contracts.Dal;
using F1Solutions.Naati.Common.Contracts.Dal.DTO;
using F1Solutions.Naati.Common.Contracts.Dal.Request;
using F1Solutions.Naati.Common.Dal.Domain;
using F1Solutions.Naati.Common.Dal.NHibernate.Configuration;
using NHibernate;
using NHibernate.Criterion;
using NHibernate.Transform;

namespace F1Solutions.Naati.Common.Dal.QueryHelper
{
    public class EndorsedQualificationQueryHelper : QuerySearchHelper
    {
        private EndorsedQualificationSearchResultDto mEndorsedQualificationSearchDto => null;
        public IList<EndorsedQualificationSearchResultDto> SearchQualification(GetEndorsedQualificationSearchRequest request)
        {
            var filterrDictionary = new Dictionary<EndorsedQualificationFilterType, Func<EndorsedQualificationSearchCriteria, Junction, Junction>>
            {
                [EndorsedQualificationFilterType.CredentialTypeIntList] = (criteria, previousJunction) => GetCredentialTypeFilter(criteria, previousJunction),
                [EndorsedQualificationFilterType.InstitutionIntList] = (criteria, previousJunction) => GetInstitutionFilter(criteria, previousJunction),
                [EndorsedQualificationFilterType.InstitutionNaatiNumberIntList] = (criteria, previousJunction) => GetInstitutionNaatiNumberFilter(criteria, previousJunction),
                [EndorsedQualificationFilterType.LocationString] = (criteria, previousJunction) => GetLocationsFilter(criteria, previousJunction),
                [EndorsedQualificationFilterType.QualificationString] = (criteria, previousJunction) => GetQualificationFilter(criteria, previousJunction),
                [EndorsedQualificationFilterType.EndorsementFromString] = (criteria, previousJunction) => GetEndorsementFromFilter(criteria, previousJunction),
                [EndorsedQualificationFilterType.EndorsementToString] = (criteria, previousJunction) => GetEndorsementToFilter(criteria, previousJunction),
                [EndorsedQualificationFilterType.EndorsedQualificationIdIntList] = (criteria, previousJunction) => GetQualificationIdFilter(criteria, previousJunction),
            };

            Junction junction = Restrictions.Conjunction();

            var validCriterias = request.Filters.Where(x => x.Values.Any(v => v != null)).ToList();
            foreach (var criteria in validCriterias)
            {
                var junctionFunc = filterrDictionary[criteria.Filter];
                junction = junctionFunc(criteria, junction);
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

            var projections = BuildProjections();

            queryOver = queryOver.Select(projections);

            var resultList = queryOver.TransformUsing(Transformers.AliasToBean<EndorsedQualificationSearchResultDto>()).List<EndorsedQualificationSearchResultDto>();

            return resultList;
        }

        private IQueryOver<EndorsedQualification, EndorsedQualification> BuildQuery()
        {
            var queryOver = NHibernateSession.Current.QueryOver(() => EndorsedQualification)
                .Inner.JoinAlias(x => EndorsedQualification.Institution, () => Institution)
                .Inner.JoinAlias(x => Institution.Entity, () => Entity)
                .Inner.JoinAlias(x => Institution.LatestInstitutionName, () => LatestInstitutionName)
                .Inner.JoinAlias(x => LatestInstitutionName.InstitutionName, () => InstitutionName)
                .Inner.JoinAlias(x => EndorsedQualification.CredentialType, () => CredentialType);
               
            queryOver.UnderlyingCriteria.AddOrder(Order.Asc(Projections.Max(Projections.Property(() => InstitutionName.Name))));
            queryOver.UnderlyingCriteria.AddOrder(Order.Asc(Projections.Max(Projections.Property(() => EndorsedQualification.Location))));
            return queryOver;
        }

        private ProjectionList BuildProjections()
        {
            var skillProjection = Projections.Conditional(Restrictions.IsNull(Projections.Property(() => Skill.Id)), Projections.Constant(null, NHibernateUtil.String), GetDirectionProjection());
            var skilSeparator = ",";
            skillProjection = StringAgg(skillProjection, skilSeparator);

            IProjection activeProperty = Projections.Property(() => EndorsedQualification.Active);
            activeProperty = GetIntValueProjectionFor(Restrictions.EqProperty(activeProperty, Projections.Constant(true, NHibernateUtil.Boolean)));
            activeProperty = Projections.Max(activeProperty);
            var isActive = GetBooleanProjectionFor(Restrictions.Gt(activeProperty, 0));

            return Projections.ProjectionList()
                .Add(Projections.GroupProperty(Projections.Property(() => EndorsedQualification.Id)).WithAlias(() => mEndorsedQualificationSearchDto.EndorsedQualificationId))
                .Add(Projections.Max(Projections.Property(() => Institution.Id)).WithAlias(() => mEndorsedQualificationSearchDto.InstitutionId))
                .Add(Projections.Max(Projections.Property(() => Entity.NaatiNumber)).WithAlias(() => mEndorsedQualificationSearchDto.InstitutionNaatiNumber))
                .Add(Projections.Max(Projections.Property(() => InstitutionName.Name)).WithAlias(() => mEndorsedQualificationSearchDto.InstitutionName))
                .Add(Projections.Max(Projections.Property(() => EndorsedQualification.Location)).WithAlias(() => mEndorsedQualificationSearchDto.Location))
                .Add(Projections.Max(Projections.Property(() => EndorsedQualification.Qualification)).WithAlias(() => mEndorsedQualificationSearchDto.Qualification))
                .Add(Projections.Max(Projections.Property(() => CredentialType.Id)).WithAlias(() => mEndorsedQualificationSearchDto.CredentialTypeId))
                .Add(Projections.Max(Projections.Property(() => CredentialType.InternalName)).WithAlias(() => mEndorsedQualificationSearchDto.CredentialTypeInternalName))
                .Add(Projections.Max(Projections.Property(() => CredentialType.ExternalName)).WithAlias(() => mEndorsedQualificationSearchDto.CredentialTypeExternalName))
                .Add(Projections.Max(Projections.Property(() => EndorsedQualification.EndorsementPeriodFrom)).WithAlias(() => mEndorsedQualificationSearchDto.EndorsementPeriodFrom))
                .Add(Projections.Max(Projections.Property(() => EndorsedQualification.EndorsementPeriodTo)).WithAlias(() => mEndorsedQualificationSearchDto.EndorsementPeriodTo))
                .Add(isActive.WithAlias(() => mEndorsedQualificationSearchDto.Active));
        }
        
        protected Junction GetCredentialTypeFilter<S>(ISearchCriteria<S> criteria, Junction junction)
        {
            var typeList = criteria.ToList<S, int>();
            junction.Add<CredentialType>(c => CredentialType.Id.IsIn(typeList));
            return junction;
        }
        protected Junction GetInstitutionFilter<S>(ISearchCriteria<S> criteria, Junction junction)
        {
            var typeList = criteria.ToList<S, int>();
            junction.Add<NaatiEntity>(c => Institution.Id.IsIn(typeList));
            return junction;
        }

        protected Junction GetInstitutionNaatiNumberFilter<S>(ISearchCriteria<S> criteria, Junction junction)
        {
            var typeList = criteria.ToList<S, int>();
            junction.Add<NaatiEntity>(c => Entity.NaatiNumber.IsIn(typeList));
            return junction;
        }

        protected Junction GetQualificationIdFilter<S>(ISearchCriteria<S> criteria, Junction junction)
        {
            var typeList = criteria.ToList<S, int>();
            junction.Add<NaatiEntity>(c => EndorsedQualification.Id.IsIn(typeList));
            return junction;
        }

        protected Junction GetLocationsFilter<S>(ISearchCriteria<S> criteria, Junction junction)
        {
            junction.Add<PersonName>(p => EndorsedQualification.Location.IsLike(criteria.Values.FirstOrDefault(), MatchMode.Anywhere));
            return junction;
        }

        protected Junction GetQualificationFilter<S>(ISearchCriteria<S> criteria, Junction junction)
        {
            junction.Add<PersonName>(p => EndorsedQualification.Qualification.IsLike(criteria.Values.FirstOrDefault(), MatchMode.Anywhere));
            return junction;
        }

        private Junction GetEndorsementFromFilter<S>(ISearchCriteria<S> criteria, Junction junction)
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

            var endorsement = GetDateProjectionFrom(Projections.Property(() => EndorsedQualification.EndorsementPeriodTo));
            junction.Add(Restrictions.Ge(endorsement, dateTime.Date));

            return junction;
        }

        private Junction GetEndorsementToFilter<S>(ISearchCriteria<S> criteria, Junction junction)
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

            var endorsement = GetDateProjectionFrom(Projections.Property(() => EndorsedQualification.EndorsementPeriodFrom));
            junction.Add(Restrictions.Le(endorsement, dateTime));

            return junction;
        }

    }
}
