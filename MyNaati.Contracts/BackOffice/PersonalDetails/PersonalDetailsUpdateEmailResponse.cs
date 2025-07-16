namespace MyNaati.Contracts.BackOffice.PersonalDetails
{
    
    public class PersonalDetailsUpdateEmailResponse
    {
        
        public string ErrorMessage { get; set; }

        public bool WasSuccessful
        {
            get { return ErrorMessage == "" || ErrorMessage == null; }
        }

        
        public bool ChangePrimaryEmail { get; set; }

    }

}
