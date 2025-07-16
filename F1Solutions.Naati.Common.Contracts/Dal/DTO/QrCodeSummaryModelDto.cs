using System;

namespace F1Solutions.Naati.Common.Contracts.Dal.DTO
{
    public class QrCodeSummaryModelDto
    {
        public string Skill { get; set; }
        public string Credential { get; set; }
        public DateTime GeneratedDate { get; set; }
        public string QrCode { get; set; }
        public DateTime? InactiveDate { get; set; }
        public DateTime ModifiedDate { get; set; }
    }
}
