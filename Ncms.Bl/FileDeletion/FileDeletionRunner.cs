using F1Solutions.Global.Common.Logging;
using F1Solutions.Naati.Common.Contracts.Bl.BackgroundTask;
using F1Solutions.Naati.Common.Contracts.Bl.DTO;
using F1Solutions.Naati.Common.Contracts.Bl.FileDeletion;
using F1Solutions.Naati.Common.Contracts.Dal.DTO;
using F1Solutions.Naati.Common.Contracts.Dal.QueryServices;
using F1Solutions.Naati.Common.Contracts.Dal.Request;
using F1Solutions.Naati.Common.Dal.Domain;
using Ncms.Bl.FileDeletion.FileDeletionGenerators;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;

namespace Ncms.Bl.FileDeletion
{
    public class FileDeletionRunner
    {
        protected int BatchSize;
        protected int IncludeQueued;
        protected string StoragePath;
        protected int CurrentUserId;
        protected IBackgroundTaskLogger TaskLogger;
        private readonly IFileDeletionDalService _fileDeletionDalService;
        private readonly ISystemQueryService _systemQueryService;

        private const string CredentialApplicationAttachmentEntity = "tblCredentialApplicationAttachment";
        private const string TestSittingDocumentEntity = "tblTestSittingDocument";
        private const string NoteAttachmentEntity = "tblNoteAttachment";
        private const string PersonAttachmentEntity = "tblPersonAttachment";
        private const string WorkPracticeAttachmentEntity = "tblWorkPracticeAttachment";
        private const string MaterialRequesstRoundAttachmentEntity = "tblMaterialRequestRoundAttachment";
        private const string ProfessionalDevelopmentActivityAttachmentEntity = "tblProfessionalDevelopmentActivityAttachment";
        private const string PersonImageEntity = "tblPersonImage";
        private const string TestMaterialAttachmentEntity = "tblTestMaterialAttachment";
        private const string TestSpecificationAttachmentEntity = "tblTestSpecificationAttachment";

        public FileDeletionRunner(int currentUserId, IFileDeletionDalService fileDeletionDalService, IBackgroundTaskLogger taskLogger, ISystemQueryService systemQueryService)
        {
            _fileDeletionDalService = fileDeletionDalService;
            _systemQueryService = systemQueryService;

            BatchSize = Convert.ToInt32(GetSystemValue("ProcessFileDeletesPastExpiryDateBatchSize"));

            IncludeQueued = Convert.ToInt32(GetSystemValue("ProcessFileDeletesPastExpiryDateIncludePreviouslyQueued"));

            StoragePath = GetSystemValue("ProcessFileDeletesPastExpiryDateStoredFileBasePath");

            CurrentUserId = currentUserId;
            TaskLogger = taskLogger;
        }

