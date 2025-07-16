using System.Collections.Generic;

namespace F1Solutions.Naati.Common.Contracts.Dal.Request
{
    public class AssignTestMaterialRequest
    {
        public Dictionary<int, IList<int>> TestComponentIds { get; set; }
        public IEnumerable<int> TestSittingIds { get; set; }
    }
}