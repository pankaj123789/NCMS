using F1Solutions.Naati.Common.Contracts.Dal.DTO;
using F1Solutions.Naati.Common.Contracts.Dal.Request;
using F1Solutions.Naati.Common.Contracts.Dal.Response;
using F1Solutions.Naati.Common.Contracts.Dal.Services;

namespace F1Solutions.Naati.Common.Contracts.Dal.QueryServices
{
    
    public interface ILanguageQueryService : IQueryService
    {
        
        GetLanguagesResponse GetLanguages(GetLanguagesRequest request);

        
        LanguageDto GetEnglishLanguage();
    }
}
