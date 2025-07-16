using System;
using F1Solutions.Naati.Common.Contracts.Bl;
using F1Solutions.Naati.Common.Contracts.Dal.Portal;
using MyNaati.Contracts.Portal.Users;

namespace MyNaati.Contracts.Portal
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IUserService" in both code and config file together.
    
    public interface IUserService : IInterceptableservice
    {
        //
        //int UserRegistrationRequest(UserRegistrationRequest request);

        //
        //int ValidateUserRegistrationRequest(UserRegistrationRequest request);

        //
        //void DeleteRegistrationRequest(DeleteRegistrationRequest request);

        //
        //void ResolveRegistrationRequest(ResolveRegistrationRequest request);

        //
        //SearchResults<RegistrationRequestSearchResult> SearchRegistrationRequests(SearchRegistrationRequest request);

        //
        //RegistrationRequestSearchResult GetRegistrationRequest(GetRegistrationRequest request);

        
        EmailChangeResponse RegisterEmailChange(EmailChangeRequest request);

        
        GetEmailChangeResponse GetRegisteredEmailChange(GetEmailChangeRequest request);

        
        GetEmailChangeResponse GetRegisteredEmailChangeByUser(Guid userId);

        
        void ChangeUserName(ChangeUserNameRequest request);

        
        void CreateUser(UserRequest model);

        
        UserResponse GetUser(Guid userId);

        
        void DeleteUser(Guid userId);
    }

    


    
    
}
