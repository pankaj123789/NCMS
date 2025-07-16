using F1Solutions.Global.Common.Mapping;
using F1Solutions.Naati.Common.Contracts.Dal.QueryServices;
using Ncms.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ncms.Bl
{
    public class VenueService : IVenueService
    {
        private readonly IVenueQueryService _venueQueryService;
        private readonly IAutoMapperHelper _autoMapperHelper;

        public VenueService(IVenueQueryService venueQueryService, IAutoMapperHelper autoMapperHelper)
        {
            _venueQueryService = venueQueryService;
            _autoMapperHelper = autoMapperHelper;
        }

        public IEnumerable<VenueSearchResultModel> VenueSearch()
        {
            return _venueQueryService.VenueSearch().Select(_autoMapperHelper.Mapper.Map<VenueSearchResultModel>).ToList();
        }
    }
}
