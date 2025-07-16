using System;

namespace F1Solutions.Naati.Common.Contracts.Dal.Response
{
    public class PersonMfaResponse
    {
        public string Email { get; set; }
        public string MfaCode { get; set; }
        public DateTime? MfaExpireStartDate { get; set; }
        public int EmailCode { get; set; }
        public DateTime? EmailCodeExpireStartDate { get; set; }
    }
}