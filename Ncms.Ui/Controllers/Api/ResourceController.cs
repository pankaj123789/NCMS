using System.Net.Http;
using System.Web.Http;
using F1Solutions.Naati.Common.Bl.Extensions;
using Ncms.Contracts;
using Ncms.Ui.Extensions;

namespace Ncms.Ui.Controllers.Api
{
    public class ResourceController : ApiController
    {
        private readonly IResourceService _resource;

        public ResourceController(IResourceService resource)
        {
            _resource = resource;
        }

        [AllowAnonymous]
        public HttpResponseMessage Get()
        {
            return this.CreateResponse(() => _resource.List());
        }

        [AllowAnonymous]
        public HttpResponseMessage Get(string id)
        {
            return this.CreateResponse(() => string.Empty);
        }
    }
}
