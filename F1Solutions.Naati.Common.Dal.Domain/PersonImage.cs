using System;

namespace F1Solutions.Naati.Common.Dal.Domain
{
    public class PersonImage : EntityBase
    {
        
        public virtual Person Person { get; set; }
        public virtual Byte[] Photo { get; set; }
        public virtual Byte[] Signature { get; set; }
        public virtual Byte[] ApplicationFirstPage { get; set; }
        public virtual Byte[] ApplicationLastPage { get; set; }
        public virtual DateTime? PhotoDate { get; set; }

        public override IAuditObject RootAuditObject
        {
            get
            {
                return Person.RootAuditObject;
            }
        }
    }
}
