using System;

namespace F1Solutions.Naati.Common.Dal.Domain
{
    public class ExaminerUnavailable : LegacyEntityBase
    {
        public virtual Person Person { get; set; }
        public virtual DateTime StartDate { get; set; }
        public virtual DateTime EndDate { get; set; }

        public ExaminerUnavailable(int id) : base(id)
        {
        }

        protected ExaminerUnavailable()
        {
        }
    }
}
