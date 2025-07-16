using System.Collections.Generic;

namespace Ncms.Contracts.Models.TestMaterial
{
    public class AssignTestMaterialModel
    {
        public IList<int> TestSittingIds { get; set; }
        public Dictionary<int, IList<int>> TestComponentIds { get; set; }
    }

    public class AssignTestMaterialRequestModel
    {
        public string[] TestSittingIds { get; set; }
        public bool ShowAllMaterials { get; set; }
    }
}
