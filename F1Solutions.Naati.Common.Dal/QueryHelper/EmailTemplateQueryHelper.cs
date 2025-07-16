using System;
using System.Collections.Generic;
using System.Linq;
using F1Solutions.Global.Common;
using F1Solutions.Naati.Common.Contracts.Dal;
using F1Solutions.Naati.Common.Contracts.Dal.Enum;
using F1Solutions.Naati.Common.Contracts.Dal.Request;
using F1Solutions.Naati.Common.Contracts.Dal.Response;
using F1Solutions.Naati.Common.Dal.Domain;
using F1Solutions.Naati.Common.Dal.NHibernate.Configuration;
using NHibernate;
using NHibernate.Criterion;
using NHibernate.Transform;

namespace F1Solutions.Naati.Common.Dal.QueryHelper
{
	internal class EmailTemplateQueryHelper : QuerySearchHelper
	{
		private EmailTemplateResponse mEmailTemplateResponse => null;

		public IList<EmailTemplateResponse> SearchEmailTemplates(EmailTemplateSearchRequest request)
		{
			var emailTemplateFilterDictionary = new Dictionary<EmailTemplateFilterType, Func<EmailTemplateSearchCriteria, Junction, Junction>>
			{
				[EmailTemplateFilterType.EmailTemplateIntList] = (criteria, previousJunction) => GetEmailTemplateFilter(criteria, previousJunction),
				[EmailTemplateFilterType.Name] = (criteria, previousJunction) => GetNameFilter(criteria, previousJunction),
				[EmailTemplateFilterType.ApplicationTypeIntList] = (criteria, previousJunction) => GetApplicationTypeFilter(criteria, previousJunction),
				[EmailTemplateFilterType.SystemActionTypeIntList] = (criteria, previousJunction) => GetSystemActionFilter(criteria, previousJunction),
			};

			Junction junction = null;

			var validCriterias = request.Filter.Where(x => x.Values.Any(v => v != null)).ToList();
			if (validCriterias.Any())
			{
				junction = Restrictions.Conjunction();
				foreach (var criteria in validCriterias)
				{
					var junctionFunc = emailTemplateFilterDictionary[criteria.Filter];
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

			var indices = projections.ToDictionary(k => k.Aliases.First(), v => projections.FindIndex(value => value == v));

			var searchResult = queryOver.TransformUsing(Transformers.AliasToBean<EmailTemplateResponse>());

			var resultList = searchResult.List<EmailTemplateResponse>();

			return resultList;
		}

		private Junction GetNameFilter<S>(ISearchCriteria<S> criteria, Junction junction)
		{
			return junction;
		}

		private Junction GetSystemActionFilter<S>(ISearchCriteria<S> criteria, Junction junction)
		{
			var typeList = criteria.ToList<S, int>();
			junction.Add<SystemActionType>(at => SystemActionType.Id.IsIn(typeList));
			return junction;
		}

		private IQueryOver<EmailTemplate, EmailTemplate> BuildQuery()
		{
			var queryOver = NHibernateSession.Current.QueryOver(() => EmailTemplate)
				.Left.JoinAlias(x => EmailTemplate.SystemActionEmailTemplates, () => SystemActionEmailTemplate)
				.Left.JoinAlias(x => SystemActionEmailTemplate.SystemActionType, () => SystemActionType)
				.Left.JoinAlias(x => SystemActionEmailTemplate.CredentialWorkflowActionEmailTemplates, () => CredentialWorkflowActionEmailTemplate)
				.Left.JoinAlias(x => CredentialWorkflowActionEmailTemplate.CredentialApplicationType, () => CredentialApplicationType);

			return queryOver;
		}

		private List<IProjection> BuildProjections()
		{
			return new List<IProjection>
			{
				Projections.Group(() => EmailTemplate.Id).WithAlias(() => mEmailTemplateResponse.Id),
				Projections.Group(() => EmailTemplate.Name).WithAlias(() => mEmailTemplateResponse.Name),
				Projections.Group(() => EmailTemplate.Subject).WithAlias(() => mEmailTemplateResponse.Subject),
				Projections.Group(() => EmailTemplate.Content).WithAlias(() => mEmailTemplateResponse.Content),
				Projections.Group(() => EmailTemplate.Active).WithAlias(() => mEmailTemplateResponse.Active),
				Projections.Group(() => EmailTemplate.FromAddress).WithAlias(() => mEmailTemplateResponse.FromAddress)
			};
		}
	}
}
