using System.Collections.Generic;

namespace F1Solutions.Naati.Common.Contracts.Dal.DTO
{
    public class ApplicationBriefDto
    {
        public int CredentialRequestId { get; set; }
        public int CredentialApplicationId { get; set; }
        public IEnumerable<CandidateBriefFileInfoDto> Briefs { get; set; }
        public bool Supplementary { get; set; }

    }
}