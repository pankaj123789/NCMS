namespace MyNaati.Contracts.BackOffice.PractitionerDirectory
{
    
    public class PractitionerDirectoryGetCurrentListingRequest
    {
        
        public int NaatiNumber { get; set; }

        
        public bool IsChangeMySelections { get; set; }

        
        public int DaysToDelayAccreditation { get; set; }
    }
}