        /// <summary>
        /// Retrieves a list of all the File Deletion Generators used for file deletion based off of 
        /// a policy execution order.
        /// </summary>
        /// <returns>List of BaseFileDeletionGenerators</returns>
        public List<BaseFileDeletionGenerator> GetFileDeletionGenerators()
        {
            var fileDeletionPoliciesResponse = _fileDeletionDalService.GetFileDeletionGeneratorPolicies();

            // Better way to do this than using object?
            var fileDeletionGenerators = new List<BaseFileDeletionGenerator>();

            foreach (var fileDeletionPolicy in fileDeletionPoliciesResponse.Data)
            {
                try
                {
                    switch (fileDeletionPolicy.EntityType)
                    {
                        case CredentialApplicationAttachmentEntity:
                            fileDeletionGenerators.Add(new CredentialApplicationAttachmentFileDeletionGenerator(this, fileDeletionPolicy.Name, fileDeletionPolicy.EntityType, BatchSize, IncludeQueued, fileDeletionPolicy.DocumentTypeId.HasValue ? fileDeletionPolicy.DocumentTypeId.Value : 0));
                            continue;
                        case TestSittingDocumentEntity:
                            fileDeletionGenerators.Add(new TestSittingDocumentFileDeletionGenerator(this, fileDeletionPolicy.Name, fileDeletionPolicy.EntityType, BatchSize, IncludeQueued, fileDeletionPolicy.DocumentTypeId.HasValue ? fileDeletionPolicy.DocumentTypeId.Value : 0));
                            continue;
                        case NoteAttachmentEntity:
                            fileDeletionGenerators.Add(new NoteAttachmentFileDeletionGenerator(this, fileDeletionPolicy.Name, fileDeletionPolicy.EntityType, BatchSize, IncludeQueued, fileDeletionPolicy.DocumentTypeId.HasValue ? fileDeletionPolicy.DocumentTypeId.Value : 0));
                            continue;
                        case PersonAttachmentEntity:
                            fileDeletionGenerators.Add(new PersonAttachmentFileDeletionGenerator(this, fileDeletionPolicy.Name, fileDeletionPolicy.EntityType, BatchSize, IncludeQueued, fileDeletionPolicy.DocumentTypeId.HasValue ? fileDeletionPolicy.DocumentTypeId.Value : 0));
                            continue;
                        case WorkPracticeAttachmentEntity:
                            fileDeletionGenerators.Add(new WorkPracticeAttachmentFileDeletionGenerator(this, fileDeletionPolicy.Name, fileDeletionPolicy.EntityType, BatchSize, IncludeQueued, fileDeletionPolicy.DocumentTypeId.HasValue ? fileDeletionPolicy.DocumentTypeId.Value : 0));
                            continue;
                        case MaterialRequesstRoundAttachmentEntity:
                            fileDeletionGenerators.Add(new MaterialRequestRoundAttachmentFileDeletionGenerator(this, fileDeletionPolicy.Name, fileDeletionPolicy.EntityType, BatchSize, IncludeQueued, fileDeletionPolicy.DocumentTypeId.HasValue ? fileDeletionPolicy.DocumentTypeId.Value : 0));
                            continue;
                        case ProfessionalDevelopmentActivityAttachmentEntity:
                            fileDeletionGenerators.Add(new ProfessionalDevelopmentActivityAttachmentFileDeletionGenerator(this, fileDeletionPolicy.Name, fileDeletionPolicy.EntityType, BatchSize, IncludeQueued, fileDeletionPolicy.DocumentTypeId.HasValue ? fileDeletionPolicy.DocumentTypeId.Value : 0));
                            continue;
                        case PersonImageEntity:
                            fileDeletionGenerators.Add(new PersonImageFileDeletionGenerator(this, fileDeletionPolicy.Name, fileDeletionPolicy.EntityType, BatchSize, IncludeQueued, fileDeletionPolicy.DocumentTypeId.HasValue ? fileDeletionPolicy.DocumentTypeId.Value : 0));
                            continue;
                        case TestMaterialAttachmentEntity:
                            // we are not currently deleting this entity type
                            //fileDeletionGenerators.Add(new TestMaterialAttachmentFileDeletionGenerator(this, fileDeletionPolicy.Name, fileDeletionPolicy.EntityType, BatchSize, IncludeQueued));
                            continue;
                        case TestSpecificationAttachmentEntity:
                            // we are not currently deleting this entity type
                            //fileDeletionGenerators.Add(new TestSpecificationAttachmentEntityFileDeletionGenerator(this, fileDeletionPolicy.Name, fileDeletionPolicy.EntityType, BatchSize, IncludeQueued));
                            continue;
                        default:
                            // policy 4 is dedicated for person images || this policy does not come with an entity type from the db so it is hardcoded
                            // policy 8 is dedicated for Marked test assets || this policy does not return with an entity type from the db so it is hardcoded
                            if (fileDeletionPolicy.StoredFileDeletePolicyId == 4)
                            {
                                fileDeletionGenerators.Add(new PersonImageFileDeletionGenerator(this, "PersonImage", PersonImageEntity, BatchSize, IncludeQueued, fileDeletionPolicy.DocumentTypeId.HasValue ? fileDeletionPolicy.DocumentTypeId.Value : 0));
                                continue;
                            }
                            if (fileDeletionPolicy.StoredFileDeletePolicyId == 8)
                            {
                                fileDeletionGenerators.Add(new TestSittingDocumentFileDeletionGenerator(this, "MarkedTestAsset", TestSittingDocumentEntity, BatchSize, IncludeQueued, fileDeletionPolicy.DocumentTypeId.HasValue ? fileDeletionPolicy.DocumentTypeId.Value : 0));
                                continue;
                            }

                            ThrowFileDeletionException(new List<string>() { $@"The policy with entity {fileDeletionPolicy.EntityType}, name {fileDeletionPolicy.Name}, and policy execution order {fileDeletionPolicy.PolicyExecutionOrder}, could not be created into a file deletion generator." });
                            continue;
                    }
                }
                catch (FileDeletionException fileDeletionException)
                {
                    TaskLogger.WriteError(fileDeletionException, "FileDeletionException", "FileDeletionException", true);
                    continue;
                }
                catch (Exception ex)
                {
                    TaskLogger.WriteError(ex, "Undefined", "Undefined", false);
                    continue;
                }
            }
            return fileDeletionGenerators;
        }

