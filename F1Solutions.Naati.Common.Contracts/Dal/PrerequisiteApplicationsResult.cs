using System.Collections.Generic;

namespace F1Solutions.Naati.Common.Contracts.Dal
{
    public class PrerequisiteApplicationsResult
    {
        public List<PrerequisiteApplicationsDalModel> PrerequisiteApplicationsModels { get; set; }

        public PrerequisiteApplicationsResult()
        {
            PrerequisiteApplicationsModels = new List<PrerequisiteApplicationsDalModel>();
        }
    }
}
