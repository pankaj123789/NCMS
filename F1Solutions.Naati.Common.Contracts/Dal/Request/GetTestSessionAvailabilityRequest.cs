using System;
using System.Collections.Generic;

namespace F1Solutions.Naati.Common.Contracts.Dal.Request
{
    public class GetTestSessionAvailabilityRequest
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public IEnumerable<TestSessionSkillFilter> Skills { get; set; }
        public int TestLocationId { get; set; }
        public int ApplicationTypeId { get; set; }
    }
}