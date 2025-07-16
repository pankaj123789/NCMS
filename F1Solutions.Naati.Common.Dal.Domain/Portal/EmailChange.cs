using System;

namespace F1Solutions.Naati.Common.Dal.Domain.Portal
{
    public class EmailChange : EntityBase
    {
        public virtual Guid UserId { get; set; }
        public virtual string PrimaryEmailAddress { get; set; }
        public virtual string SecondaryEmailAddress { get; set; }
        public virtual int Reference { get; set; }
        public virtual DateTime Expiry { get; set; }
    }
}
