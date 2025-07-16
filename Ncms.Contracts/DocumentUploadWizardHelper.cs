using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using F1Solutions.Naati.Common.Contracts.Dal.Enum;
using Ncms.Contracts.Models.File;
using Ncms.Contracts.Models.MaterialRequest.Wizard;

namespace Ncms.Contracts
{
    public static class DocumentUploadWizardHelper
    {
        public static int StoreTempFile(DirectoryInfo outputRootFolder, string wizardInstance, string sourcePath, string fileName, string title, string documenType, int fileId, bool examinersAvailable, bool mergeDocument)
        {
            if (fileName.StartsWith("\"") && fileName.EndsWith("\""))
            {
                fileName = fileName.Trim('"');
            }

            if (fileName.Contains(@"/") || fileName.Contains(@"\"))
            {
                fileName = Path.GetFileName(fileName);
            }

            fileName = Path.GetFileNameWithoutExtension(title) + Path.GetExtension(fileName);
            var tempFileId = fileId == 0 ? (int)DateTime.Now.Ticks : fileId;

            var folder = GetDirectory(outputRootFolder, wizardInstance);
            folder = GetDirectory(folder, tempFileId.ToString());

            if (fileId != 0)
            {
                folder.Delete(true);
            }
            folder = GetDirectory(folder, documenType);
            folder = GetDirectory(folder, examinersAvailable.ToString());
            folder = GetDirectory(folder, mergeDocument.ToString());
            File.Move(sourcePath, Path.Combine(folder.FullName, fileName));

            return tempFileId;
        }

        private static DirectoryInfo GetDirectory(DirectoryInfo directoryInfo, string directoryName)
        {
            var directoryPath = Path.Combine(directoryInfo.FullName, directoryName);
            if (!Directory.Exists(directoryPath))
            {
                return Directory.CreateDirectory(directoryPath);
            }

            return new DirectoryInfo(directoryPath);
        }

        public static void RemoveDocument(DirectoryInfo rootDirectory, string wizardInstanceId, string tempFileId)
        {
            var folder = GetDirectory(rootDirectory, wizardInstanceId);
            folder = GetDirectory(folder, tempFileId);
            var files = folder.GetFiles("*.*", SearchOption.AllDirectories);

            if (files.Length > 1)
            {
                throw new Exception($"More than one temp file found for id {tempFileId} on wizard intance {wizardInstanceId}");
            }

            if (files.Any())
            {
                var file = files.First();
                File.Delete(file.FullName);
            }

        }

        public static IEnumerable<DocumentUploadModel> GetDocuments(DirectoryInfo rootDirectory, string wizardInstanceId, bool includeFilePath)
        {
            var instanceDirectory = GetDirectory(rootDirectory, wizardInstanceId);
            var files = instanceDirectory.GetFiles("*.*", SearchOption.AllDirectories);
            var documents = new List<DocumentUploadModel>();
            foreach (var file in files)
            {
                var pathComponents = file.FullName.Split('\\');

                var fileName = pathComponents.Last();
                var tempId = pathComponents[pathComponents.Length - 5];
                var documentTypeName = pathComponents[pathComponents.Length - 4];
                var examinersAvailable = bool.Parse(pathComponents[pathComponents.Length - 3]);
                var mergeDocument = bool.Parse(pathComponents[pathComponents.Length - 2]);

                var documenType = (StoredFileType)Enum.Parse(typeof(StoredFileType), documentTypeName);

                documents.Add(new DocumentUploadModel
                {
                    FileName = pathComponents.Last(),
                    DocumenTypeId = (int)documenType,
                    Title = fileName,
                    TempFileId = Convert.ToInt32(tempId),
                    FileSize = file.Length,
                    UploadedDateTime = file.CreationTime,
                    FilePath = includeFilePath ? file.FullName : string.Empty,
                    ExaminersAvailable = examinersAvailable,
                    MergeDocument = mergeDocument

                });
            }

            return documents;
        }


        public static FileResponseModel GetFile(string rootPath, int tempFileId, string wizardInstanceId){
            
            var rootDirectory = new DirectoryInfo(rootPath);
            var folder = GetDirectory(rootDirectory, wizardInstanceId);
            folder = GetDirectory(folder, tempFileId.ToString());

            var files = folder.GetFiles("*.*", SearchOption.AllDirectories);
            if (files.Length != 1)
            {
                throw new Exception($"File Id {tempFileId} no found on wizard instance {wizardInstanceId}");
            }

            var file = files.First();
            var fileData = new MemoryStream(File.ReadAllBytes(file.FullName), false) { Position = 0 };

            return new FileResponseModel
            {
                FileData = fileData,
                FileName = file.Name
            };
        }
    }
}
