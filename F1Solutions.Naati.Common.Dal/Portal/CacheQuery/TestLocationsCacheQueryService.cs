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

namespace F1Solutions.Naati.Common.Dal.Portal.CacheQuery
{
    public class TestLocationsCacheQueryService : BaseCacheQueryService<int, TestLocation, Domain.TestLocation>, ITestLocationsCacheQueryService
    {

        public IEnumerable<TestLocation> GetAllTestLocations()
        {
            return Items.Values;
        }

        protected override int TransformKey(int key)
        {
            return key;
        }

        protected override Domain.TestLocation GetDbSingleValue(int key)
        {
            return NHibernateSession.Current.Query<Domain.TestLocation>().FirstOrDefault(x => x.Id == key);
        }

        protected override IReadOnlyList<KeyValuePair<int, TestLocation>> GetAllDbValues()
        {
            return NHibernateSession.Current.Query<Domain.TestLocation>().Select(GetKeyValuePair).ToList();
        }

        protected override TestLocation MapToTResultType(Domain.TestLocation item)
        {
            return new TestLocation { SamId = item.Id, DisplayText = item.Name };
        }

        private KeyValuePair<int, TestLocation> GetKeyValuePair(Domain.TestLocation item)
        {
            return new KeyValuePair<int, TestLocation>(item.Id, MapToTResultType(item));
        }
    }
}
