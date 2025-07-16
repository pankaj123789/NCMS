using System;

namespace Ncms.Contracts.Models.Person
{
    public class CreatePersonModel
    {
        public string GivenName { get; set; }
        public string OtherNames { get; set; }
        public string FamilyName { get; set; }
        public string PrimaryEmail { get; set; }
        public DateTime DateOfBirth { get; set; }
        public int CountryOfBirth { get; set; }
        public string Gender { get; set; }
    }
}
