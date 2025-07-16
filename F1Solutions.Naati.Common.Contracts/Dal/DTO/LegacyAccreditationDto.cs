using System;

namespace F1Solutions.Naati.Common.Contracts.Dal.DTO
{
    public class LegacyAccreditationDto
    {
        public int LegacyAccreditationId { get; set; }
        public int AccreditationId { get; set; }
        public string Level { get; set; }
        public string Category { get; set; }
        public string Direction { get; set; }
        public string Language1 { get; set; }
        public string Language2 { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? ExpiryDate { get; set; }
        public bool IncludeInOD { get; set; }
        public int NAATINumber { get; set; }

    }
}