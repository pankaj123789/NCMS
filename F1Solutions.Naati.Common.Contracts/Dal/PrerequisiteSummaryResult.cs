using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace F1Solutions.Naati.Common.Contracts.Dal
{
    public class PrerequisiteSummaryResult
    {
        public List<PrerequisiteSummaryModel> PrerequisiteSummaryModels { get; set; }

        public PrerequisiteSummaryResult()
        {
            PrerequisiteSummaryModels = new List<PrerequisiteSummaryModel>();
        }
    }
}
