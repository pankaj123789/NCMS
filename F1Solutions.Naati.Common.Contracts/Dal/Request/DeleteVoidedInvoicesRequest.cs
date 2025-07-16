using System.Collections.Generic;
using F1Solutions.Naati.Common.Contracts.Dal.DTO;

namespace F1Solutions.Naati.Common.Contracts.Dal.Request
{
    public class DeleteVoidedInvoicesRequest
    {
        public IEnumerable<CredentialWorkflowFeeDto> CredentialWorkflowFees;
    }
}