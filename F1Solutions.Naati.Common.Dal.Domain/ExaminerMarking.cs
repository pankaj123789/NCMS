using System;
using System.Collections.Generic;
using System.Linq;

namespace F1Solutions.Naati.Common.Dal.Domain
{
    public class ExaminerMarking : EntityBase
    {
        private IList<ExaminerTestComponentResult> mExaminerTestComponentResults;

        public ExaminerMarking()
        {
            mExaminerTestComponentResults= new List<ExaminerTestComponentResult>();
        }

        public virtual IEnumerable<ExaminerTestComponentResult> ExaminerTestComponentResults { get { return mExaminerTestComponentResults; } }

        public virtual TestResult TestResult { get; set; }
        public virtual JobExaminer JobExaminer { get; set; }
        public virtual bool CountMarks { get; set; }
        public virtual string Comments { get; set; }
        public virtual string ReasonsForPoorPerformance { get; set; }
        public virtual int PrimaryReasonForFailure { get; set; }
        public virtual string Status { get; set; }
        public virtual DateTime? SubmittedDate { get; set; }

        public virtual void AddExaminerTestComponentResult(ExaminerTestComponentResult examinerTestComponentResult)
        {
            examinerTestComponentResult.ExaminerMarking = this;
            mExaminerTestComponentResults.Add(examinerTestComponentResult);
        }

        public virtual void RemoveExaminerTestComponentResult(ExaminerTestComponentResult examinerTestComponentResult)
        {
            var result =
                (from etcr in mExaminerTestComponentResults where etcr.Id == examinerTestComponentResult.Id select etcr)
                    .FirstOrDefault();

            if(result != null)
            {
                result.ExaminerMarking = null;
                mExaminerTestComponentResults.Remove(result);
            }
        }
    }

    public static class ExaminerMarkingStatus
    {
        public const string Submitted = "S";
    }
}
