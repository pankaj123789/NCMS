using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Web.Http;
using F1Solutions.Naati.Common.Bl.Extensions;
using Ncms.Ui.Extensions;

namespace Ncms.Ui.Controllers.Api
{
    [RoutePrefix("api/eftMachine")]
    public class EFTMachineController : BaseApiController
    {
        [AllowAnonymous]
        public HttpResponseMessage Get([FromUri]GetParam request)
        {
            Func<int, dynamic> item = i => new
            {
                EntityId = i,
                Name = $"Machine {i}"
            };

            var list = new List<dynamic>();

            foreach (int i in request.OfficeId)
            {
                list.Add(item(i));
            }

            return this.CreateResponse(() => list);
        }

        public class GetParam
        {
            public int[] OfficeId { get; set; }
        }
    }
}
