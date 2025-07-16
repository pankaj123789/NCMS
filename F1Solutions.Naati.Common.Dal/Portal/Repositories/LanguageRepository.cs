using System.Collections.Generic;
using System.Linq;
using F1Solutions.Naati.Common.Contracts.Dal.Portal;
using Language = F1Solutions.Naati.Common.Dal.Domain.Language;

namespace F1Solutions.Naati.Common.Dal.Portal.Repositories
{
    public interface ILanguageRepository : Contracts.Dal.Portal.IRepository<Language>
    {
        IList<Language> FindLanguagesForWebUsage();
    }

    public class LanguageRepository : Repository<Language>, ILanguageRepository
    {
        public LanguageRepository(ICustomSessionManager sessionManager)
            : base(sessionManager)
        {
        }

        public IList<Language> FindLanguagesForWebUsage()
        {
            return Session.Query<Language>().ToList();
        }
    }
}
