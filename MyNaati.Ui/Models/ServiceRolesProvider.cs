using System;
using System.Web.Security;
using F1Solutions.Naati.Common.Contracts.Bl;
using MyNaati.Contracts.Portal;

namespace MyNaati.Ui.Models
{
    public class ServiceRolesProvider : RoleProvider
    {
        public IRoleProviderService RoleProviderService
        {
            get { return ServiceLocator.Resolve<IRoleProviderService>(); }
        }

        public override void AddUsersToRoles(string[] usernames, string[] roleNames)
        {
            RoleProviderService.AddUsersToRoles(usernames, roleNames);
        }

        public override string ApplicationName
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public override void CreateRole(string roleName)
        {
            RoleProviderService.CreateRole(roleName);
        }

        public override bool DeleteRole(string roleName, bool throwOnPopulatedRole)
        {
            return RoleProviderService.DeleteRole(roleName, throwOnPopulatedRole);
        }

        public override string[] FindUsersInRole(string roleName, string usernameToMatch)
        {
            return RoleProviderService.FindUsersInRole(roleName, usernameToMatch);
        }

        public override string[] GetAllRoles()
        {
            return RoleProviderService.GetAllRoles();
        }

        public override string[] GetRolesForUser(string username)
        {
            return RoleProviderService.GetRolesForUser(username);
        }

        public override string[] GetUsersInRole(string roleName)
        {
            return RoleProviderService.GetUsersInRole(roleName);
        }

        public override bool IsUserInRole(string username, string roleName)
        {
            return RoleProviderService.IsUserInRole(username, roleName);
        }

        public override void RemoveUsersFromRoles(string[] usernames, string[] roleNames)
        {
            RoleProviderService.RemoveUsersFromRoles(usernames, roleNames);
        }

        public override bool RoleExists(string roleName)
        {
            return RoleProviderService.RoleExists(roleName);
        }
    }
}