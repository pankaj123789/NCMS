namespace Ncms.Bl
{
    //public class JobService : IJobService
    //{
    //    private readonly Main _main;
    //    private readonly UserService _user;
    //    private readonly ITestQueryService _testQueryService;

    //    public JobService(Main main, UserService user, ITestQueryService testQueryService)
    //    {
    //        _main = main;
    //        _user = user;
    //        _testQueryService = testQueryService;
    //    }

    //    public JobModel Get(int jobId)
    //    {
    //        var response = _main.GetDataItem("dsJob", jobId);
    //        var mapper = new DataRowToEntity<JobModel>();
    //        return mapper.Map(response.DataSet.Tables["Job"].Rows[0]);
    //    }

    //    public void Save(JobModel model)
    //    {
    //        var response = _main.GetDataItem("dsJob", model.JobId);
    //        response.DataSet.Tables["Job"].Rows[0]["Note"] = model.Note;
    //        response.DataSet.DataSetName = "dsJob";
    //        _main.UpdateDataItem(response.DataSet, true);
    //    }

    //    public void DeleteExaminer(int jobId, int entityId)
    //    {
    //        _main.DeleteExaminerFromTest(new DeleteExaminerFromTestRequestDto
    //        {
    //            JobId = jobId,
    //            EntityId = entityId
    //        });
    //    }

    //    public void Approve(int jobId)
    //    {
    //        const string datasetName = "dsJob";

    //        var result = _main.GetDataItem(datasetName, jobId);
    //        var tables = result.DataSet.Tables;

    //        var job = tables["Job"].Rows[0];

    //        var user = _user.Get();
    //        var userId = user.Id;
    //        var now = DateTime.Now;

    //        job["SentToPayrollDate"] = now;
    //        job["SentToPayrollUserId"] = userId;

    //        foreach (DataRow examiner in tables["JobExaminer"].Rows)
    //        {
    //            examiner["ExaminerToPayrollDate"] = now;
    //            examiner["ExaminerToPayrollUserID"] = userId;
    //        }

    //        result.DataSet.DataSetName = datasetName;
    //        _main.UpdateDataItem(result.DataSet, true);
    //    }

    //    public void UpdateDueDate(UpdateDueDateRequestModel request)
    //    {
    //        _testQueryService.UpdateDueDate(new UpdateDueDateRequest
    //            {
    //                JobIds = request.JobIds,
    //                DueDate = request.DueDate
    //            });
    //    }
    //}
}
