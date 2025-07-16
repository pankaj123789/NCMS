using System;
using System.Collections.Generic;
using System.Linq;

namespace F1Solutions.Naati.Common.Dal.Domain
{
    public class User : EntityBase
    {
        public User()
        {
            mUserRoles = new List<UserRole>();
        }

        private IList<UserRole> mUserRoles;

        public virtual IEnumerable<UserRole> UserRoles
        {
            get
            {
                return mUserRoles;
            }
        }

        public virtual void AddUserRole(UserRole userRole)
        {
            userRole.User = this;
            mUserRoles.Add(userRole);
        }

        public virtual void RemoveUserRole(UserRole userRole)
        {
            var result = (from ur in mUserRoles
                          where ur.Id == userRole.Id
                          select ur).SingleOrDefault();

            if (result != null)
            {
                mUserRoles.Remove(result);
                userRole.User = null;
            }
        }

        private string mUserName;

        public virtual string UserName
        {
            get
            {
                return mUserName;
            }
            set
            {
                mUserName = value.ToUpper();
            }
        }

        public virtual string FullName { get; set; }
        public virtual Office Office { get; set; }
        public virtual string Note { get; set; }
        public virtual bool Active { get; set; }
        public virtual bool SystemUser { get; set; }
        public virtual string Email { get; set; }
        public virtual bool NonWindowsUser { get; set; }
        public virtual string Password { get; set; }
        public virtual DateTime? LastPasswordChangeDate { get; set; }
        public virtual bool IsLockedOut { get; set; }
        public virtual DateTime? LastLockoutDate { get; set; }
        public virtual int FailedPasswordAttemptCount { get; set; }

        public override IAuditObject RootAuditObject
        {
            get
            {
                return this;
            }
        }

        public override string ToString()
        {
            return FullName;
        }
    }
}
