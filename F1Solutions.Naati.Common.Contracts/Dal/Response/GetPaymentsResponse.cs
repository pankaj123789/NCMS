using F1Solutions.Naati.Common.Contracts.Dal.DTO;

namespace F1Solutions.Naati.Common.Contracts.Dal.Response
{
    public class GetPaymentsResponse : FinanceServiceResponse
    {
        public PaymentDto[] Payments { get; set; }
    }
}