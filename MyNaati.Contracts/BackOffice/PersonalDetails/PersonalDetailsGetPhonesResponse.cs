namespace MyNaati.Contracts.BackOffice.PersonalDetails
{
    
    public class PersonalDetailsGetPhonesResponse
    {
        
        public PersonalPhone[] Phones { get; set; }
    }

    
    public class PersonalPhone
    {
        
        public int PhoneId { get; set; }

        
        public string PhoneNumber { get; set; }

        
        public bool IsCurrentlyListed { get; set; }

        
        public bool IsLastContactInPD { get; set; }

        
        public bool IsPreferred { get; set; }

        
        public bool ExaminerCorrespondence { get; set; }
    }
}
