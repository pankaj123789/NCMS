using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.IO.Compression;
using System.Linq;
using Aspose.Words;
using F1Solutions.Global.Common.Mapping;
using F1Solutions.Naati.Common.Bl.MaterialRequest;
using F1Solutions.Naati.Common.Contracts.Bl;
using F1Solutions.Naati.Common.Contracts.Bl.DTO;
using F1Solutions.Naati.Common.Contracts.Dal;
using F1Solutions.Naati.Common.Contracts.Dal.DTO;
using F1Solutions.Naati.Common.Contracts.Dal.Enum;
using F1Solutions.Naati.Common.Contracts.Dal.QueryServices;
using F1Solutions.Naati.Common.Contracts.Dal.Request;
using F1Solutions.Naati.Common.Contracts.Dal.Response;
using F1Solutions.Naati.Common.Contracts.Dal.Services;
using F1Solutions.Naati.Common.Contracts.Security;
using Ncms.Bl.Mappers;
using Ncms.Contracts;
using Ncms.Contracts.Models.File;

namespace Ncms.Bl
{
    public class FileService : IFileService
    {
        private readonly IFileStorageService _fileStorageService;
        private readonly IUserService _userService;
        private readonly IMaterialRequestQueryService _materialRequestQueryService;
        private readonly ISharedAccessSignature _sharedAccessSignature;
        private readonly IAutoMapperHelper _autoMapperHelper;

        public FileService(IFileStorageService fileStorageService, IUserService userService, IMaterialRequestQueryService materialRequestQueryService, ISharedAccessSignature sharedAccessSignature, IAutoMapperHelper autoMapperHelper)
        {
            _fileStorageService = fileStorageService;
            _userService = userService;
            _materialRequestQueryService = materialRequestQueryService;
            _sharedAccessSignature = sharedAccessSignature;
            _autoMapperHelper = autoMapperHelper;
        }

        public void Delete(int id)
        {
            _fileStorageService.DeleteFile(new DeleteFileRequest { StoredFileId = id });
        }

        public FileInfoModel GetFile(int storedFileId)
        {
            var responseFile = _fileStorageService.GetFile(new GetFileRequest
            {
                StoredFileId = storedFileId,
                TempFileStorePath = ConfigurationManager.AppSettings["tempFilePath"]
            });

            var allowedDocumentTypes = GetAllowedDocumentTypesToDownload();

            if (allowedDocumentTypes.All(x => x.Id != (int)responseFile.Types.First()))
            {
                throw new UserFriendlySamException($"User does not have permissions to download {responseFile.Types.First()} documents");
            }

            return new FileInfoModel
            {
                FileName = responseFile.FileName,
                FilePath = responseFile.FilePaths[0],
                StoredFileTypeId = (int)responseFile.Types.First(),
            };
        }

        public FileResponseModel GetMaterialRoundFile(int materialRoundId, int storedFileId)
        {
            var responseFile = _fileStorageService.GetFile(new GetFileRequest
            {
                StoredFileId = storedFileId,
                TempFileStorePath = ConfigurationManager.AppSettings["tempFilePath"]
            });

            var allowedDocumentTypes = GetAllowedDocumentTypesToDownload();

            if (allowedDocumentTypes.All(x => x.Id != (int)responseFile.Types.First()))
            {
                throw new Exception($"User does not have permissions to download {responseFile.Types.First()} documents");
            }
            if (responseFile == null)
            {
                return null;
            }

            var filePath = responseFile.FilePaths[0];
            Stream fileData;
            var fileExtension = Path.GetExtension(filePath) ?? string.Empty;
            if (fileExtension.EndsWith(".doc") || fileExtension.EndsWith(".docx"))
            {
                var searchQuery = new TestMaterialRequestSearchRequest
                {
                    Filters = new[]
                    {
                        new TestMaterialRequestSearchCriteria()
                        {
                            Filter = TestMaterialRequestFilterType.LatestRoundIdIntList,
                            Values = new[] { materialRoundId.ToString() }
                        }
                    }
                };
                var tokenReplacementService = ServiceLocator.Resolve<ITokenReplacementService>();
                var data = _materialRequestQueryService.SearchTestMaterialRequests(searchQuery).Results.First();
                var helper = new MaterialRequestDocumentHelper(tokenReplacementService, "Aspose.Words.lic");

                fileData = helper.ReplaceDocumentTokens(
                    data,
                    filePath,
                    saveFormat: fileExtension.EndsWith(".docx") ? SaveFormat.Docx : SaveFormat.Doc);
            }
            else
            {
                fileData = new MemoryStream(File.ReadAllBytes(responseFile.FilePaths[0]), false);
                fileData.Position = 0;
            }

            File.Delete(responseFile.FilePaths[0]);

            return new FileResponseModel
            {
                FileData = fileData,
                FileName = responseFile.FileName
            };
        }

        public GenericResponse<string> GetFileToken(int id)
        {
            var responseFile = _fileStorageService.GetFile(new GetFileRequest
            {
                StoredFileId = id,
                TempFileStorePath = ConfigurationManager.AppSettings["tempFilePath"]
            });

            var allowedDocumentTypes = GetAllowedDocumentTypesToDownload();

            if (allowedDocumentTypes.All(x => x.Id != (int)responseFile.Types.First()))
            {
                throw new Exception($"User does not have permissions to download {responseFile.Types.First()} documents");
            }
            if (responseFile == null)
            {
                return null;
            }

            var token = _sharedAccessSignature.GetUrlForFile(responseFile.FilePaths[0]);

            return token;
        }

