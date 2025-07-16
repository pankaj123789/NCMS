namespace F1Solutions.Naati.Common.Contracts.Dal.Request
{
    public class ValidateExaminerSecurityCodeRequest
    {
        public string SecurityCode { get; set; }
        public int NAATINumber { get; set; }
    }
}