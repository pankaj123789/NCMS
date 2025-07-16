using F1Solutions.Naati.Common.Contracts.Dal.Portal;
using F1Solutions.Naati.Common.Dal.Portal.Repositories;
using MyNaati.Contracts.Portal;

namespace MyNaati.Bl.Portal
{
   
    public class PasswordService : IPasswordService
    {
        private readonly IPasswordHistoryRepository mPasswordHistoryRepository;

        public PasswordService(IPasswordHistoryRepository passwordHistoryRepository)
        {
            mPasswordHistoryRepository = passwordHistoryRepository;
        }

        public void SavePasswordHistory(PasswordHistoryRequest model)
        {
            mPasswordHistoryRepository.CreatePasswordHistory(model);
            mPasswordHistoryRepository.RemovePasswordHistory(model);
        }

        public string HashPassword(string salt,string data)
        {
            return mPasswordHistoryRepository.HashPassword(salt, data);
        }

        public bool ExistedPasswordHistory(PasswordHistoryRequest model)
        {
            return mPasswordHistoryRepository.ExistedPasswordHistory(model);
        }
    }
}