        /// <summary>
        /// Runs through the generators to get file deletion details
        /// </summary>
        /// <param name="fileDeletionGenerators"></param>
        /// <returns>IEnumerable of FileDeletionDetails</returns>
        public IEnumerable<FileDeletionDetails> GetFileDeletionDetails(List<BaseFileDeletionGenerator> fileDeletionGenerators)
        {
            var fileDeletionDetailsList = new List<FileDeletionDetails>();
            var currentFileDeletionDetails = new FileDeletionDetails();
            var fileCount = 0;
            var currentStatusCount = 0;
            var queuedStatusCount = 0;

            foreach (var generator in fileDeletionGenerators)
            {
                // if fileCount has reached the maximum batch size then stop collecting files to delete
                if (fileCount >= BatchSize)
                {
                    break;
                }

                // if generator is a person image generator then get person images not stored files for the list of ids
                if (generator.GetType().Name == nameof(PersonImageFileDeletionGenerator))
                {
                    currentFileDeletionDetails = generator.GetPersonImagesForDeletion(fileCount);
                    if (currentFileDeletionDetails.IdsToDelete.Count == 0)
                    {
                        continue;
                    }
                    fileDeletionDetailsList.Add(currentFileDeletionDetails);
                }
                else
                {
                    currentFileDeletionDetails = generator.GetExpiredStoredFileDetails(fileCount);

                    if (currentFileDeletionDetails.IdsToDelete.Count == 0)
                    {
                        continue;
                    }

                    fileDeletionDetailsList.Add(currentFileDeletionDetails);
                }

                if (fileDeletionDetailsList.Any())
                {
                    var latestFileCount = fileDeletionDetailsList.Last().IdsToDelete.Count;

                    fileCount += latestFileCount;
                }
            }

            foreach(var fileDeletionDetails in fileDeletionDetailsList)
            {
                currentStatusCount += fileDeletionDetails.CurrentStatusCount;
                queuedStatusCount += fileDeletionDetails.QueuedStatusCount;
            }

            var statusCountMessage = new StringBuilder();
            
            statusCountMessage.Append($"Files selected for deletion with status type 1 (Current): {currentStatusCount} | ");
            statusCountMessage.Append($"Files selected for deletion with status type 2 (Queued): {queuedStatusCount} | ");
            statusCountMessage.Append($"Total number of files selected for deletion: {currentStatusCount + queuedStatusCount}");

            TaskLogger.WriteInfo(statusCountMessage.ToString());

            return fileDeletionDetailsList;
        }

        /// <summary>
        /// For testing purposes. Creates stored files in storage. Configurable in System Values
        /// </summary>
        /// <param name="fileDeletionDetails"></param>
        public void CreateStoredFiles(IEnumerable<FileDeletionDetails> fileDeletionDetailsList)
        {
            // !-- FOR TESTING PURPOSES --!
            // Create file if it does not exist
            foreach(var fileDeletionDetails in fileDeletionDetailsList)
            {
                // Person images are not required to be created. They are different to stored files
                if (fileDeletionDetails.EntityType == PersonImageEntity)
                {
                    continue;
                }

                foreach (var storedFileId in fileDeletionDetails.IdsToDelete)
                {
                    var storedFileResponse = new GenericResponse<StoredFile>();
                    try
                    {
                        storedFileResponse = _fileDeletionDalService.GetStoredFileById(storedFileId);

                        if (storedFileResponse.Success)
                        {
                            CreateStoredFile(storedFileResponse.Data);
                            continue;
                        }

                        ThrowFileDeletionException(storedFileResponse.Errors);
                    }

                    catch (FileDeletionException fileDeletionException)
                    {
                        LoggingHelper.LogError($@"File {storedFileResponse.Data.FileName} of type {storedFileResponse.Data.DocumentType} failed to be created at {StoragePath}{storedFileResponse.Data.ExternalFileId}: {fileDeletionException.Message}.");
                        continue;
                    }
                    catch (Exception ex)
                    {
                        LoggingHelper.LogError($@"File {storedFileResponse.Data.FileName} of type {storedFileResponse.Data.DocumentType} failed to be created at {StoragePath}{storedFileResponse.Data.ExternalFileId}: {ex.Message}.");
                        continue;
                    }
                }
            }
        }

