using System;

namespace F1Solutions.Naati.Common.Dal.Domain
{
    public class AuditLog : EntityBase
    {
        public virtual string RecordName{ get; set; }
        public virtual int? ParentID { get; set; }
        public virtual int RecordID { get; set; }
        public virtual string ChangeDetail { get; set; }
        public virtual User User { get; set; }
        public virtual AuditType AuditType { get; set; }
        public virtual DateTime DateTime  { get; set; }
        public virtual string ParentName { get; set; }
    }
}
