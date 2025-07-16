using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using F1Solutions.Global.Common.Mapping;
using F1Solutions.Naati.Common.Contracts.Bl.DTO;
using F1Solutions.Naati.Common.Contracts.Dal;
using F1Solutions.Naati.Common.Contracts.Dal.DTO;
using F1Solutions.Naati.Common.Contracts.Dal.Enum;
using F1Solutions.Naati.Common.Contracts.Dal.QueryServices;
using F1Solutions.Naati.Common.Contracts.Dal.Request;
using F1Solutions.Naati.Common.Contracts.Dal.Response;
using F1Solutions.Naati.Common.Dal.Domain;
using F1Solutions.Naati.Common.Dal.NHibernate.Configuration;

namespace F1Solutions.Naati.Common.Dal
{
    public class FileSystemFileStorageService : IFileStorageService
    {
        private readonly IAutoMapperHelper _autoMapperHelper;
        private readonly string _basePath;
        private const string UniqueIdTag = "[UniqueID]";
        private const string TypeTag = "[DocumentType]";

        public FileSystemFileStorageService(IAutoMapperHelper autoMapperHelper)
        {
            _autoMapperHelper = autoMapperHelper;

            // all file paths provided are relative to the base path
            _basePath = ConfigurationManager.AppSettings["fileSystemFileStorageServiceBasePath"];

            if (Directory.Exists(_basePath))
            {
                return;
            }

            try
            {
                Directory.CreateDirectory(_basePath);
            }
            catch (IOException ex)
            {
                throw new Exception("Failed to create file storage base directory: " + _basePath, ex);
            }
        }

        private string GetStoredFileFullPath(StoredFile storedFile)
        {
            return Path.Combine(_basePath, storedFile.ExternalFileId);
        }

        private static StoredFile GetStoredFile(int storedFileId)
        {
            var storedFile = NHibernateSession.Current.Query<StoredFile>().SingleOrDefault(x => x.Id == storedFileId);

            if (storedFile == null)
            {
                throw new Exception("StoredFile does not exist. ID: " + storedFileId);
            }

            return storedFile;
        }

        public GetFileInfoResponse GetStoredFileInfo(int storedFileId)
        {
            var documentInfo = GetStoredFile(storedFileId);

            return new GetFileInfoResponse
            {
                ExternalFileId = documentInfo.ExternalFileId,
                DocumentTypeId = documentInfo.DocumentType.Id,
                FileName = documentInfo.FileName,
                FileSize = documentInfo.FileSize,
                UploadedByPersonId = documentInfo.UploadedByPerson?.Id,
                UploadedByUserId = documentInfo.UploadedByUser?.Id,
                UploadedDateTime = documentInfo.UploadedDateTime
            };
        }

        public GetFileResponse GetFile(GetFileRequest request)
        {
            if (request == null)
            {
                throw new ArgumentNullException("request");
            }

            var storedFile = GetStoredFile(request.StoredFileId);
            var filePath = Path.Combine(_basePath, storedFile.ExternalFileId);

            if (!File.Exists(filePath))
            {
                throw new WebServiceException("File missing: " + filePath);
            }

            int a = 0;
            bool fileExists;
            do
            {
                fileExists = File.Exists(request.TempFileStorePath + '\\' + AppendFileName(storedFile.FileName, a));
                if (fileExists)
                {
                    a++;
                }
            } while (fileExists);

            File.Copy(filePath, request.TempFileStorePath + '\\' + AppendFileName(storedFile.FileName, a));

            return new GetFileResponse
            {
                FilePaths = new[] { request.TempFileStorePath + '\\' + AppendFileName(storedFile.FileName, a) },
                FileName = storedFile.FileName,
                Types =  new[] { (StoredFileType)storedFile.DocumentType.Id }
            };
        }

