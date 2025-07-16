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
    internal class VenueQueryHelper : QuerySearchHelper
	{
		private VenueSearchResultDto mVenueSearchResultDto => null;
		public IList<VenueSearchResultDto> SearchVenues(VenueSearchRequest request)
		{
			var applicationFilterDictionary = new Dictionary<VenueFilterType, Func<VenueSearchCriteria, Junction, Junction>>
			{
			    [VenueFilterType.VenueIdIntList] = (criteria, previousJunction) => GetVenueFilter(criteria, previousJunction)
            };

			Junction junction = Restrictions.Conjunction();

			var validCriteria = request.Filters.Where(x => x.Values.Any(v => v != null)).ToList();
			foreach (var criteria in validCriteria)
			{
				Func<VenueSearchCriteria, Junction, Junction> junctionFunc;
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

			var searchResult = queryOver.TransformUsing(Transformers.AliasToBean<VenueSearchResultDto>());

			var resultList = searchResult.List<VenueSearchResultDto>();

			return resultList;
		}

		private IQueryOver<Venue, Venue> BuildQuery()
		{
		    var queryOver = NHibernateSession.Current.QueryOver(() => Venue)
		        .Left.JoinAlias(x => Venue.TestLocation, () => TestLocation);
            return queryOver;
		}

		private List<IProjection> BuildProjections()
		{
			return new List<IProjection>
			{
				Projections.Property(() => Venue.Id).WithAlias(() => mVenueSearchResultDto.VenueId),
				Projections.Property(() => Venue.Name).WithAlias(() => mVenueSearchResultDto.Name),
			    Projections.Property(() => Venue.Capacity).WithAlias(() => mVenueSearchResultDto.Capacity),
			    Projections.Property(() => Venue.PublicNotes).WithAlias(() => mVenueSearchResultDto.PublicNotes),
			    Projections.Property(() => Venue.TestLocation.Id).WithAlias(() => mVenueSearchResultDto.TestLocationId),
			    Projections.Property(() => Venue.Address).WithAlias(() => mVenueSearchResultDto.Address),
			    Projections.Property(() => Venue.Coordinates).WithAlias(() => mVenueSearchResultDto.Coordinates),
				Projections.Property(() => Venue.Inactive).WithAlias(() => mVenueSearchResultDto.Inactive),
                Projections.Property(() => TestLocation.Name).WithAlias(() => mVenueSearchResultDto.Location)
            };
		}

	    protected Junction GetVenueFilter<S>(ISearchCriteria<S> criteria, Junction junction)
	    {
	        var venueList = criteria.ToList<S, int>();
	        junction.Add<Venue>(x => Venue.Id.IsIn(venueList));
	        return junction;
	    }
    }
}

