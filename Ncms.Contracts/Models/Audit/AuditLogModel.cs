using System;

namespace Ncms.Contracts.Models.Audit
{
    public class AuditLogModel
    {
        public string RecordName { get; set; }
        public int ParentID { get; set; }
        public int RecordID { get; set; }
        public string ChangeDetail { get; set; }
        public string Username { get; set; }
        public string AuditType { get; set; }
        public DateTime DateTime { get; set; }
    }
}
