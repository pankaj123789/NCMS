using F1Solutions.Naati.Common.Contracts.Dal.Portal;
using NHibernate;

namespace F1Solutions.Naati.Common.Dal.Portal
{
   

    public class Repository
    {
        protected readonly ICustomSessionManager mSessionManager;

        public Repository(ICustomSessionManager sessionManager)
        {
            mSessionManager = sessionManager;
        }

        public ISession Session => mSessionManager.OpenSession();

        public ITransaction CreateSyncTransaction()
        {
            return mSessionManager.CreateSyncTransaction();
        }
    }

    public class Repository<T, TId> : Repository, IRepository<T, TId>
    {
        public Repository(ICustomSessionManager sessionManager)
            : base(sessionManager)
        {
        }

        public virtual void Delete(T entity)
        {
            this.Session.Delete(entity);
            this.Session.Flush();
        }

        public virtual void DeleteWithFlush(T entity)
        {
            this.Session.Delete(entity);
            this.Session.Flush();
        }

        public virtual T Get(TId id)
        {
            return this.Session.Get<T>(id);
        }

        public virtual void SaveOrUpdate(T entity)
        {
            var session = Session;
            session.SaveOrUpdate(entity);
            session.Flush();
        }
    }

    public class Repository<T> : Repository<T, int>, IRepository<T>
    {
        public Repository(ICustomSessionManager sessionManager)
            : base(sessionManager)
        {
        }
    }
}
