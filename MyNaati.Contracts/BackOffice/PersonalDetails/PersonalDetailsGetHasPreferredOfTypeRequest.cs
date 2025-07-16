namespace MyNaati.Contracts.BackOffice.PersonalDetails
{
    
    public class PersonalDetailsGetHasPreferredOfTypeRequest
    {
        
        public int NaatiNumber { get; set; }

        
        public PersonalDetailType Type { get; set; }
    }

    public enum PersonalDetailType
    {
        Address,
        Email,
        Phone
    }
}
