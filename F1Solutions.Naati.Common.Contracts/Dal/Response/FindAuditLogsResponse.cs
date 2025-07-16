using F1Solutions.Naati.Common.Contracts.Dal.DTO;

namespace F1Solutions.Naati.Common.Contracts.Dal.Response
{
    public class FindAuditLogsResponse
    {
        public AuditLogDto[] AuditLogDTOs { get; set; }
        public int TotalResultsCount { get; set; }
        public int LastPage { get; set; }
    }
}