namespace MyNaati.Contracts.BackOffice.PersonalDetails
{
    
    public class PersonalDetailsGetEmailsResponse
    {
        
        public PersonalEmail[] Emails { get; set; }
    }

    
    public class PersonalEmail
    {
        
        public int EmailId { get; set; }

        
        public string Email { get; set; }

        
        public string ContactType { get; set; }

        
        public bool IsPreferred { get; set; }

        
        public bool IsCurrentlyListed { get; set; }

        
        public bool IsLastContactInPD { get; set; }

        
        public bool ExaminerCorrespondence { get; set; }
    }
}
