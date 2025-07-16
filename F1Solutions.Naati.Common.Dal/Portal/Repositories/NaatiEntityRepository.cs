using System.Linq;
using F1Solutions.Naati.Common.Contracts.Dal.Portal;
using F1Solutions.Naati.Common.Dal.Domain;

namespace F1Solutions.Naati.Common.Dal.Portal.Repositories
{
    public interface INaatiEntityRepository : IRepository<NaatiEntity>
    {
        NaatiEntity FindByNaatiNumber(int naatiNumber);

        NaatiEntity FindByEntityId(int entityId);
    }

    public class NaatiEntityRepository : SecuredRepository<NaatiEntity>, INaatiEntityRepository
    {
        public NaatiEntityRepository(ICustomSessionManager sessionManager, IDataSecurityProvider dataSecurityProvider)
            : base(sessionManager, dataSecurityProvider)
        {
        }

        public NaatiEntity FindByNaatiNumber(int naatiNumber)
        {
            return Session.Query<NaatiEntity>()
                .Single(e => e.NaatiNumber == naatiNumber);
        }

        public NaatiEntity FindByEntityId(int entityId)
        {
            return base.Get(entityId);
        }

        protected override string GetPrimaryEmailOfRecordOwner(NaatiEntity record)
        {
            return record.PrimaryEmail?.EmailAddress;
        }
    }
}
