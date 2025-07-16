using System;
using System.Configuration;
using System.IO;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web.Http;
using F1Solutions.Naati.Common.Bl.Extensions;
using F1Solutions.Naati.Common.Contracts.Security;
using Ncms.Contracts;
using Ncms.Contracts.Models;
using Ncms.Contracts.Models.File;
using Ncms.Contracts.Models.User;
using Ncms.Ui.Extensions;
using Ncms.Ui.Security;

namespace Ncms.Ui.Controllers.Api
{
    public class FileController : BaseApiController
    {
        private readonly IFileService _file;
        private readonly IUserService _user;
        private readonly IAssetService _asset;
        private readonly ITestService _testService;

        public FileController(IFileService file, IUserService user, IAssetService asset, ITestService test)
        {
            _file = file;
            _user = user;
            _asset = asset;
            _testService = test;
        }

        [NcmsAuthorize(SecurityVerbName.Update, SecurityNounName.Application)]
        public HttpResponseMessage Get([FromUri]FileSearchRequestModel request)
        {
            return this.CreateResponse(() => _file.List(request));
        }

        [NcmsAuthorize(SecurityVerbName.Update, SecurityNounName.Application)]
        public HttpResponseMessage Post(CreateOrUpdateRelatedRequestModel request)
        {
            return this.CreateResponse(() =>
            {
                CreateOrUpdateFile(request);

                _file.Update(new CreateOrUpdateFileRequestModel
                {
                    StoredFileId = request.StoredFileId,
                    Title = request.Title,
                    Type = request.Type,
                    UploadedByUserId = (_user.Get() ?? new UserModel()).Id,
                    StoragePath = $@"{request.Type}\{request.RelatedId}\{request.Title}{Path.GetExtension(request.File)}"
                });
            });
        }

        [NcmsAuthorize(SecurityVerbName.Update, SecurityNounName.Application)]
        private void CreateOrUpdateFile(CreateOrUpdateRelatedRequestModel request)
        {
            var materialModel = new MaterialModel
            {
                TestMaterialId = request.RelatedId,
                MaterialId = request.Id,
                Title = request.Title,
                StoredFileId = request.StoredFileId,
            };

            if (request.Id > 0)
            {
                _asset.UpdateMaterial(materialModel);
            }
            else
            {
                _asset.CreateMaterial(materialModel);
            }
        }

        [HttpGet]
        [NcmsAuthorize(SecurityVerbName.Update, SecurityNounName.Application)]
        [Route("api/file/types")]
        public HttpResponseMessage Types()
        {
            return this.CreateSearchResponse(() => _file.ListTypes());
        }

        [HttpGet]
        //called on main test page screen
        [NcmsAuthorize(SecurityVerbName.Read, SecurityNounName.General)]
        [Route("api/file/documentTypes")]
        public HttpResponseMessage DocumentTypes()
        {
            return this.CreateResponse(() => _file.GetDocumentTypes());
        }


        [HttpGet]
        [NcmsAuthorize(SecurityVerbName.Read, SecurityNounName.General)]
        [Route("api/file/documentTypesForCategory/{categoryId}")]
        public HttpResponseMessage DocumentTypesForCategory(int categoryId)
        {
            return this.CreateSearchResponse(() => _file.GetDocumentTypesForCategory(categoryId));
        }

        [NcmsAuthorize(SecurityVerbName.Update, SecurityNounName.Application)]
        public HttpResponseMessage Delete(int id)
        {
            return this.CreateResponse(() => _file.Delete(id));
        }

        [HttpGet]
        [NcmsAuthorize(SecurityVerbName.Download, SecurityNounName.Document)]
        [Route("api/file/download/{id}")]
        public HttpResponseMessage Download(int id)
        {
            return this.FileStreamResponse(() => _file.GetData(id));
        }

        [HttpGet]
        [NcmsAuthorize(SecurityVerbName.Download, SecurityNounName.Document)]
        [Route("api/file/downloadWithToken/{id}")]
        public HttpResponseMessage DownloadWithToken(int id)
        {
            return this.CreateResponse(() => _file.GetFileToken(id));
        }

        [HttpGet]
        [NcmsAuthorize(SecurityVerbName.Update, SecurityNounName.Application)]
        [Route("api/file/downloadTempFile/{name}")]
        public HttpResponseMessage Download(string name)
        {
            return this.FileStreamResponse(() => _file.GetTempFile(name));
        }

        [Route("api/file/upload")]
        [NcmsAuthorize(SecurityVerbName.Upload, SecurityNounName.Application)]
        [HttpPost]
        public async Task<HttpResponseMessage> Upload()
        {
            return await this.ProcessMultipartFileData((Func<string, MultipartFileData, MultipartFormDataStreamProvider, int>)((fileName, fileData, provider) =>
            {
                var relatedId = Convert.ToInt32(provider.FormData["relatedId"]);

                if (relatedId != 0)
                {
                    bool? readyResponse = _testService.TestReadyForAssets(relatedId);

                    if (readyResponse != true)
                    {
                        throw new Exception(Naati.Resources.Test.CannotSubmitAssets);
                    }
                }

                var id = Convert.ToInt32(provider.FormData["id"]);
                var type = provider.FormData["type"];
                var title = provider.FormData["title"];
                var storedFileId = Convert.ToInt32(provider.FormData["storedFileId"]);
                var testAsset = bool.Parse(provider.FormData["testAsset"]);
                var eportalDownload = bool.Parse(provider.FormData["eportalDownload"]);

                if (fileName.StartsWith("\"") && fileName.EndsWith("\""))
                {
                    fileName = fileName.Trim('"');
                }

                if (fileName.Contains(@"/") || fileName.Contains(@"\"))
                {
                    fileName = Path.GetFileName(fileName);
                }

                var user = _user.Get() ?? new UserModel();
                var request = new CreateOrUpdateFileRequestModel
                {
                    Title = title,
                    Type = type,
                    FilePath = fileData.LocalFileName,
                    UploadedByUserId = user.Id,
                    StoragePath = $@"{type}\{relatedId}\{title}{Path.GetExtension(fileName)}",
                    StoredFileId = storedFileId
                };

                var response = _file.Create(request);
                title = title.Replace("[UniqueID]", response.StoredFileId.ToString());
                title = title.Replace("[DocumentType]", Regex.Replace(type, "(\\B[A-Z])", " $1"));
                CreateOrUpdateFile(new CreateOrUpdateRelatedRequestModel
                {
                    Id = id,
                    File = fileName,
                    Title = title,
                    Type = type,
                    StoredFileId = response.StoredFileId,
                    RelatedId = relatedId,
                    TestAsset = testAsset,
                    EportalDownload = eportalDownload
                });

                return response.StoredFileId;
            }));
        }

        [HttpGet]
        [Route("api/file/defaultassetfilename")]
        [NcmsAuthorize(SecurityVerbName.Read, SecurityNounName.General)]
        public HttpResponseMessage DefaultAssetFileName()
        {
            return this.CreateResponse(() => ConfigurationManager.AppSettings["sam:defaultAssetFileName"]);
        }
    }
}
