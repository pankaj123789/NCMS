using System.Net.Http;
using System.Web.Http;
using F1Solutions.Naati.Common.Bl.Extensions;
using F1Solutions.Naati.Common.Contracts.Security;
using Ncms.Contracts;
using Ncms.Ui.Extensions;
using Ncms.Ui.Security;

namespace Ncms.Ui.Controllers.Api
{
    public class EntityController : BaseApiController
    {
        private readonly IEntityService _entity;

        public EntityController(IEntityService entity)
        {
            _entity = entity;
        }

        [Route("api/entity/{entityName}")]
        [NcmsAuthorize(SecurityVerbName.Read, SecurityNounName.General)]
        public HttpResponseMessage Get(string entityName)
        {
            return this.CreateResponse(() => _entity.List(entityName));
        }
    }
}
