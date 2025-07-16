using System;
using System.Collections.Generic;
using System.Linq;
using F1Solutions.Naati.Common.Contracts.Dal.Portal;
using F1Solutions.Naati.Common.Dal.Domain.Portal;
using NHibernate;

namespace F1Solutions.Naati.Common.Dal.Portal.Repositories
{
    public interface IEmailChangeRepository : IRepository<EmailChange>
    {
        EmailChange GetByUserId(Guid userId);

        IEnumerable<EmailChange> GetByReference(int reference, DateTime expiryDate);

        void ChangeUserName(int emailChangeId);
        void RemoveEmailChangesForUser(Guid userId);
    }
    public class EmailChangeRepository : Repository<EmailChange>, IEmailChangeRepository
    {
        public EmailChangeRepository(ICustomSessionManager sessionManager) : base(sessionManager)
        {
        }

        public EmailChange GetByUserId(Guid userId)
        {
            return this.Session.Query<EmailChange>().SingleOrDefault(x => x.UserId == userId);
        }

        public IEnumerable<EmailChange> GetByReference(int reference, DateTime expiryDate)
        {
            return this.Session.Query<EmailChange>().Where(x => x.Reference == reference && x.Expiry >= expiryDate).ToList();
        }

        public void ChangeUserName(int emailChangeId)
        {
            using (var transaction = CreateSyncTransaction())
            {
                var emailChange = Session.Get<EmailChange>(emailChangeId);
                var userName = emailChange.SecondaryEmailAddress.ToLower();

                var query = Session.CreateSQLQuery("UPDATE aspnet_Users set UserName = :NewUserName, LoweredUserName = :NewUserName where UserId = :UserId")
                    .SetParameter("NewUserName", userName, NHibernateUtil.String)
                    .SetParameter("UserId", emailChange.UserId, NHibernateUtil.Guid);

                query.ExecuteUpdate();

                Session.Delete(emailChange);
                transaction.Commit();
            }
        }

        public override void SaveOrUpdate(EmailChange entity)
        {
            base.SaveOrUpdate(entity);
            Session.Flush();
        }

        public void RemoveEmailChangesForUser(Guid userId)
        {
            using (var transaction = CreateSyncTransaction())
            {
                var requests = this.Session.Query<EmailChange>().Where(x => x.UserId == userId);
                foreach (var request in requests)
                {
                    this.Session.Delete(request);
                }
                transaction.Commit();
            }}
    }
}
