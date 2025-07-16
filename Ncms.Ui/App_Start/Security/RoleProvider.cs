using System;
using System.Linq;
using Ncms.Contracts;
using Ninject;

namespace Ncms.Ui.Security
{
    public class RoleProvider : System.Web.Security.RoleProvider
    {
        public override string[] GetRolesForUser(string username)
        {
            var service = NinjectWebCommon.Kernel.Get<IUserService>();
            return service.GetRolesByUserName(username).ToArray();
        }                   

        public override bool IsUserInRole(string username, string roleName)
        {
            return NinjectWebCommon.Kernel.Get<IUserService>().IsInRole(username, roleName);
        }

        public override string[] FindUsersInRole(string roleName, string usernameToMatch)
        {
            throw new NotImplementedException();
        }

        public override string[] GetAllRoles()
        {
            throw new NotImplementedException();
        }

        public override void CreateRole(string roleName)
        {
            throw new NotImplementedException("Roles cannot be creating using the role provider.");
        }

        public override bool DeleteRole(string roleName, bool throwOnPopulatedRole)
        {
            throw new NotImplementedException("Roles cannot be deleted using the role provider.");
        }

        public override string ApplicationName
        {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }

        public override bool RoleExists(string roleName)
        {
            throw new NotImplementedException();
        }

        public override string[] GetUsersInRole(string roleName)
        {
            throw new NotImplementedException();
        }

        public override void RemoveUsersFromRoles(string[] usernames, string[] roleNames)
        {
            throw new NotImplementedException();
        }

        public override void AddUsersToRoles(string[] usernames, string[] roleNames)
        {
            throw new NotImplementedException();
        }
    }
}