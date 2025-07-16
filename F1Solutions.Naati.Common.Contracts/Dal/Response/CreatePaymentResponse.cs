namespace F1Solutions.Naati.Common.Contracts.Dal.Response
{
    public class CreatePaymentResponse : FinanceServiceResponse
    {
        public string PaymentId { get; set; }
        public string Reference { get; set; }
        public int? OperationId { get; set; }
    }
}