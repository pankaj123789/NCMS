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
    public class LanguagesCacheQueryService : BaseCacheQueryService<int, Language, Domain.Language>, ILanguagesCacheQueryService
    {

        public IEnumerable<Language> GetAllLanguages()
        {
            return Items.Values;
        }
        protected override int TransformKey(int key)
        {
            return key;
        }

        protected override Domain.Language GetDbSingleValue(int key)
        {
            return NHibernateSession.Current.Query<Domain.Language>().FirstOrDefault(x => x.Id == key);
        }

        protected override IReadOnlyList<KeyValuePair<int, Language>> GetAllDbValues()
        {
            return NHibernateSession.Current.Query<Domain.Language>().Select(GetKeyValuePair).ToList();
        }

        protected override Language MapToTResultType(Domain.Language item)
        {
            return new Language { SamId = item.Id, DisplayText = item.Name };
        }

        private KeyValuePair<int, Language> GetKeyValuePair(Domain.Language item)
        {
            return new KeyValuePair<int, Language>(item.Id, MapToTResultType(item));
        }
    }
}
