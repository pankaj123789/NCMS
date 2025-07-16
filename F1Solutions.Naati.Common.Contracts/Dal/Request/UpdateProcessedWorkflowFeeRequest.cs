using System.Collections.Generic;
using F1Solutions.Naati.Common.Contracts.Dal.DTO;

namespace F1Solutions.Naati.Common.Contracts.Dal.Request
{
    public class UpdateProcessedWorkflowFeeRequest
    {
        public IEnumerable<ProcessFeeDto> Fees;
    }
}