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
    public class PostcodesCacheQueryService : BaseCacheQueryService<int, Postcode, Domain.Postcode>, IPostcodesCacheQueryService
    {

        public IEnumerable<Postcode> GetAllPostcodes()
        {
            return Items.Values;
        }

        protected override int TransformKey(int key)
        {
            return key;
        }

        protected override Domain.Postcode GetDbSingleValue(int key)
        {
            return NHibernateSession.Current.Query<Domain.Postcode>().FirstOrDefault(x => x.Id == key);
        }

        protected override IReadOnlyList<KeyValuePair<int, Postcode>> GetAllDbValues()
        {
            return NHibernateSession.Current.Query<Domain.Postcode>().Select(GetKeyValuePair).ToList();
        }

        protected override Postcode MapToTResultType(Domain.Postcode item)
        {
            return new Postcode
            {
                SamId = item.Id,
                DisplayText = string.Format("{0} {1} {2}", item.Suburb.Name, item.Suburb.State.Abbreviation, item.PostCode),
                Code = item.PostCode,
                SuburbId = item.Suburb?.Id ?? 0,
                Suburb = item.Suburb?.Name,
                State = item.Suburb?.State?.Abbreviation
            };
        }

        private KeyValuePair<int, Postcode> GetKeyValuePair(Domain.Postcode item)
        {
            return new KeyValuePair<int, Postcode>(item.Id, MapToTResultType(item));
        }
    }
}
