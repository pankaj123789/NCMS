namespace MyNaati.Contracts.BackOffice.PersonalDetails
{
    
    public class PersonalDetailsUpdatePhoneResponse
    {
        
        public string ErrorMessage { get; set; }

        public bool WasSuccessful { get { return ErrorMessage == "" || ErrorMessage == null; } }
    }
}
