using System;

namespace MyNaati.Contracts.BackOffice.PersonalDetails
{
    
    public class PersonalDetailsGetExtendedInformationResponse
    {
        
        public PersonalDetailsExtended Person { get; set; }

        
        public PersonalViewAddress Address { get; set; }

        
        public PersonalViewPhone[] Phones { get; set; }

        
        public PersonalViewEmail[] Emails { get; set; }
    }

    
    public class PersonalDetailsExtended
    {
        
        public string NaatiNumber { get; set; }

        
        public string GivenName { get; set; }

        
        public string OtherNames { get; set; }

        
        public string AlternativeGivenName { get; set; }

        
        public string FamilyName { get; set; }

        
        public string AlternativeFamilyName { get; set; }

        
        public DateTime? DateOfBirth { get; set; }

        
        public bool? IsGenderMale { get; set; }

        
        public string Title { get; set; }

        
        public int? CountryId { get; set; }

        
        public int EntityTypeId { get; set; }
    }
}
