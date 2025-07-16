namespace MyNaati.Contracts.BackOffice.Common
{
    
    public class PersonNaatiNumberRequest
    {
        
        public int NaatiNumber { get; set; }

        
        public int? DaysToDelayAccreditation { get; set; }
    }
}