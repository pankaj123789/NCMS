using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace F1Solutions.Naati.Common.Dal.Domain
{
    public class TestComponent : EntityBase
    {
        private IList<TestSittingTestMaterial> mTestSittingTestMaterials = new List<TestSittingTestMaterial>();

        public virtual TestSpecification TestSpecification { get; set; }
        public virtual TestComponentType Type { get; set; }
        public virtual int ComponentNumber { get; set; }
        public virtual string Label { get; set; }
        public virtual string Name { get; set; }
        public virtual int GroupNumber { get; set; }
        
        public virtual IList<TestSittingTestMaterial> TestSittingTestMaterials => mTestSittingTestMaterials;
        public virtual User ModifiedUser { get; set; }
        public virtual DateTime ModifiedDate { get; set; }
        public virtual bool ModifiedByNaati { get; set; }
    }
}
