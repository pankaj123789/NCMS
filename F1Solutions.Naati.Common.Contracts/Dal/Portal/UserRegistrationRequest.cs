using System;

namespace F1Solutions.Naati.Common.Contracts.Dal.Portal
{
    
    public class UserRegistrationRequest
    {
        
        public int NaatiNumber { get; set; }

        
        public string Email { get; set; }

        
        public string GivenName { get; set; }

        
        public string FamilyName { get; set; }

        
        public DateTime DateOfBirth { get; set; }

        
        public int? Gender { get; set; }

        
        public string Title { get; set; }
    }
}