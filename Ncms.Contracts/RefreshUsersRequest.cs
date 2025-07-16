using System.Collections.Generic;
using F1Solutions.Naati.Common.Contracts.Dal.BackOffice.CacheQuery;
using F1Solutions.Naati.Common.Contracts.Dal.DTO;

namespace Ncms.Contracts
{
    public class RefreshUsersRequest
    {
        public IEnumerable<NcmsUserRefreshDto> Users { get; set; }
    }
}
