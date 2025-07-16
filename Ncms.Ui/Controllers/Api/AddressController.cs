using System.Net.Http;
using System.Web.Http;
using F1Solutions.Naati.Common.Bl.Extensions;
using F1Solutions.Naati.Common.Contracts.Security;
using Ncms.Contracts;
using Ncms.Contracts.Models.Address;
using Ncms.Ui.Extensions;
using Ncms.Ui.Security;

namespace Ncms.Ui.Controllers.Api
{
    [RoutePrefix("api/address")]
    public class AddressController : BaseApiController
    {
        private readonly IAddressService _addressService;

        public AddressController(IAddressService addressService)
        {
            _addressService = addressService;
        }

        [NcmsAuthorize(SecurityVerbName.Update, new SecurityNounName[] { SecurityNounName.Person , SecurityNounName .Organisation})]
        public HttpResponseMessage Get([FromUri]AddressListRequestModel request)
        {
            return this.CreateResponse(() => _addressService.List(request));
        }

        [HttpGet]
        [NcmsAuthorize(SecurityVerbName.Update, new SecurityNounName[] { SecurityNounName.Person, SecurityNounName.Organisation })]
        [Route("parse")]
        public HttpResponseMessage Parse([FromUri]GeoResultModel request)
        {
            return this.CreateResponse(() => _addressService.ParseAddress(request));
        }
    }
}
