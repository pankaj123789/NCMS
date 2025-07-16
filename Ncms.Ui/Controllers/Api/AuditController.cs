using System.Net.Http;
using System.Web.Http;
using F1Solutions.Naati.Common.Bl.Extensions;
using F1Solutions.Naati.Common.Contracts.Security;
using Ncms.Contracts;
using Ncms.Contracts.Models.Audit;
using Ncms.Ui.Extensions;
using Ncms.Ui.Security;

namespace Ncms.Ui.Controllers.Api
{
    public class AuditController : BaseApiController
    {
        private readonly IAuditService _audit;

        public AuditController(IAuditService audit)
        {
            _audit = audit;
        }

        [NcmsAuthorize(SecurityVerbName.Read, SecurityNounName.Audit)]
        public HttpResponseMessage Get([FromUri]AuditListRequestModel request)
        {
            return this.CreateResponse(() => _audit.List(request));
        }
    }
}
