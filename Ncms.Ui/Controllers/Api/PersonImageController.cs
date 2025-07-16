using System;
using System.Configuration;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using F1Solutions.Global.Common;
using F1Solutions.Naati.Common.Bl.Extensions;
using F1Solutions.Naati.Common.Contracts.Security;
using Ncms.Contracts;
using Ncms.Contracts.Models.Person;
using Ncms.Ui.Extensions;
using Ncms.Ui.Helpers;
using Ncms.Ui.Security;

namespace Ncms.Ui.Controllers.Api
{
    [RoutePrefix("api/personimage")]
    public class PersonImageController : BaseApiController
    {
        private readonly IPersonImageService _personImageService;

        public PersonImageController(IPersonImageService personImageService)
        {
            _personImageService = personImageService;
        }

        [HttpGet]
        [Route("{naatiNumber}")]
        [NcmsAuthorize(SecurityVerbName.Read, SecurityNounName.Person)]
        public HttpResponseMessage Get(int naatiNumber, [FromUri]GetImageRequestModel request)
        {
            return this.FileStreamResponse(() => _personImageService.GetPersonImage(naatiNumber, request));
        }

        [HttpPost]
        [NcmsAuthorize(SecurityVerbName.Update, SecurityNounName.Person)]
        public async Task<HttpResponseMessage> Post()
        {
            return await this.ProcessMultipartFileData((fileName, fileData, provider) =>
            {
                var naatiNumber = Convert.ToInt32(provider.FormData["NaatiNumber"]);
                try
                {
                    Image.FromFile(fileData.LocalFileName).Dispose();
                }
                catch (OutOfMemoryException)
                {
                    var fi = new FileInfo(fileData.LocalFileName);
                    throw new Exception($"File failed to process.");
                }
                catch(Exception ex)
                {
                    var fi = new FileInfo(fileData.LocalFileName);
                    throw new Exception($"Unsupported File Type: {fileData.LocalFileName} ({fi.Length})", ex);
                }

                _personImageService.UpdatePhoto(new UpdatePhotoRequestModel
                {
                    NaatiNumber = naatiNumber,
                    FilePath = fileData.LocalFileName,
                });
                return naatiNumber;
            });
        }
    }
}
