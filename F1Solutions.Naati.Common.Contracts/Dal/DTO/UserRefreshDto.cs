using System.Collections.Generic;

namespace F1Solutions.Naati.Common.Contracts.Dal.DTO
{
    public abstract class UserRefreshDto<TKey>
    {
        public TKey Identifier { get; set; }
        public IEnumerable<UserRefreshType> RefreshTypes { get; set; }
        public IEnumerable<string> InvalidCookies { get; set; }
        public IEnumerable<int> NotificationIds { get; set; }
    }

   
}
