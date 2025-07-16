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
using F1Solutions.Naati.Common.Dal.QueryHelper;

namespace F1Solutions.Naati.Common.Dal.Portal.CacheQuery
{
    public class CredentialLanguage2CacheQueryService : BaseCacheQueryService<int, LanguageLookup, LookupTypeDto>, ICredentialLanguage2CacheQueryService
    {

        public IEnumerable<LanguageLookup> GetAllLanguages()
        {
            return Items.Values;
        }

        protected override int TransformKey(int key)
        {
            return key;
        }

        protected override LookupTypeDto GetDbSingleValue(int key)
        {
            var practitioner = new PractitionerQueryHelper();
            var response = practitioner.GetLookup(PractitionerLookupType.ActiveCredentialsLanguages2);
            return response.FirstOrDefault(x => x.Id == key);
        }

        protected override IReadOnlyList<KeyValuePair<int, LanguageLookup>> GetAllDbValues()
        {
            var practitioner = new PractitionerQueryHelper();
            var response = practitioner.GetLookup(PractitionerLookupType.ActiveCredentialsLanguages2).ToList();

            return response.Select(GetKeyValuePair).ToList();
        }

        protected override LanguageLookup MapToTResultType(LookupTypeDto item)
        {
            return new LanguageLookup { SamId = item.Id, DisplayText = item.DisplayName };
        }

        private KeyValuePair<int, LanguageLookup> GetKeyValuePair(LookupTypeDto item)
        {
            return new KeyValuePair<int, LanguageLookup>(item.Id, MapToTResultType(item));
        }
    }
}
