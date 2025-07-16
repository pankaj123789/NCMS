using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using F1Solutions.Global.Common;
using F1Solutions.Naati.Common.Contracts.Bl;
using F1Solutions.Naati.Common.Contracts.Bl.DTO;
using F1Solutions.Naati.Common.Contracts.Dal;
using F1Solutions.Naati.Common.Contracts.Dal.DTO;
using F1Solutions.Naati.Common.Contracts.Dal.Enum;
using F1Solutions.Naati.Common.Contracts.Dal.QueryServices;
using F1Solutions.Naati.Common.Contracts.Dal.Request;
using F1Solutions.Naati.Common.Contracts.Dal.Response;
using F1Solutions.Naati.Common.Contracts.Security;
using Ncms.Bl.Export;
using Ncms.Contracts;
using Ncms.Contracts.Models.Application;
using Ncms.Contracts.Models.Common;
using Ncms.Contracts.Models.Person;

namespace Ncms.Bl
{
    public class CredentialService : ICredentialService
    {
        private readonly ICredentialQueryService _credentialQueryService;
        private readonly IPersonQueryService _personQueryService;
        private readonly ISharedAccessSignature _sharedAccessSignature;
        private readonly IFileCompressionHelper _fileCompressionHelper;

        public CredentialService(ICredentialQueryService credentialQueryService, 
            IPersonQueryService personQueryService, 
            ISharedAccessSignature sharedAccessSignature, 
            IFileCompressionHelper fileCompressionHelper)
        {
            _credentialQueryService = credentialQueryService;
            _personQueryService = personQueryService;
            _sharedAccessSignature = sharedAccessSignature;
            _fileCompressionHelper = fileCompressionHelper;
        }

        public GenericResponse<CertificationPeriodModel> GetCertificationPeriod(int certificationPeriodId)
        {
            GetCertificationPeriodResponse response = _credentialQueryService.GetCertificationPeriod(certificationPeriodId);
            return response.ConvertServiceResponse<CertificationPeriodDto, CertificationPeriodModel>();
        }

        public GenericResponse<IEnumerable<CredentialResultModel>> Search(CredentialSearchRequest request)
        {
            var getRequest = new GetCredentialSearchRequest
            {
                Skip = request.Skip,
                Take = request.Take,
                Filters = request.Filter.ToFilterList<CredentialSearchCriteria, CredentialFilterTypeName>()
            };
            CredentialSearchResponse serviceReponse = null;
            serviceReponse = _credentialQueryService.SearchCredential(getRequest);

            var models = serviceReponse.Results.Select(x => new CredentialResultModel
            {
                CredentialId = x.CredentialId,
                CredentialRequestId = x.CredentialRequestId,
                CredentialApplicationId = x.CredentialApplicationId,
                NaatiNumber = x.NaatiNumber,
                CredentialTypeInternalName = x.CredentialTypeInternalName,
                Direction = x.Direction,
                StartDate = x.StartDate,
                EndDate = x.EndDate,
                IssuedDate = x.IssuedDate,
                StatusId = x.StatusId,
                ShowInOnlineDirectory = x.ShowInOnlineDirectory,
                Category = x.Category
            }).ToList();

            var response = new GenericResponse<IEnumerable<CredentialResultModel>>(models);

            if (request.Take.HasValue && models.Count == request.Take.Value)
            {
                response.Warnings.Add($"Search result were limited to {request.Take.Value} records.");
            }

            if (request.Skip.HasValue)
            {
                response.Warnings.Add($"First {request.Skip.Value} records were skipped.");
            }

            return response;
        }

        public GenericResponse<string> GetPhotosAndExcel(CredentialSearchRequest request)
        {
            var getRequest = new GetCredentialSearchRequest
            {
                Skip = request.Skip,
                Take = request.Take,
                Filters = request.Filter.ToFilterList<CredentialSearchCriteria, CredentialFilterTypeName>()
            };

            var credentialSearchResults = _credentialQueryService.SearchCredential(getRequest).Results.ToList();

            //Photos
            var personNaatiNumbers = credentialSearchResults.Select(x => x.NaatiNumber);

            var getPhotoRequest = new GetPersonPhotoFileRequest
            {
                PersonNaatiNumbers = personNaatiNumbers,
                FolderPath = ConfigurationManager.AppSettings["tempFilePath"]
            };

            var photoFilePaths = _personQueryService.GetPersonPhotoFiles(getPhotoRequest);

            //Excel Export
            var credentials = credentialSearchResults.Select(x => new CredentialExportModel
            {
                NaatiNumber = x.NaatiNumber,
                PractitionerNumber = x.PractitionerNumber,
                Title = x.Title,
                GivenName = x.GivenName,
                FamilyName = x.FamilyName,
                Address = x.Address,
                Suburb = x.Suburb,
                State = x.State,
                Postcode = x.Postcode,
                Country = x.Country,
                PrimaryEmail = x.PrimaryEmail,
                CredentialId = x.CredentialId,
                ApplicationType = x.ApplicationType,
                InternalName = x.CredentialTypeInternalName,
                ExternalName = x.CredentialTypeExternalName,
                Language1 = x.Language1,
                Language1Code = x.Language1Code,
                Language1Group = x.Language1Group,
                Language2 = x.Language2,
                Language2Code = x.Language2Code,
                Language2Group = x.Language2Group,
                DirectionDisplayName = x.Direction,
                StartDate = x.StartDate,
                EndDate = x.EndDate,
                Status = x.Status,
                StatusChangeDate = x.IssuedDate,
                ExportedDate = DateTime.Now
            });

            var exporter = new CredentialExporter(credentials);

            var fileName = $"Photos-{DateTime.Now.Year}-{DateTime.Now.Month}-{DateTime.Now.Day}.xlsx";

            var folderName = Guid.NewGuid().ToString();
            var folderPath = Path.Combine(ConfigurationManager.AppSettings["tempFilePath"], folderName);
            Directory.CreateDirectory(folderPath);

            var excelFilePath = Path.Combine(folderPath, fileName);
            using (var excelFileStream = new FileStream(excelFilePath, FileMode.CreateNew))
            {
                exporter.Export(FileType.Xlsx, excelFileStream);
            }

            var files = photoFilePaths.Concat(new[] {excelFilePath});

            var zipFile = _fileCompressionHelper.CreateZipFile(files, "Photos And Export", true);
            var fileToken = _sharedAccessSignature.GetUrlForFile(zipFile);

            return fileToken;
        }
    }
}