using System;
using System.Collections.Generic;

namespace F1Solutions.Naati.Common.Contracts.Dal.DTO
{
    public class UserDto
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Office { get; set; }
        public int OfficeId { get; set; }
        public bool Active { get; set; }
        public bool SystemUser { get; set; }
        public IList<string> UserRoles { get; set; }
        public IList<int> RoleIds { get; set; }
        public string Notes { get; set; }
        public bool NonWindowsUser { get; set; }
        public string Password { get; set; }
        public DateTime? LastPasswordChangeDate { get; set; }
        public bool IsLockedOut { get; set; }
        public DateTime? LastLockoutDate { get; set; }
        public int FailedPasswordAttemptCount { get; set; }
    }
}
