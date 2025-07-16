using System;

namespace F1Solutions.Naati.Common.Contracts.Dal.Request
{
    public class PersonMfaRequest
    {
        public int NaatiNumber { get; set; }
        public string MfaCode { get; set; }
        public DateTime? MfaExpireStartDate { get; set; }
        public bool Disable { get; set; }
    }
}