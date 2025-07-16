using F1Solutions.Naati.Common.Contracts.Dal.Enum;

namespace F1Solutions.Naati.Common.Contracts.Dal.Request
{
    public class GetTokenRequest
    {
        public TokenRequestType Type { get; set; }
        public int Value { get; set; }
    }
}