        /// <summary>
        /// Marks files as queued and moves them to the recycle bin in cloud storage and then marks them as soft deleted.
        /// </summary>
        /// <param name="fileDeletionDetails">FileDeletionDetails</param>
        public void ProcessSoftDeletes(IEnumerable<FileDeletionDetails> fileDeletionDetailsList, DateTime startTime)
        {
            var deletionReportInformation = new FileDeletionProgressInfo()
            {
                SuccessfulIdCount = 0,
                FailedIdCount = 0,
                BatchJobStartTime = startTime
            };

            foreach (var fileDeletionDetails in fileDeletionDetailsList)
            {
                // person images are not soft deleted, they are hard deleted without being moved to recycle bin
                if (fileDeletionDetails.EntityType == PersonImageEntity)
                {
                    // Add the current file deletion details to the list of file deletion details
                    deletionReportInformation.FileDeletionDetails.Add(fileDeletionDetails);
                    deletionReportInformation = ProcessPersonImageDeletion(deletionReportInformation);
                    if (deletionReportInformation.TimeExceeded)
                    {
                        break;
                    }
                    continue;
                }

                // Add the current file deletion details to the list of file deletion details
                deletionReportInformation.FileDeletionDetails.Add(fileDeletionDetails);
                deletionReportInformation = ProcessStoredFileDeletion(deletionReportInformation);
                if (deletionReportInformation.TimeExceeded)
                {
                    break;
                }
                continue;
            }

            TaskLogger.WriteInfo($"File Deletion Summary:" + " | " + $"Processed Successfully: {deletionReportInformation.SuccessfulIdCount}" + " | " + $"Failed To Process: {deletionReportInformation.FailedIdCount}");
        }

