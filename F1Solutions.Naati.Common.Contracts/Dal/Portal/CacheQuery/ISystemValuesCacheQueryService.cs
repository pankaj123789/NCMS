using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using F1Solutions.Naati.Common.Contracts.Dal.DTO;
using F1Solutions.Naati.Common.Contracts.Dal.QueryServices;

namespace F1Solutions.Naati.Common.Contracts.Dal.Portal.CacheQuery
{
    public interface ISystemValuesCacheQueryService : ICacheQueryService
    {

        SystemValueDto GetSystemValue(string valueKey,bool refresh = false);
    }
}
