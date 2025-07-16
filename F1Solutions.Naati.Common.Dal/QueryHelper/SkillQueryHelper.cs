using System;
using System.Collections;
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
    internal class SkillQueryHelper : QuerySearchHelper
    {
        private SkillSearchResultDto mSkillSearchResultDto => null;
        public IList<SkillSearchResultDto> SearchSkills(SkillSearchRequest request)
        {
            var skillFilterDictionary = new Dictionary<SkillFilterType, Func<SkillSearchCriteria, Junction, Junction>>
            {
                [SkillFilterType.SkillTypeIntList] = (criteria, previousJunction) => GetSkillTypeFilter(criteria, previousJunction),
                [SkillFilterType.SkillIdIntList] = (criteria, previousJunction) => GetSkillIdFilter(criteria, previousJunction),
                [SkillFilterType.LanguageIntList] = (criteria, previousJunction) => GetLanguageFilter(criteria, previousJunction),
                [SkillFilterType.DirectionTypeIntList] = (criteria, previousJunction) => GetDirectionFilter(criteria, previousJunction),
                [SkillFilterType.ApplicationTypeIntList] = (criteria, previousJunction) => GetSkillApplicationTypeFilter(criteria, previousJunction),

            };

            Junction junction = Restrictions.Conjunction();

            var validCriterias = request.Filters.Where(x => x.Values.Any(v => v != null)).ToList();
            foreach (var criteria in validCriterias)
            {
                var junctionFunc = skillFilterDictionary[criteria.Filter];
                junction = junctionFunc(criteria, junction);
            }

            var queryOver = BuildQuery();

            var subquery = GetSubquery(queryOver, junction, request.Take, request.Skip);
            var skilIds = subquery.List<int>().ToList();

            queryOver = queryOver.Where(Restrictions.In(Projections.Property(()=> Skill.Id), skilIds));

            var projections = BuildProjections();
            queryOver = queryOver.Select(projections.ToArray());
            var resultList = queryOver.List<IList>();
            var data = resultList.GroupBy(d => d[0])
                .Select(g=>
            {
              
                var applicationTypes = new List<LookupTypeDto>();
                var skill = new SkillSearchResultDto
                {
                    SkillId = (int)g.First()[0],
                    SkillTypeId = (int)g.First()[2],
                    Language1Id = (int)g.First()[3],
                    Language2Id = (int?)g.First()[4],
                    DirectionTypeId = (int)g.First()[5],
                    TypeDisplayName = g.First()[6] as string,
                    DisplayName = g.First()[7] as string,
                    Note = g.First()[9] as string,
                    NumberOfExistingCredentials = (int)g.First()[10],
                    NumberOfCredentialRequests = (int)g.First()[11],
                    ApplicationTypes = applicationTypes
                };

                g.ForEach(i =>
                {
                    if (i[1] == null)
                    {
                        return;
                    }
                    applicationTypes.Add(new LookupTypeDto() { Id = (int)i[1], DisplayName =i[8] as string });
                });
                return skill;
            }).ToList();

            return data;
        }

        protected IQueryOver<Skill, Skill> GetSubquery(IQueryOver<Skill, Skill> queryOver, Junction junction, int? take, int? skip)
        {
            var subQueryOver = queryOver.Clone();

            var junc = junction ?? Restrictions.Conjunction();

            var skillId = Projections.Distinct(Projections.Property(() => Skill.Id));

            subQueryOver = subQueryOver.Where(junc).OrderBy(x => Skill.Id).Asc;
            subQueryOver.Select(skillId);
      

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

        private IQueryOver<Skill, Skill> BuildQuery()
        {
            var queryOver = NHibernateSession.Current.QueryOver(() => Skill)
                .Left.JoinAlias(x => Skill.SkillType, () => SkillType)
                .Left.JoinAlias(x => Skill.DirectionType, () => DirectionType)
                .Left.JoinAlias(x => Skill.Language1, () => Language1)
                .Left.JoinAlias(x => Skill.Language2, () => Language2)
                .Left.JoinAlias(x => Skill.CredentialRequests, () => CredentialRequest)
                .Left.JoinAlias(x => CredentialRequest.CredentialRequestStatusType, () => CredentialRequestStatusType)
                .Left.JoinAlias(x => CredentialRequest.CredentialCredentialRequests, () => CredentialCredentialRequest)
                .Left.JoinAlias(x => CredentialCredentialRequest.Credential, () => Credential)
                .Left.JoinAlias(x => Skill.SkillApplicationTypes, () => SkillApplicationType)
                .Left.JoinAlias(x => SkillApplicationType.CredentialApplicationType, () => CredentialApplicationType);
            return queryOver;
        }

        private List<IProjection> BuildProjections()
        {
            return new List<IProjection>
            {
                Projections.Group(() => Skill.Id),
                Projections.Group(() => CredentialApplicationType.Id),
                Projections.Max(() => SkillType.Id),
                Projections.Max(() => Language1.Id),
                Projections.Max(() => Language2.Id),
                Projections.Max(() => DirectionType.Id),
                Projections.Max(() => SkillType.DisplayName),
                Projections.Max(GetDirectionProjection()),
                Projections.Max(()=>CredentialApplicationType.DisplayName),
                Projections.Max(() => Skill.Note),
                GetNumberOfExistingCredentialsProjection(),
                GetNumberOfCredentialRequestsProjection(),
            };
        }

        private Junction GetSkillTypeFilter<S>(ISearchCriteria<S> criteria, Junction junction)
        {
            var typeList = criteria.ToList<S, int>();
            junction.Add<SkillType>(at => SkillType.Id.IsIn(typeList));
            return junction;
        }

        private Junction GetSkillIdFilter<S>(ISearchCriteria<S> criteria, Junction junction)
        {
            var idList = criteria.ToList<S, int>();
            junction.Add<SkillType>(at => Skill.Id.IsIn(idList));
            return junction;
        }

        private Junction GetDirectionFilter<S>(ISearchCriteria<S> criteria, Junction junction)
        {
            var typeList = criteria.ToList<S, int>();
            junction.Add<DirectionType>(at => DirectionType.Id.IsIn(typeList));
            return junction;
        }

        protected IProjection GetNumberOfExistingCredentialsProjection()
        {
            return Projections.Count(
                    Projections.Distinct(
                        Projections.Property(() => Credential.Id)
                    )
                );
        }

        protected IProjection GetNumberOfCredentialRequestsProjection()
        {
            return Projections.Count(
                Projections.Distinct(
                      Projections.Conditional(GetCredentialRequestStatusTypeClause(),
                             Projections.Property(() => CredentialRequest.Id),
                            Projections.Constant(null, NHibernateUtil.Int32)))
            );

        }

        private ICriterion GetCredentialRequestStatusTypeClause()
        {
            var invalidStatuses = new[]
            {
                (int) CredentialRequestStatusTypeName.Deleted,
                (int) CredentialRequestStatusTypeName.Cancelled,
                (int) CredentialRequestStatusTypeName.Withdrawn,
                (int) CredentialRequestStatusTypeName.Rejected,
            };

            return Restrictions.Not(Restrictions.In(Projections.Property(() => CredentialRequestStatusType.Id), invalidStatuses));
        }

        protected Junction GetSkillApplicationTypeFilter<S>(ISearchCriteria<S> criteria, Junction junction)
        {
            var typeList = criteria.ToList<S, int>();
            var subQuery = QueryOver.Of<SkillApplicationType>()
                .Where(x => x.CredentialApplicationType.IsIn(typeList))
                .Select(y => y.Skill.Id);
           var filter =  Subqueries.WhereProperty<Skill>(e => Skill.Id).In(subQuery);

            return junction.Add(filter);
        }
    }
}

