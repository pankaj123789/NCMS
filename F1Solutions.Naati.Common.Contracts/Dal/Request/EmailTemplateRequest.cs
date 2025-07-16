using F1Solutions.Naati.Common.Contracts.Dal.Response;

namespace F1Solutions.Naati.Common.Contracts.Dal.Request
{
    public class EmailTemplateRequest : EmailTemplateResponse
    {
        public int UserId { get; set; }
    }
}