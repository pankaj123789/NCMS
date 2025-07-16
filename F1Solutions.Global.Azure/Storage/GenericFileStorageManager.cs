using System;
using System.Configuration;
using System.IO;
using F1Solutions.Naati.Common.Contracts.Dal.CacheQuery;
using F1Solutions.Naati.Common.Contracts.Dal.QueryServices;
using F1Solutions.Naati.Common.Contracts.Security;

namespace F1Solutions.Naati.Common.Azure.Storage
{
    public class GenericFileStorageManager : ISharedAccessSignature
    {
        private readonly ISecretsCacheQueryService _secretsProvider;
        private readonly ISystemQueryService _systemQueryService;

        public GenericFileStorageManager(ISecretsCacheQueryService secretsProvider, ISystemQueryService systemQueryService)
        {
            _secretsProvider = secretsProvider;
            _systemQueryService = systemQueryService;
        }
        public string GetUrlForFile(string filePath)
        {
            // IMPLEMENTATION OF SAS TOKEN  FOR DEV ENVIRONMENT
            // IF SASDemoFile Is specified then SAS Token is created for SASDemoFile d
            // IF SASDemoFile Not Specified then files is upload to CLOUD and then SAS token is created

            var azureStorage = new AzureFileStorageManager(_secretsProvider, _systemQueryService);

            if (!string.IsNullOrWhiteSpace(ConfigurationManager.AppSettings["SASDemoFile"]))
            {
                var demoFile = Path.Combine(ConfigurationManager.AppSettings["tempFilePath"], ConfigurationManager.AppSettings["SASDemoFile"]);
                return azureStorage.GetUrlForFile(demoFile);
            }
            var fileInfo = new FileInfo(filePath);
            if (!fileInfo.Exists)
            {
                throw new Exception($"file {filePath} does not exists");
            }

            var machineName = Environment.MachineName;
            var newFilePath = Path.Combine(fileInfo.Directory.FullName, machineName, Guid.NewGuid().ToString(), fileInfo.Name);

            azureStorage.UploadFile(filePath, newFilePath);

            return azureStorage.GetUrlForFile(newFilePath);
        }
    }
}
