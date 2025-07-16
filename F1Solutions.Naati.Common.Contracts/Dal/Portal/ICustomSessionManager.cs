using NHibernate;

namespace F1Solutions.Naati.Common.Contracts.Dal.Portal
{
     public interface IRepository
    {
        ISession Session { get; }
        /// <summary>
        /// Create a transaction sync between all the repositories used inside the the transaction scope.
        /// </summary>
        ITransaction CreateSyncTransaction();
    }

    public interface IRepository<T, TId> : IRepository
    {
        void Delete(T entity);
        void DeleteWithFlush(T entity);
        T Get(TId id);
        void SaveOrUpdate(T entity);
    }

    public interface ICustomSessionManager
    {
        ITransaction CreateSyncTransaction();

        ISession OpenSession();
    }

    public interface IRepository<T> : IRepository<T, int>
    {
    }
}
