using System;
using System.Collections.Generic;

namespace F1Solutions.Naati.Common.Dal.Domain
{
    public class RubricMarkingAssessmentCriterion : EntityBase
    {
        private IList<RubricMarkingBand> mRubricMarkingBands = new List<RubricMarkingBand>();
        public virtual IEnumerable<RubricMarkingBand> RubricMarkingBands => mRubricMarkingBands;
        public virtual RubricMarkingCompetency RubricMarkingCompetency { get; set; }
        public virtual string Name { get; set; }
        public virtual string Label { get; set; }
        public virtual int DisplayOrder { get; set; }
        public virtual User ModifiedUser { get; set; }
        public virtual DateTime ModifiedDate { get; set; }
        public virtual bool ModifiedByNaati { get; set; }
    }
}
