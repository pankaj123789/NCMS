using System.Collections.Generic;

namespace F1Solutions.Naati.Common.Contracts.Dal.Response
{
    public class GetRolePlaySessionResponse
    {
        public List<RolePlaySessionContract> Sessions { get; set; }
    }
}