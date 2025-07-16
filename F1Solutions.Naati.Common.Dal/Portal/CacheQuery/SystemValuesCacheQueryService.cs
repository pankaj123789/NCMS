using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using F1Solutions.Naati.Common.Contracts.Dal.DTO;
using F1Solutions.Naati.Common.Contracts.Dal.Enum;
using F1Solutions.Naati.Common.Contracts.Dal.Portal;
using F1Solutions.Naati.Common.Contracts.Dal.Portal.CacheQuery;
using F1Solutions.Naati.Common.Contracts.Dal.QueryServices;
using F1Solutions.Naati.Common.Dal.CacheQuery;
using F1Solutions.Naati.Common.Dal.NHibernate.Configuration;
using F1Solutions.Naati.Common.Dal.QueryHelper;
using SystemValue = F1Solutions.Naati.Common.Dal.Domain.SystemValue;

namespace F1Solutions.Naati.Common.Dal.Portal.CacheQuery
{
    public class SystemValuesCacheQueryService : BaseCacheQueryService<string, SystemValueDto, SystemValue>, ISystemValuesCacheQueryService
    {
        protected override string TransformKey(string key)
        {
            return key;
        }

        protected override SystemValue GetDbSingleValue(string key)
        {
            return NHibernateSession.Current.Query<SystemValue>().FirstOrDefault(x => x.ValueKey == key);
        }

        protected override IReadOnlyList<KeyValuePair<string, SystemValueDto>> GetAllDbValues()
        {
            return NHibernateSession.Current.Query<SystemValue>().Select(GetKeyValuePair).ToList();
        }

        protected override SystemValueDto MapToTResultType(SystemValue item)
        {
            return new SystemValueDto { Id = item.Id, Value = item.Value, ValueKey = item.ValueKey};
        }

        private KeyValuePair<string, SystemValueDto> GetKeyValuePair(SystemValue item)
        {
            return new KeyValuePair<string, SystemValueDto>(item.ValueKey, MapToTResultType(item));
        }

        public SystemValueDto GetSystemValue(string valueKey, bool refresh = false)
        {
            if (refresh)
            {
                RefreshItem(valueKey);
            }
            return GetItem(valueKey);
        }
    }
}
