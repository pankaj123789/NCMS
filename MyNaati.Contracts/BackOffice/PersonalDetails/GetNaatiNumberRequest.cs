namespace MyNaati.Contracts.BackOffice.PersonalDetails
{
    
    public class GetNaatiNumberRequest
    {
        
        public string PrimaryEmail { get; set; }

        
        public int? DaysToDelayAccreditation { get; set; }
    }
}
