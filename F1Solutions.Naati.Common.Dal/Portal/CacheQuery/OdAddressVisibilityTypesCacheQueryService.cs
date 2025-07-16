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
using F1Solutions.Naati.Common.Dal.Domain;
using F1Solutions.Naati.Common.Dal.NHibernate.Configuration;
using F1Solutions.Naati.Common.Dal.QueryHelper;
using OdAddressVisibilityTypeLookup = F1Solutions.Naati.Common.Contracts.Dal.Portal.OdAddressVisibilityTypeLookup;

namespace F1Solutions.Naati.Common.Dal.Portal.CacheQuery
{
    public class OdAddressVisibilityTypesCacheQueryService : BaseCacheQueryService<int, OdAddressVisibilityTypeLookup, OdAddressVisibilityType>, IOdAddressVisibilityTypesCacheQueryService
    {

        public IEnumerable<OdAddressVisibilityTypeLookup> GetAllOdAddressVisibilityTypes()
        {
            return Items.Values;
        }

        protected override int TransformKey(int key)
        {
            return key;
        }

        protected override OdAddressVisibilityType GetDbSingleValue(int key)
        {
            return NHibernateSession.Current.Query<OdAddressVisibilityType>().FirstOrDefault(x => x.Id == key);
        }

        protected override IReadOnlyList<KeyValuePair<int, OdAddressVisibilityTypeLookup>> GetAllDbValues()
        {
            return NHibernateSession.Current.Query<OdAddressVisibilityType>().Select(GetKeyValuePair).ToList();
        }

        protected override OdAddressVisibilityTypeLookup MapToTResultType(OdAddressVisibilityType item)
        {
            return new OdAddressVisibilityTypeLookup { SamId = item.Id, DisplayText = item.DisplayName };
        }

        private KeyValuePair<int, OdAddressVisibilityTypeLookup> GetKeyValuePair(OdAddressVisibilityType item)
        {
            return new KeyValuePair<int, OdAddressVisibilityTypeLookup>(item.Id, MapToTResultType(item));
        }
    }
}
