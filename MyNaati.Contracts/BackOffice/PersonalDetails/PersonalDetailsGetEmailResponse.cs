using System;

namespace MyNaati.Contracts.BackOffice.PersonalDetails
{
    
    public class PersonalDetailsGetEmailResponse
    {
        
        public PersonalViewEmail Email { get; set; }

        
        public string ErrorMessage { get; set; }

        public bool WasSuccessful { get { return String.IsNullOrEmpty(ErrorMessage); } }
    }

    
    public class PersonalViewEmail
    {
        
        public int EmailId { get; set; }

        
        public string Email { get; set; }

        
        public bool IsPreferred { get; set; }

        
        public bool IsCurrentlyListed { get; set; }

        
        public bool ExaminerCorrespondence { get; set; }
    }
}
