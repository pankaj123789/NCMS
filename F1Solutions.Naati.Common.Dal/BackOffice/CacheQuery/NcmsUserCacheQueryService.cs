using System.Collections.Generic;
using System.Linq;
using F1Solutions.Naati.Common.Contracts.Dal.BackOffice.CacheQuery;
using F1Solutions.Naati.Common.Contracts.Dal.DTO;
using F1Solutions.Naati.Common.Contracts.Dal.QueryServices;
using F1Solutions.Naati.Common.Dal.CacheQuery;
using F1Solutions.Naati.Common.Dal.Domain;
using F1Solutions.Naati.Common.Dal.NHibernate.Configuration;

namespace F1Solutions.Naati.Common.Dal.BackOffice.CacheQuery
{
    public class NcmsUserCacheQueryService : BaseLazyCacheQueryService<string, UserDetailsDto, User>, INcmsUserCacheQueryService
    {
        protected override string TransformKey(string userName)
        {
            return userName?.ToUpper();
        }

        protected override User GetDbSingleValue(string userName)
        {
            return NHibernateSession.Current.Query<User>().FirstOrDefault(x => x.Active && x.UserName == userName);
        }

        protected override UserDetailsDto MapToTResultType(User item)
        {
            return new UserDetailsDto
            {
                Id = item.Id,
                FullName = item.FullName,
                Name = item.UserName,
                OfficeId = item.Office.Id,
                FailedPasswordAttemptCount = item.FailedPasswordAttemptCount,
                IsLockedOut = item.IsLockedOut,
                LastLockoutDate = item.LastLockoutDate,
                Password = item.Password
            };
        }

        public UserDetailsDto GetUser(string userName)
        {
            return GetItem(userName);
        }

        public void RefreshUserCache(string userName)
        {
            RefreshItem(userName);
        }
    }
 
}
