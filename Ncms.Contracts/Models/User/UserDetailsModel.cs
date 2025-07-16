using System.Collections.Generic;

namespace Ncms.Contracts.Models.User
{
    public class UserDetailsModel
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Notes { get; set; }
        public string Password { get; set; }
        public bool Active { get; set; }
        public bool SystemUser { get; set; }
        public bool NonWindowsUser { get; set; }
        public bool UpdatePassword { get; set; }
        public int OfficeId { get; set; }
        public IList<int> RoleIds { get; set; }
    }
}