using System;

namespace Ncms.Contracts.Models.UserSearch
{
    public class UserSearchModel
    {
        public virtual int SearchId { get; set; }
        public virtual string SearchName { get; set; }
        public virtual string SearchType { get; set; }
        public virtual DateTime ModifiedDate { get; set; }
        public virtual string CriteriaJson { get; set; }
        public virtual int UserId { get; set; }
    }
}