        public GetFileResponse GetTestMaterialFilesByAttendee(GetFilesRequest request)
        {
            if (request == null)
            {
                throw new ArgumentNullException("request");
            }
            var fileTypes = new HashSet<StoredFileType>();
            var filePaths = new string[request.StoredFileIds.Length];

            int i = 0;
            var nameList = new List<string>();
            foreach (var storedFileId in request.StoredFileIds)
            {
                var storedFile = GetStoredFile(storedFileId);
                var filePath = Path.Combine(_basePath, storedFile.ExternalFileId);


                if (File.Exists(filePath))
                {
                    int a = 0;
                    while (nameList.Contains(AppendFileName(storedFile.FileName, a)))
                    {
                        a++;
                    }

                    bool fileExists;
                    do
                    {
                        fileExists = File.Exists(request.TempFileStorePath + '\\' + AppendFileName(storedFile.FileName, a));
                        if (fileExists)
                        {
                            a++;
                        }
                    } while (fileExists);

                    File.Copy(filePath, request.TempFileStorePath + '\\' + AppendFileName(storedFile.FileName, a));

                    filePaths[i] = request.TempFileStorePath + '\\' + AppendFileName(storedFile.FileName, a);
                    nameList.Add(AppendFileName(storedFile.FileName, a));
                    fileTypes.Add((StoredFileType)storedFile.DocumentType.Id);
                }
                else
                {
                    throw new Exception("File missing: " + filePath);
                }
                i++;
            }

            return new GetFileResponse
            {
                FileName = "files.zip",
                FilePaths = filePaths,
                Types = fileTypes
            };
        }
        public GetFileResponse GetFiles(GetFilesRequest request)
        {
            if (request == null)
            {
                throw new ArgumentNullException("request");
            }

            var filePaths = new string[request.StoredFileIds.Length];

            int i = 0;
            var nameList = new List<string>();
            var fileTypes = new HashSet<StoredFileType>();
            foreach (var storedFileId in request.StoredFileIds)
            {
                var storedFile = GetStoredFile(storedFileId);
                var filePath = Path.Combine(_basePath, storedFile.ExternalFileId);


                if (File.Exists(filePath))
                {
                    int a = 0;
                    while (nameList.Contains(AppendFileName(storedFile.FileName, a)))
                    {
                        a++;
                    }

                    bool fileExists;
                    do
                    {
                        fileExists = File.Exists(request.TempFileStorePath + '\\' + AppendFileName(storedFile.FileName, a));
                        if (fileExists)
                        {
                            a++;
                        }
                    } while (fileExists);

                    File.Copy(filePath, request.TempFileStorePath + '\\' + AppendFileName(storedFile.FileName, a));

                    filePaths[i] = request.TempFileStorePath + '\\' + AppendFileName(storedFile.FileName, a);
                    nameList.Add(AppendFileName(storedFile.FileName, a));
                    fileTypes.Add((StoredFileType)storedFile.DocumentType.Id);
                }
                else
                {
                    throw new Exception("File missing: " + filePath);
                }
                i++;
            }

            return new GetFileResponse
            {
                FileName = "files.zip",
                FilePaths = filePaths,
                Types = fileTypes
            };
        }

