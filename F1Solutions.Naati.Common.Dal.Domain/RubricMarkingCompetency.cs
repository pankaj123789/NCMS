using System;
using System.Collections.Generic;

namespace F1Solutions.Naati.Common.Dal.Domain
{
    public class RubricMarkingCompetency :  EntityBase
    {

        private IList<RubricMarkingAssessmentCriterion> mRubricMarkingAssessmentCriteria = new List<RubricMarkingAssessmentCriterion>();
        public virtual IEnumerable<RubricMarkingAssessmentCriterion> RubricMarkingAssessmentCriteria =>
            mRubricMarkingAssessmentCriteria;

        public virtual TestComponentType TestComponentType { get; set; }
        public virtual string Name { get; set; }
        public virtual string Label { get; set; }
        public virtual int DisplayOrder { get; set; }
        public virtual User ModifiedUser { get; set; }
        public virtual DateTime ModifiedDate { get; set; }
        public virtual bool ModifiedByNaati { get; set; }

    }
}
