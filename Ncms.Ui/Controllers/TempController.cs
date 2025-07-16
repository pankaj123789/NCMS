using F1Solutions.Naati.Common.Bl.Extensions;
using F1Solutions.Naati.Common.Contracts.Dal.Enum;
using Ncms.Bl.ApplicationActions;
using Ncms.Contracts;
using Ncms.Contracts.Models.Application;
using Ncms.Contracts.Models.CredentialPrerequisite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Http;

namespace Ncms.Ui.Controllers
{
    [RoutePrefix("api/temp")]
    public class TempController : ApiController
    {
        private readonly IApplicationService _applicationService;
        private readonly ICredentialPrerequisiteService _credentialPrerequisiteService;

        public TempController(IApplicationService applicationService, ICredentialPrerequisiteService credentialPrerequisiteService)
        {
            _applicationService = applicationService;
            _credentialPrerequisiteService = credentialPrerequisiteService;
        }

        [HttpPost]
        public HttpResponseMessage CreatePrequisites(dynamic request)
        {
#if DEBUG

            var model = new ApplicationActionWizardModel
            {
                ApplicationId = (int)request.ApplicationId.Value,
                CredentialRequestId = (int)(request.CredentialRequestId?.Value ?? 0),
                ActionType = (int)request.ActionId.Value,
                Data = request.Steps
            };

            //model.Data.EnteredUserId = 2404;

            return this.CreateResponse(() => _applicationService.PerformAction(model));
#else
            return null;
#endif
        }

        [HttpPost]
        [Route("wizard/field_mandatories")]
        public HttpResponseMessage ValidateFieldMandatories(CreatePrerequisiteRequest createPrerequisiteRequest)
        {
            createPrerequisiteRequest.EnteredUserId = 2404;
            try
            {
                var sucessResponse = this.CreateResponse(() => new { InvalidFields = Enumerable.Empty<object>() });

                var validationErrors = _credentialPrerequisiteService.ValidateMandatoryFields(createPrerequisiteRequest).Data;

                return this.CreateResponse(() => new { validationErrors });

            }
            catch (Exception ex)
            {
                return this.FailureResponse(ex);
            }
        }

        [HttpPost]
        [Route("wizard/document_mandatories")]
        public HttpResponseMessage ValidateDocumentMandatories(CreatePrerequisiteRequest createPrerequisiteRequest)
        {
            createPrerequisiteRequest.EnteredUserId = 2404;
            try
            {
                var sucessResponse = this.CreateResponse(() => new { InvalidFields = Enumerable.Empty<object>() });

                var validationErrors = _credentialPrerequisiteService.ValidateMandatoryDocuments(createPrerequisiteRequest).Data;

                return this.CreateResponse(() => new { validationErrors });

            }
            catch (Exception ex)
            {
                return this.FailureResponse(ex);
            }
        }
    }


}