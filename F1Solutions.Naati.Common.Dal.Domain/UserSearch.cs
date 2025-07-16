using System;

namespace F1Solutions.Naati.Common.Dal.Domain
{
    public class UserSearch : EntityBase
    {
        public virtual string SearchName { get; set; }
        public virtual string SearchType { get; set; }
        public virtual DateTime ModifiedDate { get; set; }
        public virtual string CriteriaJson { get; set; }
        public virtual User User { get; set; }
    }
}
