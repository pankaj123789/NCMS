using System;
using F1Solutions.Naati.Common.Contracts.Dal.Enum;

namespace F1Solutions.Naati.Common.Dal.Finance
{
    public class ExternalAccountingOperationResult<T>
    {
        public ExternalAccountingOperationStatusName Status { get; set; }
        public int? OperationId { get; set; }
        public bool Success { get; set; }
        public string ErrorMessage { get; set; }
        public string WarningMessage { get; set; }
        public Exception Exception { get; set; }
        public T Result { get; set; }
    }
}