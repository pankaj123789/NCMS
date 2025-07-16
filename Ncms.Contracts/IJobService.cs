using Ncms.Contracts.Models.Job;

namespace Ncms.Contracts
{
    public interface IJobService
    {
        JobModel Get(int jobId);
        void Save(JobModel model);
        void DeleteExaminer(int jobId, int entityId);
        void Approve(int jobId);
        void UpdateDueDate(UpdateDueDateRequestModel request);
    }
}
