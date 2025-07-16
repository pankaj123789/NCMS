using System;

namespace F1Solutions.Naati.Common.Contracts.Dal
{
    public class Sam4PersonNameData
    {
        public int? TitleId { get; set; }
        public string GivenName { get; set; }
        public string OtherNames { get; set; }
        public string Surname { get; set; }
        public DateTime EffectiveDate { get; set; }
    }
}