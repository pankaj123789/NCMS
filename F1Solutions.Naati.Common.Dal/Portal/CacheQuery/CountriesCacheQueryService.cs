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
using DbCountry = F1Solutions.Naati.Common.Dal.Domain.Country;

namespace F1Solutions.Naati.Common.Dal.Portal.CacheQuery
{
    public class CountriesCacheQueryService : BaseCacheQueryService<int, Country, DbCountry>, ICountriesCacheQueryService
    {
        private int? _defaultCountryId;
        protected int DefaultCountryId => (_defaultCountryId ?? (_defaultCountryId = GetDefaultCountryId())).Value;

        public IEnumerable<Country> GetAllCountries()
        {
            return Items.Values;
        }

        protected override int TransformKey(int key)
        {
            return key;
        }
        
        protected override DbCountry GetDbSingleValue(int key)
        {
            return NHibernateSession.Current.Query<DbCountry>().FirstOrDefault(x => x.Id == key);
        }

        private int GetDefaultCountryId()
        {

            return  int.Parse( NHibernateSession.Current.Query<Domain.SystemValue>().First(x => x.ValueKey == "DefaultCountryId").Value);
        }


        protected override IReadOnlyList<KeyValuePair<int, Country>> GetAllDbValues()
        {
            GetDefaultCountryId();
            return NHibernateSession.Current.Query<DbCountry>().Select(GetKeyValuePair).ToList();
        }

        protected override Country MapToTResultType(DbCountry item)
        {
            return new Country { SamId = item.Id, DisplayText = item.Name, IsHomeCountry = item.Id == DefaultCountryId };
        }

        private KeyValuePair<int, Country> GetKeyValuePair(DbCountry item)
        {
            return new KeyValuePair<int, Country>(item.Id, MapToTResultType(item));
        }
    }
}
