using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace F1Solutions.Naati.Common.Contracts.Dal.DTO
{
    public class PodHistoryModel
    {
        public int PodHistoryId { get; set; }
        public string PodName { get; set; }
        public DateTime StartedDate { get; set; }
        public DateTime? TerminationDate { get; set; }
        public string FolderPath { get; set; }
        public string DeletionError { get; set; }
    }
}
