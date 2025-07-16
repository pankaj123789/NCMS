using System;

namespace F1Solutions.Naati.Common.Dal.Domain.Portal
{
    public class MyNaatiUser : EntityBase
    {
        public virtual Guid AspUserId { get; set; }
        public virtual int NaatiNumber { get; set; }
    }
}
