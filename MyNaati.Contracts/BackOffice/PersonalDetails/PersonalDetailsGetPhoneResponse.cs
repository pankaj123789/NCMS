using System;

namespace MyNaati.Contracts.BackOffice.PersonalDetails
{
    
    public class PersonalDetailsGetPhoneResponse
    {
        
        public PersonalViewPhone Phone { get; set; }

        
        public string ErrorMessage { get; set; }

        public bool WasSuccessful { get { return String.IsNullOrEmpty(ErrorMessage); } }
    }

    
    public class PersonalViewPhone
    {
        
        public int PhoneId { get; set; }

        
        public string CountryCode { get; set; }

        
        public string AreaCode { get; set; }

        
        public string PhoneNumber { get; set; }

        
        public bool IsCurrentlyListed { get; set; }

        
        public bool IsPreferred { get; set; }

        
        public bool ExaminerCorrespondence { get; set; }
    }
}
