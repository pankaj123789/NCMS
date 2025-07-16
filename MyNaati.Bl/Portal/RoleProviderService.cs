using System.Web.Security;
using MyNaati.Contracts.Portal;

namespace MyNaati.Bl.Portal
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "RoleProviderService" in both code and config file together.
    public class RoleProviderService : IRoleProviderService
    {
        public string[] GetRolesForUser(string username)
        {
            return Roles.GetRolesForUser(username);
        }

        public bool IsInRole(string username, string roleName)
        {
            return Roles.IsUserInRole(username, roleName);
        }

        public void AddUsersToRoles(string[] usernames, string[] roleNames)
        {
            Roles.AddUsersToRoles(usernames, roleNames);
        }

        public void CreateRole(string roleName)
        {
            Roles.CreateRole(roleName);
        }

        public bool DeleteRole(string roleName, bool throwOnPopulatedRole)
        {
            return Roles.DeleteRole(roleName, throwOnPopulatedRole);
        }

        public string[] FindUsersInRole(string roleName, string usernameToMatch)
        {
            return Roles.FindUsersInRole(roleName, usernameToMatch);
        }

        public string[] GetAllRoles()
        {
            return GetAllRoles();
        }

        public string[] GetUsersInRole(string roleName)
        {
            return Roles.GetUsersInRole(roleName);
        }

        public bool IsUserInRole(string username, string roleName)
        {
            return Roles.IsUserInRole(username, roleName);
        }

        public void RemoveUsersFromRoles(string[] usernames, string[] roleNames)
        {
            Roles.RemoveUsersFromRoles(usernames, roleNames);
        }

        public bool RoleExists(string roleName)
        {
            return Roles.RoleExists(roleName);
        }

    }
}
