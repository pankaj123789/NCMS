using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using F1Solutions.Naati.Common.Bl.Extensions;
using F1Solutions.Naati.Common.Contracts.Bl;
using F1Solutions.Naati.Common.Contracts.Dal;
using F1Solutions.Naati.Common.Contracts.Dal.CacheQuery;
using F1Solutions.Naati.Common.Contracts.Dal.Request;
using F1Solutions.Naati.Common.Contracts.Security;
using Ncms.Bl;
using Ncms.Contracts;
using Ncms.Contracts.Models.User;
using Ncms.Ui.Extensions;
using Ncms.Ui.Security;

namespace Ncms.Ui.Controllers.Api
{
    [RoutePrefix("api/logbook")]
    public class LogbookController : BaseApiController
    {
        private readonly ILogbookService _logbookService;
        private readonly IUserService _user;
        private readonly ISecretsCacheQueryService _secretsProvider;
        private readonly ISystemService _systemService;

        public LogbookController(ILogbookService logbookService, IUserService user, ISecretsCacheQueryService secretsProvider, ISystemService systemService)
        {
            _logbookService = logbookService;
            _user = user;
            _secretsProvider = secretsProvider;
            _systemService = systemService;
        }

        [HttpGet]
        [NcmsAuthorize(SecurityVerbName.Read, SecurityNounName.Logbook)]
        [Route("WorkPracticeActivities")]
        public HttpResponseMessage GetProfessionalDevelopmentActivities(int credentialId, int naatiNumber, int certificationPeriodId)
        {
            return this.CreateResponse(() => _logbookService.GetWorkPracticeActivities(credentialId, naatiNumber, certificationPeriodId));
        }


        [HttpGet]
        [Route("workPractices")]
        [NcmsAuthorize(SecurityVerbName.Read, SecurityNounName.Logbook)]
        public HttpResponseMessage GetWorkPractices(int credentialId, int naatiNumber, int certificationPeriodId)
        {
            return this.CreateResponse(() => _logbookService.GetWorkPractices(credentialId, naatiNumber, certificationPeriodId));
        }

        [HttpDelete]
        [Route("deleteWorkPractice/{id}")]
        [NcmsAuthorize(SecurityVerbName.Delete, SecurityNounName.Logbook)]
        public HttpResponseMessage DeleteWorkPractice(int id)
        {
            return this.CreateResponse(() => _logbookService.DeleteWorkPractice(id));
        }

        [HttpGet]
        [Route("value/PdCatalogue")]
        [NcmsAuthorize(SecurityVerbName.Read, SecurityNounName.Logbook)]
        public HttpResponseMessage SystemValue()
        {
            return this.CreateResponse(() => _systemService.GetSystemValue("PdCatalogue"));
        }

        [Route("upload")]
        [HttpPost]
        [NcmsAuthorize(SecurityVerbName.Update, SecurityNounName.Logbook)]
        public async Task<HttpResponseMessage> Upload()
        {
            return await this.ProcessMultipartFileData((Func<string, MultipartFileData, MultipartFormDataStreamProvider, int>)((fileName, fileData, provider) =>
            {
                var workPracticeId = Convert.ToInt32(provider.FormData["WorkPracticeId"]);
                var workPracticeAttachmentId = Convert.ToInt32(provider.FormData["Id"]);
                var type = provider.FormData["type"];
                var title = provider.FormData["title"];
                var storedFileId = Convert.ToInt32(provider.FormData["storedFileId"]);

                var user = _user.Get() ?? new UserModel();
                var defaultIdentity = _secretsProvider.Get(SecuritySettings.NcmsDefaultIdentityKey);
                var request = new WorkPracticeAttachmentRequest()
                {
                    WorkPracticeId = workPracticeId,
                    Id = workPracticeAttachmentId,
                    FileName = fileName,
                    FilePath = fileData.LocalFileName,
                    UserName = defaultIdentity,
                    Description = title,
                    UploadedByUserId = user.Id,
                    StoragePath = $@"{type}\{workPracticeId}\{fileName}",
                    StoredFileId = storedFileId
                };

                return _logbookService.CreateOrUpdateWorkPracticeAttachment(request);
            }));
        }

