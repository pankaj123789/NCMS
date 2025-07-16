using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace F1Solutions.Naati.Common.Contracts.Dal
{
    public class PrerequisiteSummaryModel
    {
        public string PreRequisiteCredential { get; set; }
        public string MatchingCredential { get; set; }
        public bool Match { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int? CertificationPeriodId { get; set; }
    }
}
