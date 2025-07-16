using F1Solutions.Naati.Common.Contracts.Bl.FileDeletion;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ncms.Bl.FileDeletion.FileDeletionGenerators
{
    public class PersonImageFileDeletionGenerator : BaseFileDeletionGenerator
    {
        public PersonImageFileDeletionGenerator(FileDeletionRunner runner, string documentType, string entityType, int batchSize, int includeQueued, int documentTypeId)
            : base(runner, documentType, entityType, batchSize, includeQueued, documentTypeId)
        {
        }

        public override FileDeletionDetails GetPersonImagesForDeletion(int fileCount)
        {
            var fileDeletionDetails = base.GetPersonImagesForDeletion(fileCount);

            return fileDeletionDetails;
        }

    }
}