        /// <summary>
        /// A chomper. Deletes all files in a dated folder that is a configurable amount of days less than todays date.
        /// </summary>
        /// <param name="retensionDays">System value for the amount of retension days for a dated file to be deleted.</param>
        public void DeleteFilesInRecycleBin(int retensionDays)
        {
            // get all directories in recycle bin
            TaskLogger.WriteInfo("Hard Delete - Begin");
            TaskLogger.WriteInfo("Hard Delete - GetDirectories");
            var recycleBinSubDirectories = Directory.GetDirectories($@"{StoragePath}RecycleBin\", "*", SearchOption.TopDirectoryOnly);
            var datesToDelete = new List<string>();
            TaskLogger.WriteInfo($"Hard Delete - Directory Count {recycleBinSubDirectories.Length}");

            foreach (var directory in recycleBinSubDirectories)
            {
                try
                {
                    var subDirectoryInfo = new DirectoryInfo(directory);

                    // try convert directory to date format, if it is not in date format then bomb out of this one and continue with the next
                    var date = DateTime.ParseExact(subDirectoryInfo.Name, "yyyyMMdd",
                        CultureInfo.InvariantCulture);

                    // if the subdirectory date is less than or equal to todays date minus retention days then add it to be deleted
                    if (date <= DateTime.Now.AddDays(-retensionDays))
                    {
                        datesToDelete.Add(date.ToString("yyyyMMdd"));
                        TaskLogger.WriteInfo($"Hard Delete - Folder {date.ToString("yyyyMMdd")} queued");
                    }
                    continue;
                }
                catch(FormatException ex)
                {
                    TaskLogger.WriteError(ex, "FormatException", "FormatException", false);
                    continue;
                }
                catch (Exception ex)
                {
                    TaskLogger.WriteError(ex, "Undefined", "Undefined", false);
                    continue;
                }

            }

            // if there are no dates to delete then no action needed
            if (datesToDelete.Count == 0)
            {
                TaskLogger.WriteInfo($"There are no folders to delete in the recycle bin.");
                return;
            }

            // for each date to delete, delete the folder
            foreach(var dateToDelete in datesToDelete)
            {
                TaskLogger.WriteInfo($"Hard Delete - Process {dateToDelete}");
                TaskLogger.WriteInfo($"Hard Delete - Delete Directory {dateToDelete}");
                Directory.Delete($@"{StoragePath}RecycleBin\{dateToDelete}",true);
            }
        }

        public void LogPreviousWeeksFileDeletionCounts()
        {
            try
            {
                // get the count of files deleted for each day for the last 7 days including today
                var filesDeletedInLast7DaysResponse = _fileDeletionDalService.GetCountOfFilesDeletedEachDayLast7Days();
                // set total to 0. This will be added up to get the total over all 7 days
                var totalCountOfFilesDeleted = 0;
                // reverse the list to log the most recent day first
                foreach(var dateAndCount in filesDeletedInLast7DaysResponse.Data.Reverse())
                {
                    TaskLogger.WriteInfo($"{dateAndCount.Value} files were processed on {dateAndCount.Key}");
                    // add to total count
                    totalCountOfFilesDeleted += dateAndCount.Value;
                }
                // log total files processed for the whole 7 days
                TaskLogger.WriteInfo($"{totalCountOfFilesDeleted} files processed in the last 7 days.");
            }
            catch(FileDeletionException ex)
            {
                TaskLogger.WriteError(ex, "FileDeletionException", "FileDeletionException", true);
                return;
            }
            catch(Exception ex)
            {
                TaskLogger.WriteError(ex, "Undefined", "Undefined", false);
                return;
            }
            
        }

        /// <summary>
        /// Soft deletes stored files
        /// </summary>
        /// <param name="deletionReportInformation"></param>
        /// <returns>Updated file deletion progress information</returns>
        private FileDeletionProgressInfo ProcessStoredFileDeletion(FileDeletionProgressInfo deletionReportInformation)
        {
            // the latest file deletion details will be the current one being iterated
            var currentFileDeletionDetails = deletionReportInformation.FileDeletionDetails.Last();
            // create a list of ids for a hearbeat report, this will be cleared after 100 ids have been processed
            var heartbeatReportIds = new List<int>();

            foreach (var storedFileId in currentFileDeletionDetails.IdsToDelete)
            {
                // check if file deletion has been running for more than 55 minutes if it has then stop processing and return
                if (DateTime.UtcNow >= deletionReportInformation.BatchJobStartTime.AddMinutes(55))
                {
                    if (heartbeatReportIds.Count > 0)
                    {
                        TaskLogger.WriteInfo($"The following {heartbeatReportIds.Count} Stored Files were processed: {string.Join(" | ", heartbeatReportIds)}");
                    }

                    TaskLogger.WriteInfo("File deletion has been running for more than 55 minutes. Ending file deletion process.");

                    // if the time has exceeded then set value and return so for each loop that calls this function knows to break
                    deletionReportInformation.TimeExceeded = true;
                    return deletionReportInformation;
                }
                // if 100 ids have been processed the log the hearbeat and clear it 
                if (heartbeatReportIds.Count >= 100)
                {
                    TaskLogger.WriteInfo($"The following {heartbeatReportIds.Count} Stored Files were processed: {string.Join(" | ", heartbeatReportIds)}");
                    // clear heartbeat ids
                    heartbeatReportIds.Clear();
                    //Introduced to try and stop irregular thread aborted messages
                    Thread.Sleep(5000);
                }
                // add to heartbeat
                heartbeatReportIds.Add(storedFileId);

                try
                {
                    // get stored file now so NHibernate knows it exists
                    var storedFileResponse = _fileDeletionDalService.GetStoredFileById(storedFileId);

                    if (storedFileResponse.Success)
                    {
                        var storedFile = storedFileResponse.Data;
                        var documentType = storedFile.DocumentType;
                        
                        var documentTypeDto = new DocumentTypeDto()
                        {
                            Id = documentType.Id,
                            Name = documentType.Name,
                            DisplayName = documentType.DisplayName,
                            ExaminerToolsDownload = documentType.ExaminerToolsDownload,
                            ExaminerToolsUpload = documentType.ExaminerToolsUpload,
                            MergeDocument = documentType.MergeDocument
                        };

                        var storedFileToDelete = new StoredFileDto()
                        {
                            Id = storedFile.Id,
                            ExternalFileId = storedFile.ExternalFileId,
                            DocumentType = documentTypeDto,
                            FileName = storedFile.FileName,
                            FileSize = storedFile.FileSize,
                            UploadedByName = storedFile.UploadedByUser == null ? "" : storedFile.UploadedByUser.FullName,
                            UploadedDateTime = storedFile.UploadedDateTime,
                            StoredFileStatusType = storedFile.StoredFileStatusType.Id,
                            StoredFileStatusChangedDate = storedFile.StoredFileStatusChangeDate
                        };
                        // change status from current to Queued to signal the stored file has been queued for deletion process
                        MarkFileAsQueuedForDelete(storedFileToDelete);
                        // move file to the recycle bin in the respective date folder so that it can be deleted in a configurable
                        // amount of days
                        MoveFileToRecycleBin(storedFileToDelete);
                        // change status of the storedfile to SoftDeleted to signal the storedfile was successfully moved
                        // into the recycle bin folder to be deleted in a configurable amount of days
                        MarkFileAsSoftDeleted(storedFileToDelete);
                        // create an entry in tblNote to show in the UI for the current stored file that was soft deleted
                        CreateNote(storedFileToDelete, currentFileDeletionDetails.EntityType);
                        // add to successfully processed id count and continue to iterate
                        deletionReportInformation.SuccessfulIdCount++;
                        continue;
                    }
                    // if we did not succeed in getting the stored file then bomb out on this id and continue;
                    ThrowFileDeletionException(storedFileResponse.Errors);
                }
                catch (FileDeletionException fileDeletionException)
                {
                    TaskLogger.WriteError(fileDeletionException, "FileDeletionException", "FileDeletionException", true);
                    // add to failed id count and continue
                    deletionReportInformation.FailedIdCount++;
                    continue;
                }
                catch (Exception ex)
                {
                    TaskLogger.WriteError(ex, "Undefined", "Undefined", true);
                    // add to failed id count and continue
                    deletionReportInformation.FailedIdCount++;
                    continue;
                }
            }
            // after the stored files are processed then log the stragglers who did not make it to the 100 count
            if (heartbeatReportIds.Count > 0)
            {
                TaskLogger.WriteInfo($"The following {heartbeatReportIds.Count} Stored Files were processed: {string.Join(" | ", heartbeatReportIds)}");
            }
            return deletionReportInformation;
        }

        /// <summary>
        /// Hard deletes person images
        /// </summary>
        /// <param name="deletionReportInfo"></param>
        /// <returns>Updated file deletion progress information</returns>
        private FileDeletionProgressInfo ProcessPersonImageDeletion(FileDeletionProgressInfo deletionReportInfo)
        {
            // the latest file deletion detail will be the current file deletion detail being iterated.
            var currentFileDeletionDetails = deletionReportInfo.FileDeletionDetails.Last();
            // create a list of ids for a hearbeat report, this will be cleared after 100 ids have been processed
            var heartbeatReportIds = new List<int>();

            foreach (var personImageId in currentFileDeletionDetails.IdsToDelete)
            {
                // after 100 ids have been processed, write out to the task logger
                if (heartbeatReportIds.Count >= 100)
                {
                    TaskLogger.WriteInfo($"The following {heartbeatReportIds.Count} PersonImages were processed: {string.Join(" | ", heartbeatReportIds)}");
                    // clear heartbeat ids
                    heartbeatReportIds.Clear();
                }
                // if the batch job has been running for over an hour then stop processing and return
                if (DateTime.UtcNow >= deletionReportInfo.BatchJobStartTime.AddHours(1))
                {
                    if (heartbeatReportIds.Count > 0)
                    {
                        TaskLogger.WriteInfo($"The following {heartbeatReportIds.Count} PersonImages were processed: {string.Join(" | ", heartbeatReportIds)}");
                    }

                    TaskLogger.WriteInfo("File deletion has been running for more than 55 minutes. Ending file deletion process.");

                    // if the time has exceeded then set value and return so for each loop that calls this function knows to break
                    deletionReportInfo.TimeExceeded = true;
                    return deletionReportInfo;
                }
                // +1 to heartbeat count
                heartbeatReportIds.Add(personImageId);
                // delete the images
                try
                {
                    _fileDeletionDalService.DeletePersonImage(personImageId, CurrentUserId);
                    // update success count
                    deletionReportInfo.SuccessfulIdCount++;
                }
                catch (Exception ex)
                {
                    TaskLogger.WriteError(ex, "Undefined", "Undefined", false);
                    // update fail count
                    deletionReportInfo.FailedIdCount++;
                    continue;
                }
            }

            return deletionReportInfo;
        }

        /// <summary>
        /// For testing purposes. Creates a single KB file in storage based on the ExternalFileId of a StoredFile.
        /// </summary>
        /// <param name="storedFileToDelete">StoredFile record in the database</param>
        private void CreateStoredFile(StoredFile storedFileToDelete)
        {
            // if stored file is not current then do not create the file
            if (storedFileToDelete.StoredFileStatusType.Id != 1)
            {
                return;
            }

            // path to possible file in storage
            var pathToFile = $@"{StoragePath}{storedFileToDelete.ExternalFileId}";

            // if the path exists then no need to create file
            if (File.Exists(pathToFile))
            {
                return;
            }

            // create a single byte
            byte[] bytes = new byte[] { 0 };

            // If the directory to the file *NOT INCLUDING FILE NAME* exists then just create the file
            if (Directory.Exists(Path.GetDirectoryName(pathToFile)))
            {
                File.WriteAllBytes(pathToFile, bytes);
                return;
            }

            // If not then create the directory and the file
            Directory.CreateDirectory(Path.GetDirectoryName(pathToFile));
            File.WriteAllBytes(pathToFile, bytes);
            return;
        }

        /// <summary>
        /// Calls the DAL to update the given StoredFile to a Queued Status
        /// </summary>
        /// <param name="storedFileToDelete">StoredFile record in the database</param>
        private void MarkFileAsQueuedForDelete(StoredFileDto storedFileToDelete)
        {
            try
            {
                var response = _fileDeletionDalService.MarkStoredFileAsQueuedForDelete(storedFileToDelete);

                if (response.Success)
                {
                    return;
                }

                ThrowFileDeletionException(response.Errors);
            }
            catch (FileDeletionException fileDeletionException)
            {
                LoggingHelper.LogError(fileDeletionException.Message);
            }
            catch (Exception ex)
            {
                LoggingHelper.LogError($"Unexpected Error occured when trying to mark stored file {storedFileToDelete.Id} as queued for delete: {ex.Message}.");
            }
        }

        /// <summary>
        /// Moves the StoredFile from storage to the recycle bin
        /// </summary>
        /// <param name="storedFileDto">StoredFile record in the database</param>
        private void MoveFileToRecycleBin(StoredFileDto storedFileDto)
        {
            var externalFileId = storedFileDto.ExternalFileId;
            // path to the recycle bin in storage with iso 8601 folder on end
            var recycleBinPath = $@"{StoragePath}RecycleBin\{DateTime.Now.ToString("yyyyMMdd")}";
            // existing file path to the file we want to move
            var existingFilePath = $@"{StoragePath}{externalFileId}";

            try
            {
                // If the recycle bin path does not exist then create it
                if (!Directory.Exists(recycleBinPath))
                {
                    Directory.CreateDirectory(recycleBinPath);
                    TaskLogger.WriteInfo($"Recycle bin directory was created at { recycleBinPath}");
                }

                // if the file we want to move does not exist then there is nothing to do, return
                if (!File.Exists(existingFilePath))
                {
                    try
                    {
                        _fileDeletionDalService.MarkFileAsSoftDeleted(storedFileDto);
                    }
                    catch (Exception ex)
                    {
                        TaskLogger.WriteError(ex, "Undefined", $"Unexpected error occured when soft deleting file to move at {existingFilePath} which didn't exist.", true);
                        return;
                    }
                    TaskLogger.WriteInfo($"File to move at {existingFilePath} did not exist. File Marked as Soft Deleted.");
                    return;
                }

                // new path we want to move the file to
                var newFilePath = recycleBinPath + @"\" + $"{externalFileId}";

                // if new directory already exists and old directory does not, then there is no need to move file
                if (File.Exists(newFilePath) && !File.Exists(existingFilePath))
                {
                    try
                    {
                        _fileDeletionDalService.MarkFileAsSoftDeleted(storedFileDto);
                    }
                    catch (Exception ex)
                    {
                        TaskLogger.WriteError(ex, "Undefined", $"Unexpected error occured when soft deleting file {existingFilePath} which already existed in recycling bin and not in storage.", true);
                        return;
                    }
                }

                // if the file exists in both locations then there is an issue.
                if (File.Exists(newFilePath) && File.Exists(existingFilePath))
                {
                    throw new FileDeletionException($"Stored File {storedFileDto.Id} exists in both storage and recycle bin.");
                }

                // If new directory does not exist then create it
                Directory.CreateDirectory(Path.GetDirectoryName(newFilePath));
                // move file to new file path
                File.Move(existingFilePath, newFilePath);
            }
            catch(DirectoryNotFoundException directoryNotFoundEx)
            {
                try
                {
                    TaskLogger.WriteInfo($"Could not move the file {externalFileId} to the recycle bin. Deleting the file from storage. Exception: {directoryNotFoundEx.Message}");
                    File.Delete(existingFilePath);

                    _fileDeletionDalService.MarkFileAsSoftDeleted(storedFileDto);
                }
                catch (Exception ex)
                {
                    TaskLogger.WriteError(ex, "Undefined", $"Unexpected error occured when soft deleting file {existingFilePath} which already existed in recycling bin and not in storage.", true);
                    return;
                }
            }
        }

        /// <summary>
        /// Calls the DAL to update the given StoredFile to a SoftDeleted Status
        /// </summary>
        /// <param name="storedFileDto">StoredFile record in the database</param>
        private void MarkFileAsSoftDeleted(StoredFileDto storedFileDto)
        {
            var externalFileId = storedFileDto.ExternalFileId;
            var recycleBinPath = $@"{StoragePath}RecycleBin\{DateTime.Now.ToString("yyyyMMdd")}";

            // if for any reason the recycle bin folder does not exist, then bomb out
            if (!Directory.Exists(recycleBinPath))
            {
                throw new FileDeletionException($"Recycle bin folder {recycleBinPath} does not exist.");
            }

            // existing file path to the file that should have moved
            var oldFilePath = $@"{StoragePath}{externalFileId}";

            // new path we want to move the file to
            var newFilePath = recycleBinPath + @"\" + $"{externalFileId}";

            // if file still exists in storage then don't update and log error
            if (File.Exists(oldFilePath))
            {
                ThrowFileDeletionException(new List<string>()
                {
                    $"The stored file at {oldFilePath} still exists in storage. Cannot update to soft deleted status."
                });
            }

            // if new file was moved then change status of storedfile to soft deleted
            if (File.Exists(newFilePath))
            {
                try
                {
                    var response = _fileDeletionDalService.MarkFileAsSoftDeleted(storedFileDto);

                    if (response.Success)
                    {
                        return;
                    }

                    ThrowFileDeletionException(response.Errors);
                }
                catch (FileDeletionException fileDeletionException)
                {
                    LoggingHelper.LogError(fileDeletionException.Message);
                }
                catch (Exception ex)
                {
                    LoggingHelper.LogError($"Unexpected Error occured when trying to mark stored file {storedFileDto.Id} as soft deleted: {ex.Message}.");
                }
            }
        }

        /// <summary>
        /// Calls the DAL layer to create a Note entity in the database depending on the passed in entity type
        /// </summary>
        /// <param name="storedFileDto"></param>
        /// <param name="entityType"></param>
        private void CreateNote(StoredFileDto storedFileDto, string entityType)
        {
            // Create the required note based off of entity type
            switch (entityType)
            {
                case CredentialApplicationAttachmentEntity:
                    var credentialAttachmentResponse = _fileDeletionDalService.GetCredentialApplicationAttachmentByStoredFileId(storedFileDto);
                    
                    if (credentialAttachmentResponse.Success)
                    {
                        _fileDeletionDalService.CreateCredentialApplicationNote(storedFileDto, credentialAttachmentResponse.Data, CurrentUserId);
                        break;
                    }

                    ThrowFileDeletionException(credentialAttachmentResponse.Errors);
                    break;

                case TestSittingDocumentEntity:
                    var testSittingDocumentResponse = _fileDeletionDalService.GetTestSittingDocumentByStoredFileId(storedFileDto);

                    if (testSittingDocumentResponse.Success)
                    {
                        _fileDeletionDalService.CreateTestSittingNote(storedFileDto, testSittingDocumentResponse.Data, CurrentUserId);
                        break;
                    }
                    break;
                case NoteAttachmentEntity:
                    _fileDeletionDalService.CreateNote(storedFileDto, CurrentUserId);
                    break;
                case PersonAttachmentEntity:
                    var personAttachmentResponse = _fileDeletionDalService.GetPersonAttachmentByStoredFileId(storedFileDto);
                    if (personAttachmentResponse.Success)
                    {
                        _fileDeletionDalService.CreateEntityNote(storedFileDto, personAttachmentResponse.Data, CurrentUserId);
                        break;
                    }
                    ThrowFileDeletionException(personAttachmentResponse.Errors);
                    break;
                case MaterialRequesstRoundAttachmentEntity:
                    var materialRequesstRoundAttachment = _fileDeletionDalService.GetMaterialRequestRoundAttachmentByStoredFileId(storedFileDto);
                    if (materialRequesstRoundAttachment.Success)
                    {
                        _fileDeletionDalService.CreateMaterialRequestNote(storedFileDto, materialRequesstRoundAttachment.Data, CurrentUserId);
                        break;
                    }
                    ThrowFileDeletionException(materialRequesstRoundAttachment.Errors);
                    break;
                case WorkPracticeAttachmentEntity:
                    var workPracticeAttachmentResponse = _fileDeletionDalService.GetWorkPracticeAttachmentByStoredFileId(storedFileDto);
                    if (workPracticeAttachmentResponse.Success)
                    {
                        _fileDeletionDalService.CreateEntityNote(storedFileDto, workPracticeAttachmentResponse.Data, CurrentUserId);
                        break;
                    }
                    ThrowFileDeletionException(workPracticeAttachmentResponse.Errors);
                    break;
                case ProfessionalDevelopmentActivityAttachmentEntity:
                    var professionalDevelopmentActivityAttachmentResponse = _fileDeletionDalService.GetProfessionalDevelopmentActivityAttachmentByStoredFileId(storedFileDto);
                    if (professionalDevelopmentActivityAttachmentResponse.Success)
                    {
                        _fileDeletionDalService.CreateEntityNote(storedFileDto, professionalDevelopmentActivityAttachmentResponse.Data, CurrentUserId);
                        break;
                    }
                    ThrowFileDeletionException(professionalDevelopmentActivityAttachmentResponse.Errors);
                    break;
                default:
                    break;
            }  
        }

        /// <summary>
        /// Builds an error string and throws a custom file deletion exception
        /// </summary>
        /// <param name="errors"></param>
        private void ThrowFileDeletionException(IEnumerable<string> errors)
        {
            var errorMessage = new StringBuilder();

            foreach (var error in errors)
            {
                errorMessage.Append(error);
            }

            throw new FileDeletionException(errorMessage.ToString());
        }

        private string GetSystemValue(string valueKey)
        {
            return _systemQueryService.GetSystemValue(new GetSystemValueRequest() { ValueKey = valueKey, ForceRefresh = true }).Value;
        }
    }
}

