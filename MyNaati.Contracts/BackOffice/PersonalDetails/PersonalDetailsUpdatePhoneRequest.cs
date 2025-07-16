namespace MyNaati.Contracts.BackOffice.PersonalDetails
{
    
    public class PersonalDetailsUpdatePhoneRequest
    {
        
        public PersonalEditPhone Phone { get; set; }

        
        public bool Delete { get; set; }

        
        public string PrimaryEmail { get; set; }

        
        public int NaatiNumber { get; set; }
    }

    
    public class PersonalEditPhone
    {
        
        public int PhoneId { get; set; }

        
        public string PhoneNumber { get; set; }

        
        public bool IsPreferred { get; set; }

        
        public bool IsCurrentlyListed { get; set; }

        
        public bool ExaminerCorrespondence { get; set; }
    }
}
