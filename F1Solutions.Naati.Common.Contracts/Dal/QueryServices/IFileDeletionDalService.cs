using F1Solutions.Naati.Common.Contracts.Bl.DTO;
using F1Solutions.Naati.Common.Contracts.Bl.FileDeletion;
using F1Solutions.Naati.Common.Contracts.Dal.DTO;
using F1Solutions.Naati.Common.Contracts.Dal.FileDeletion;
using F1Solutions.Naati.Common.Dal.Domain;
using System.Collections.Generic;

namespace F1Solutions.Naati.Common.Contracts.Dal.QueryServices
{
    public interface IFileDeletionDalService
    {
        /// <summary>
        /// Querys the database and gets a list of stored files based on document type and the entity. Will only allow the collection of files to the size of a given
        /// batch size.
        /// </summary>
        /// <param name="documentType">Document Type to be queried</param>
        /// <param name="batchSize">Size of the maximum amount of files allowed to be deleted in this iteration</param>
        /// <param name="fileCount">Amount of files already selected for the batch</param>
        /// <returns>IEnumerable of type StoredFile</returns>
        IEnumerable<int> GetExpiredStoredFiles(string documentType, string entity, int batchSize, int fileCount, int includeQueued, int documentTypeId);
        /// <summary>
        /// Updates the given Stored Files Status Type to Queued
        /// </summary>
        /// <param name="storedFileId">Id of the Stored File to update</param>
        GenericResponse<StoredFile> MarkStoredFileAsQueuedForDelete(StoredFileDto storedFileDto);
        /// <summary>
        /// Updates the given Stored Files Status Type to SoftDeleted
        /// </summary>
        /// <param name="storedFileId">Id of the Stored File to update</param>
        GenericResponse<StoredFile> MarkFileAsSoftDeleted(StoredFileDto storedFileDto);
        /// <summary>
        /// Gets a StoredFile Entity from database using a StoredFileId
        /// </summary>
        /// <returns>StoredFile Entity from database</returns>
        /// <param name="storedFileId">ID</param>
        GenericResponse<StoredFile> GetStoredFileById(int storedFileId);
        /// <summary>
        /// Gets the Credential Application Attachment by a stored file id
        /// </summary>
        /// <param name="storedFileDto"></param>
        /// <returns>Generic Response of CredentialApplicationAttachment</returns>
        GenericResponse<CredentialApplicationAttachment> GetCredentialApplicationAttachmentByStoredFileId(StoredFileDto storedFileDto);
        /// <summary>
        /// Gets the Test Sitting Document by a stored file id
        /// </summary>
        /// <param name="storedFileDto"></param>
        /// <returns>Generic Response of TestSittingDocument</returns>
        GenericResponse<TestSittingDocument> GetTestSittingDocumentByStoredFileId(StoredFileDto storedFileDto);
        /// <summary>
        /// Gets the Person Attachment by the stored file id
        /// </summary>
        /// <param name="storedFileDto"></param>
        /// <returns>Generic Response of PersonAttachment</returns>
        GenericResponse<PersonAttachment> GetPersonAttachmentByStoredFileId(StoredFileDto storedFileDto);
        /// <summary>
        /// Gets the Material Request Round Attachment by the stored file id
        /// </summary>
        /// <param name="storedFileDto"></param>
        /// <returns>Generic Response of MaterialRequestRoundAttachment</returns>
        GenericResponse<MaterialRequestRoundAttachment> GetMaterialRequestRoundAttachmentByStoredFileId(StoredFileDto storedFileDto);
        /// <summary>
        /// Gets the Work Practice Attachment by the stored file id
        /// </summary>
        /// <param name="storedFileDto"></param>
        /// <returns>Generic Response of WorkPracticeAttachment</returns>
        GenericResponse<WorkPracticeAttachment> GetWorkPracticeAttachmentByStoredFileId(StoredFileDto storedFileDto);
        /// <summary>
        /// Gets the Professional Development Activity Attachment by the stored file id
        /// </summary>
        /// <param name="storedFileDto"></param>
        /// <returns>Generic Response of ProfessionalDevelopmentActivityAttachment</returns>
        GenericResponse<ProfessionalDevelopmentActivityAttachment> GetProfessionalDevelopmentActivityAttachmentByStoredFileId(StoredFileDto storedFileDto);
        /// <summary>
        /// Creates a Credential Application Note from a Credential Application Attachment that says the Stored File has been deleted.
        /// </summary>
        /// <param name="storedFileDto"></param>
        /// <param name="credentialApplicationAttachment"></param>
        void CreateCredentialApplicationNote(StoredFileDto storedFileDto, CredentialApplicationAttachment credentialApplicationAttachment, int currentUserId);
        /// <summary>
        /// Creates a Test Sitting Note from a Test Sitting Document that says the Stored File has been deleted.
        /// </summary>
        /// <param name="storedFileDto"></param>
        /// <param name="testSittingDocument"></param>
        void CreateTestSittingNote(StoredFileDto storedFileDto, TestSittingDocument testSittingDocument, int currentUserId);
        /// <summary>
        /// Creates an Entity Note from a Person Attachment that says the Stored File has been deleted.
        /// </summary>
        /// <param name="storedFileDto"></param>
        /// <param name="personAttachment"></param>
        void CreateEntityNote(StoredFileDto storedFileDto, PersonAttachment personAttachment, int currentUserId);
        /// <summary>
        /// Creates an Entity Note from a Work Practice Attachment that says the Stored File has been deleted.
        /// </summary>
        /// <param name="storedFileDto"></param>
        /// <param name="workPracticeAttachment"></param>
        void CreateEntityNote(StoredFileDto storedFileDto, WorkPracticeAttachment workPracticeAttachment, int currentUserId);
        /// <summary>
        /// Creates an Entity Note from a Professional Development Activity Attachment that says the Stored File has been deleted.
        /// </summary>
        /// <param name="storedFileDto"></param>
        /// <param name="professionalDevelopmentActivityAttachment"></param>
        void CreateEntityNote(StoredFileDto storedFileDto, ProfessionalDevelopmentActivityAttachment professionalDevelopmentActivityAttachment, int currentUserId);
        /// <summary>
        /// Creates a Material Request Note from a Material Request Round Attachment that says the Stored File has been deleted.
        /// </summary>
        /// <param name="storedFileDto"></param>
        /// <param name="materialRequestRoundAttachment"></param>
        void CreateMaterialRequestNote(StoredFileDto storedFileDto, MaterialRequestRoundAttachment materialRequestRoundAttachment, int currentUserId);
        /// <summary>
        /// Creates a Note Entity in the database
        /// </summary>
        /// <param name="storedFileDto"></param>
        /// <returns>Note Entity</returns>
        Note CreateNote(StoredFileDto storedFileDto, int currentUserId);
        /// <summary>
        /// Get a list of person image ids beyond a days to keep value and if the person is not a practitioner
        /// </summary>
        /// <param name="batchSize"></param>
        /// <param name="fileCount"></param>
        /// <returns>List of Person Image ids</returns>
        List<int> GetPersonImagesToDelete(int batchSize, int fileCount);
        /// <summary>
        /// Deletes the person image given a person image id and calls an internal function to create an entity note
        /// </summary>
        /// <param name="personImageId"></param>
        /// <param name="currentUserId"
        void DeletePersonImage(int personImageId, int currentUserId);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="fileDeletionDetails"></param>
        /// <returns></returns>
        FileDeletionDetails GetStatusCount(FileDeletionDetails fileDeletionDetails);
        /// <summary>
        /// Runs the stored procedure GetRemainingStoredFilesForDeletionCount to get the remaining stored files to be deleted
        /// </summary>
        /// <returns>Count (int) of stored files left to be deleted</returns>
        GenericResponse<FileDeletionEstimationDetails> GetTotalStoredFilesLeftToDelete();
        /// <summary>
        /// Gets a dictionary with a date as a key and count of files processed on that date as an int for the last week.
        /// </summary>
        /// <returns>GenericResponse with a dictionary of the count of processed files for each day in the last week</returns>
        GenericResponse<IDictionary<string, int>> GetCountOfFilesDeletedEachDayLast7Days();
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        GenericResponse<List<FileDeletionGeneratorPolicy>> GetFileDeletionGeneratorPolicies();
    }
}