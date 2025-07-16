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
using PersonTitle = F1Solutions.Naati.Common.Contracts.Dal.Portal.PersonTitle;

namespace F1Solutions.Naati.Common.Dal.Portal.CacheQuery
{
    public class PersonTitlesCacheQueryService : BaseCacheQueryService<int, PersonTitle, Title>, IPersonTitlesCacheQueryService
    {

        public IEnumerable<PersonTitle> GetAllPersonTitles()
        {
            return Items.Values;
        }

        protected override int TransformKey(int key)
        {
            return key;
        }

        protected override Title GetDbSingleValue(int key)
        {
            return NHibernateSession.Current.Query<Title>().FirstOrDefault(x => x.Id == key);
        }

        protected override IReadOnlyList<KeyValuePair<int, PersonTitle>> GetAllDbValues()
        {
            return NHibernateSession.Current.Query<Title>().Select(GetKeyValuePair).ToList();
        }

        protected override PersonTitle MapToTResultType(Title item)
        {
            return new PersonTitle { SamId = item.Id, DisplayText = item.TitleName, IsStandardTitle = item.StandardTitle };
        }

        private KeyValuePair<int, PersonTitle> GetKeyValuePair(Title item)
        {
            return new KeyValuePair<int, PersonTitle>(item.Id, MapToTResultType(item));
        }
    }
}
