using System.Collections.Generic;
using F1Solutions.Naati.Common.Contracts.Dal.DTO;
using F1Solutions.Naati.Common.Contracts.Dal.Request;
using F1Solutions.Naati.Common.Contracts.Dal.Response;
using F1Solutions.Naati.Common.Contracts.Dal.Services;

namespace F1Solutions.Naati.Common.Contracts.Dal.QueryServices
{
    
    public interface IUserQueryService : IQueryService
    {
        
        UserDetailsDto GetUser(string userName);

        
        IEnumerable<UserSearchDto> FindUserSearch(UserSearchRequest request);

        
        int SaveUserSearch(UserSearchDto request);

        
        void DeleteUserSearch(int searchId);

        
        IEnumerable<string> GetUserRolesByUserName(string userName);

        
        bool IsUserInRoles(UserInRoleRequest request);

        
        IEnumerable<UserDto> UserSearch();

        
        IEnumerable<UserRoleDto> GetUserRoles();

        
        UserCheckResponse UserCheck(UserRequest request);

        
        CreateOrUpdateResponse CreateOrUpdateUser(UserRequest request);

        
        UserDetailsResponse GetUserDetailsById(int id);

        
        IEnumerable<int> GetExistingRolesByUserId(int userId);
      

        void RefreshLocalUserCache(string userName);
    }
}
