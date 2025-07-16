using System;

namespace F1Solutions.Naati.Common.Dal.Domain
{
    public class TestSpecificationStandardMarkingScheme: EntityBase
    {
        public virtual int OverallPassMark { get; set; }
        public virtual int MinOverallMarkForPaidReview { get; set; }
    
        public virtual TestSpecification TestSpecification { get; set; }
        public virtual User ModifiedUser { get; set; }
        public virtual bool ModifiedByNaati { get; set; }
        public virtual DateTime ModifiedDate { get; set; }

    }
}
