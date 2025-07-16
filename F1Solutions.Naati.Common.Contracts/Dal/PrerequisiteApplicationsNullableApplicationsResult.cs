using F1Solutions.Naati.Common.Contracts.Dal.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace F1Solutions.Naati.Common.Contracts.Dal
{
    public class PrerequisiteApplicationsNullableApplicationsResult
    {
        public List<PrerequisiteApplicationsNullableApplicationsDto> PrerequisiteApplicationsNullableApplicationsDtos { get; set; }

        public PrerequisiteApplicationsNullableApplicationsResult()
        {
            PrerequisiteApplicationsNullableApplicationsDtos = new List<PrerequisiteApplicationsNullableApplicationsDto>();
        }
    }
}

