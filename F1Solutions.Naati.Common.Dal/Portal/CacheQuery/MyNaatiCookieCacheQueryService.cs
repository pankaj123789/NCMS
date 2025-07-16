using System;
using F1Solutions.Naati.Common.Dal.CacheQuery;
using F1Solutions.Naati.Common.Dal.Domain;

namespace F1Solutions.Naati.Common.Dal.Portal.CacheQuery
{
    public class MyNaatiCookieCacheQueryService : BaseCookieCacheQueryService<MyNaatiInvalidCookie>
    {
        protected override MyNaatiInvalidCookie CreateNewInvalidCookie(string cookie, DateTime expiryDate)
        {
            return new MyNaatiInvalidCookie
            {
                Cookie = cookie,
                ExpiryDate = expiryDate
            };
        }
    }
}
