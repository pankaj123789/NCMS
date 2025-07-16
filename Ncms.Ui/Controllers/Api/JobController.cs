using System.Net.Http;
using System.Web.Http;
using F1Solutions.Naati.Common.Bl.Extensions;
using F1Solutions.Naati.Common.Contracts.Security;
using Ncms.Contracts;
using Ncms.Contracts.Models.Job;
using Ncms.Ui.Extensions;
using Ncms.Ui.Security;

namespace Ncms.Ui.Controllers.Api
{
    [RoutePrefix("api/job")]
    public class JobController : BaseApiController
    {
        private readonly IJobService _job;

        public JobController(IJobService job)
        {
            _job = job;
        }

        [NcmsAuthorize(SecurityVerbName.Read, SecurityNounName.Examiner)]
        public HttpResponseMessage Get(int id)
        {
            return this.CreateResponse(() => _job.Get(id));
        }

        [NcmsAuthorize(SecurityVerbName.Update, SecurityNounName.Examiner)]
        public HttpResponseMessage Post(JobModel model)
        {
            return this.CreateResponse(() => _job.Save(model));
        }

        [HttpPost]
        [NcmsAuthorize(SecurityVerbName.Update, SecurityNounName.Examiner)]
        [Route("{jobId}/approve")]
        public HttpResponseMessage Approve([FromUri]int jobId)
        {
            return this.CreateResponse(() => _job.Approve(jobId));
        }

        [HttpDelete]
        [NcmsAuthorize(SecurityVerbName.Delete, SecurityNounName.Examiner)]
        [Route("{jobId}/examiner/{entityId}")]
        public HttpResponseMessage Examiner(int jobId, int entityId)
        {
            return this.CreateResponse(() => _job.DeleteExaminer(jobId, entityId));
        }

        [HttpPost]
        [NcmsAuthorize(SecurityVerbName.Update, SecurityNounName.Examiner)]
        [Route("updateduedate")]
        public HttpResponseMessage UpdateDueDate(UpdateDueDateRequestModel request)
        {
            return this.CreateResponse(() => _job.UpdateDueDate(request));
        }
    }
}
