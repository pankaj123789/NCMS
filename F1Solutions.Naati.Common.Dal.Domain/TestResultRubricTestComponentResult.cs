using System;

namespace F1Solutions.Naati.Common.Dal.Domain
{
    public class TestResultRubricTestComponentResult : EntityBase
    {
        public virtual TestResult TestResult { get; set; }

        public virtual RubricTestComponentResult RubricTestComponentResult { get; set; }
        public virtual User ModifiedUser { get; set; }
        public virtual DateTime ModifiedDate { get; set; }
    }
}