        public GetAttendeesTestSpecificationTestMaterialResponse GetFileStoreTestSpecificationTestMaterialList(GetFileStoreTestSpecificationTestMaterialRequest request)
        {
            if (request == null)
            {
                throw new ArgumentNullException("request");
            }

            var response = new GetAttendeesTestSpecificationTestMaterialResponse();
            bool isMissingFiles = false;
           

            foreach (var attendeeTestSpecificationTestMaterial in request.AttendeeTestSpecificationTestMaterialList)
            {
            
                //for test specification
                var attendeeTestSpecification = attendeeTestSpecificationTestMaterial.AttendeeTestSpecification;
                var testSpecificationFileNameList = new List<string>();
                var missingTestSpecificationStoredFileList = new List<StoredFileMarterialDto>();

                foreach (var storedFileDto in attendeeTestSpecification.StoredFileList)
                {
                    var storedFile = GetStoredFile(storedFileDto.Id);
                    var filePath = Path.Combine(_basePath, storedFile.ExternalFileId);

                    if (File.Exists(filePath))
                    {
                        int a = 0;
                        while (testSpecificationFileNameList.Contains(AppendFileName(storedFile.FileName, a)))
                        {
                            a++;
                        }

                        bool fileExists;
                        do
                        {
                            fileExists = File.Exists(request.TempFileStorePath + '\\' + AppendFileName(storedFile.FileName, a));
                            if (fileExists)
                            {
                                a++;
                            }
                        } while (fileExists);

                        File.Copy(filePath, request.TempFileStorePath + '\\' + AppendFileName(storedFile.FileName, a));

                        storedFileDto.FilePath = request.TempFileStorePath + '\\' + AppendFileName(storedFile.FileName, a);

                        testSpecificationFileNameList.Add(AppendFileName(storedFile.FileName, a));
                    }
                    else
                    {
                        missingTestSpecificationStoredFileList.Add(storedFileDto);
                    }
                }
                foreach (var missingTestSpecificationStoredFile in missingTestSpecificationStoredFileList)
                {
                    attendeeTestSpecification.StoredFileList.Remove(missingTestSpecificationStoredFile);
                    isMissingFiles = true;
                }
                
                //for test materials
                foreach (var attendeeTestMaterial in attendeeTestSpecificationTestMaterial.AttendeeTestMaterialList)
                {

                    var i = 0;
                    var testMaterialFilenameList = new List<string>();
                    var missingStoredFileList = new List<StoredFileMarterialDto>();

                    foreach (var storedFileDto in attendeeTestMaterial.StoredFileList)
                    {
                        var storedFile = GetStoredFile(storedFileDto.Id);
                        var filePath = Path.Combine(_basePath, storedFile.ExternalFileId);

                        if (File.Exists(filePath))
                        {
                            var a = 0;
                            while (testMaterialFilenameList.Contains(AppendFileName(storedFile.FileName, a)))
                            {
                                a++;
                            }

                            bool fileExists;
                            do
                            {
                                fileExists = File.Exists(request.TempFileStorePath + '\\' + AppendFileName(storedFile.FileName, a));
                                if (fileExists)
                                {
                                    a++;
                                }
                            } while (fileExists);

                            File.Copy(filePath, request.TempFileStorePath + '\\' + AppendFileName(storedFile.FileName, a));

                            storedFileDto.FilePath = request.TempFileStorePath + '\\' + AppendFileName(storedFile.FileName, a);

                            testMaterialFilenameList.Add(AppendFileName(storedFile.FileName, a));
                        }
                        else
                        {
                            missingStoredFileList.Add(storedFileDto);
                        }
                        i++;
                    }
                    foreach (var missingStoredFile in missingStoredFileList)
                    {
                        attendeeTestMaterial.StoredFileList.Remove(missingStoredFile);
                        isMissingFiles = true;
                    }
                    
                }
                
            }

            if (isMissingFiles)
            {
                var updatedAttendeeTestSpecificationTestMaterialList = GetUpdateAttendeeTestSpecificationTestMaterialList(request.AttendeeTestSpecificationTestMaterialList);
                response.AttendeeTestSpecificationTestMaterialList = updatedAttendeeTestSpecificationTestMaterialList;
                response.IsMissingFiles = true;
            }
            else
            {
                response.AttendeeTestSpecificationTestMaterialList = request.AttendeeTestSpecificationTestMaterialList;
            }

            return response;
        }

