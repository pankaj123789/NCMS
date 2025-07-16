using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace F1Solutions.Naati.Common.Contracts.Bl.FileDeletion
{
    public class FileDeletionGeneratorPolicy
    {
        public int PolicyExecutionOrder { get; set; }
        public int StoredFileDeletePolicyId { get; set; }
        public string EntityType { get; set; }
        public string Name { get; set; }
        public int? DocumentTypeId { get; set; }
        public string PolicyDescription { get; set; }
        public int DaysToKeep { get; set; }
    }
}
