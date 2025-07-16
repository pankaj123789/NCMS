using System;
using System.Collections.Generic;
using System.Linq;

namespace F1Solutions.Naati.Common.Dal.Domain
{
    public class Job : EntityBase
    {
        private IList<JobExaminer> mJobExaminers = new List<JobExaminer>();
        private IEnumerable<TestResult> mTestResults = Enumerable.Empty<TestResult>();

        public virtual IEnumerable<JobExaminer> JobExaminers { get { return mJobExaminers; } }

        public virtual void AddJobExaminer(JobExaminer examiner)
        {
            examiner.Job = this;
            mJobExaminers.Add(examiner);
        }

        public virtual void RemoveJobExaminer(JobExaminer examiner)
        {
            var result = (from e in mJobExaminers where e.Id == examiner.Id select e).FirstOrDefault();

            if(result != null)
            {
                examiner.Job = null;
                mJobExaminers.Remove(result);
            }
        }

        public virtual Language Language { get; set; }
        public virtual DateTime? SentDate { get; set; }
        public virtual User SentUser { get; set; }
        public virtual DateTime? DueDate { get; set; }
        public virtual DateTime? ReceivedDate { get; set; }
        public virtual User ReceivedUser { get; set; }
        public virtual DateTime? SentToPayrollDate { get; set; }
        public virtual User SentToPayrollUser { get; set; }
        public virtual Job InitialJob { get; set; }
        public virtual string Note { get; set; }
        public virtual string Name { get; set; }
        public virtual PanelMembership SentToPanelMembership { get; set; }
        public virtual TestMaterial SettingMaterial { get; set; }
        public virtual int? NumberOfPapers { get; set; }
        public virtual string NumberOfItems { get; set; }
        public virtual Decimal? JobCost { get; set; }
        public virtual bool? JobLost { get; set; }
        public virtual string JobType { get; set; }
        public virtual int JobCategory { get; set; }
        public virtual int? ReviewFromJobId { get; set; }

        public virtual IEnumerable<TestResult> TestResults => mTestResults;
    }
}
