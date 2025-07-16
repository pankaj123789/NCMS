using System.Collections.Generic;

namespace F1Solutions.Naati.Common.Dal.Domain
{
    public class TestMaterial : EntityBase
    {
        private IList<TestMaterialAttachment> mTestMaterialAttachments = new List<TestMaterialAttachment>();
        private IList<TestSittingTestMaterial> mTestSittingTestMaterials = new List<TestSittingTestMaterial>();
        private IList<MaterialRequest> mMaterialRequests = new List<MaterialRequest>();

        public virtual Language Language { get; set; }
        public virtual Skill Skill { get; set; }
        public virtual string Title { get; set; }
        public virtual bool Available { get; set; }
        public virtual string Notes { get; set; }
        public virtual TestComponentType TestComponentType { get; set; }

        public virtual TestMaterialType TestMaterialType { get; set; }
        public virtual TestMaterialDomain TestMaterialDomain { get; set; }

        public virtual TestMaterialLastUsed TestMaterialLastUsed { get; set; }

        public virtual IList<TestMaterialAttachment> TestMaterialAttachments => mTestMaterialAttachments;
        public virtual IList<TestSittingTestMaterial> TestSittingTestMaterials => mTestSittingTestMaterials;
        public virtual IList<MaterialRequest> MaterialRequests => mMaterialRequests;
    }
}
