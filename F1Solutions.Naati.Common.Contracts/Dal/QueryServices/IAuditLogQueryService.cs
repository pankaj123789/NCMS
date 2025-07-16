using F1Solutions.Naati.Common.Contracts.Dal.Request;
using F1Solutions.Naati.Common.Contracts.Dal.Response;

namespace F1Solutions.Naati.Common.Contracts.Dal.QueryServices
{
    
    public interface IAuditLogQueryService : IQueryService
    {
        
        FindAuditLogsResponse FindAuditLogs(FindAuditLogsRequest request);
    }
}
