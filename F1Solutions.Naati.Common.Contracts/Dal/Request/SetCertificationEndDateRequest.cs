using System;

namespace F1Solutions.Naati.Common.Contracts.Dal.Request
{
    public class SetCertificationEndDateRequest
    {
        public int CertificationPeriodId { get; set; }
        public DateTime NewEndDate { get; set; }
        public string Notes { get; set; }
        public int UserId { get; set; }
    }
}