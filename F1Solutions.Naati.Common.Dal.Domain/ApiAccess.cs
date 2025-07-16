using System;
using System.Data.SqlTypes;

namespace F1Solutions.Naati.Common.Dal.Domain
{
    public class ApiAccess : EntityBase
    {
        public virtual Institution Institution { get; set; }
        public virtual string PublicKey { get; set; }
        public virtual string PrivateKey { get; set; }
        public virtual DateTime CreatedDate { get; set; }
        public virtual DateTime ModifiedDate { get; set; }
        public virtual User ModifiedUser { get; set; }
        public virtual bool Inactive { get; set; }
        public virtual int Permissions { get; set; }
    }
}
