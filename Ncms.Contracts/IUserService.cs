using System.Collections.Generic;
using F1Solutions.Naati.Common.Contracts.Bl.DTO;
using F1Solutions.Naati.Common.Contracts.Dal.Response;
using F1Solutions.Naati.Common.Contracts.Security;
using Ncms.Contracts.Models;
using Ncms.Contracts.Models.User;

namespace Ncms.Contracts
{
    public interface IUserService
    {
        UserModel Get();
        UserModel GetUser(string domainAndUserName);
        string GetUserName();
        bool IsInRoles(IEnumerable<string> roles);
        bool IsInRole(string role);
        bool IsInRole(string userName,  string role);
        bool HasPermission(SecurityNounName noun, SecurityVerbName verb);
      
        IEnumerable<string> GetRolesByUserName(string userName);
        IEnumerable<UsersSearchResultModel> UserSearch();
        IEnumerable<UserRolesModel> GetUserRoles();
        /// <summary>
        /// Gets a list of all rights for the current user. NOTE: It is faster to get this list from (CurrentPrincipal as NcmsPrincipal).Rights.
        /// </summary>
        IEnumerable<UserPermissionsModel> GetUserPermissions();
        GenericResponse<CreateOrUpdateResponse>  CreateOrUpdateUser(UserDetailsModel model);
        IEnumerable<int> GetExistingRoleByUserId(int id);
        GenericResponse<UserDetailsModel> GetUserDetailsById(int id);

    }
}
