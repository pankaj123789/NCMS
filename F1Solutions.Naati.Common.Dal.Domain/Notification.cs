using System;

namespace F1Solutions.Naati.Common.Dal.Domain
{
    public class Notification : EntityBase
    {
        public virtual NotificationType NotificationType { get; set; }
        public virtual string Parameter { get; set; }
        public virtual DateTime CreatedDate { get; set; }
        public virtual DateTime ExpiryDate { get; set; }
        public virtual User FromUser { get; set; }
        public virtual User ToUser { get; set; }
    }
}
