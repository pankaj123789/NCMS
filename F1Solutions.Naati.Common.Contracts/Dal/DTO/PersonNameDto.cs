using System;

namespace F1Solutions.Naati.Common.Contracts.Dal.DTO
{
    public class PersonNameDto
    {
        public int PersonNameId { get; set; }
        public int? TitleId { get; set; }
        public string Title { get; set; }
        public string GivenName { get; set; }
        public string AlternativeGivenName { get; set; }
        public string OtherNames { get; set; }
        public string Surname { get; set; }
        public string AlternativeSurname { get; set; }
        public DateTime EffectiveDate { get; set; }
    }
}