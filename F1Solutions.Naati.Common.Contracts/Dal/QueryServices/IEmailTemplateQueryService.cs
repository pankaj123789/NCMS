using F1Solutions.Naati.Common.Contracts.Dal.Request;
using F1Solutions.Naati.Common.Contracts.Dal.Response;

namespace F1Solutions.Naati.Common.Contracts.Dal.QueryServices
{
    
	public interface IEmailTemplateQueryService : IQueryService
    {
		
		EmailTemplateResponse Get(EmailTemplateRequest id);

		
		void Save(EmailTemplateRequest model);

		
		EmailTemplateSearchResultResponse SearchEmailTemplates(EmailTemplateSearchRequest request);
	}
}
