using System;

namespace F1Solutions.Naati.Common.Contracts.Dal.Request
{
    public class CreatePersonRequest
    {
        public string GivenName { get; set; }
        public string OtherNames { get; set; }
        public string SurName { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string PrimaryEmail { get; set; }
        public string Gender { get; set; }
        public DateTime EnteredDate { get; set; }
        public int? BirthCountryId  { get; set; }
        public int? Title { get; set; }
        public bool AllowAutoRecertification { get; set; }
    }
}