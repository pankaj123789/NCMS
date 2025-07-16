using System;
using System.Collections.Generic;

namespace F1Solutions.Naati.Common.Contracts.Dal.DTO
{
    public class RolePlayerAvailabilityDto
    {
        public int NaatiNumber { get; set; }
        public int RolePlayerId { get; set; }
        public string GivenName { get; set; }
        public string Surname { get; set; }
        public int SessionLimit { get; set; }
        public decimal? Rating { get; set; }
        public bool Senior { get; set; }
        public bool Available { get; set; }
        public bool HasCapacity { get; set; }
        public bool IsInTestLocation { get; set; }
        public int? LastAttendedTestSessionId { get; set; }
        public DateTime? LastAttendedTestSessionDateTime { get; set; }
        public IEnumerable<LookupTypeDto> AvailableTestLocations { get; set; }
        public int LanguageId { get; set; }
        public string Gender { get; set; }
        public int Age { get; set; }
    }
}