using System;
using System.Net.Http;
using System.Web.Http;
using F1Solutions.Naati.Common.Bl.Extensions;
using Ncms.Ui.Extensions;

namespace Ncms.Ui.Controllers.Api
{
    [RoutePrefix("api/office")]
    public class OfficeController : BaseApiController
    {
        public HttpResponseMessage Get()
        {
            Func<int, dynamic> item = i => new
            {
                EntityId = i,
                Name = $"Office {i}",
                Abbreviation = $"ABV{i}"
            };

            return this.CreateResponse(() => CreateDummyList(item, 10));
        }

        [HttpGet]
        [AllowAnonymous]
        [Route("{officeId}/eftMachine")]
        public HttpResponseMessage EFTMachineGet([FromUri]int officeId)
        {
            var rnd = new Random();

            Func<int, dynamic> item = i => new
            {
                EntityId = i,
                Name = $"Machine {i}"
            };

            return this.CreateResponse(() => CreateDummyList(item, rnd.Next(1, 11)));
        }
    }
}
