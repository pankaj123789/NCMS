using System.Collections.Generic;
using System.Linq;
using F1Solutions.Naati.Common.Contracts.Dal.DTO;
using F1Solutions.Naati.Common.Contracts.Dal.QueryServices;
using F1Solutions.Naati.Common.Contracts.Dal.Request;
using F1Solutions.Naati.Common.Contracts.Dal.Response;
using Ncms.Bl.Mappers;
using Ncms.Contracts.Models;
using Newtonsoft.Json;
using ILanguageService = Ncms.Contracts.ILanguageService;

namespace Ncms.Bl
{
    public class LanguageService : Contracts.ILanguageService
    {
        private readonly ILanguageQueryService _languageQueryService;

        public LanguageService(ILanguageQueryService languageQueryService)
        {
            _languageQueryService = languageQueryService;
        }
        public IList<LanguageModel> List(string request)
        {
            var panelsRequest = JsonConvert.DeserializeObject<GetLanguagesRequest>(request);
            GetLanguagesResponse panelsResponse = null;

          panelsResponse = _languageQueryService.GetLanguages(panelsRequest);

            var mapper = new LanguageMapper();
            return panelsResponse?.Languages.Select(mapper.Map).ToList() ?? new List<LanguageModel>();
        }

        public LanguageModel English()
        {
            LanguageDto englishLanguage = null;

             englishLanguage = _languageQueryService.GetEnglishLanguage();

            var mapper = new LanguageMapper();
            return mapper.Map(englishLanguage);
        }
    }
}
