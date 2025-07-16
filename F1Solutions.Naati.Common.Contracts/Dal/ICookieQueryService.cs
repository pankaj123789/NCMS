using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using F1Solutions.Naati.Common.Contracts.Dal.QueryServices;

namespace F1Solutions.Naati.Common.Contracts.Dal
{
    public interface ICookieQueryService : ICacheQueryService
    {
        bool IsInvalidCookie(string cookie);

        void InvalidateCacheCookie(string cookie);
        void InvalidateDbCookie(string cookie, DateTime expiryDate);

        void RemoveExpiredInvalidCookies();
    }
}
