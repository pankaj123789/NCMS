using F1Solutions.Naati.Common.Contracts.Bl;

namespace MyNaati.Contracts.Portal
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IRoleProviderService" in both code and config file together.
    
    public interface IRoleProviderService : IInterceptableservice
    {
        
        string[] GetRolesForUser(string username);

        
        bool IsInRole(string username, string roleName);

        
        void AddUsersToRoles(string[] usernames, string[] roleNames);

        
        void CreateRole(string roleName);

        
        bool DeleteRole(string roleName, bool throwOnPopulatedRole);

        
        string[] FindUsersInRole(string roleName, string usernameToMatch);

        
        string[] GetAllRoles();

        
        string[] GetUsersInRole(string roleName);

        
        bool IsUserInRole(string username, string roleName);

        
        void RemoveUsersFromRoles(string[] usernames, string[] roleNames);

        
        bool RoleExists(string roleName);
    }
}
