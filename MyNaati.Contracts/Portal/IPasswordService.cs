using F1Solutions.Naati.Common.Contracts.Bl;
using F1Solutions.Naati.Common.Contracts.Dal.Portal;

namespace MyNaati.Contracts.Portal
{
    
    public interface IPasswordService : IInterceptableservice
    {
        
        void SavePasswordHistory(PasswordHistoryRequest model);
        
        string HashPassword(string salt, string data);
        
        bool ExistedPasswordHistory(PasswordHistoryRequest model);
    }

    
   
}
