using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ncms.Contracts
{
    public interface IVenueService
    {
        IEnumerable<VenueSearchResultModel> VenueSearch();
    }
}
