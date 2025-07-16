using F1Solutions.Naati.Common.Contracts.Dal.Portal;
using F1Solutions.Naati.Common.Dal.NHibernate.Configuration;
using NHibernate;

namespace F1Solutions.Naati.Common.Dal.NHibernate
{
    /// <summary>
    /// Sync session transactions on the same thread
    /// </summary>
    public class CustomSessionManager : ICustomSessionManager
    {
        public CustomSessionManager()
        {
           
           
        }

        public ITransaction CreateSyncTransaction()
        {
            return this.OpenSession().BeginTransaction();
        }

        public ISession OpenSession()
        {
            return NHibernateSession.Current;
        }
      
    }
}
