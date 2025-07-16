using System;
using F1Solutions.Naati.Common.Contracts.Dal.Portal;

namespace MyNaati.Contracts.BackOffice.PersonalDetails
{
    
    public class PersonalDetailsCreateNonCandidateRequest
    {
        
        public string Email { get; set; }

        
        public string GivenName { get; set; }

        
        public string FamilyName { get; set; }

        
        public DateTime BirthDate { get; set; }

        
        public string Gender { get; set; }

        
        public PersonTitle Title { get; set; }

        
        public bool IsEPortalActive { get; set; }

        public PersonalDetailsCreateNonCandidateRequest()
        {
            IsEPortalActive = true; // Assume this is created via ePortal and therefore the candidate has an account
        }
    }
}
