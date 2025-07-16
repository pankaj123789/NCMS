using System;
using System.Collections.Generic;

namespace F1Solutions.Naati.Common.Contracts.Dal.DTO
{
    public class TestMaterialApplicantDto
    {
        public int NaatiNumber { get; set; }
        public int TestSittingId { get; set; }
        public string Name { get; set; }
        public int PreviousTestSessionId { get; set; }
        public string PreviousTestSessionName { get; set; }
        public DateTime PreviousTestSessionDate { get; set; }
        public ICollection<int> ConflictingTestMaterialsIds { get; set; }
        public int PersonId { get; set; }
    }
}