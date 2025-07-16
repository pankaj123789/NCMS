namespace MyNaati.Contracts.BackOffice.PersonalDetails
{
    
    public class PersonalDetailsUpdateAddressResponse
    {
        
        public string ErrorMessage { get; set; }

        public bool WasSuccessful { get { return string.IsNullOrEmpty(ErrorMessage); } }
    }
}