using System;
using System.Collections.Generic;

namespace F1Solutions.Naati.Common.Contracts.Dal.DTO
{
    public class TestResultMarkingDto
    {
        public int TestSpecificationId { get; set; }
        public int TestResultId { get; set; }
        public int AttendanceId { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public int TestResultStatusTypeId { get; set; }
        public bool ResultAutoCalculation { get; set; }
        public IEnumerable<TestMarkingComponentDto> TestComponents { get; set; }
    }
}