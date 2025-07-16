using System;

namespace Ncms.Contracts.Models.Person
{
    public class QrCodeSummaryModel
    {
        public string PractitionerVerificationUrl { get; set; }
        public string QrCode { get; set; }
        public DateTime GeneratedDate { get; set; }
        public DateTime? InactiveDate { get; set; }
        public string Credential { get; set; }
        public string Skill { get; set; }
        public DateTime ModifiedDate { get; set; }
    }
}
