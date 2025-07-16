using System;

namespace F1Solutions.Naati.Common.Contracts.Dal.DTO
{
    public class UserSearchDto
    {
        public virtual int SearchId { get; set; }
        public virtual int UserId { get; set; }
        public virtual string SearchName { get; set; }
        public virtual string SearchType { get; set; }
        public virtual DateTime ModifiedDate { get; set; }
        public virtual string CriteriaJson { get; set; }
    }
}
