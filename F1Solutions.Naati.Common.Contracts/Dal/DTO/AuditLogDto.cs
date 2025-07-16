using System;

namespace F1Solutions.Naati.Common.Contracts.Dal.DTO
{
    public class AuditLogDto
    {
        public virtual string RecordName { get; set; }
        public virtual int ParentID { get; set; }
        public virtual int RecordID { get; set; }
        public virtual string ChangeDetail { get; set; }
        public virtual string Username { get; set; }
        public virtual string AuditType { get; set; }
        public virtual DateTime DateTime { get; set; }
    }
}
