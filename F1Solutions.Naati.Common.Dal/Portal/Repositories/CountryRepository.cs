using System.Collections.Generic;
using System.Linq;
using F1Solutions.Naati.Common.Contracts.Dal.Portal;
using Country = F1Solutions.Naati.Common.Dal.Domain.Country;

namespace F1Solutions.Naati.Common.Dal.Portal.Repositories
{
    public interface ICountryRepository : Contracts.Dal.Portal.IRepository<Country>
    {
        IList<Country> GetAll();
        Country GetAustralia();
    }

    public class CountryRepository : Repository<Country>, ICountryRepository
    {
        public CountryRepository(ICustomSessionManager sessionManager)
            : base(sessionManager)
        {
        }

        public IList<Country> GetAll()
        {
            return Session.Query<Country>().ToList();
        }

        public Country GetAustralia()
        {
            return Session.Query<Country>().SingleOrDefault(e => e.Name == "Australia");
        }
    }
}
