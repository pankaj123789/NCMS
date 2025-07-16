using System;

namespace MyNaati.Contracts.BackOffice.PersonalDetails
{
    
    public class PersonalDetailsGetWebsiteResponse
    {
        
        public string Website { get; set; }

        
        public DateTime? LastUpdated { get; set; }

        
        public bool IsCurrentlyListed { get; set; }
    }
}
