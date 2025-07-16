using System.Collections.Generic;
using System.Linq;
using F1Solutions.Naati.Common.Contracts.Dal.Portal;
using NHibernate.Linq;
using Postcode = F1Solutions.Naati.Common.Dal.Domain.Postcode;

namespace F1Solutions.Naati.Common.Dal.Portal.Repositories
{
    public interface IPostcodeRepository : Contracts.Dal.Portal.IRepository<Postcode>
    {
        IList<Postcode> GetAll();
    }

    public class PostcodeRepository : Repository<Postcode>, IPostcodeRepository
    {
        public PostcodeRepository(ICustomSessionManager sessionManager)
            : base(sessionManager)
        { }

        public IList<Postcode> GetAll()
        {
            return Session.Query<Postcode>().Fetch(x=>x.Suburb).ToList();
        }
    }
}