        private List<AttendeeTestSpecificationTestMaterial> GetUpdateAttendeeTestSpecificationTestMaterialList(List<AttendeeTestSpecificationTestMaterial> attendeeTestSpecificationTestMaterialList)
        {

            var updatedAttendeeTestSpecificationTestMaterialList = new List<AttendeeTestSpecificationTestMaterial>();
            
            foreach (var attendeeTestSpecificationTestMaterial in attendeeTestSpecificationTestMaterialList)
            {
                var updatedAttendeeTestSpecificationTestMaterial = new AttendeeTestSpecificationTestMaterial
                {
                    AttendanceId = attendeeTestSpecificationTestMaterial.AttendanceId,
                    CustomerNumber = attendeeTestSpecificationTestMaterial.CustomerNumber,
                    AttendeeTestMaterialList = new List<AttendeeTestMaterial>(),
                    AttendeeTestSpecification = new AttendeeTestSpecification()
                };

                //for test specification
                var attendeeTestSpecification = attendeeTestSpecificationTestMaterial.AttendeeTestSpecification;
                var updatedAttendeeTestSpecification = new AttendeeTestSpecification
                {
                    Id = attendeeTestSpecification.Id
                };

                if (attendeeTestSpecification.StoredFileList.Count > 0)
                {
                    updatedAttendeeTestSpecification.StoredFileList = attendeeTestSpecification.StoredFileList;
                }


                //for test materials
                var updatedAttendeeTestMaterialList = new List<AttendeeTestMaterial>();

                foreach (var attendeeTestMaterial in attendeeTestSpecificationTestMaterial.AttendeeTestMaterialList)
                {
                    var updatedAttendeeTestMaterial = new AttendeeTestMaterial
                    {
                        Id = attendeeTestMaterial.Id
                    };

                    if (attendeeTestMaterial.StoredFileList.Count > 0)
                    {
                        updatedAttendeeTestMaterial.StoredFileList = attendeeTestMaterial.StoredFileList;
                        updatedAttendeeTestMaterialList.Add(updatedAttendeeTestMaterial);
                    }
                }

                if (updatedAttendeeTestMaterialList.Count > 0)
                {
                    updatedAttendeeTestSpecificationTestMaterial.AttendeeTestMaterialList = updatedAttendeeTestMaterialList;
                    updatedAttendeeTestSpecificationTestMaterialList.Add(attendeeTestSpecificationTestMaterial);
                }
            }

            return updatedAttendeeTestSpecificationTestMaterialList;
        }


        public void MoveFile(MoveFileRequest request)
        {
            if (request.StoredFileId ==0)
            {
                throw new ArgumentException(nameof(request.StoredFileId));
            }
            if (string.IsNullOrWhiteSpace(request.StotoragePath))
            {
                throw new ArgumentException(nameof(request.StotoragePath));
            }

            var storedFile = NHibernateSession.Current.Load<StoredFile>(request.StoredFileId);
            var oldPath = Path.Combine(_basePath, storedFile.ExternalFileId);
            
            var newPath = MakeUnique(Path.Combine(_basePath, request.StotoragePath));

            var fileName = Path.GetFileName(newPath);

            Directory.CreateDirectory(Path.GetDirectoryName(newPath));
            File.Copy(oldPath, newPath);

            var externaFileId = newPath.Replace(_basePath, string.Empty);
            if (externaFileId.StartsWith("\\"))
            {
                externaFileId = externaFileId.Substring(1);
            }
            storedFile.ExternalFileId = externaFileId;
            storedFile.FileName = fileName;
            NHibernateSession.Current.SaveOrUpdate(storedFile);
            NHibernateSession.Current.Flush();
            File.Delete(oldPath);
        }

