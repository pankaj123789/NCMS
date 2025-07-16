using System.Collections.Generic;

namespace F1Solutions.Naati.Common.Contracts.Dal.Request
{
    public class UserInRoleRequest
    {
        public string UserName { get; set; }
        public IEnumerable<string> Roles { get; set; }
    }
}