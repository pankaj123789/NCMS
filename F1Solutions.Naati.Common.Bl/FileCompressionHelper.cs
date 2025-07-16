using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.IO.Compression;
using System.Linq;
using Aspose.Words.Fonts;
using F1Solutions.Global.Common.Logging;
using F1Solutions.Naati.Common.Contracts.Bl;
using F1Solutions.Naati.Common.Contracts.Dal.QueryServices;
using F1Solutions.Naati.Common.Contracts.Dal.Request;

namespace F1Solutions.Naati.Common.Bl
{
    public class FileCompressionHelper : IFileCompressionHelper
    {
        private readonly IFileStorageService _fileStorageService;

        public FileCompressionHelper(IFileStorageService fileStorageService)
        {
            _fileStorageService = fileStorageService;
        }

        public string CreateZipFile(IEnumerable<int> storageFileIds, string zipFileName)
        {
            var folderName = Guid.NewGuid().ToString();
            var folderPath = Path.Combine(ConfigurationManager.AppSettings["tempFilePath"], folderName);
            Directory.CreateDirectory(folderPath);
            var filesResponse = _fileStorageService.GetFiles(new GetFilesRequest() { StoredFileIds = storageFileIds.ToArray(), TempFileStorePath = folderPath });

            var compressibleFiles = filesResponse.FilePaths.Select(
                f => new CompressibleFile()
                {
                    FileName = Path.GetFileName(f),
                    FilePath = f
                }).ToArray();

            return InnerCreateZipFile(compressibleFiles, zipFileName, folderPath, true);
        }

        public string CreateZipFile(IEnumerable<string> filePaths, string zipFileName, bool deleteFiles = true)
        {
            var files = filePaths.Select(
                f => new CompressibleFile()
                {
                    FileName = Path.GetFileName(f),
                    FilePath = f
                });

            return CreateZipFile(files, zipFileName, deleteFiles);
        }

        public string CreateZipFile(IEnumerable<CompressibleFile> files, string zipFileName, bool deleteFiles = true)
        {
            var folderName = Guid.NewGuid().ToString();
            var folderPath = Path.Combine(ConfigurationManager.AppSettings["tempFilePath"], folderName);
            Directory.CreateDirectory(folderPath);

            var paths = files.ToArray();

            return InnerCreateZipFile(paths, zipFileName, folderPath, deleteFiles);
        }

        private string InnerCreateZipFile(CompressibleFile[] files, string zipFileName, string folderPath, bool deleteFiles)
        {
            var zipPath = Path.Combine(folderPath, $"{Path.GetFileNameWithoutExtension(zipFileName)}.zip");

            try
            {
                using (var zipStream = new FileStream(zipPath, FileMode.CreateNew))
                {
                    using (var zip = new ZipArchive(zipStream, ZipArchiveMode.Create, true))
                    {
                        foreach (var file in files)
                        {
                            zip.CreateEntryFromFile(file.FilePath, file.FileName);
                        }
                    }
                }

                if (deleteFiles)
                {
                    foreach (var file in files)
                    {
                        File.Delete(file.FilePath);
                    }
                }

            }
            catch (Exception ex)
            {
                LoggingHelper.LogException(ex);
            }

            return zipPath;
        }

    }
}
