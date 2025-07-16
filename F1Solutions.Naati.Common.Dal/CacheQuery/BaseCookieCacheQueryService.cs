using System;
using System.Collections.Generic;
using System.Linq;
using F1Solutions.Naati.Common.Contracts.Dal;
using F1Solutions.Naati.Common.Contracts.Dal.DTO;
using F1Solutions.Naati.Common.Dal.Domain;
using F1Solutions.Naati.Common.Dal.NHibernate.Configuration;
using F1Solutions.Naati.Common.Dal.NHibernate.Extensions;

namespace F1Solutions.Naati.Common.Dal.CacheQuery
{
    public abstract class BaseCookieCacheQueryService<TInvalidCookie> : BaseCacheQueryService<string, CookieDto, TInvalidCookie>, ICookieQueryService where TInvalidCookie : IInvalidCookie
    {
        protected override string TransformKey(string key)
        {
            return key?.ToUpper();
        }

        protected override TInvalidCookie GetDbSingleValue(string key)
        {
            return NHibernateSession.Current.Query<TInvalidCookie>().FirstOrDefault(x => x.Cookie == key);
        }

        protected override IReadOnlyList<KeyValuePair<string, CookieDto>> GetAllDbValues()
        {
            return NHibernateSession.Current.Query<TInvalidCookie>()
                .Where(x => x.ExpiryDate > DateTime.Now).ToList()
                .GroupBy(x => x.Cookie)
                .Select(y => GetKeyValuePair(y.First())).ToList();
        }

        protected override CookieDto MapToTResultType(TInvalidCookie item)
        {
            return new CookieDto
            {
                Cookie = item.Cookie
            };
        }

        public bool IsInvalidCookie(string cookie)
        {
            return HasItem(cookie);
        }

        public void InvalidateCacheCookie(string cookie)
        {
            SetItem(cookie, new CookieDto() { Cookie = cookie });
        }

        public void RemoveExpiredInvalidCookies()
        {
            var iterationNumber = 0;
            List<TInvalidCookie> results;
            do
            {
                results = NHibernateSession.Current.Query<TInvalidCookie>().Where(x => x.ExpiryDate < DateTime.Now).Take(300).ToList();
                NHibernateSession.Current.DeleteList(results);
                iterationNumber++;
            }
            while (results.Any() && (iterationNumber < 10));
        }

        private KeyValuePair<string, CookieDto> GetKeyValuePair(TInvalidCookie item)
        {
            return new KeyValuePair<string, CookieDto>(item.Cookie, MapToTResultType(item));
        }

        protected abstract TInvalidCookie CreateNewInvalidCookie(string cookie, DateTime expiryDate);

        public void InvalidateDbCookie(string cookie, DateTime expiryDate)
        {
            if (string.IsNullOrWhiteSpace(cookie) || (expiryDate == default))
            {
                return;
            }

            var invalidCookie = CreateNewInvalidCookie(cookie, expiryDate);
            NHibernateSession.Current.SaveOrUpdate(invalidCookie);
        }
    }
}
