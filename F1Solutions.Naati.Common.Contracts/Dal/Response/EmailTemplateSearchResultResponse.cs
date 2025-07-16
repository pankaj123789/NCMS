using System.Collections.Generic;

namespace F1Solutions.Naati.Common.Contracts.Dal.Response
{
    public class EmailTemplateSearchResultResponse
    {
        public IEnumerable<EmailTemplateResponse> Results { get; set; }
    }
}