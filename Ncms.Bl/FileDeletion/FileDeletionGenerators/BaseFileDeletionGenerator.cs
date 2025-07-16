using F1Solutions.Naati.Common.Contracts.Bl.FileDeletion;
using F1Solutions.Naati.Common.Contracts.Dal.QueryServices;
using F1Solutions.Naati.Common.Dal.FileDeletion;
using Ncms.Contracts;
using System.Collections.Generic;

namespace Ncms.Bl.FileDeletion.FileDeletionGenerators
{
    public abstract class BaseFileDeletionGenerator : IFileDeletionGenerator
    {
        protected FileDeletionRunner DeletionRunner { get; }
        protected int BatchSize { get; }
        protected int IncludeQueued { get; }
        protected string DocumentTypeToDelete { get; }
        protected int DocumentTypeToDeleteId { get; }
        protected string EntityTypeToDelete { get; }
        protected IFileDeletionDalService FileDeletionDalService { get; set; }

        public BaseFileDeletionGenerator(FileDeletionRunner runner, string documentTypeToDelete, string entityType, int batchSize, int includeQueued, int documentTypeId)
        {
            DeletionRunner = runner;
            DocumentTypeToDelete = documentTypeToDelete;
            EntityTypeToDelete = entityType;
            BatchSize = batchSize;
            IncludeQueued = includeQueued;
            DocumentTypeToDeleteId = documentTypeId;
            FileDeletionDalService = new FileDeletionDalService();
        }

        public virtual FileDeletionDetails GetExpiredStoredFileDetails(int fileCount)
        {
            var fileDeletionDetails = new FileDeletionDetails()
            {
                IdsToDelete = (List<int>)FileDeletionDalService.GetExpiredStoredFiles(DocumentTypeToDelete, EntityTypeToDelete, BatchSize, fileCount, IncludeQueued, DocumentTypeToDeleteId),
                EntityType = EntityTypeToDelete,
                DocumentType = DocumentTypeToDelete
            };

            FileDeletionDalService.GetStatusCount(fileDeletionDetails);

            return fileDeletionDetails;
        }

        // Person Images are not stored files so are treated differently
        public virtual FileDeletionDetails GetPersonImagesForDeletion(int fileCount)
        {
            var fileDeletionDetails = new FileDeletionDetails()
            {
                // Actually are person image ids
                IdsToDelete = FileDeletionDalService.GetPersonImagesToDelete(BatchSize, fileCount),
                EntityType = EntityTypeToDelete,
                DocumentType = DocumentTypeToDelete
            };

            fileDeletionDetails.CurrentStatusCount = fileDeletionDetails.IdsToDelete.Count;

            return fileDeletionDetails;
        }
    }
}
