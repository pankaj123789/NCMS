namespace F1Solutions.Naati.Common.Contracts.Dal.Response
{
    public class FinanceServiceResponse : ServiceResponse
    {
        public bool ApiKeyError { get; set; }
        public bool RateLimitExceeded { get; set; }
    }
}