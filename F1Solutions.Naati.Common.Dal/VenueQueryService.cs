using F1Solutions.Naati.Common.Contracts.Dal.DTO;
using F1Solutions.Naati.Common.Contracts.Dal.QueryServices;
using F1Solutions.Naati.Common.Dal.Domain;
using F1Solutions.Naati.Common.Dal.NHibernate.Configuration;
using NHibernate.Transform;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace F1Solutions.Naati.Common.Dal
{
    class VenueQueryService : IVenueQueryService
    {
        public IEnumerable<VenueDto> VenueSearch()
        {
            var venues = NHibernateSession.Current.QueryOver<Venue>()
                .TransformUsing(Transformers.DistinctRootEntity).List();

            var result = new List<VenueDto>();

            foreach (var venue in venues)
            {
                result.Add(MapToVenueDto(venue));
            }
            return result;
        }

        private VenueDto MapToVenueDto(Venue domain)
        {
            return new VenueDto
            {
                VenueId = domain.Id,
                TestLocationId = domain.TestLocation.Id,
                Address = domain.Address,
                Coordinates = domain.Coordinates,
                Capacity = domain.Capacity,
                Name = domain.Name,
                PublicNotes = domain.PublicNotes
            };
        }
    }
}