        public CreateOrUpdateFileResponse CreateOrUpdateFile(CreateOrUpdateFileRequest request)
        {
            if (request == null)
            {
                throw new ArgumentNullException("request");
            }

            if (!request.UpdateStoredFileId.HasValue && request.FilePath == null)
            {
                throw new ArgumentNullException("request.FilePath");
            }

            var storedFile = new StoredFile();

            if (request.UpdateStoredFileId.HasValue)
            {
                storedFile = GetStoredFile(request.UpdateStoredFileId.Value);
                if (storedFile.FileName != (request.UpdateFileName ?? storedFile.FileName) && String.IsNullOrWhiteSpace(request.FilePath))
                {
                    var oldFileName = storedFile.FileName;
                    var filePath = Path.Combine(_basePath, storedFile.ExternalFileId);
                    storedFile.FileName = $"{request.UpdateFileName}{Path.GetExtension(filePath)}";

                    var newFilePath = MakeUnique($@"{Path.GetDirectoryName(filePath)}\{storedFile.FileName}");
                    File.Move(filePath, newFilePath);

                    var pathName = storedFile.ExternalFileId.Substring(0, storedFile.ExternalFileId.LastIndexOf(oldFileName));
                    storedFile.ExternalFileId = $"{pathName}{storedFile.FileName}";
                }
            }

            UpdateFileData(request, storedFile);

            var hasFilePath = !String.IsNullOrWhiteSpace(request.FilePath);
            if (hasFilePath)
            { 
                var originalPath = Path.Combine(_basePath, request.StoragePath);
                if (!string.IsNullOrEmpty(request.TokenToRemoveFromFilename))
                {
                    originalPath = originalPath.Replace(request.TokenToRemoveFromFilename,string.Empty);
                }

                var uniquePath = MakeUnique(originalPath);
                var fileName = Path.GetFileName(uniquePath);

                Directory.CreateDirectory(Path.GetDirectoryName(uniquePath));

                bool fileRemovedFromTempFolder = false;

                try
                {
                    File.Move(request.FilePath, uniquePath);
                    fileRemovedFromTempFolder = true;
                }
                finally
                {
                    if (!fileRemovedFromTempFolder)
                    {
                        File.Delete(request.FilePath);
                    }
                }

                //delete old file
                if (request.UpdateStoredFileId.HasValue)
                {
                    DeleteFile(storedFile);
                }

                storedFile.FileSize = new FileInfo(uniquePath).Length;
                storedFile.ExternalFileId = uniquePath.Replace(_basePath + "\\", string.Empty);
                storedFile.FileName = fileName;
            }

            var storedFileId = (int)NHibernateSession.Current.Save(storedFile);
            NHibernateSession.Current.Flush();

            var fieType =
                NHibernateSession.Current.Query<DocumentType>()
                    .Single(dt => dt.Name == request.Type.ToString())
                    .DisplayName;

            storedFile.FileName = storedFile.FileName.Replace(UniqueIdTag, storedFileId.ToString());
            storedFile.FileName = storedFile.FileName.Replace(TypeTag, fieType);

            var oldExternalFileId = storedFile.ExternalFileId;
            storedFile.ExternalFileId = storedFile.ExternalFileId.Replace(UniqueIdTag, storedFileId.ToString());
            storedFile.ExternalFileId = storedFile.ExternalFileId.Replace(TypeTag, fieType);

            if (oldExternalFileId != storedFile.ExternalFileId)
            {
                File.Move(Path.Combine(_basePath, oldExternalFileId), Path.Combine(_basePath, storedFile.ExternalFileId));
            }

            NHibernateSession.Current.Save(storedFile);
            NHibernateSession.Current.Flush();

            return new CreateOrUpdateFileResponse
            {
                StoredFileId = storedFileId
            };
        }  

        public CreateOrUpdateFileResponse ReplaceFile(FileMetadataRequest request)
        {
            if (request == null)
            {
                throw new ArgumentNullException("request");
            }

            if (!request.UpdateStoredFileId.HasValue)
            {
                throw new ArgumentNullException("UpdateStoredFileId");
            }

            var storedFile = GetStoredFile(request.UpdateStoredFileId.Value);

            UpdateFileData(request, storedFile);

            var originalPath = Path.Combine(_basePath, storedFile.ExternalFileId);

            var newPath = Path.Combine(_basePath, request.StoragePath);

            if (newPath != originalPath)
            {
                var uniquePath = MakeUnique(newPath);
                Directory.CreateDirectory(GetDirectory(uniquePath));
                File.Move(originalPath, uniquePath);

                storedFile.ExternalFileId = uniquePath.Replace(_basePath + "\\", string.Empty);
                storedFile.FileName = Path.GetFileName(uniquePath);
            }

            var storedFileId = (int)NHibernateSession.Current.Save(storedFile);
            NHibernateSession.Current.Flush();

            return new CreateOrUpdateFileResponse
            {
                StoredFileId = storedFileId
            };
        }

