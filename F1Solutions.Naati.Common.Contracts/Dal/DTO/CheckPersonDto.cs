using System;

namespace F1Solutions.Naati.Common.Contracts.Dal.DTO
{
    public class CheckPersonDto
    {
        public string GivenName { get; set; }
        public string OtherNames { get; set; }
        public string SurName { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string PrimaryEmail { get; set; }
        public string Gender { get; set; }
        public DateTime EnteredDate { get; set; }
        public string BirthCountry { get; set; }
    }
}