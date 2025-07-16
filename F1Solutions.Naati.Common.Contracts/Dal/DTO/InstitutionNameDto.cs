using System;

namespace F1Solutions.Naati.Common.Contracts.Dal.DTO
{
    public class InstitutionNameDto
    {
        public int InstitutionNameId { get; set; }
        public string Name { get; set; }
        public string Abbreviation { get; set; }
        public string TradingName { get; set; }
        public DateTime EffectiveDate { get; set; }
    }
}