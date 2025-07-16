using F1Solutions.Naati.Common.Contracts.Bl.FileDeletion;

namespace Ncms.Bl.FileDeletion.FileDeletionGenerators
{
    class MaterialRequestRoundAttachmentFileDeletionGenerator : BaseFileDeletionGenerator
	{
		public MaterialRequestRoundAttachmentFileDeletionGenerator(FileDeletionRunner runner, string documentType, string entityType, int batchSize, int includeQueued, int documentTypeId)
			: base(runner, documentType, entityType, batchSize, includeQueued, documentTypeId)
		{
		}

		public override FileDeletionDetails GetExpiredStoredFileDetails(int fileCount)
		{
			var fileDeletionDetails = base.GetExpiredStoredFileDetails(fileCount);

			return fileDeletionDetails;
		}
	}
}
