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
using NHibernate.Transform;

namespace F1Solutions.Naati.Common.Dal.QueryHelper
{
    internal class LanguageQueryHelper : QuerySearchHelper
	{
		private LanguageSearchResultDto mLanguageSearchResultDto => null;
		public IList<LanguageSearchResultDto> SearchLanguages(LanguageSearchRequest request)
		{
			var applicationFilterDictionary = new Dictionary<LanguageFilterType, Func<LanguageSearchCriteria, Junction, Junction>>
			{
				[LanguageFilterType.NameString] = (criteria, previousJunction) => GetNameFilter(criteria, previousJunction),
				[LanguageFilterType.GroupIntList] = (criteria, previousJunction) => GetGroupFilter(criteria, previousJunction),
				[LanguageFilterType.LanguageIdIntList] = (criteria, previousJunction) => GetLanguageFilter(criteria, previousJunction),
			};

			Junction junction = Restrictions.Conjunction();

			var validCriterias = request.Filters.Where(x => x.Values.Any(v => v != null)).ToList();
			foreach (var criteria in validCriterias)
			{
				Func<LanguageSearchCriteria, Junction, Junction> junctionFunc;
				if (applicationFilterDictionary.TryGetValue(criteria.Filter, out junctionFunc))
				{
					junction = junctionFunc(criteria, junction);
				}
			}

			var queryOver = BuildQuery();

			if (request.Skip.HasValue)
			{
				queryOver.Skip(request.Skip.Value);
			}

			if (request.Take.HasValue)
			{
				queryOver.Take(request.Take.Value);
			}

			if (junction != null)
			{
				queryOver = queryOver.Where(junction);
			}

			var projections = BuildProjections();
			queryOver = queryOver.Select(projections.ToArray());

			var searchResult = queryOver.TransformUsing(Transformers.AliasToBean<LanguageSearchResultDto>());

			var resultList = searchResult.List<LanguageSearchResultDto>();

			return resultList;
		}

		private IQueryOver<Language, Language> BuildQuery()
		{
			var queryOver = NHibernateSession.Current.QueryOver(() => Language)
				.Left.JoinAlias(x => Language.LanguageGroup, () => LanguageGroup);
			return queryOver;
		}

		private List<IProjection> BuildProjections()
		{
			return new List<IProjection>
			{
				Projections.Property(() => Language.Id).WithAlias(() => mLanguageSearchResultDto.LanguageId),
				Projections.Property(() => Language.Name).WithAlias(() => mLanguageSearchResultDto.Name),
				Projections.Property(() => Language.Code).WithAlias(() => mLanguageSearchResultDto.Code),
				Projections.Property(() => Language.Note).WithAlias(() => mLanguageSearchResultDto.Note),

				Projections.Property(() => LanguageGroup.Id).WithAlias(() => mLanguageSearchResultDto.GroupLanguageId),
				Projections.Property(() => LanguageGroup.Name).WithAlias(() => mLanguageSearchResultDto.GroupLanguageName),		
			};
		}

		private Junction GetNameFilter<S>(ISearchCriteria<S> criteria, Junction junction)
		{
			if (String.IsNullOrWhiteSpace(criteria.Values.FirstOrDefault()))
			{
				return junction;
			}

			junction.Add<Language>(p => Language.Name.IsLike(criteria.Values.FirstOrDefault(), MatchMode.Start));
			return junction;
		}

		private Junction GetGroupFilter<S>(ISearchCriteria<S> criteria, Junction junction)
		{
			var typeList = criteria.ToList<S, int>();
			junction.Add<LanguageGroup>(at => LanguageGroup.Id.IsIn(typeList));
			return junction;
		}

		protected override Junction GetLanguageFilter<S>(ISearchCriteria<S> criteria, Junction junction)
		{
			var languageList = criteria.ToList<S, int>();
			junction.Add<Language>(x => Language.Id.IsIn(languageList));
			return junction;
		}
	}
}

