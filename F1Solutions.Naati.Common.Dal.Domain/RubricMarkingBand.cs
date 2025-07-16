using System;

namespace F1Solutions.Naati.Common.Dal.Domain
{
    public class RubricMarkingBand : EntityBase
    {
        public virtual RubricMarkingAssessmentCriterion RubricMarkingAssessmentCriterion { get; set; }
        public virtual string Label { get; set; }
        public virtual string Description { get; set; }
        public virtual int Level { get; set; }

        public virtual User ModifiedUser { get; set; }
        public virtual bool ModifiedByNaati { get; set; }
        public virtual DateTime ModifiedDate { get; set; }

        public override string ToString()
        {
            return Level.ToString();
        }
    }
}
