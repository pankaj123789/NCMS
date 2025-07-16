using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace F1Solutions.Naati.Common.Dal.Domain
{
    public class Payroll : EntityBase
    {
        public virtual DateTime ModifiedDate { get; set; }
        public virtual User ModifiedUser { get; set; }
        public virtual PayrollStatus PayrollStatus { get; set; }
        public virtual bool LegacyAccounting { get; set; }
        private IList<JobExaminerPayroll> mJobExaminerPayrolls;

        public Payroll()
        {
            mJobExaminerPayrolls = new List<JobExaminerPayroll>();
        }

        public virtual IEnumerable<JobExaminerPayroll> JobExaminerPayrolls
        {
            get
            {
                return mJobExaminerPayrolls;
            }
        }

        public virtual void AddJobExaminer(JobExaminer jobExaminer)
        {
            if (mJobExaminerPayrolls.Any(x => x.JobExaminer.Id == jobExaminer.Id && x.Payroll.Id == Id))
            {
                throw new InvalidOperationException("Cannot add duplicate JobExaminer to this Payroll");
            }

            var jobExaminerPayroll = new JobExaminerPayroll();
            jobExaminerPayroll.Payroll = this;
            jobExaminerPayroll.JobExaminer = jobExaminer;
            mJobExaminerPayrolls.Add(jobExaminerPayroll);
        }

        public virtual void RemoveJobExaminer(IEnumerable<int> jobExaminerIds)
        {
            foreach (var jobExaminerId in jobExaminerIds)
            {
                var jep = mJobExaminerPayrolls.SingleOrDefault(x => x.JobExaminer.Id == jobExaminerId);
                if (jep != null)
                {
                    mJobExaminerPayrolls.Remove(jep);
                }
            }
        }

        /// <summary>
        /// This class exposes the private field mJobExaminerPayrolls for Fluent NHibernate.
        /// </summary>
        public class Expressions
        {
            public static readonly Expression<Func<Payroll, object>> JobExaminerPayrolls = x => x.mJobExaminerPayrolls;
        }
    }
}