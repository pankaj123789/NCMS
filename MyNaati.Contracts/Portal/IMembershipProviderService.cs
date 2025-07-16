using F1Solutions.Naati.Common.Contracts.Bl;
using F1Solutions.Naati.Common.Contracts.Bl.DTO;
using F1Solutions.Naati.Common.Contracts.Dal.Response;

namespace MyNaati.Contracts.Portal
{
    
    public interface IMembershipProviderService : IInterceptableservice
    {
        
        void ActivateUser(string userName);

        
        void DeactivateUser(string userName);
        
        
        bool ChangePassword(string username, string oldPassword, string newPassword);

        
        MembershipCreateResult CreateUser(string username, string password, string passwordQuestion,
                                          string passwordAnswer, bool isApproved, object providerUserKey, bool addEmail, string givenName, 
                                          string familyName, bool isNonCandidate, int naatiNumber);

        
        ePortalUser GetUser(string username, bool userIsOnline);

        
        ePortalUser[] GetUsers(string[] userNames);

        
        string ResetPassword(string primaryEmail);

        
        bool UpdateUserEmailAddressIfPresent(string usernameToFind, string newEmail);

        
        bool ValidateUser(string username, string password);

        
        bool IsLockedOut(string username);

        
        bool UnlockUser(string username);

        
        bool DeleteUser(string username);


        BusinessServiceResponse RenameUser(string oldUsername, string newUsername);

        BusinessServiceResponse SetMfaDetails(int naatiNumber, string mfaSecret);

        GenericResponse<PersonMfaResponse> GetMfaDetails(int naatiNumber);

        BusinessServiceResponse DisableMfa(int naatiNumber);

        /// <summary>
        /// Determine if NCMS Users have disabled the account
        /// </summary>
        /// <param name="naatiNumber"></param>
        /// <returns></returns>
        GenericResponse<bool> GetAccessDisabledByNcms(int naatiNumber);
    }
}