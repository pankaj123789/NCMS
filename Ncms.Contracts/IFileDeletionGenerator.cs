using F1Solutions.Naati.Common.Contracts.Bl.FileDeletion;

namespace Ncms.Contracts
{
    public interface IFileDeletionGenerator
    {
        /// <summary>
        /// Gets expired stored files ids based off policy
        /// </summary>
        /// <param name="fileCount">Amount of files selected so far</param>
        /// <returns>IEnumerable of StoredFileIds</returns>
        FileDeletionDetails GetExpiredStoredFileDetails(int fileCount);
        FileDeletionDetails GetPersonImagesForDeletion(int fileCount);
    }
}
