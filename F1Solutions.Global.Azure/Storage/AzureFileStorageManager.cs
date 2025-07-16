using System;
using System.Configuration;
using System.Globalization;
using System.IO;
using F1Solutions.Global.Common.Logging;
using F1Solutions.Naati.Common.Contracts.Dal.CacheQuery;
using F1Solutions.Naati.Common.Contracts.Dal.QueryServices;
using F1Solutions.Naati.Common.Contracts.Security;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.File;

namespace F1Solutions.Naati.Common.Azure.Storage
{
    public class AzureFileStorageManager : ISharedAccessSignature
    {
        private readonly ISystemQueryService _systemQueryService;
        private readonly StorageAccountConfiguration _tempStorageConfiguration;

        private int? _minBytesPerSecond;

        protected int MinBytesPerSecond
        {
            get
            {
                if (!_minBytesPerSecond.HasValue)
                {
                    var downloadRate = _systemQueryService.GetSystemValue(new Contracts.Dal.Request.GetSystemValueRequest { ValueKey = "MinDownloadKbRate" }).Value;
                    _minBytesPerSecond = (Convert.ToInt32(downloadRate) * 1024) / 8;
                }
                return _minBytesPerSecond.Value;
            }
        }

        private const int DownloadTolerance = 5 * 60; //5 minutes

        public AzureFileStorageManager(ISecretsCacheQueryService secretsProvider, ISystemQueryService systemQueryService)
        {
            _systemQueryService = systemQueryService;

            _tempStorageConfiguration = new StorageAccountConfiguration(secretsProvider.Get("SASAccountName"),
                secretsProvider.Get("SASAccountKey"),
                secretsProvider.Get("SASShareName"),
                ConfigurationManager.AppSettings["tempFilePath"]);
        }

        public string GetUrlForFile(string filePath)
        {
            return CreateSharedAccessSignatureToken(filePath, _tempStorageConfiguration);
        }

        private CloudStorageAccount GetStorageAccount(StorageAccountConfiguration storageAccountConfiguration)
        {
            var storageAccountTemplate = ConfigurationManager.AppSettings["StorageAccountTemplate"];
            var storageAccount = CloudStorageAccount.Parse(string.Format(storageAccountTemplate, storageAccountConfiguration.AccountName, storageAccountConfiguration.AccountKey));
            return storageAccount;
        }

        private string GetCloudPath(string filePath, StorageAccountConfiguration storageAccountConfiguration)
        {
            if (!filePath.StartsWith(storageAccountConfiguration.MappedDirectory.FullName, true, CultureInfo.InvariantCulture))
            {
                LoggingHelper.LogError($"Invalid file path '{filePath}'.  Only files from {storageAccountConfiguration.MappedDirectory.Name} are supported by the storage account. Please copy the file to the folder first!");
                throw new Exception($" Invalid file path '{filePath}'. Only files from {storageAccountConfiguration.MappedDirectory.Name} are supported by the storage account. Pleas to the folder first!");
            }

            var path = filePath.Replace(storageAccountConfiguration.MappedDirectory.FullName, storageAccountConfiguration.MappedDirectory.Name);
            path = path.Replace("\\\\", "/").Replace("\\", "/");
            return path;
        }
        private string CreateSharedAccessSignatureToken(string filePath, StorageAccountConfiguration storageAccountConfiguration)
        {
            var file = GetCloudFile(filePath, storageAccountConfiguration);
            if (!file.Exists())
            {
                LoggingHelper.LogError($"Impossible to create SAS Token for Path: {file.Uri}. The file does not exist");
                throw new Exception($"Impossible to create SAS Token for Path: {file.Uri}. The file does not exist");
            }
            file.FetchAttributes();
            var seconds = (file.Properties.Length / MinBytesPerSecond) + DownloadTolerance;

            var sasConstraints = new SharedAccessFilePolicy
            {
                SharedAccessStartTime = DateTime.UtcNow.AddSeconds(-1 * DownloadTolerance),
                SharedAccessExpiryTime = DateTime.UtcNow.AddSeconds(seconds),
                Permissions = SharedAccessFilePermissions.Read
            };

            var sasBlobToken = file.GetSharedAccessSignature(sasConstraints);
            LoggingHelper.LogInfo($"Requested SAS URL: {file.Uri} ");
            return $"{file.Uri}{sasBlobToken}";
        }

        private CloudFile GetCloudFile(string filePath, StorageAccountConfiguration storageAccountConfiguration)
        {
            var cloudFilePath = GetCloudPath(filePath, storageAccountConfiguration);

            var sharedSignatureUriTemplate = ConfigurationManager.AppSettings["StorageUriTemplate"];
            var myUri = new Uri(string.Format(sharedSignatureUriTemplate, storageAccountConfiguration.AccountName, storageAccountConfiguration.ShareName, cloudFilePath));
            var storageAccount = GetStorageAccount(storageAccountConfiguration);
            var file = new CloudFile(myUri, storageAccount.Credentials);
            return file;
        }

        public void UploadFile(string sourcePath, string destinationPath)
        {
            if (!File.Exists(sourcePath))
            {
                throw new Exception($"source Path: {sourcePath} does not exits");
            }
            
            var destinationCloudFile = GetCloudFile(destinationPath, _tempStorageConfiguration);

            if (destinationCloudFile.Exists())
            {
                throw new Exception($"destination Path: {destinationCloudFile.Uri} already exists");
            }

            CreateCloudDirectory(destinationCloudFile.Parent);
            destinationCloudFile.Create(0);
            destinationCloudFile.UploadFromFile(sourcePath);
        }

        public void CreateCloudDirectory(CloudFileDirectory directory)
        {
            if (!directory.Exists())
            {
                if (!directory.Parent.Exists())
                {
                    CreateCloudDirectory(directory.Parent);
                }
                directory.Create();
            }
        }
    }

}
