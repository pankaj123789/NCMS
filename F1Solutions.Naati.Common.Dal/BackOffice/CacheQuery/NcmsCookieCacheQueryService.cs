using System;
using F1Solutions.Naati.Common.Dal.CacheQuery;
using F1Solutions.Naati.Common.Dal.Domain;

namespace F1Solutions.Naati.Common.Dal.BackOffice.CacheQuery
{
    public class NcmsCookieCacheQueryService : BaseCookieCacheQueryService<NcmsInvalidCookie>
    {
        protected override NcmsInvalidCookie CreateNewInvalidCookie(string cookie, DateTime expiryDate)
        {
            return new NcmsInvalidCookie
            {
                Cookie = cookie,
                ExpiryDate = expiryDate
            };
        }
    }
}
