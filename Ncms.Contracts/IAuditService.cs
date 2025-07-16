using System.Collections.Generic;
using Ncms.Contracts.Models.Audit;

namespace Ncms.Contracts
{
    public interface IAuditService
    {
        IList<AuditLogModel> List(AuditListRequestModel request);
    }
}