        [Route("workPracticeAttachment")]
        [NcmsAuthorize(SecurityVerbName.Read, SecurityNounName.Logbook)]
        public HttpResponseMessage GetWorkPracticeAttachment(int workPracticeId)
        {
            return this.CreateResponse(() => _logbookService.GetWorkPracticeAttachments(workPracticeId));
        }

        [HttpPost]
        [Route("updateWorkPractice")]
        [NcmsAuthorize(SecurityVerbName.Update, SecurityNounName.Logbook)]
        public HttpResponseMessage CreateOrUpdateWorkPractice(WorkPracticeRequest request)
        {
            return this.CreateResponse(() => _logbookService.CreateOrUpdateWorkPractice(request));
        }

        [HttpDelete]
        [Route("deleteWorkPracticeAttachment/{id}")]
        [NcmsAuthorize(SecurityVerbName.Delete, SecurityNounName.Logbook)]
        public HttpResponseMessage DeleteWorkPracticeAttachment(int id)
        {
            return this.CreateResponse(() => _logbookService.DeleteWorkPracticeAttachment(id));
        }

        [HttpPost]
        [Route("updateWorkPracticeAttachment")]
        [NcmsAuthorize(SecurityVerbName.Update, SecurityNounName.Logbook)]
        public HttpResponseMessage Post(WorkPracticeAttachmentRequest request)
        {
            var user = _user.Get() ?? new UserModel();
            request.UploadedByUserId = user.Id;
            return this.CreateResponse(() => _logbookService.CreateOrUpdateWorkPracticeAttachment(request));
        }

        [HttpDelete]
        [Route("professionaldevelopmentactivity/{id}")]
        [NcmsAuthorize(SecurityVerbName.Delete, SecurityNounName.Logbook)]
        public HttpResponseMessage DeleteProfessionalDevelopmentActivity(int id)
        {
            return this.CreateResponse(() => _logbookService.DeleteProfessionalDevelopmentActivity(id));
        }


        [Route("uploadprofessionaldevelopmentactivityattachment")]
        [HttpPost]
        [NcmsAuthorize(SecurityVerbName.Update, SecurityNounName.Logbook)]
        public async Task<HttpResponseMessage> UploadProfessionalDevelopmentActivityAttachment()
        {
            return await this.ProcessMultipartFileData((Func<string, MultipartFileData, MultipartFormDataStreamProvider, int>)((fileName, fileData, provider) =>
            {
                var professionalDevelopmentActivityId = Convert.ToInt32(provider.FormData["ProfessionalDevelopmentActivityId"]);
                var professionalDevelopmentActivityAttachmentId = Convert.ToInt32(provider.FormData["ProfessionalDevelopmentActivityAttachmentId"]);
                var type = provider.FormData["type"];
                var title = provider.FormData["title"];
                var storedFileId = Convert.ToInt32(provider.FormData["storedFileId"]);

                var user = _user.Get() ?? new UserModel();
                var defaultIdentity = _secretsProvider.Get(SecuritySettings.NcmsDefaultIdentityKey);
                var request = new ProfessionalDevelopmentActivityAttachmentRequest()
                {
                    ProfessionalDevelopmentActivityId = professionalDevelopmentActivityId,
                    Id = professionalDevelopmentActivityAttachmentId,
                    FileName = fileName,
                    FilePath = fileData.LocalFileName,
                    UserName = defaultIdentity,
                    Description = title,
                    UploadedByUserId = user.Id,
                    StoragePath = $@"{type}\{professionalDevelopmentActivityId}\{fileName}",
                    StoredFileId = storedFileId
                };

                return _logbookService.CreateOrUpdateProfessionalDevelopmentActivityAttachment(request);
            }));
        }

        [HttpGet]
        [Route("professionaldevelopmentactivityattachment")]
        [NcmsAuthorize(SecurityVerbName.Read, SecurityNounName.Logbook)]
        public HttpResponseMessage GetProfessionalDevelopmentActivityAttachments(int activityId)
        {
            return this.CreateSearchResponse(() => _logbookService.GetProfessionalDevelopmentActivityAttachments(activityId));
        }

