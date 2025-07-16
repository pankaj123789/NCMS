using System;
using System.Linq;
using F1Solutions.Naati.Common.Contracts.Dal.Portal;
using F1Solutions.Naati.Common.Dal.Domain.Portal;

namespace F1Solutions.Naati.Common.Dal.Portal.Repositories
{
    public interface IUserRepository : IRepository<MyNaatiUser>
    {
        void CreateUser(UserRequest model);
        UserResponse GetUser(Guid userId);
        void DeleteUser(Guid userId);
    }

    public class UserRepository : Repository<MyNaatiUser>, IUserRepository
    {
        public UserRepository(ICustomSessionManager sessionManager) : base(sessionManager)
        {
        }

        public UserResponse GetUser(Guid userId)
        {
            return Session.Query<MyNaatiUser>().Where(x => x.AspUserId == userId).Select(x => new UserResponse()
            {
                NaatiNumber = x.NaatiNumber,
                AspUserId = x.AspUserId
            }).SingleOrDefault();
        }

        public void DeleteUser(Guid userId)
        {
            var user = this.Session.Query<MyNaatiUser>().First(x => x.AspUserId == userId);
            this.Session.Delete(user);
            this.Session.Flush();
        }

        public void CreateUser(UserRequest model)
        {
            if (model == null) return;

            var user = new MyNaatiUser
            {
                NaatiNumber = model.NaatiNumber,
                AspUserId = model.AspUserId
            };
          
            SaveOrUpdate(user);
            Session.Flush();
        }
    }
}
