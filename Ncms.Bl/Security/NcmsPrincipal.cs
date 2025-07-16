using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Threading;
using F1Solutions.Global.Common;
using F1Solutions.Naati.Common.Contracts.Bl;
using F1Solutions.Naati.Common.Contracts.Dal;
using F1Solutions.Naati.Common.Contracts.Security;
using Ncms.Contracts;
using Ncms.Contracts.Models.User;

namespace Ncms.Bl.Security
{
    public class NcmsPrincipal : IPrincipal
    {
        private readonly IUserService _userService;
        public UserModel User { get; }
        public IIdentity Identity { get; }

        private IList<UserPermissionsModel> _permissions;
        public IList<UserPermissionsModel> Permissions => _permissions ?? (_permissions = _userService.GetUserPermissions().ToList());

        public NcmsPrincipal(IIdentity identity)
        {
            _userService = ServiceLocator.Resolve<IUserService>();
            User = _userService.Get();
            Identity = identity;
        }

        public NcmsPrincipal(string userName)
        {
            var identity = new NcmsSystemUserIdentity(userName);
            Identity = identity;
            Thread.CurrentPrincipal = this;           
            _userService = ServiceLocator.Resolve<IUserService>();
            User = _userService.Get();
        }

        /// <summary>
        /// Does NOT check roles. Checks permissions in the form of "Noun.Verb"
        /// </summary>
        /// <param name="role">Noun.Verb</param>
        public bool IsInRole(string permission)
        {
            var parts = permission.Split('.');
            parts.Requires(x => x.Length == 2);

            if (!Enum.TryParse(parts[0], out SecurityNounName noun))
            {
                throw new Exception($"Invalid security noun: {noun}");
            }
            if (!Enum.TryParse(parts[1], out SecurityVerbName verb))
            {
                throw new Exception($"Invalid security verb: {verb}");
            }

            return HasPermission(noun, verb);
        }

        public bool HasPermission(SecurityNounName noun, SecurityVerbName verb)
        {
            return (Permissions.FirstOrDefault(x => x.Noun == noun)?.Permissions & (long)verb) == (long)verb;
        }


    }

    public class NcmsSystemUserIdentity : IIdentity
    {
        public NcmsSystemUserIdentity(string userName)
        {
            Name = userName;
        }

        public string Name { get; }

        public string AuthenticationType { get; } = string.Empty;

        public bool IsAuthenticated { get; } = true;
    }
}