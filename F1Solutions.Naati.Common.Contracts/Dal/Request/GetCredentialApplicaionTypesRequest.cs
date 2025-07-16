using System.Collections.Generic;

namespace F1Solutions.Naati.Common.Contracts.Dal.Request
{
    public class GetCredentialApplicaionTypesRequest
    {
        public IEnumerable<int> SkillTypeIds { get; set; }
    }
}