        [HttpPost]
        [Route("professionaldevelopmentactivity")]
        [NcmsAuthorize(SecurityVerbName.Update, SecurityNounName.Logbook)]
        public HttpResponseMessage CreateOrUpdateProfessionalDevelopmentActivity(ProfessionalDevelopmentActivityRequest request)
        {
            return this.CreateResponse(() => _logbookService.CreateOrUpdateProfessionalDevelopmentActivity(request));
        }


        [HttpDelete]
        [Route("professionaldevelopmentactivityattachment/{id}")]
        [NcmsAuthorize(SecurityVerbName.Delete, SecurityNounName.Logbook)]
        public HttpResponseMessage DeleteProfessionalDevelopmentActivityAttachment(int id)
        {
            return this.CreateResponse(() => _logbookService.DeleteProfessionalDevelopmentActivityAttachment(id));
        }

        [HttpPost]
        [Route("attachProfessionaldevelopmentActivity")]
        [NcmsAuthorize(SecurityVerbName.Update, SecurityNounName.Logbook)]
        public HttpResponseMessage AttachProfessionalDevelopmentActivity(AttachActivityRequest request)
        {
            return this.CreateResponse(() => _logbookService.AttachProfessionalDevelopmentActivity(request));
        }

        [HttpDelete]
        [Route("detachProfessionaldevelopmentactivity/{activityId}/{credentialApplicationId}")]
        [NcmsAuthorize(SecurityVerbName.Delete, SecurityNounName.Logbook)]
        public HttpResponseMessage DetachProfessionalDevelopmentActivity(int activityId, int credentialApplicationId)
        {
            return this.CreateResponse(() => _logbookService.DetachProfessionalDevelopmentActivity(activityId, credentialApplicationId));
        }

        [HttpPost]
        [Route("professionaldevelopmentactivityattachment")]
        [NcmsAuthorize(SecurityVerbName.Update, SecurityNounName.Logbook)]
        public HttpResponseMessage PostProfessionalDevelopmentActivityAttachment(ProfessionalDevelopmentActivityAttachmentRequest request)
        {
            var user = _user.Get() ?? new UserModel();
            request.UploadedByUserId = user.Id;
            return this.CreateResponse(() => _logbookService.CreateOrUpdateProfessionalDevelopmentActivityAttachment(request));
        }

        [HttpGet]
        [Route("CredentialCertificationPeriods")]
        [NcmsAuthorize(SecurityVerbName.Read, SecurityNounName.Logbook)]
        public HttpResponseMessage CredentialCertificationPeriods(int naatiNumber, int credentialId)
        {
            var certificationPeriods = _logbookService.GetCredentialCertificationPeriodDetails(naatiNumber, credentialId);
            return this.CreateResponse(() => certificationPeriods);
        }

        [HttpGet]
        [Route("CertificationPeriodCredential")]
        [NcmsAuthorize(SecurityVerbName.Read, SecurityNounName.Logbook)]
        public HttpResponseMessage CertificationPeriodCredential(int naatiNumber, int credentialId, int certificationPeriodId)
        {
            var credential =
                _logbookService.GetCertificationPeriodCredential(naatiNumber, certificationPeriodId, credentialId);

            return this.CreateResponse(() => credential);
        }

        [HttpPost]
        [Route("attachWorkPractice")]
        [NcmsAuthorize(SecurityVerbName.Update, SecurityNounName.Logbook)]
        public HttpResponseMessage AttachWorkPractice(dynamic request)
        {
            var workPracticeId = (int)request.workPracticeId;
            var credentialApplicationId = (int)request.credentialApplicationId;
            var credentialId = (int)request.credentialId;
            return this.CreateResponse(() =>
                _logbookService.AttachWorkPractice(workPracticeId, credentialApplicationId, credentialId));
        }

        [HttpDelete]
        [Route("detachWorkPractice/{id}")]
        [NcmsAuthorize(SecurityVerbName.Delete, SecurityNounName.Logbook)]
        public HttpResponseMessage DetachWorkPractice(int id)
        {
            return this.CreateResponse(() =>
                _logbookService.DetachWorkPractice(id));
        }
    }
}
