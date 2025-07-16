using System;
using System.Collections.Generic;

namespace F1Solutions.Naati.Common.Contracts.Bl.FileDeletion
{
    public class FileDeletionDetails
    {
        public FileDeletionDetails()
        {
            IdsToDelete = new List<int>();
        }

        public List<int> IdsToDelete { get; set; }
        public int CurrentStatusCount { get; set; }
        public int QueuedStatusCount { get; set; }
        public string EntityType { get; set; }
        public string DocumentType { get; set; }
        public int DocumentTypeId { get; set; }
    }

    public class FileDeletionProgressInfo
    {
        public FileDeletionProgressInfo()
        {
            FileDeletionDetails = new List<FileDeletionDetails>();
        }

        public List<FileDeletionDetails> FileDeletionDetails { get; set; }
        public DateTime BatchJobStartTime { get; set; }
        public int SuccessfulIdCount { get; set; }
        public int FailedIdCount { get; set; }
        public bool TimeExceeded { get; set; } = false;
    }
}
