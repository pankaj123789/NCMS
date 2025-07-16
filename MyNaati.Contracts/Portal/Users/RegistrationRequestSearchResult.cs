using System;

namespace MyNaati.Contracts.Portal.Users
{
    
    public class RegistrationRequestSearchResult
    {
        
        public int Id { get; set; }

        
        public int NaatiNumber { get; set; }

        public string NaatiNumberOnlineDisplay => (NaatiNumber > 0) ? NaatiNumber.ToString() : "Eoi" + NaatiNumber;

        
        public int? Gender { get; set; }

        
        public string Title { get; set; }

        
        public string GivenName { get; set; }

        
        public string FamilyName { get; set; }

        
        public DateTime DateOfBirth { get; set; }

        
        public string EmailAddress { get; set; }

        
        public DateTime DateRequested { get; set; }

        
        public bool Deceased { get; set; }

    }
}
