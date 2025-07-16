using System;
using System.Collections.Generic;
using System.Linq;
using F1Solutions.Global.Common.Logging;
using F1Solutions.Naati.Common.Contracts.Dal.BackOffice.CacheQuery;
using F1Solutions.Naati.Common.Contracts.Dal.DTO;
using F1Solutions.Naati.Common.Contracts.Security;
using F1Solutions.Naati.Common.Dal.CacheQuery;
using F1Solutions.Naati.Common.Dal.Domain;
using F1Solutions.Naati.Common.Dal.NHibernate.Configuration;

namespace F1Solutions.Naati.Common.Dal.BackOffice.CacheQuery
{
    public class NcmsUserPermissionCacheQueryService : BaseLazyCacheQueryService<string, IReadOnlyList<UserPermissionsDto>, IEnumerable<SecurityRule>>, INcmsUserPermissionQueryService
    {
        protected override string TransformKey(string userName)
        {
            return userName?.ToUpper();
        }

        protected override IEnumerable<SecurityRule> GetDbSingleValue(string key)
        {
            var userGroups = NHibernateSession.Current.Query<UserRole>()
                  .Where(x => x.User.Active && (x.User.UserName == key))
                  .GroupBy(x => x.User.UserName)
                  .ToList();

            if (!userGroups.Any())
            {
                return null;
            }

            return userGroups.First().SelectMany(y => y.SecurityRole.SecurityRules);
        }

        protected override IReadOnlyList<UserPermissionsDto> MapToTResultType(IEnumerable<SecurityRule> item)
        {
            return item.Select(
                x => new UserPermissionsDto
                {
                    Noun = (SecurityNounName)Enum.Parse(typeof(SecurityNounName), x.SecurityNoun.Name),
                    NounName = x.SecurityNoun.Name,
                    NounDisplayName = x.SecurityNoun.DisplayName,
                    VerbMask = x.SecurityVerbMask
                }).ToList();
        }

        public IReadOnlyList<UserPermissionsDto> GetUserPermissionsByUserName(string userName)
        {
            var result = GetItem(userName);
            if (!(result?.Any() ?? false))
            {
                LoggingHelper.LogWarning($"Permissions not found for user {userName}");
                return new List<UserPermissionsDto>();
            }

            return result;
        }

        public void RefreshUserCache(string userName)
        {
            RefreshItem(userName);
        }
    }
}