        [Obsolete]
        public FileResponseModel GetData(int id)
        {            
            var responseFile = _fileStorageService.GetFile(new GetFileRequest
            {
                StoredFileId = id,
                TempFileStorePath = ConfigurationManager.AppSettings["tempFilePath"]
            });

            var allowedDocumentTypes = GetAllowedDocumentTypesToDownload();

            if (allowedDocumentTypes.All(x => x.Id != (int)responseFile.Types.First()))
            {
                throw new Exception($"User does not have permissions to download {responseFile.Types.First()} documents");
            }
            if (responseFile == null)
            {
                return null;
            }

            var fileData = new MemoryStream(File.ReadAllBytes(responseFile.FilePaths[0]), false);
            fileData.Position = 0;

            File.Delete(responseFile.FilePaths[0]);

            return new FileResponseModel
            {
                FileData = fileData,
                FileName = responseFile.FileName
            };
        }

        public FileResponseModel GetTempFile(string filename)
        {
            var tempFileStorePath = ConfigurationManager.AppSettings["tempFilePath"];
            var filePath = Path.Combine(tempFileStorePath, filename);
            var fileData = new MemoryStream(File.ReadAllBytes(filePath), false);
            fileData.Position = 0;

            return new FileResponseModel
            {
                FileData = fileData,
                FileName = filename
            };
        }

        public CreateOrUpdateFileResponseModel Create(CreateOrUpdateFileRequestModel request)
        {
            var serviceRequest = _autoMapperHelper.Mapper.Map<CreateOrUpdateFileRequest>(request);
            serviceRequest.UploadedDateTime = DateTime.Now;

            serviceRequest.UpdateStoredFileId = request.StoredFileId != 0 ? request.StoredFileId : (int?)null;

            CreateOrUpdateFileResponse fileResult = null;
            fileResult = _fileStorageService.CreateOrUpdateFile(serviceRequest);

            return _autoMapperHelper.Mapper.Map<CreateOrUpdateFileResponseModel>(fileResult);
        }

        public CreateOrUpdateFileResponseModel Update(CreateOrUpdateFileRequestModel request)
        {
            var serviceRequest = _autoMapperHelper.Mapper.Map<FileMetadataRequest>(request);
            serviceRequest.UploadedDateTime = DateTime.Now;
            serviceRequest.UpdateStoredFileId = request.StoredFileId;

            CreateOrUpdateFileResponse fileResult = null;
            fileResult = _fileStorageService.ReplaceFile(serviceRequest);

            return _autoMapperHelper.Mapper.Map<CreateOrUpdateFileResponseModel>(fileResult);
        }

        public IList<FileResponseModel> List(FileSearchRequestModel request)
        {
            var fileRequest = new ListFilesRequest
            {
                Name = request.Name,
                Types = (request.Types ?? new List<string>()).Select(t => (StoredFileType)Enum.Parse(typeof(StoredFileType), t)).ToList(),
                RelatedId = request.RelatedId
            };

            ListFilesResponse result = null;
            result = _fileStorageService.ListFiles(fileRequest);

            return result.Files.Select(x => _autoMapperHelper.Mapper.Map<FileResponseModel>(x)).ToList();
        }

        public IList<string> ListTypes()
        {
            return Enum.GetValues(typeof(StoredFileType)).Cast<StoredFileType>().Select(s => s.ToString()).ToList();
        }

        public IList<DocumentTypeModel> GetDocumentTypes()
        {
            List<DocumentTypeModel> response = new List<DocumentTypeModel>();
            response = _fileStorageService.ListDocumentTypes(new ListDocumentTypesRequest()
            {
                UserRestriction = new DocumentTypeRoleRequest
                {
                    UserId = _userService.Get().Id,
                    Download = true,
                    Upload = true
                }
            }).Types.Select(_autoMapperHelper.Mapper.Map<DocumentTypeModel>).ToList();
            return response;
        }

        public IEnumerable<DocumentTypeDto> GetAllowedDocumentTypesToDownload()
        {
            var documentTypes = _fileStorageService.ListDocumentTypes(new ListDocumentTypesRequest
            {
                UserRestriction = new DocumentTypeRoleRequest
                {
                    UserId = _userService.Get().Id,
                    Download = true,
                }
            });

            return documentTypes.Types;
        }

        public IEnumerable<DocumentTypeDto> GetAllowedDocumentTypesToUpload()
        {
            var documentTypes = _fileStorageService.ListDocumentTypes(new ListDocumentTypesRequest
            {
                UserRestriction = new DocumentTypeRoleRequest
                {
                    UserId = _userService.Get().Id,
                    Upload = true,
                }
            });

            return documentTypes.Types;
        }

        public IList<DocumentTypeModel> GetDocumentTypesForCategory(int categoryId)
        {

            var response = new List<DocumentTypeModel>();
            response = _fileStorageService.ListDocumentTypes(new ListDocumentTypesRequest { Category = (DocumentTypeCategoryTypeName)categoryId }).Types.Select(_autoMapperHelper.Mapper.Map<DocumentTypeModel>).ToList();
            return response;
        }

        internal static MemoryStream CreateZip(string[] paths)
        {
            var zipStream = new MemoryStream();
            var zip = new ZipArchive(zipStream, ZipArchiveMode.Create, true);

            foreach (var path in paths)
            {
                var splitPath = path.Split('\\');
                var fileName = splitPath.Last();
                zip.CreateEntryFromFile(path, fileName);
            }

            zip.Dispose();
            zipStream.Position = 0;

            foreach (var path in paths)
            {
                File.Delete(path);
            }

            return zipStream;
        }
    }
}