        private static void UpdateFileData(FileMetadataRequest request, StoredFile storedFile)
        {
            storedFile.DocumentType = NHibernateSession.Current.Query<DocumentType>().SingleOrDefault(d => d.Name == request.Type.ToString());
            storedFile.UploadedDateTime = request.UploadedDateTime;
            storedFile.StoredFileStatusChangeDate = request.UploadedDateTime;
            storedFile.StoredFileStatusType = NHibernateSession.Current.Query<Domain.StoredFileStatusType>().SingleOrDefault(d => d.Id == 1);

            if (request.UploadedByPersonId.HasValue)
            {
                var person = NHibernateSession.Current.Query<Person>().SingleOrDefault(x => x.Id == request.UploadedByPersonId);

                if (person == null)
                {
                    throw new Exception("Person does not exist. ID: " + request.UploadedByPersonId);
                }

                storedFile.UploadedByPerson = person;
            }

            if (!request.UploadedByUserId.HasValue)
            {
                return;
            }

            var user = NHibernateSession.Current.Query<User>().SingleOrDefault(x => x.Id == request.UploadedByUserId);

            if (user == null)
            {
                throw new Exception("User does not exist. ID: " + request.UploadedByUserId);
            }

            storedFile.UploadedByUser = user;
        }

        private void DeleteFile(StoredFile storedFile)
        {
            var filePath = GetStoredFileFullPath(storedFile);
            DeleteFile(filePath);
        }

        private static void DeleteFile(string filePath)
        {
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }

            if (File.Exists(filePath))
            {
                throw new Exception("File delete failed: " + filePath);
            }
        }

        public string MakeUnique(string path)
        {
            var dir = Path.GetDirectoryName(path);
            var fileName = Path.GetFileNameWithoutExtension(path);
            var fileExt = Path.GetExtension(path);

            for (var i = 1; ; ++i)
            {
                if (!File.Exists(path))
                {
                    return path;
                }

                path = Path.Combine(dir, fileName + " " + i + fileExt);
            }
        }

        public string GetDirectory(string path)
        {
            var returnVal = path.Split('.')[0];
            return returnVal.Remove(returnVal.IndexOf(returnVal.Split('\\').Last()) - 1);
        }

        // todo move to some common library
        private static void CopyStream(Stream input, Stream output)
        {
            var buffer = new byte[32 * 1024];
            int len;
            while ((len = input.Read(buffer, 0, buffer.Length)) > 0)
            {
                output.Write(buffer, 0, len);
            }
        }

        public void DeleteFile(DeleteFileRequest request)
        {
            if (request == null)
            {
                throw new ArgumentNullException("request");
            }


            var storedFile = NHibernateSession.Current.Query<StoredFile>().SingleOrDefault(x => x.Id == request.StoredFileId);

            if (storedFile == null)
            {
                return;
            }

            DeleteFile(storedFile);

            object deleteRelation = null;
            switch ((StoredFileType)storedFile.DocumentType.Id)
            {
                case StoredFileType.MarkedTestAsset:
                case StoredFileType.UnmarkedTestAsset:
                case StoredFileType.GeneralTestDocument:
                case StoredFileType.ReviewReport:
                case StoredFileType.EnglishMarking:
                case StoredFileType.TestMaterial:
                case StoredFileType.ProblemSheet:
                case StoredFileType.MedicalCertificate:
                {
                    deleteRelation = NHibernateSession.Current.Query<TestSittingDocument>().SingleOrDefault(x => x.StoredFile.Id == request.StoredFileId);

                    if (storedFile.DocumentType.Id == (int)StoredFileType.TestMaterial && deleteRelation == null)
                    {
                        deleteRelation = NHibernateSession.Current.Query<TestMaterialAttachment>().SingleOrDefault(x => x.StoredFile.Id == request.StoredFileId);
                    }
                    break;
                }
                case StoredFileType.Certificate:
                case StoredFileType.CredentialLetter:
                {
                    deleteRelation = NHibernateSession.Current.Query<CredentialAttachment>().SingleOrDefault(x => x.StoredFile.Id == request.StoredFileId);
                    break;
                }
            }

            if (deleteRelation != null)
            {
                NHibernateSession.Current.Delete(deleteRelation);
            }

            NHibernateSession.Current.Delete(storedFile);
            NHibernateSession.Current.Flush();
        }

