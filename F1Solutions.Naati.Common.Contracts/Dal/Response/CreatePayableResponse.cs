using System;

namespace F1Solutions.Naati.Common.Contracts.Dal.Response
{
    public class CreatePayableResponse : FinanceServiceResponse
    {
        public string Number { get; set; }
        public Guid Id { get; set; }
        public string Reference { get; set; }
        public int? OperationId { get; set; }
        public string PaymentErrorMessage { get; set; }
        public string PaymentWarningMessage { get; set; }
    }
}