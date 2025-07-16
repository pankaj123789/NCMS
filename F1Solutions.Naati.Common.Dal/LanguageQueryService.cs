using System.Linq;
using F1Solutions.Naati.Common.Contracts.Dal.DTO;
using F1Solutions.Naati.Common.Contracts.Dal.QueryServices;
using F1Solutions.Naati.Common.Contracts.Dal.Request;
using F1Solutions.Naati.Common.Contracts.Dal.Response;
using F1Solutions.Naati.Common.Dal.Domain;
using F1Solutions.Naati.Common.Dal.NHibernate.Configuration;

namespace F1Solutions.Naati.Common.Dal
{
   
    public class LanguageQueryService : ILanguageQueryService
    {
        public GetLanguagesResponse GetLanguages(GetLanguagesRequest request)
        {
            var query = NHibernateSession.Current.Query<Language>();

            if (request.HasPanel.HasValue)
            {
                if (request.HasPanel.Value)
                {
                    query = from language in query
                            join panel in NHibernateSession.Current.Query<Panel>() on language.Id equals panel.Language.Id
                            where panel != null
                            select language;
                }
                else
                {
                    query = from language in query
                            join panel in NHibernateSession.Current.Query<Panel>() on language.Id equals panel.Language.Id
                            where panel == null
                            select language;
                }
            }

            var languages = query.Select(MapLanguage);

            return new GetLanguagesResponse
            {
                Languages = languages.ToArray()
            };
        }

        public LanguageDto GetEnglishLanguage()
        {
            var systemParameterQuery = NHibernateSession.Current.Query<SystemParameter>();
            var languageQuery = NHibernateSession.Current.Query<Language>();

            var englishLanguageQuery =
                from language in languageQuery
                where language.Id ==
                (
                    from systemParameter in systemParameterQuery
                    select systemParameter.EnglishLanguageId
                ).First()
                select language;

            var englishLanguage = englishLanguageQuery.First();

            return MapLanguage(englishLanguage);
        }

        private static LanguageDto MapLanguage(Language language)
        {
            return new LanguageDto
            {
                LanguageId = language.Id,
                Name = language.Name,
                Code = language.Code,
                GroupLanguageId = language.LanguageGroup != null ? language.LanguageGroup.Id : (int?)null
            };
        }
    }
}
