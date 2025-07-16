using System.Collections.Generic;
using F1Solutions.Naati.Common.Contracts.Dal.DTO;

namespace F1Solutions.Naati.Common.Contracts.Dal.Response
{
    public class AvailableTestSessionsResponse
    {
        public IEnumerable<AvailableTestSessionDto> AvailableTestSessions { get; set; }
        public AvailableTestSessionDto AllocatedTestSession { get; set; }
    }
}