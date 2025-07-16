using System.Collections.Generic;

namespace F1Solutions.Naati.Common.Contracts.Dal.DTO
{
    public class TestMaterialSummaryDto
    {
        public IEnumerable<TestSittingInfoDto> NewMaterialApplicantsIds { get; set; }
        public IEnumerable<TestSittingInfoDto> ApplicantIdsToOverride { get; set; }
        public IEnumerable<TestMaterialApplicantDto> ApplicantsAlreadySat { get; set; }
        public IEnumerable<int> TestSittingIds { get; set; }
    }
}