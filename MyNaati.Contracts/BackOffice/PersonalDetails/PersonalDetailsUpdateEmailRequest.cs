namespace MyNaati.Contracts.BackOffice.PersonalDetails
{
    
    public class PersonalDetailsUpdateEmailRequest
    {
        
        public PersonalEditEmail Email { get; set; }

        
        public bool Delete { get; set; }

        
        public int NaatiNumber { get; set; }

        
        public string UserName { get; set; }

        
        public bool AllowChangePrimary { get; set; }

        
        public string PrimaryEmail { get; set; }
    }

    
    public class PersonalEditEmail
    {
        
        public int EmailId { get; set; }

        
        public string Email { get; set; }
      
        
        public bool IsPreferred { get; set; }
        
        public bool IsCurrentlyListed { get; set; }

        
        public bool IsExaminer { get; set; }

        
        public bool ExaminerCorrespondence { get; set; }
    }
}
