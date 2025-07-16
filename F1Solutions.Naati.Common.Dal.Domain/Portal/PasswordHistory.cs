using System;

namespace F1Solutions.Naati.Common.Dal.Domain.Portal
{
    public class PasswordHistory : EntityBase
    {
        public virtual string Password { get; set; }
        public virtual Guid UserId{ get; set; }
        public virtual DateTime CreatedDateTime{ get; set; }
    }
}
