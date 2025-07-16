using System.Collections.Generic;
using System.Linq;
using F1Solutions.Global.Common.Mapping;
using F1Solutions.Naati.Common.Contracts.Dal.DTO;
using F1Solutions.Naati.Common.Contracts.Dal.QueryServices;
using F1Solutions.Naati.Common.Contracts.Dal.Request;
using Ncms.Bl.Mappers;
using Ncms.Contracts;
using Ncms.Contracts.Models.Audit;

namespace Ncms.Bl
{
    public class AuditService : IAuditService
    { 
        private readonly IAuditLogQueryService _auditLogQueryService;
        private readonly IAutoMapperHelper _autoMapperHelper;

        public AuditService(IAuditLogQueryService auditLogQueryService, IAutoMapperHelper autoMapperHelper)
        {
            _auditLogQueryService = auditLogQueryService;
            _autoMapperHelper = autoMapperHelper;
        }

        public IList<AuditLogModel> List(AuditListRequestModel request)
        {
            var response = _auditLogQueryService.FindAuditLogs(_autoMapperHelper.Mapper.Map<FindAuditLogsRequest>(request));

            return response?.AuditLogDTOs?.Select(r => _autoMapperHelper.Mapper.Map<AuditLogModel>(r))?.ToList() ?? new List<AuditLogModel>();
        }
    }
}
    