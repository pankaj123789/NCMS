using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using F1Solutions.Naati.Common.Contracts.Bl.Attributes;
using F1Solutions.Naati.Common.Contracts.Dal.BackOffice.CacheQuery;
using F1Solutions.Naati.Common.Contracts.Dal.Enum;
using F1Solutions.Naati.Common.Contracts.Dal.QueryServices;
using F1Solutions.Naati.Common.Contracts.Dal.Response;
using F1Solutions.Naati.Common.Dal.CacheQuery;

namespace F1Solutions.Naati.Common.Dal.BackOffice.CacheQuery
{
    public class LookupTypeCacheQueryService :BaseLazyCacheQueryService<LookupType, IDictionary<int, string>, LookupTypeResponse>, ILookupTypeConverterHelper
    {
        private readonly IUtilityQueryService _utilityQueryService;

        public LookupTypeCacheQueryService(IUtilityQueryService utilityQueryService)
        {
            _utilityQueryService = utilityQueryService;
        }

        public string GetDisplayName(LookupType lookupType, int value)
        {
            if (value == 0)
            {
                return string.Empty;
            }

            var lookupDictionary = GetItem(lookupType);

            if (!lookupDictionary.TryGetValue(value, out var displayName))
            {
                RefreshItem(lookupType);
                lookupDictionary = GetItem(lookupType);

                displayName = lookupDictionary[value];
            }

            return displayName;
        }

        protected override LookupType TransformKey(LookupType key)
        {
            return key;
        }

        protected override LookupTypeResponse GetDbSingleValue(LookupType key)
        {
            // TODO: Remove this dependency
            return _utilityQueryService.GetLookupType(key);
        }

        protected override IDictionary<int, string> MapToTResultType(LookupTypeResponse item)
        {
            return item.Results.ToDictionary(x => x.Id, y => y.DisplayName);
        }
    }
}
