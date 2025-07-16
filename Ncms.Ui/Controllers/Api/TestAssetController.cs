using System;
using System.Linq;
using System.Net.Http;
using System.Web.Http;
using F1Solutions.Naati.Common.Bl.Extensions;
using F1Solutions.Naati.Common.Contracts.Security;
using Ncms.Contracts;
using Ncms.Contracts.Models;
using Ncms.Ui.Extensions;
using Ncms.Ui.Security;
using Newtonsoft.Json;

namespace Ncms.Ui.Controllers.Api
{
    public class TestAssetController : BaseApiController
    {
        private readonly IAssetService _assetService;

        public TestAssetController(IAssetService assetService)
        {
            _assetService = assetService;
        }

        [NcmsAuthorize(SecurityVerbName.Read, SecurityNounName.TestMaterial)]
        public HttpResponseMessage Get([FromUri] string request)
        {
            return this.CreateSearchResponse(() =>
            {
                var model = JsonConvert.DeserializeObject<TestAttendanceAssetSearchRequestModel>(request);
                return _assetService.SearchAssets(model);
            });
        }

        [HttpGet]
        [NcmsAuthorize(SecurityVerbName.Read, SecurityNounName.TestMaterial)]
        public HttpResponseMessage BulkDownloadAssets(string storedFileIds)
        {
            return this.FileStreamResponse(() =>
            {
                var ids = storedFileIds.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(int.Parse);
                return _assetService.GetAssetFilesAsZip(ids);
            });
        }

        [HttpGet]
        [NcmsAuthorize(SecurityVerbName.Read, SecurityNounName.TestMaterial)]
        public HttpResponseMessage DownloadAsset(int testAttendanceAssetId)
        {
            return this.FileStreamResponse(() => _assetService.GetAsset(testAttendanceAssetId));
        }
    }
}
