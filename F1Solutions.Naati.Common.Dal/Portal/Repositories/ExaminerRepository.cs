using System;
using System.Linq;
using F1Solutions.Naati.Common.Contracts.Dal.Portal;
using F1Solutions.Naati.Common.Dal.Domain;

namespace F1Solutions.Naati.Common.Dal.Portal.Repositories
{
    public interface IExaminerRepository : IRepository<JobExaminer>
    {
        bool SaveExaminerPapersRecievedDateRequested(SaveExaminerPapersRecievedDateRequest request);
    }

    public class ExaminerRepository : Repository<JobExaminer>, IExaminerRepository
    {
        public ExaminerRepository(ICustomSessionManager sessionManager)
            : base(sessionManager)
        {
        }

        public bool SaveExaminerPapersRecievedDateRequested(SaveExaminerPapersRecievedDateRequest request)
        {
            using (var transaction = this.CreateSyncTransaction())
            {
                try
                {
                    var testResult = Session.Query<TestResult>().Single(tr => tr.Id == request.TestResultId).CurrentJobId;
                    var job = Session.Query<Job>().Single(j => j.Id == testResult);
                    var jobExaminer = Session.Query<JobExaminer>().Single(je => je.Job.Id == job.Id && je.PanelMembership.Person.Entity.NaatiNumber == request.NAATINumber);

                    if (jobExaminer.ExaminerPaperReceivedDate == null)
                    {
                        jobExaminer.ExaminerPaperReceivedDate = DateTime.Now;
                    }

                    Session.Save(jobExaminer);
                    Session.Flush();
                    transaction.Commit();
                }
                catch
                {
                    transaction.Rollback();
                    return false;
                }
            }



            return true;
        }
    }
}
