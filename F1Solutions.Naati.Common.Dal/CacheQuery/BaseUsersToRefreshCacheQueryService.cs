using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using F1Solutions.Naati.Common.Contracts;
using F1Solutions.Naati.Common.Contracts.Dal.DTO;

namespace F1Solutions.Naati.Common.Dal.CacheQuery
{
    public abstract class BaseUsersToRefreshCacheQueryService<TIdentifierKey, TResultType> : BaseLazyCacheQueryService<TIdentifierKey, UserRefreshData<TIdentifierKey>, UserRefreshData<TIdentifierKey>>, IUsersToRefreshQueryService<TIdentifierKey, TResultType> where TResultType : UserRefreshDto<TIdentifierKey>
    {
        protected DateTime LastRefreshRequestDate { get; set; }
        protected virtual int MaxJobDelaySeconds { get; }
        
        protected BaseUsersToRefreshCacheQueryService()
        {
            LastRefreshRequestDate = DateTime.Now;
            MaxJobDelaySeconds = Convert.ToInt32(ConfigurationManager.AppSettings["MaxUserRefreshDelaySeconds"]);
        }

        protected override TIdentifierKey TransformKey(TIdentifierKey key)
        {
            return key;
        }
       
        protected override UserRefreshData<TIdentifierKey> GetDbSingleValue(TIdentifierKey key)
        {
            return new UserRefreshData<TIdentifierKey>()
            {
                Identifier = key,
                InvalidCookies = new ConcurrentDictionary<string, string>(),
                RefreshTypes = new HashSet<UserRefreshType>(),
                NotificationIds = new HashSet<int>()
            };
        }

        protected override UserRefreshData<TIdentifierKey> MapToTResultType(UserRefreshData<TIdentifierKey> item)
        {
            return item;
        }

        public int RegisterUserCacheRefreshJob(TIdentifierKey userIdentifier, string invalidCookie)
        {
            var user = GetItem(userIdentifier);
            if (invalidCookie != null)
            {
                user.InvalidCookies.Add(invalidCookie, string.Empty);
            }
            user.RefreshTypes.Add(UserRefreshType.Cache);

            return GetNextJobDelay();
        }

        public int RegisterUserHubNotificationJob(TIdentifierKey userIdentifier, int notificationId)
        {
            var user = GetItem(userIdentifier);
           
            user.RefreshTypes.Add(UserRefreshType.Notification);
            user.NotificationIds.Add(notificationId);
            return GetNextJobDelay();
        }

        protected virtual int GetNextJobDelay()
        {
            if (Items.Any() && (DateTime.Now > LastRefreshRequestDate.AddSeconds(MaxJobDelaySeconds)))
            {
                LastRefreshRequestDate = DateTime.Now;
                return MaxJobDelaySeconds;
            }

            return 0;
        }

        public virtual IReadOnlyList<TResultType> GetUsersToRefresh()
        {
            var currentItems = Items.Values.Select(
                x =>
                {
                    var item = CreateInstance();
                    item.Identifier = x.Identifier;
                    item.RefreshTypes = x.RefreshTypes.ToList();
                    item.InvalidCookies = x.InvalidCookies.Keys.ToList();
                    item.NotificationIds = x.NotificationIds.ToList();
                    return item;
                }).ToList();

            return currentItems;
        }

        public int GetDefaultRefreshDelay()
        {
            return MaxJobDelaySeconds;
        }

        protected abstract TResultType CreateInstance();
        public void DeRegisterPendingUsers(IReadOnlyList<TResultType> users)
        {
            foreach (var user in users)
            {
                var transformedKey = TransformKey(user.Identifier);

                if (!Items.TryGetValue(transformedKey, out var userData))
                {
                    continue;
                }

                foreach (var invalidCookie in user.InvalidCookies)
                {
                    userData.InvalidCookies.Remove(invalidCookie);
                }

                foreach (var notificationId in user.NotificationIds)
                {
                    userData.NotificationIds.Remove(notificationId);
                }

                if (!userData.InvalidCookies.Any())
                {
                    userData.RefreshTypes.Remove(UserRefreshType.Cache);
                }

                if (!userData.NotificationIds.Any())
                {
                    userData.RefreshTypes.Remove(UserRefreshType.Notification);
                }

                if (!userData.InvalidCookies.Any() && !userData.NotificationIds.Any())
                {
                    RefreshItem(user.Identifier);
                }
            }
        }
    }

    public class UserRefreshData<TKey>
    {
        public TKey Identifier { get; set; }
        public IDictionary<string, string> InvalidCookies { get; set; }
        public ICollection<UserRefreshType> RefreshTypes { get; set; }
        public ICollection<int> NotificationIds { get; set; }
    }

 
}
