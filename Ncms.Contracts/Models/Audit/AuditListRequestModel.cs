using System;

namespace Ncms.Contracts.Models.Audit
{
    public class AuditListRequestModel
    {
        public int? AuditTypeId { get; set; }
        public string Username { get; set; }
        public int PageSize { get; set; }
        public int PageNumber { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public string RecordName { get; set; }
        public string ChangeDetailPart { get; set; }
        public int? ParentID { get; set; }
        public int? RecordID { get; set; }
        public string ParentName { get; set; }


    }
}