        public ListFilesResponse ListFiles(ListFilesRequest listFilesRequest)
        {
            var storedFileQuery = NHibernateSession.Current.Query<StoredFile>();
            var query = new List<KeyValuePair<int, StoredFile>>();

            if (!string.IsNullOrEmpty(listFilesRequest.Name))
            {
                storedFileQuery = storedFileQuery.Where(x => x.FileName.Contains(listFilesRequest.Name));
            }

            if ((listFilesRequest.Types ?? (listFilesRequest.Types = new List<StoredFileType>())).Any())
            {
                var types = listFilesRequest.Types.Select(t => (int)t).ToArray();
                storedFileQuery = storedFileQuery.Where(x => types.Contains(x.DocumentType.Id));
            }

            if (listFilesRequest.StoredFileId.HasValue)
            {
                storedFileQuery = storedFileQuery.Where(x => x.Id == listFilesRequest.StoredFileId.Value);
            }

            bool assets = false;
            if (listFilesRequest.RelatedId.HasValue)
            {
                foreach (var type in listFilesRequest.Types)
                {
                    var typeName = (int)type;

                    switch (type)
                    {
                        case StoredFileType.GeneralTestDocument:
                        case StoredFileType.UnmarkedTestAsset:
                        case StoredFileType.MarkedTestAsset:
                        case StoredFileType.ReviewReport:
                        case StoredFileType.EnglishMarking:
                            assets = true;
                            var testAttendanceAssetQuery = NHibernateSession.Current.Query<TestSittingDocument>();
                            query.AddRange(from sf in storedFileQuery
                                           join tra in testAttendanceAssetQuery on sf.Id equals tra.StoredFile.Id
                                           where tra.TestSitting.Id == listFilesRequest.RelatedId.Value && (listFilesRequest.IncludeDeleted || !tra.Deleted)
                                                 && sf.DocumentType.Id == typeName
                                                 &&
                                                 (
                                                    (typeName != (int)StoredFileType.MarkedTestAsset &&
                                                     typeName != (int)StoredFileType.ReviewReport) ||
                                                    sf.UploadedByPerson == null ||
                                                    (
                                                        from tr in tra.TestSitting.TestResults
                                                        from em in tr.ExaminerMarkings
                                                        where em.JobExaminer.PanelMembership.Person.Id == sf.UploadedByPerson.Id
                                                        select 1
                                                    ).Count() > 0
                                                 )
                                           select new KeyValuePair<int, StoredFile>(tra.Id, sf));
                            break;
                        case StoredFileType.TestMaterial:
                            if (listFilesRequest.Types.Count > 1)
                            {
                                var testAttendanceMaterialAssetQuery = NHibernateSession.Current.Query<TestSittingDocument>();
                                query.AddRange(from sf in storedFileQuery
                                               join tra in testAttendanceMaterialAssetQuery on sf.Id equals tra.StoredFile.Id
                                               where tra.TestSitting.Id == listFilesRequest.RelatedId.Value && (listFilesRequest.IncludeDeleted || !tra.Deleted)
                                                     && sf.DocumentType.Id == typeName
                                                     &&
                                                     (
                                                        (typeName != (int)StoredFileType.MarkedTestAsset &&
                                                         typeName != (int)StoredFileType.ReviewReport) ||
                                                        sf.UploadedByPerson == null ||
                                                        (
                                                            from tr in tra.TestSitting.TestResults
                                                            from em in tr.ExaminerMarkings
                                                            where em.JobExaminer.PanelMembership.Person.Id == sf.UploadedByPerson.Id
                                                            select 1
                                                        ).Count() > 0
                                                     )
                                               select new KeyValuePair<int, StoredFile>(tra.Id, sf));
                            }
                            else
                            {
                                var testMaterialQuery = NHibernateSession.Current.Query<TestMaterialAttachment>();
                                query.AddRange(from sf in storedFileQuery
                                               join tra in testMaterialQuery on sf.Id equals tra.StoredFile.Id
                                               where tra.TestMaterial.Id == listFilesRequest.RelatedId.Value && (listFilesRequest.IncludeDeleted || !tra.Deleted)
                                                     && sf.DocumentType.Id == typeName
                                               select new KeyValuePair<int, StoredFile>(tra.Id, sf));
                            }
                            break;
                        case StoredFileType.Undefined:
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }
            }

            var files = query.Select(q =>
            {
                var sf = q.Value;
                var person = sf.UploadedByPerson ?? new Person();
                var user = sf.UploadedByUser ?? new User();

                return new FileDto
                {
                    ParentId = q.Key,
                    StoredFileId = sf.Id,
                    FileName = sf.FileName,
                    FileSize = sf.FileSize,
                    Type = (StoredFileType)sf.DocumentType.Id,
                    UploadedByPersonId = person.Id,
                    UploadedByPersonName = person.FullName,
                    UploadedByUserId = user.Id,
                    UploadedByUserName = user.FullName,
                    UploadedDateTime = sf.UploadedDateTime,
                    EportalDownload = assets ? NHibernateSession.Current.Query<TestSittingDocument>().Single(x => x.StoredFile.Id == sf.Id).EportalDownload : false
                };
            });

            return new ListFilesResponse { Files = files.ToList() };
        }
        
       

        public DocumentTypesResponse ListDocumentTypes(ListDocumentTypesRequest request)
        {

            var query = NHibernateSession.Current.Query<DocumentTypeRole>();

            if (request.Category.HasValue)
            {
                query = query.Where(x => x.DocumentType.DocumentTypeCategory.Id == (int) request.Category);
            }

            if (request.UserRestriction != null)
            {
                var roles = NHibernateSession.Current.Query<UserRole>()
                    .Where(x => x.User.Id == request.UserRestriction.UserId)
                    .Select(y => y.SecurityRole.Id)
                    .ToList();

                query = query.Where(x => roles.Contains(x.Role.Id));

                if (request.UserRestriction.Download)
                {
                    query = query.Where(x => x.Download);
                }

                if (request.UserRestriction.Upload)
                {
                    query = query.Where(x => x.Upload);
                }
            }

            var result = query.Select(x=> x.DocumentType).ToList().GroupBy(y=> y.Id)
                .Select(z=> _autoMapperHelper.Mapper.Map<DocumentTypeDto>(z.First())).ToList();

            return new DocumentTypesResponse
            {
                Types = result
            };
        }

        private string AppendFileName(string rawFileName, int num)
        {
            var fileName = rawFileName.Replace("#", "_");
            if (num == 0)
            {
                return fileName;
            }
            var baseSnippet = fileName.Split('.')[0];

            string returnVal = baseSnippet;
            returnVal += string.Format("({0})", num);

            foreach (var snippet in fileName.Split('.'))
            {
                if (snippet != baseSnippet)
                {
                    returnVal += '.' + snippet;
                }
            }

            return returnVal;
        }
    }
}
