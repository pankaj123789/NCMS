using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using F1Solutions.Naati.Common.Contracts.Dal.DTO;
using F1Solutions.Naati.Common.Contracts.Dal.Portal.CacheQuery;

namespace MyNaati.Contracts.Portal.Users
{
    public class RefreshUsersRequest
    {
        public IEnumerable<MyNaatiUserRefreshDto> Users { get; set; }
    }
}
