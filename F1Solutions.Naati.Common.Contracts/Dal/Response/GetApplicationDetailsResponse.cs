using System.Collections.Generic;
using F1Solutions.Naati.Common.Contracts.Dal.DTO;

namespace F1Solutions.Naati.Common.Contracts.Dal.Response
{
    public class GetApplicationDetailsResponse : ServiceResponse
    {
        public CredentialApplicationDto ApplicationInfo { get; set; }
        public PersonDetailsBasicDto ApplicantDetails { get; set; }
        public CredentialApplicationTypeDto ApplicationType { get; set; }
        public LookupTypeDetailedDto ApplicationStatus { get; set; }
        public IEnumerable<CredentialRequestDto> CredentialRequests { get; set; }
        public IEnumerable<CredentialApplicationFieldDataDto> Fields { get; set; }
        public IEnumerable<CredentialWorkflowFeeDto> CredentialWorkflowFees { get; set; }
    }
}