using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using F1Solutions.Naati.Common.Contracts.Dal.Portal;
using F1Solutions.Naati.Common.Dal.Domain.Portal;

namespace F1Solutions.Naati.Common.Dal.Portal.Repositories
{
    public interface IPasswordHistoryRepository
    {
        void CreatePasswordHistory(PasswordHistoryRequest model);
        void RemovePasswordHistory(PasswordHistoryRequest model);
        string HashPassword(string salt, string data);
        bool ExistedPasswordHistory(PasswordHistoryRequest model);
    }

    public class PasswordHistoryRepository : Repository<PasswordHistory>, IPasswordHistoryRepository
    {
        public PasswordHistoryRepository(ICustomSessionManager sessionManager) : base(sessionManager)
        {
        }

        public void CreatePasswordHistory(PasswordHistoryRequest model)
        {
            if (model == null) return;

            var passwordHistory = new PasswordHistory()
            {
                UserId = model.UserId,
                Password = model.Password,
                CreatedDateTime = DateTime.Now
            };
          
            SaveOrUpdate(passwordHistory);
            Session.Flush();
        }

        public void RemovePasswordHistory(PasswordHistoryRequest model)
        {
            if (model == null) return;

            var count = Session.Query<PasswordHistory>().Count(x => x.UserId == model.UserId);
            if (count > model.DeleteCount)
            {
                var deletePassword =
                    Session.Query<PasswordHistory>()
                        .Where(x => x.UserId == model.UserId)
                        .OrderBy(x => x.CreatedDateTime)
                        .First();
                Session.Delete(deletePassword);
                Session.Flush();
            }
        }

        public bool ExistedPasswordHistory(PasswordHistoryRequest model)
        {
            if (model == null) return false;
            var isExisted = Session.Query<PasswordHistory>().Where(x => x.UserId == model.UserId)
                .OrderByDescending(x => x.CreatedDateTime).Take(model.DeleteCount).ToList().Any(x =>
                    x.Password == HashPassword(model.UserId.ToString(), model.Password));
            return isExisted;
        }

        public string HashPassword(string salt, string data)
        {
            using (var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(salt)))
            {
                return Convert.ToBase64String(hmac.ComputeHash(Encoding.UTF8.GetBytes(data)));
            }
        }
    }
}
