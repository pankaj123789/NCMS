using System.Collections.Generic;

namespace F1Solutions.Naati.Common.Contracts.Dal.Response
{
    public class ScreenPermissionResponse
    {
        public IEnumerable<int> PermissionTypeIds { get; set; }
    }
}