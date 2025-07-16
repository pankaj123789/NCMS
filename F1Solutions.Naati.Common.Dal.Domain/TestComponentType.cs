using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace F1Solutions.Naati.Common.Dal.Domain
{
    public class TestComponentType : EntityBase
    {
        private IEnumerable<TestComponentTypeStandardMarkingScheme> mTestComponentTypeStandardMarkingSchemes = Enumerable.Empty<TestComponentTypeStandardMarkingScheme>();
        private IEnumerable<TestMaterial> mTestMaterials = Enumerable.Empty<TestMaterial>();
        private IList<RubricMarkingCompetency> mRubricMarkingCompetencies = new List<RubricMarkingCompetency>();

        public virtual string Name { get; set; }
        public virtual string Label { get; set; }
        public virtual string Description { get; set; }
        public virtual bool CandidateBriefRequired { get; set; }
        public virtual double DefaultMaterialRequestHours { get; set; }
        public virtual int DefaultMaterialRequestDueDays { get; set; }
        public virtual int? CandidateBriefAvailabilityDays { get; set; }

        
        public virtual IEnumerable<RubricMarkingCompetency> RubricMarkingCompetencies =>
            mRubricMarkingCompetencies;
        public virtual TestComponentBaseType TestComponentBaseType { get; set; }
        public virtual IEnumerable<TestComponentTypeStandardMarkingScheme> TestComponentTypeStandardMarkingSchemes => mTestComponentTypeStandardMarkingSchemes;
        public virtual TestSpecification TestSpecification { get; set; }
        public virtual TestComponentTypeStandardMarkingScheme ActiveTestComponentTypeStandardMarkingScheme => mTestComponentTypeStandardMarkingSchemes.FirstOrDefault();
        public virtual IEnumerable<TestMaterial> TestMaterials => mTestMaterials;

        public virtual int MinNaatiCommentLength { get; set; }
        public virtual int MinExaminerCommentLength { get; set; }
        public virtual User ModifiedUser { get; set; }
        public virtual bool ModifiedByNaati { get; set; }
        public virtual DateTime ModifiedDate {get; set;}

        public virtual bool RoleplayersRequired { get; set; }
        public override string ToString()
        {
            return Description;
        }
    }
}
