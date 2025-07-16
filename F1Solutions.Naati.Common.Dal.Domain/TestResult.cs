using System;
using System.Collections.Generic;
using System.Linq;

namespace F1Solutions.Naati.Common.Dal.Domain
{
    public class TestResult : EntityBase
    {
        private IList<ExaminerMarking> mExaminerMarkings = new List<ExaminerMarking>();
        private IList<TestComponentResult> mTestComponentResults = new List<TestComponentResult>();
        private IList<TestResultRubricTestComponentResult> mTestResultRubricTestComponentResults = new List<TestResultRubricTestComponentResult>();

        public virtual IEnumerable<TestComponentResult> TestComponentResults { get { return mTestComponentResults; } }
        public virtual IEnumerable<ExaminerMarking> ExaminerMarkings { get { return mExaminerMarkings; } }
        
        public virtual void AddTestComponentResult(TestComponentResult testComponentResult)
        {
            testComponentResult.TestResult = this;
            mTestComponentResults.Add(testComponentResult);
        }

        public virtual void RemoveTestComponentResult(TestComponentResult testComponentResult)
        {
            throw new NotSupportedException("Cannot remove TestComponentResults from TestResult");
        }

        public virtual void AddExaminerMarking(ExaminerMarking examinerMarking)
        {
            examinerMarking.TestResult = this;
            mExaminerMarkings.Add(examinerMarking);
        }

        public virtual void RemoveExaminerMarking(ExaminerMarking examinerMarking)
        {
            var result = (from em in mExaminerMarkings where em.Id == examinerMarking.Id select em).FirstOrDefault();

            if(result != null)
            {
                result.TestResult = null;
                mExaminerMarkings.Remove(result);
            }
        }
        
        public virtual IEnumerable<TestResultRubricTestComponentResult> TestResultRubricTestComponentResults =>
            mTestResultRubricTestComponentResults;

        public virtual IEnumerable<RubricTestComponentResult> RubricTestComponentResults => TestResultRubricTestComponentResults
            .Select(x => x.RubricTestComponentResult);

        public virtual TestSitting TestSitting { get; set; }
        public virtual int? CurrentJobId => CurrentJob?.Id;
        public virtual ResultType ResultType { get; set; }
        public virtual string CommentsGeneral { get; set; }
        public virtual bool ThirdExaminerRequired { get; set;}
        public virtual bool IncludePreviousMarks { get; set; }
        // this is currently mapped as the id, rather than a reference, due to problems with review jobs - nick bourke
        public virtual int? ReviewInvoiceLineId { get; set; }

        public virtual DateTime? ProcessedDate { get; set; }
        public virtual DateTime? SatDate { get; set; }

        public virtual bool ResultChecked { get; set; }
        public virtual bool AllowCalculate { get; set; }
        public virtual bool AllowIssue { get; set; }

        public virtual Job CurrentJob { get; set; }

        public virtual bool EligibleForConcededPass { get; set; }
        public virtual bool EligibleForSupplementary { get; set; }
        public virtual bool? AutomaticIssuingExaminer { get; set; }
    }
}
